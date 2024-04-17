using ContentManagement.DTOs;
using ContentManagement.WPF.Core;
using ContentManagement.WPF.Services.Contracts;
using ContentManagement.WPF.Validators;
using FluentValidation.Results;
using System.Collections.ObjectModel;
using System.Windows;
using Log = Serilog.Log;

namespace ContentManagement.WPF.ViewModels.Administration
{
    public class SubCategoryManagementViewModel : ViewModel
    {
        public INavigationService NavigationService { get; set; }
        public ISubCategoryService SubCategoryService { get; set; }

        private string _viewState;
        private const string VIEW = "View";
        private const string EDIT = "Edit";
        private const string DELETE = "Delete";
        private const string ADD_NEW = "AddNew";
        private const string SAVE = "Save";

        private ObservableCollection<SubCategoryDTO> _subCategories = new ObservableCollection<SubCategoryDTO>();
        public ObservableCollection<SubCategoryDTO> SubCategories
        {
            get { return _subCategories; }
            set { _subCategories = value; OnPropertyChanged(); }
        }

        private SubCategoryDTO? _selectedSubCategory;
        public SubCategoryDTO? SelectedSubCategory
        {
            get { return _selectedSubCategory; }
            set { _selectedSubCategory = value; OnPropertyChanged(); }
        }

