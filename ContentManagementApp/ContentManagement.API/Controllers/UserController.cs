using ContentManagement.API.Extensions;
using ContentManagement.API.LoginData;
using ContentManagement.DTOs;
using ContentManagement.Models;
using ContentManagement.Models.ValidationClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ilogger = Serilog.ILogger;
using Serilog;
using ContentManagement.Repositories.Contracts;
using System;

namespace ContentManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly Ilogger _logger;
        private readonly IPersonRepository _personRepository;

        public UserController(SignInManager<ApplicationUser> signInManager,
                              UserManager<ApplicationUser> userManager,
                              IConfiguration config,
                              Ilogger logger,
                              IPersonRepository personRepository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
            _logger = logger;
            _personRepository = personRepository;
        }

        [HttpPost]
        [AllowAnonymous] // TODO - Change to Adminstrator role once working
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO userDTO)
        {
            // convert to user registration model
            UserRegistrationModel userRegistrationModel = userDTO.ConvertToUserRegistrationModel();

            // Validate user registration model
            var registrationErrors = ValidationHelper.Validate(userRegistrationModel);
            if (registrationErrors.Count > 0)
            {
                // Log validation errors
                foreach (var error in registrationErrors)
                {
                    _logger.Error(error.ErrorMessage!);
                }
                await Log.CloseAndFlushAsync();
                return StatusCode(StatusCodes.Status400BadRequest, "Validation Error: See log for details");
            }

            // Add new user to Access database
            ApplicationUser applicationUser = new ApplicationUser
            {
                Email = userDTO.EmailAddress,
                EmailConfirmed = true,
                UserName = userDTO.EmailAddress,
                DisplayName = userDTO.DisplayName,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName
            };


            IdentityResult userResult = await _userManager.CreateAsync(applicationUser, userDTO.Password);
            IdentityResult roleResult = await _userManager.AddToRoleAsync(applicationUser, userDTO.Role);

            //if successful
            if (userResult.Succeeded && roleResult.Succeeded)
            {
                //save user as a person in ContentManagementDB
                try
                {
                    PersonModel person = new PersonModel
                    {
                        DisplayName = userRegistrationModel.DisplayName!,
                        UserName = userRegistrationModel.EmailAddress!,
                        Role = userRegistrationModel.Role!
                    };
                    
                    // Add person to ContentManagementDB
                    person = await _personRepository.AddPerson(person);

                }
                catch (Exception)
                {
                    // Log any exceptions
                    foreach (var error in userResult.Errors)
                    {
                        _logger.Error(error.Description);
                    }

                    // Remove the the user from access control
                    await _userManager.DeleteAsync(applicationUser);

                    return StatusCode(StatusCodes.Status500InternalServerError, "Error saving user to database");
                }

                return Ok(new { userResult.Succeeded });
            }
            else
            {
                foreach (var error in userResult.Errors)
                {
                    // TODO - Log any errors
                    _logger.Error(error.Description);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "Error saving user to database");
            }
        }
    }
}
