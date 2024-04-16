using ContentManagement.API.Extensions;
using ContentManagement.API.ValidationClasses;
using ContentManagement.DTOs;
using ContentManagement.Models;
using ContentManagement.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContentManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger _logger;

        public CategoryController(ICategoryRepository categoryRepository, ILogger logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }


        // GET: api/<CategoryController>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> Get()
        {
            try
            {
                IEnumerable<CategoryModel> categoryModels = await _categoryRepository.GetCategories();

                if (categoryModels == null || categoryModels.Count() == 0)
                {
                    return NoContent();
                }

                //Convert to DTO
                IEnumerable<CategoryDTO> categoryDTOs = categoryModels.ConvertToCategoryDTOs();
                return Ok(categoryDTOs);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve the categories!");
            }
        }

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CategoryModel>> Get(int id)
        {
            try
            {
                CategoryModel categoryModel = await _categoryRepository.GetCategory(id);

                if (categoryModel == null || categoryModel.Id == default)
                {
                    return NoContent();
                }

                //convert to DTO
                CategoryDTO categoryDTO = categoryModel.ConvertToCategoryDTO();
                return Ok(categoryDTO);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to retrieve the category!");
            }
        }

        // POST api/<CategoryController>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<CategoryDTO>> Post([FromBody] CategoryDTO categoryDTO)
        {
            try
            {
                // Convert to model
                CategoryModel categoryModel = categoryDTO.ConvertToCategoryModel();

                //validate model
                var validationErrors = ValidationHelper.Validate(categoryModel);
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
                categoryModel = await _categoryRepository.AddCategory(categoryModel);
                if (categoryModel == null || categoryModel.Id == default)
                {
                    return NoContent();
                }
                // convert to dto
                categoryDTO = categoryModel.ConvertToCategoryDTO();
                return CreatedAtAction(nameof(Get), new { id = categoryDTO.Id }, categoryDTO);

            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to complete post operation");
            }
        }

        // PUT api/<CategoryController>/5
        [HttpPut]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<bool>> Put([FromBody] CategoryDTO categoryDTO)
        {
            try
            {
                // Convert to model
                CategoryModel categoryModel = categoryDTO.ConvertToCategoryModel();

                //Validate
                var validationErrors = ValidationHelper.Validate(categoryModel);
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
                bool updated = await _categoryRepository.UpdateCategory(categoryModel);

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

        // DELETE api/<CategoryController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                bool canDelete = await _categoryRepository.CanDeleteCategory(id);
                if (canDelete)
                {
                    bool deleted = await _categoryRepository.DeleteCategory(id);
                    if (!deleted)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete");
                    }
                    return Ok(deleted);
                }
                else
                {
                    //Not allowed to delete this category
                    return BadRequest("This category may not be deleted");
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