        private bool _enabled;
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; OnPropertyChanged(); }
        }

        private bool _readOnly;
        public bool ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = value; OnPropertyChanged(); }
        }

        private bool _canCheck;
        public bool CanCheck
        {
            get { return _canCheck; }
            set { _canCheck = value; OnPropertyChanged(); }
        }

        //When in EDIT State this will hold the origianl values
        //until selected category is saved
        private SubCategoryDTO? UndoEdit { get; set; }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand EditCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddNewCommand { get; set; }


        public SubCategoryManagementViewModel(INavigationService navigationService, ISubCategoryService subCategoryService)
        {

            NavigationService = navigationService;
            SubCategoryService = subCategoryService;

            SaveCommand = new RelayCommand(SaveSubCategory, CanSaveSubCategory);
            CancelCommand = new RelayCommand(CancelSubCategory, CanCancelSubCategory);
            EditCommand = new RelayCommand(Edit, CanEdit);
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            AddNewCommand = new RelayCommand(AddNew, CanAddNew);

            LoadSubCategories();


            SetViewState(VIEW);
        }

        private void SetViewState(string state)
        {
            _viewState = state;

            switch (_viewState)
            {
                case VIEW:
                    Enabled = true;
                    ReadOnly = true;
                    CanCheck = false;
                    break;
                case ADD_NEW:
                    Enabled = false;
                    ReadOnly = false;
                    CanCheck = true;
                    break;
                case EDIT:
                    Enabled = false;
                    ReadOnly = false;
                    CanCheck = true;
                    break;
            }
        }

        private async void LoadSubCategories()
        {
            try
            {
                var subCategories = await  SubCategoryService.GetSubCategories();
                foreach (SubCategoryDTO subCategory in subCategories)
                {
                    SubCategories.Add(subCategory);
                }
                if (SubCategories.Count > 0)
                {
                    SelectedSubCategory = SubCategories[0];
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }

        private bool CanAddNew(object obj)
        {
            return (_viewState.Equals(VIEW));
        }

        private void AddNew(object obj)
        {
            SetViewState(ADD_NEW);
            SelectedSubCategory = new SubCategoryDTO();
            SelectedSubCategory.CreatedOn = DateTime.Now;
        }

        private bool CanDelete(object obj)
        {
            return (SelectedSubCategory != null && _viewState.Equals(VIEW));
        }

        private async void Delete(object obj)
        {
            //Make sure the user really wants to delete the category
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this sub category?",
                                                      "Delete Sub Category",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question,
                                                      MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                SetViewState(DELETE);
                try
                {
                    int id = SelectedSubCategory!.Id;
                    bool deleted = await SubCategoryService.RemoveSubCategory(id);
                    if (deleted)
                    {
                        SubCategories.Remove(SelectedSubCategory);
                        if (SubCategories.Count > 0)
                        {
                            SelectedSubCategory = SubCategories[0];
                        }

                        MessageBox.Show("Sub Category deleted", "Delete Sub Category", MessageBoxButton.OK, MessageBoxImage.Information);
                        SetViewState(VIEW);
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Error deleting the sub category. Please try again", "Delete Sub Category", MessageBoxButton.OK, MessageBoxImage.Error);
                        SetViewState(DELETE);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, ex.Message);
                    string message = "Unexpected error occured when trying to delete the sub category. \r\n\r\nPlease try again!";
                    MessageBox.Show(message, "Delete Sub Category", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private bool CanEdit(object obj)
        {
            return (SelectedSubCategory != null && _viewState.Equals(VIEW));
        }

        private void Edit(object obj)
        {
            SetViewState("Edit");
            //Record current category details
            //in case the operation is cancelled
            UndoEdit = new SubCategoryDTO
            {
                Id = SelectedSubCategory!.Id,
                Name = SelectedSubCategory.Name,
                Description = SelectedSubCategory.Description,
                IsPublished = SelectedSubCategory.IsPublished,
                CreatedOn = SelectedSubCategory.CreatedOn,
                LastModified = SelectedSubCategory.LastModified,
                PublishedOn = SelectedSubCategory.PublishedOn
            };
        }

        private bool CanCancelSubCategory(object obj)
        {
            return true;
        }

        private void CancelSubCategory(object obj)
        {
            if (_viewState.Equals(VIEW))
            {
                NavigationService.NavigateTo<HomeViewModel>();
            }
            else if (_viewState.Equals(ADD_NEW))
            {
                MessageBoxResult result = MessageBox.Show("This will not save any data that may have been entered.\r\nAre you sure you want to cancel this add new operation?\r\n",
                                                          "Cancel Edit",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Warning,
                                                          MessageBoxResult.No);
                if (result == MessageBoxResult.Yes)
                {
                    SelectedSubCategory = SubCategories[0];
                }
            }
            else if (_viewState.Equals(EDIT))
            {
                MessageBoxResult result = MessageBox.Show("This will undo any changes that may have been made.\r\nAre you sure you want to cancel this edit operation?\r\n",
                                                          "Cancel Edit",
                                                          MessageBoxButton.YesNo,
                                                          MessageBoxImage.Warning,
                                                          MessageBoxResult.No);
                if (result == MessageBoxResult.Yes)
                {
                    SelectedSubCategory!.Id = UndoEdit!.Id;
                    SelectedSubCategory.Name = UndoEdit!.Name;
                    SelectedSubCategory.Description = UndoEdit!.Description;
                    SelectedSubCategory.IsPublished = UndoEdit!.IsPublished;
                    SelectedSubCategory.CreatedOn = UndoEdit!.CreatedOn;
                    SelectedSubCategory.LastModified = UndoEdit!.LastModified;
                    SelectedSubCategory.PublishedOn = UndoEdit!.PublishedOn;

                    SetViewState(VIEW);
                }
                else
                {
                    return;
                }
            }
        }

        private bool CanSaveSubCategory(object obj)
        {
            return (SelectedSubCategory != null && (_viewState.Equals(EDIT) || _viewState.Equals(ADD_NEW)));
        }

        private async void SaveSubCategory(object obj)
        {
            //Validate new user
            string? validationResults = Validate();
            if (validationResults != null)
            {
                MessageBox.Show(validationResults, "Validation Errors", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                switch (_viewState)
                {
                    case ADD_NEW:
                        SetViewState(SAVE);
                        await SaveNewSubCategory();
                        break;
                    case EDIT:
                        SetViewState(SAVE);
                        await SaveAlteredSubCategory();
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                string message = "Unexpected error occured when trying to save the sub category. \r\n\r\nPlease try again!";
                MessageBox.Show(message, "Save Sub Category", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private async Task SaveAlteredSubCategory()
        {
            //change last modified date to current date
            SelectedSubCategory!.LastModified = DateTime.Now;

            //check if IsPublished was changed
            if (SelectedSubCategory.IsPublished != UndoEdit!.IsPublished)
            {
                if (SelectedSubCategory.IsPublished)
                {
                    SelectedSubCategory.PublishedOn = DateTime.Now;
                }
                else
                {
                    SelectedSubCategory.PublishedOn = default;
                }
            }


            bool updated = await SubCategoryService.UpdateSubCategory(SelectedSubCategory);

            if (updated)
            {
                MessageBox.Show("Sub Category successfully saved", "Update Sub Category", MessageBoxButton.OK, MessageBoxImage.Information);
                SetViewState(VIEW);
            }
            else
            {
                MessageBox.Show("Error updating sub category. Please try again", "Update Sub Category", MessageBoxButton.OK, MessageBoxImage.Error);
                SetViewState(EDIT);
                return;
            }
        }

        private async Task SaveNewSubCategory()
        {
            var newSubCategory = await SubCategoryService.AddSubCategory(SelectedSubCategory!);
            if (newSubCategory == null)
            {
                MessageBox.Show("Error saving sub category. Please try again", "Add New Sub Category", MessageBoxButton.OK, MessageBoxImage.Error);
                SetViewState(ADD_NEW);
                return;
            }
            else
            {
                SubCategories.Add(newSubCategory);
                SelectedSubCategory = newSubCategory;
                MessageBox.Show("Sub Category succesfully saved.", "Add New Sub Category", MessageBoxButton.OK, MessageBoxImage.Information);
                SetViewState(VIEW);
            }

        }

        private string? Validate()
        {
            var validator = new SubCategoryValidator();
            ValidationResult validationResult = validator.Validate(SelectedSubCategory!);

            string errors = "";
            if (validationResult.IsValid == false)
            {
                foreach (var failure in validationResult.Errors)
                {
                    errors += $"{failure.ErrorMessage}\r\n";
                }
                return errors;
            }
            return null;
        }
    }
}
