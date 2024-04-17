using ContentManagement.API.Extensions;
using ContentManagement.API.ValidationClasses;
using ContentManagement.DTOs;
using ContentManagement.Models;
using ContentManagement.Repositories;
using ContentManagement.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace ContentManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly ILogger _logger;

        public SubCategoryController(ISubCategoryRepository subCategoryRepository, ILogger logger)
        {
            _subCategoryRepository = subCategoryRepository;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SubCategoryDTO>>> Get()
        {
            try
            {
                IEnumerable<SubCategoryModel> subCategoryModels = await _subCategoryRepository.GetSubCategories();

                if (subCategoryModels == null || subCategoryModels.Count() == 0)
                {
                    return NoContent();
                }

                //Convert to DTO
                IEnumerable<SubCategoryDTO> subCategoryDTOs = subCategoryModels.ConvertToSubCategoryDTOs();
                return Ok(subCategoryDTOs);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve the sub categories!");
            }
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<SubCategoryDTO>> Get(int id)
        {
            try
            {
                SubCategoryModel subCategoryModel = await _subCategoryRepository.GetSubCategory(id);

                if (subCategoryModel == null || subCategoryModel.Id == default)
                {
                    return NoContent();
                }

                //convert to DTO
                SubCategoryDTO subCategoryDTO = subCategoryModel.ConvertToSubCategoryDTO();
                return Ok(subCategoryDTO);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve the sub category!");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<SubCategoryDTO>> Post([FromBody] SubCategoryDTO subCategoryDTO)
        {
            try
            {
                // Convert to model
                SubCategoryModel subCategoryModel = subCategoryDTO.ConvertToSubCategoryModel();

                //validate model
                var validationErrors = ValidationHelper.Validate(subCategoryModel);
                List<string> errors = new List<string>();
                if (validationErrors.Count > 0)
                {
                    foreach (var error in validationErrors)
                    {
                        _logger.Error(error.ErrorMessage);
                        errors.Add(error.ErrorMessage);
                    }
                    return StatusCode(StatusCodes.Status400BadRequest, errors);
                }
                //save model
                subCategoryModel = await _subCategoryRepository.AddSubCategory(subCategoryModel);
                if (subCategoryModel == null || subCategoryModel.Id == default)
                {
                    return NoContent();
                }
                // convert to dto
                subCategoryDTO = subCategoryModel.ConvertToSubCategoryDTO();
                return CreatedAtAction(nameof(Get), new { id = subCategoryDTO.Id }, subCategoryDTO);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to complete post operation");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<bool>> Put([FromBody] SubCategoryDTO subCategoryDTO)
        {
            try
            {
                // Convert to model
                SubCategoryModel subCategoryModel = subCategoryDTO.ConvertToSubCategoryModel();

                //Validate
                var validationErrors = ValidationHelper.Validate(subCategoryModel);
                List<string> errors = new List<string>();
                if (validationErrors.Count > 0)
                {
                    foreach (var error in validationErrors)
                    {
                        _logger.Error(error.ErrorMessage);
                        errors.Add(error.ErrorMessage);
                    }
                    return StatusCode(StatusCodes.Status400BadRequest, errors);
                }

                //Update
                bool updated = await _subCategoryRepository.UpdateSubCategory(subCategoryModel);

                if (!updated)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update");
                }
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                bool canDelete = await _subCategoryRepository.CanDeleteSubCategory(id);
                if (canDelete)
                {
                    bool deleted = await _subCategoryRepository.DeleteSubCategory(id);
                    if (!deleted)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete");
                    }
                    return Ok(deleted);
                }
                else
                {
                    //Not allowed to delete this category
                    return BadRequest("This sub category may not be deleted");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete");
            }
        }
    }
}
