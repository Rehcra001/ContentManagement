using ContentManagement.DTOs;
using ContentManagement.WPF.Core;
using ContentManagement.WPF.Services.Contracts;
using ContentManagement.WPF.Validators;
using FluentValidation;
using FluentValidation.Results;
using System.Collections.ObjectModel;
using System.Windows;
using Log = Serilog.Log;

namespace ContentManagement.WPF.ViewModels.Administration
{
    public class CategoryManagementViewModel : ViewModel
    {
        public INavigationService NavigationService { get; set; }
        public ICategoryService CategoryService { get; set; }

        private string _viewState;
        private const string VIEW = "View";
        private const string EDIT = "Edit";
        private const string DELETE = "Delete";
        private const string ADD_NEW = "AddNew";
        private const string SAVE = "Save";

        private ObservableCollection<CategoryDTO> _categories = new ObservableCollection<CategoryDTO>();
        public ObservableCollection<CategoryDTO> Categories
        {
            get { return _categories; }
            set { _categories = value; OnPropertyChanged(); }
        }

        private CategoryDTO? _selectedCategory;
        public CategoryDTO? SelectedCategory
        {
            get { return _selectedCategory; }
            set { _selectedCategory = value; OnPropertyChanged(); }
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
        private CategoryDTO? UndoEdit { get; set; }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand EditCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddNewCommand { get; set; }


        public CategoryManagementViewModel(INavigationService navigationService, ICategoryService categoryService)
        {

            NavigationService = navigationService;
            CategoryService = categoryService;

            SaveCommand = new RelayCommand(SaveCategory, CanSaveCategory);
            CancelCommand = new RelayCommand(CancelCategory, CanCancelCategory);
            EditCommand = new RelayCommand(Edit, CanEdit);
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            AddNewCommand = new RelayCommand(AddNew, CanAddNew);
            
            LoadCategories();

            
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

        private async void LoadCategories()
        {
            try
            {
                var categories = await CategoryService.GetCategories();
                foreach (CategoryDTO category in categories)
                {
                    Categories.Add(category);
                }
                if (Categories.Count > 0)
                {
                    SelectedCategory = Categories[0];
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
            SelectedCategory = new CategoryDTO();
            SelectedCategory.CreatedOn = DateTime.Now;
        }

        private bool CanDelete(object obj)
        {
            return (SelectedCategory != null && _viewState.Equals(VIEW));
        }

        private async void Delete(object obj)
        {
            //Make sure the user really wants to delete the category
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this category?",
                                                      "Delete Category",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question,
                                                      MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                SetViewState(DELETE);
                try
                {
                    int id = SelectedCategory!.Id;
                    bool deleted = await CategoryService.RemoveCategory(id);
                    if (deleted)
                    {
                        Categories.Remove(SelectedCategory);
                        if (Categories.Count > 0)
                        {
                            SelectedCategory = Categories[0];
                        }

                        MessageBox.Show("Category deleted", "Delete Category", MessageBoxButton.OK, MessageBoxImage.Information);
                        SetViewState(VIEW);
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Error deleting the category. Please try again", "Delete Category", MessageBoxButton.OK, MessageBoxImage.Error);
                        SetViewState(DELETE);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, ex.Message);
                    string message = "Unexpected error occured when trying to delete the category. \r\n\r\nPlease try again!";
                    MessageBox.Show(message, "Delete Category", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private bool CanEdit(object obj)
        {
            return (SelectedCategory != null && _viewState.Equals(VIEW));
        }

        private void Edit(object obj)
        {
            SetViewState("Edit");
            //Record current category details
            //in case the operation is cancelled
            UndoEdit = new CategoryDTO
            {
                 Id = SelectedCategory!.Id,
                 Name = SelectedCategory.Name,
                 Description = SelectedCategory.Description,
                 IsPublished = SelectedCategory.IsPublished,
                 CreatedOn = SelectedCategory.CreatedOn,
                 LastModified = SelectedCategory.LastModified,
                 PublishedOn = SelectedCategory.PublishedOn
            };
        }

        private bool CanCancelCategory(object obj)
        {
            return true;
        }

        private void CancelCategory(object obj)
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
                    SelectedCategory = Categories[0];
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
                    SelectedCategory!.Id = UndoEdit!.Id;
                    SelectedCategory.Name = UndoEdit!.Name;
                    SelectedCategory.Description = UndoEdit!.Description;
                    SelectedCategory.IsPublished = UndoEdit!.IsPublished;
                    SelectedCategory.CreatedOn = UndoEdit!.CreatedOn;
                    SelectedCategory.LastModified = UndoEdit!.LastModified;
                    SelectedCategory.PublishedOn = UndoEdit!.PublishedOn;

                    SetViewState(VIEW);
                }
                else
                {
                    return;
                }                
            }
        }

        private bool CanSaveCategory(object obj)
        {
            return (SelectedCategory != null && (_viewState.Equals(EDIT) || _viewState.Equals(ADD_NEW)));
        }

        private async void SaveCategory(object obj)
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
                        await SaveNewCategory();
                        break;
                    case EDIT:
                        SetViewState(SAVE);
                        await SaveAlteredCategory();
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                string message = "Unexpected error occured when trying to save the category. \r\n\r\nPlease try again!";
                MessageBox.Show(message, "Save Category", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private async Task SaveAlteredCategory()
        {
            //change last modified date to current date
            SelectedCategory!.LastModified = DateTime.Now;

            //check if IsPublished was changed
            if (SelectedCategory.IsPublished != UndoEdit!.IsPublished)
            {
                if (SelectedCategory.IsPublished)
                {
                    SelectedCategory.PublishedOn = DateTime.Now;
                }
                else
                {
                    SelectedCategory.PublishedOn = default;
                }
            }
            

            bool updated = await CategoryService.UpdateCategory(SelectedCategory);

            if (updated)
            {
                MessageBox.Show("Category successfully saved", "Update Category", MessageBoxButton.OK, MessageBoxImage.Information);
                SetViewState(VIEW);
            }
            else
            {
                MessageBox.Show("Error updating category. Please try again", "Update Category", MessageBoxButton.OK, MessageBoxImage.Error);
                SetViewState(EDIT);
                return;
            }
        }

        private async Task SaveNewCategory()
        {
            var newCategory = await CategoryService.AddCategory(SelectedCategory!);
            if (newCategory == null)
            {
                MessageBox.Show("Error saving category. Please try again", "Add New Category", MessageBoxButton.OK, MessageBoxImage.Error);
                SetViewState(ADD_NEW);
                return;
            }
            else
            {
                Categories.Add(newCategory);
                SelectedCategory = newCategory;
                MessageBox.Show("Category succesfully saved.", "Add New Category", MessageBoxButton.OK, MessageBoxImage.Information);
                SetViewState(VIEW);
            }
            
        }

        private string? Validate()
        {
            var validator = new CategoryValidator();
            ValidationResult validationResult = validator.Validate(SelectedCategory);

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
