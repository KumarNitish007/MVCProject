using AuthenticationAPI.Models;
using AuthenticationAPI.Models.Authentication.Login;
using AuthenticationAPI.Models.Authentication.SignUp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagementService.Models;
using UserManagementService.Services;

namespace AuthenticationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IEmailService _emailService;

        public AuthenticationController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ILogger<AuthenticationController> logger, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, [FromQuery] string role)
        {
            _logger.LogInformation("Register method hit");
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if (userExist != null)
            {
                _logger.LogWarning("User already exists");
                return StatusCode(StatusCodes.Status403Forbidden, new Response { Status = "Error", Message = "User Already Exists" });
            }

            IdentityUser user = new IdentityUser
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.UserName,
            };

            if (await _roleManager.RoleExistsAsync(role))
            {
                var result = await _userManager.CreateAsync(user, registerUser.password);
                if (!result.Succeeded)
                {
                    _logger.LogError("User creation failed");
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed" });
                }

                // Add Role to the User
                await _userManager.AddToRoleAsync(user, role);

                // Generate email confirmation token and link
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationlink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token, email = user.Email }, Request.Scheme);
                if (confirmationlink == null)
                {
                    _logger.LogError("Failed to generate confirmation link");
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Failed to generate confirmation link" });
                }

                var message = new Message(new[] { user.Email }, "Confirmation Email link", confirmationlink);
                _emailService.SendEmail(message);

                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = $"User Created & Email to Send {user.Email} Successfully" });
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "This Role Does Not Exist" });
            }
        }


        [HttpGet]
        [Route("TestEmail")]
        public IActionResult TestEmail()
        {
            var message = new Message(new string[] { "nknitish1993@gmail.com" }, "Test", "<h1>Subscribe to my channel</h1>");
            try
            {
                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Email Sent Successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Failed to send email" });
            }
        }

        //[HttpGet]
        //[Route("TestEmail")]
        //public IActionResult TestEmail()
        //{
        //    var recipients = new string[] { "nknitish1993@gmail.com" };

        //    if (recipients == null || recipients.Length == 0)
        //        return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = "Recipient email addresses cannot be null or empty." });

        //    var message = new Message(recipients, "Test", "<h1>Subscribe to my channel</h1>");

        //    try
        //    {
        //        _emailService.SendEmail(message);
        //        return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Email Sent Successfully" });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error sending email: {ex.Message}");
        //        return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Failed to send email" });
        //    }
        //}

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string tocken, string email)
        {
            if (string.IsNullOrWhiteSpace(tocken) || string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "Token and email are required." });
            }
            var user = await _userManager.FindByNameAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, tocken);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = "Success", Message = "Email veryfied Successfully" });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "This User Dose not Exist!.." });
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginmodel)
        {
            //Checking the User
            var User = await _userManager.FindByNameAsync(loginmodel.UserName);

            // Checking The Password
            if (User != null && await _userManager.CheckPasswordAsync(User, loginmodel.password))
            {
                //Claimlist Creation
                var authClaim = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,User.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                };
                var userRole = await _userManager.GetRolesAsync(User);
                foreach (var role in userRole) 
                {
                    authClaim.Add(new Claim(ClaimTypes.Role, role));
                }

                //We add role to the list
                var jwttoken = GetToken(authClaim);


                //Generate the token with the claim
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwttoken),
                    Exception = jwttoken.ValidTo
                });
                // Returning the token
            }
            return Unauthorized();
           
        }

        private JwtSecurityToken GetToken(List<Claim> authclaim)
        {

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["ValidIssuer"],
                audience: _configuration["ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: authclaim,
                signingCredentials: new SigningCredentials(authSigningKey,SecurityAlgorithms.HmacSha256)
                );
            return token;
        }
    }
}
