using ContentManagement.API.Extensions;
using ContentManagement.API.Helpers;
using ContentManagement.API.LoginData;
using ContentManagement.API.ValidationClasses;
using ContentManagement.DTOs;
using ContentManagement.Models;
using ContentManagement.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ilogger = Serilog.ILogger;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

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
        private readonly IUserRepository _userRepository;

        public UserController(SignInManager<ApplicationUser> signInManager,
                              UserManager<ApplicationUser> userManager,
                              IConfiguration config,
                              Ilogger logger,
                              IPersonRepository personRepository,
                              IUserRepository userRepository)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
            _logger = logger;
            _personRepository = personRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("user/{email}")]
        public async Task<ActionResult<UserModel>> GetUser(string email)
        {
            try
            {
                //Get user
                UserModel? userModel = await _userRepository.GetUser(email);
                if (userModel == null || String.IsNullOrWhiteSpace(userModel.EmailAddress))
                {
                    return NoContent();
                }

                //Convert to DTO
                UserDTO userDTO = userModel!.ConvertToUserDTO();
                return Ok(userDTO);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected Error");
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Administrator")]
        [Route("deleteuser/{email}")]
        public async Task<ActionResult<bool>> DeleteUser(string email)
        {
            try
            {
                bool deleted = await _userRepository.RemoveUser(email);

                if (deleted)
                {
                    return Ok(deleted);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected Error");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected Error");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [Route("Users")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> Users()
        {
            try
            {
                //Get users
                IEnumerable<UserModel> userModels = await _userRepository.GetUsers();
                if (userModels == null || userModels.Count() == 0)
                {
                    return NoContent();
                }

                //Convert to DTO
                IEnumerable<UserDTO> userDTOs = userModels.ConvertToUserDTOs();

                return Ok(userModels);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected Error");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Administrator")]
        [Route("user/{email}")]
        public async Task<ActionResult<bool>> UpdateUser(string email, [FromBody] UserDTO userDTO)
        {
            try
            {
                if (!email.Equals(userDTO.EmailAddress))
                {
                    _logger.Error("{emailId} does not match {bodyEmail}", email, userDTO.EmailAddress);
                    return StatusCode(StatusCodes.Status400BadRequest, "Email error");
                }

                // Convert to model
                UserModel userModel = userDTO.ConvertToUserModel();

                //Validate
                var validationErrors = ValidationHelper.Validate(userModel);
                if (validationErrors.Count > 0)
                {
                    foreach (var error in validationErrors)
                    {
                        _logger.Error("Validation Error: {error}", error.ErrorMessage);
                    }
                    return StatusCode(StatusCodes.Status400BadRequest, "Validation Error");
                }

                //Additional validation to ensure Display name is unique
                var people = await _personRepository.GetPeople();
                var person = people.FirstOrDefault(x => x.DisplayName.ToLower() == userModel.DisplayName.ToLower());
                if (person != null)
                {
                    //check if emails are the same
                    if (person.UserName != userModel.EmailAddress)
                    {
                        //Display name cannot be used
                        List<string> error = new List<string>() { "Display name already exists. Please try a different name." };
                        return StatusCode(StatusCodes.Status400BadRequest, error);
                    }
                }

                //Save changes
                bool succeeded = await _userRepository.UpdateUser(userModel);
                PersonModel personUser = new PersonModel
                {
                    UserName = userModel.EmailAddress!,
                    DisplayName = userModel.DisplayName!
                };

                bool updated = await _personRepository.UpdatePerson(personUser);

                if (succeeded && updated)
                {
                    return Ok(succeeded);
                }
                else
                {
                    _logger.Information("Updated User = {succeeded}", succeeded);
                    _logger.Information("Updated Person = {updated]", updated);
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected Error. Please see Log");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected Erorr");
            }
        }

        [HttpPut]
        [Authorize]
        [Route("user")]
        public async Task<ActionResult<bool>> UpdateUser([FromBody] UserDTO userDTO)
        {
            try
            {
                // Convert to model
                UserModel userModel = userDTO.ConvertToUserModel();

                //Validate
                var validationErrors = ValidationHelper.Validate(userModel);
                if (validationErrors.Count > 0)
                {
                    foreach (var error in validationErrors)
                    {
                        _logger.Error("Validation Error: {error}", error.ErrorMessage);
                    }
                    return StatusCode(StatusCodes.Status400BadRequest, "Validation Error");
                }

                //Additional validation to ensure Display name is unique
                var people = await _personRepository.GetPeople();
                var person = people.FirstOrDefault(x => x.DisplayName.ToLower() == userModel.DisplayName.ToLower());
                if (person != null)
                {
                    //check if emails are the same
                    if (person.UserName != userModel.EmailAddress)
                    {
                        //Display name cannot be used
                        List<string> error = new List<string>() { "Display name already exists. Please try a different name." };
                        return StatusCode(StatusCodes.Status400BadRequest, error);
                    }
                }

                //Save changes
                bool succeeded = await _userRepository.UpdateUser(userModel);
                PersonModel personUser = new PersonModel
                {
                    UserName = userModel.EmailAddress!,
                    DisplayName = userModel.DisplayName!
                };

                bool updated = await _personRepository.UpdatePerson(personUser);

                if (succeeded && updated)
                {
                    return Ok(succeeded);
                }
                else
                {
                    _logger.Information("Updated User = {succeeded}", succeeded);
                    _logger.Information("Updated Person = {updated]", updated);
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected Error. Please see Log");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected Erorr");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO userDTO)
        {
            // convert to user registration model
            UserRegistrationModel userRegistrationModel = userDTO.ConvertToUserRegistrationModel();

            // Validate user registration model
            var registrationErrors = ValidationHelper.Validate(userRegistrationModel);
            var returnErrors = new List<string>();

            if (registrationErrors.Count > 0)
            {
                // Log validation errors
                foreach (var error in registrationErrors)
                {
                    _logger.Error(error.ErrorMessage!);
                    returnErrors.Add(error.ErrorMessage!);
                }
                await Log.CloseAndFlushAsync();
                
                return StatusCode(StatusCodes.Status400BadRequest, returnErrors);
            }

            //Additional validation to ensure Display name is unique
            var people = await _personRepository.GetPeople();
            var person = people.FirstOrDefault(x => x.DisplayName.ToLower() == userRegistrationModel.DisplayName.ToLower());
            if (person != null)
            {
                //check if emails are the same
                if (person.UserName != userRegistrationModel.EmailAddress)
                {
                    //Display name cannot be used
                    List<string> error = new List<string>() { "Display name already exists. Please try a different name." };
                    return StatusCode(StatusCodes.Status400BadRequest, error);
                }
            }

            // Add new user to Access database
            ApplicationUser applicationUser = new ApplicationUser
            {
                Email = userRegistrationModel.EmailAddress,
                EmailConfirmed = true,
                UserName = userRegistrationModel.EmailAddress,
                DisplayName = userRegistrationModel.DisplayName,
                FirstName = userRegistrationModel.FirstName,
                LastName = userRegistrationModel.LastName
            };


            IdentityResult userResult = await _userManager.CreateAsync(applicationUser, userRegistrationModel.Password!);
            IdentityResult roleResult = await _userManager.AddToRoleAsync(applicationUser, userRegistrationModel.Role!);

            //if successful
            if (userResult.Succeeded && roleResult.Succeeded)
            {
                //save user as a person in ContentManagementDB
                try
                {
                    PersonModel personUser = new PersonModel
                    {
                        DisplayName = userRegistrationModel.DisplayName!,
                        UserName = userRegistrationModel.EmailAddress!
                    };
                    
                    // Add person to ContentManagementDB
                    person = await _personRepository.AddPerson(personUser);

                }
                catch (Exception)
                {
                    List<string> errors = new List<string>();
                    // Log any exceptions
                    foreach (var error in userResult.Errors)
                    {
                        _logger.Error(error.Description);
                        errors.Add(error.Description);
                    }

                    // Remove the the user from access control
                    await _userManager.DeleteAsync(applicationUser);

                    return StatusCode(StatusCodes.Status400BadRequest, errors);
                }

                return Ok(new { userResult.Succeeded });
            }
            else
            {
                List<string> errors = new List<string>();
                foreach (var error in userResult.Errors)
                {
                    _logger.Error(error.Description);
                    errors.Add(error.Description);
                }

                return StatusCode(StatusCodes.Status400BadRequest, errors);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("signin")]
        public async Task<ActionResult<string>> SignIn([FromBody] UserSignInDTO userSignInDTO)
        {
            if (String.IsNullOrWhiteSpace(userSignInDTO.EmailAddress) || String.IsNullOrWhiteSpace(userSignInDTO.Password)){
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            // Convert to model
            UserSignInModel userSignInModel = userSignInDTO.ConvertToUserSignInModel();

            // Validate model
            var signInErrors = ValidationHelper.Validate(userSignInModel);
            if (signInErrors.Count > 0)
            {
                foreach (var error in signInErrors)
                {
                    _logger.Error(error.ErrorMessage!);
                }

                return StatusCode(StatusCodes.Status400BadRequest, "Validation Error: Please see logs");
            }

            string username = userSignInModel.EmailAddress!;
            string password = userSignInModel.Password!;
            // Attempt sign in
            SignInResult signInResult = await _signInManager.PasswordSignInAsync(username, password, false, false);

            // If successful
            if (signInResult.Succeeded)
            {
                //create JWT Token and return
                ApplicationUser? applicationUser = await _userManager.FindByNameAsync(username);

                String JSONWebTokenAsString = await GenerateJSONWebToken(applicationUser);

                return Ok(JSONWebTokenAsString);
            }
            else
            {
                return Unauthorized(userSignInDTO);
            }

        }


        [HttpPost]
        [Authorize]
        [Route("ChangePassword")]
        public async Task<ActionResult<string>> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            //Convert to model
            ChangePasswordModel changePasswordModel = changePasswordDTO.ConvertToChangePasswordModel();

            //Validate model
            var changePasswordErrors = ValidationHelper.Validate(changePasswordModel);
            List<string> validationErrors = new List<string>();
            if (changePasswordErrors.Count > 0)
            {
                //log
                foreach (var error in changePasswordErrors)
                {
                    _logger.Error(error.ErrorMessage!);
                    validationErrors.Add(error.ErrorMessage!);
                }

                return StatusCode(StatusCodes.Status400BadRequest, validationErrors);                
            }


            string? email = GetAuthorisedUserEmail(HttpContext);

            if (String.IsNullOrWhiteSpace(email))
            {
                _logger.Error("Email address not found");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            //Email is unique in the system
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            string currentPassword = changePasswordModel.OldPassword;
            string newPassword = changePasswordModel.NewPassword;

            //Attempt to change password
            var changePasswordResult = await _userManager.ChangePasswordAsync(user!, currentPassword, newPassword);
            if (changePasswordResult.Succeeded == false)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    _logger.Error(error.Description);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, "Error changing password.");
            }

            return Ok("Password Changed");
        }

        private static string? GetAuthorisedUserEmail(HttpContext ctx)
        {
            var userIdentity = ctx.User.Identity as ClaimsIdentity;
            if (userIdentity.IsAuthenticated)
            {
                string? email = userIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

                return email;
            }

            return null;
            
        }

        private async Task<string> GenerateJSONWebToken(ApplicationUser? applicationUser)
        {
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            SigningCredentials credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            //claim = who is the person trying to to sign in claiming to be
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, applicationUser.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, applicationUser.LastName),
                new Claim("DisplayName", applicationUser.DisplayName),
                new Claim("Role", await MainUserRole.GetUserMainRole(_userManager, applicationUser)),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, applicationUser.Id)
            };

            IList<string> roles = await _userManager.GetRolesAsync(applicationUser);
            claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            //generate the token
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
                (
                    _config["Jwt:Issuer"],
                    _config["Jwt:Issuer"],
                    claims,
                    null,
                    expires: DateTime.UtcNow.AddDays(1),
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }        


    }
}
