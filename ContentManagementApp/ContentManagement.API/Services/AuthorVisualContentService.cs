using ContentManagement.API.Extensions;
using ContentManagement.API.Services.Contracts;
using ContentManagement.API.ValidationClasses;
using ContentManagement.DTOs;
using ContentManagement.Models;
using ContentManagement.Repositories.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using ILogger = Serilog.ILogger;

namespace ContentManagement.API.Services
{
    public class AuthorVisualContentService : IAuthorVisualContentService
    {
        private readonly IAuthorVisualContentRepository _authorVisualContentRepository;
        private readonly IPersonRepository _personRepository;
        private readonly ILogger _logger;

        public AuthorVisualContentService(IAuthorVisualContentRepository authorVisualContentRepository,
                                          ILogger logger,
                                          IPersonRepository personRepository)
        {
            _authorVisualContentRepository = authorVisualContentRepository;
            _logger = logger;
            _personRepository = personRepository;
        }

        public async Task<AuthorVisualContentDTO?> AddAuthorVisualContent(AuthorVisualContentDTO authorVisualContentDTO, string username)
        {
            try
            {
                if (authorVisualContentDTO.AuthorId == default)
                {
                    //get author id using username
                    int? id = await _personRepository.GetPersonId(username);

                    if (id is null || id == 0)
                    {
                        return null;
                    }

                    authorVisualContentDTO.AuthorId = (int)id;
                }

                //Convert to model
                AuthorVisualContentModel? authorVisualContentModel = authorVisualContentDTO.ConvertToAuthorVisualContentModel();

                //Validate model
                var validationErrors = ValidationHelper.Validate(authorVisualContentModel);
                string message = "";
                if (validationErrors.Count > 0)
                {
                    message += "Validation Errors:\r\n";
                    foreach (var error in validationErrors)
                    {
                        _logger.Error(error.ErrorMessage!);
                        message += error.ErrorMessage + "\r\n";
                    }
                    throw new Exception(message);
                }

                authorVisualContentModel = await _authorVisualContentRepository.AddAuthorVisualContent(authorVisualContentModel);

                if (authorVisualContentModel is null)
                {
                    return null;
                }

                authorVisualContentDTO = authorVisualContentModel.ConvertToAuthorVisualContentModel();

                return authorVisualContentDTO;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteAuthorVisualContent(int id)
        {
            if (id == default)
            {
                throw new Exception("Validation Error: Id is required");
            }

            try
            {
                //Check if this content can be deleted
                bool canDelete = await _authorVisualContentRepository.CanDeleteAuthorVisualContent(id);
                if (canDelete == false)
                {
                    throw new Exception("Not Allowed: Unable to delete this visual content as it is beign used in a post");
                }

                //Can be deleted
                bool deleted = await _authorVisualContentRepository.DeleteAuthorVisualContent(id);
                return deleted;

            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<AuthorVisualContentDTO>> GetAllAuthorVisualContent(string username)
        {
            try
            {
                int authorId = await _personRepository.GetPersonId(username);

                IEnumerable<AuthorVisualContentModel> authorVisualContentModels = await _authorVisualContentRepository.GetAllAuthorVisualContent(authorId);
                if (authorVisualContentModels == null || authorVisualContentModels.Count() == 0)
                {
                    return Enumerable.Empty<AuthorVisualContentDTO>();
                }

                //convert to dto
                IEnumerable<AuthorVisualContentDTO> authorVisualContentDTOs = authorVisualContentModels.ConvertToAuthorVisualContentDTOs();
                return (authorVisualContentDTOs);


            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<AuthorVisualContentDTO> GetAuthorVisualContent(int id)
        {
            try
            {
                AuthorVisualContentModel authorVisualContentModel = await _authorVisualContentRepository.GetAuthorVisualContent(id);
                if (authorVisualContentModel == null || authorVisualContentModel.Id == default)
                {
                    return new AuthorVisualContentDTO();
                }

                //convert to dto
                AuthorVisualContentDTO authorVisualContentDTO = authorVisualContentModel.ConvertToAuthorVisualContentModel();
                return authorVisualContentDTO;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateAuthorVisualContent(AuthorVisualContentDTO authorVisualContentDTO)
        {
            try
            {
                //Convert to model
                AuthorVisualContentModel authorVisualContentModel = authorVisualContentDTO.ConvertToAuthorVisualContentModel();

                //Validate
                var validationErrors = ValidationHelper.Validate(authorVisualContentModel);
                string errors = "";
                if (validationErrors.Count > 0)
                {
                    errors += "Validation Errors: \r\n";
                    foreach (var error in validationErrors)
                    {
                        _logger.Error(error.ErrorMessage);
                        errors += error.ErrorMessage + "\r\n";
                    }
                    throw new Exception(errors);
                }

                bool updated = await _authorVisualContentRepository.UpdateAuthorVisualContent(authorVisualContentModel);
                return updated;

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
