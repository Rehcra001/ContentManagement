using ContentManagement.DTOs;
using ContentManagement.WPF.Core;
using ContentManagement.WPF.Enums;
using ContentManagement.WPF.Services;
using ContentManagement.WPF.Services.Contracts;
using ContentManagement.WPF.Validators;
using FluentValidation.Results;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace ContentManagement.WPF.ViewModels.ContentViewer
{
    public class ContentViewerViewModel : ViewModel
    {
        public INavigationService NavigationService { get; set; }
        public IAuthorVisualContentService AuthorVisualContentService { get; set; }
        public IFileService FileService { get; set; }
        public IUserDetailService UserDetailService { get; set; }

        private string _viewState;
        private const string VIEW = "View";
        private const string EDIT = "Edit";
        private const string DELETE = "Delete";
        private const string ADD_NEW = "AddNew";
        private const string SAVE = "Save";


        private ObservableCollection<AuthorVisualContentDTO> _authorlContent;
        public ObservableCollection<AuthorVisualContentDTO> AuthorContent
        {
            get { return _authorlContent; }
            set { _authorlContent = value; OnPropertyChanged(); }
        }

        private AuthorVisualContentDTO? _selectedContent;
        public AuthorVisualContentDTO? SelectedContent
        {
            get { return _selectedContent; }
            set { _selectedContent = value; LoadVisualContent();  OnPropertyChanged(); }
        }


        private ObservableCollection<string> _contentTypes;
        public ObservableCollection<string> ContentTypes
        {
            get { return _contentTypes; }
            set { _contentTypes = value; OnPropertyChanged(); }
        }


        private byte[]? _visualContentImage;
        public byte[]? VisualContentImage
        {
            get { return _visualContentImage; }
            set { _visualContentImage = value; OnPropertyChanged(); }
        }


        public string LocalPath { get; set; } = "";

        //When in EDIT State this will hold the origianl values
        //until selected category is saved
        private AuthorVisualContentDTO? UndoEdit { get; set; }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand EditCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddNewCommand { get; set; }
        public RelayCommand BrowseCommand { get; set; }

        public ContentViewerViewModel(IAuthorVisualContentService authorVisualContentService,
                                      INavigationService navigationService,
                                      IFileService fileService,
                                      IUserDetailService userDetailService)
        {
            AuthorVisualContentService = authorVisualContentService;
            NavigationService = navigationService;
            FileService = fileService;
            UserDetailService = userDetailService;

            LoadAuthorContent();

            LoadContentTypes();

            //GetByteArray();

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);
            EditCommand = new RelayCommand(Edit, CanEdit);
            DeleteCommand = new RelayCommand(Delete, CanDelete);
            AddNewCommand = new RelayCommand(AddNew, CanAddNew);

            BrowseCommand = new RelayCommand(Browse, CanBrowse);

            SetViewState(VIEW);
        }

        private void SetViewState(string state)
        {
            _viewState = state;

            switch (_viewState)
            {
                case (VIEW):

                    break;
            }
        }

        private bool CanBrowse(object obj)
        {
            return _viewState.Equals(VIEW) == false;
        }

        private async void Browse(object obj)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();

            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                LocalPath = dialog.FileName;
                VisualContentImage = await File.ReadAllBytesAsync(LocalPath);
            }
        }

        private async Task<byte[]> ReadAllBytes(Stream stream)
        {
            if (stream is MemoryStream)
            {
                return ((MemoryStream)stream).ToArray();
            }

            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private bool CanAddNew(object obj)
        {
            return _viewState.Equals(VIEW);
        }

        private void AddNew(object obj)
        {
            SetViewState(ADD_NEW);

            SelectedContent = new AuthorVisualContentDTO();
        }

        private bool CanDelete(object obj)
        {
            return SelectedContent != null && _viewState.Equals(VIEW);
        }

        private void Delete(object obj)
        {
            throw new NotImplementedException();
        }

        private bool CanEdit(object obj)
        {
            return SelectedContent != null && _viewState.Equals(VIEW);
        }

        private void Edit(object obj)
        {
            throw new NotImplementedException();
        }

        private bool CanCancel(object obj)
        {
            return true;
        }

        private void Cancel(object obj)
        {
            NavigationService.NavigateTo<HomeViewModel>();
        }

        private bool CanSave(object obj)
        {
            return _viewState.Equals(VIEW) == false;
        }

        private async void Save(object obj)
        {
            //Validate content
            string? validationResults = Validate();
            if (validationResults != null)
            {
                MessageBox.Show(validationResults,
                                "Validation Errors",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            if (VisualContentImage is null)
            {
                MessageBox.Show("An image is required",
                                "Validation Errors",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            switch (_viewState)
            {
                case ADD_NEW:
                    if (SelectedContent!.IsHttpLink == false)
                    {
                        var content = await AuthorVisualContentService.AddAuthorVisualContent(SelectedContent, LocalPath);
                        if (content is not null)
                        {
                            AuthorContent.Add(content);
                            SelectedContent = content;
                            MessageBox.Show("Content successfully saved.",
                                            "Save Content",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Information);
                            SetViewState(VIEW);
                        }
                    }
                    break;
                case EDIT:

                    break;
            }
        }

        private async void LoadVisualContent()
        {
            if (SelectedContent is not null && SelectedContent.Id != default)
            {
                switch (SelectedContent.IsHttpLink)
                {
                    case false:
                        Stream image = await FileService.GetImageStreamAsync(SelectedContent.FileName);
                        if (image is not null)
                        {
                            VisualContentImage = await ReadAllBytes(image);
                        }
                        else
                        {
                            MessageBox.Show("Unable to retrieve the image.",
                                            "Retrieving Image",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Error);
                        }                        
                        break;
                    case true:

                        break;
                }
            }
        }

        private async void LoadAuthorContent()
        {
            AuthorContent = new ObservableCollection<AuthorVisualContentDTO>();

            IEnumerable<AuthorVisualContentDTO> authorContent = await AuthorVisualContentService.GetAllAuthorVisualContent();

            foreach (var content in authorContent)
            {
                AuthorContent.Add(content);
            }
        }

        private void LoadContentTypes()
        {
            ContentTypes = new ObservableCollection<string>();
            var types = Enum.GetValues(typeof(VisualContentType));

            foreach (var type in types)
            {
                ContentTypes.Add(type.ToString()!);
            }
        }

        private string? Validate()
        {
            var validator = new AuthorVisualContentValidator();
            ValidationResult validationResult = validator.Validate(SelectedContent!);

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
