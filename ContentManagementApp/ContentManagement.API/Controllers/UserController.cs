using ContentManagement.API.Extensions;
using ContentManagement.API.LoginData;
using ContentManagement.DTOs;
using ContentManagement.Models;
using ContentManagement.Models.ValidationClasses;
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
                    _logger.Error(error.Description);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, "Error saving user to database");
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
            if (changePasswordErrors.Count > 0)
            {
                //log
                foreach (var error in changePasswordErrors)
                {
                    _logger.Error(error.ErrorMessage!);
                }

                return StatusCode(StatusCodes.Status400BadRequest, "Validation Errors: Please see log.");                
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
