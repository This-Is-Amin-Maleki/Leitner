using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ModelsLeit.DTOs.User;
using SharedLeit;
using ServicesLeit.Services;
using ModelsLeit.ViewModels.User;
using ServicesLeit.Interfaces;
using ServicesLeit.Extensions;

namespace APILeit.Controllers
{
    [AllowAnonymous]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserService _userService;
        private readonly ITokenService _tokenService;

        public UserController(IHttpContextAccessor httpContext,
            UserService userService,
            ITokenService token)
        {
            _httpContext = httpContext;
            _userService = userService;
            _tokenService = token;
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterDto input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserRegisterViewModel model = new UserRegisterViewModel()
            {
                UserName = input.UserName,
                Email = input.Email,
                Password = input.Password,
                Name = input.Name,
                Domain = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value,
            };

            UserRegisterDto result = await _userService.RegisterAsync(model);
            if (result.Result is not RegisterResult.Success)
            {
                if ((int)result.Result > 7)
                {
#warning report!!
                    //Id = "Register",
                    //Title = "Registration Faild",
                    //Message = "Registration failed! Please try again later.",
                }
                var error = result.Result.RegisterResultError();
                ModelState.AddModelError(error.key, error.message);
                return BadRequest(ModelState);
            }

            LoginResultViewModel outModel = new()
            {
                Id = "Register",
                Title = "Registration Email Sent",
                Message = "Registration successful! Please check your email to confirm your account.",
            };
            //email or phone in use
            if ((int)result.Result is 7)
            {
                outModel.Title = "The email address is already in use.";
                outModel.Message = "Check your inbox (and junk folder) for the registration confirmation email, or reset your password using the \"Forgot Password\" button.";
                return BadRequest(outModel);
            }
            return Ok(outModel);
        }

        [HttpGet]
        [Route("api/[controller]/[action]/{input}")]
        public async Task<IActionResult> Confirmation(string input)
        {
            var result = await _userService.ConfirmationAsync(input);

            //error
            if (result.Status is ComfirmationStatus.Fail)
            {
                return BadRequest("Invalid token. Please try again or request a new one!");
            }

            //email confirmed
            if (result.Status is ComfirmationStatus.SuccessEmail) //just email confirm
            {
                return Ok("Email confirmed successfully. Now you can log in to your account.");
            }

            return Ok("/Password reset successful! You can now log in.");
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> Forgot([FromBody] string identifier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            LoginResultViewModel output = new()
            {
                Id = "ForgotSent",
                Title = "Failed to send the password reset email.",
                Message = "An error has occurred while processing your request! Please try again later.",
            };

            try {
                var model = new ResetPasswordRequestViewModel
                {
                    Mode = UserCheckMode.EmailOnly,
                    Identifier = identifier,
                    Domain = $"{_httpContext.HttpContext.Request.Scheme}://{_httpContext.HttpContext.Request.Host.Value}",
                };
                await _userService.SendPasswordResetTokenAsync(model);
            }
            catch (Exception ex)
            {
                return BadRequest(output);
            }

            output.Title = "Password Reset Email Sent";
            output.Message = "Password reset instructions have been sent to your email. Please check your inbox to proceed with resetting your password.";
            return Ok(output);
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto input)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserResetPasswordViewModel model = new()
            {
                Password = input.Password,
                Identifier = input.Identifier,
                Token = input.Token,
                Mode = UserCheckMode.EmailOnly
            };

            var result = await _userService.ResetPasswordConfirmAsync(model);

            if (!result)
            {
                return BadRequest("There was an issue resetting your password! Please try again or contact our support team if the problem persists.");
            }

            return Ok("Your password has been successfully reset.You can now log in using your new password.Please ensure to keep your password secure and do not share it with anyone.");
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> Login([FromBody] LoginApiDto input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = new PreLoginViewModel()
            {
                Identifier = input.Identifier,
                Password = input.Password,
                Mode = UserCheckMode.EmailAndUserName
            };

            var preLoginData = await _userService.PreLoginAsync(model);
            var result = await _userService.LoginApiAsync(preLoginData);

            if ((int)result.Result >= 9)
            {
                return BadRequest("An error has occurred while processing your request. Please try again later.");
            }

            if (result.Result is LoginResult.TwoFactorRequire)
            {
                return BadRequest("Two-factor authentication required. Please enter the code from your authentication app.");
            }

            if (result.Result is LoginResult.Success)
            {
                return Ok(result);
            }

            var error = result.Result.LoginResultError(preLoginData.User.LockoutEnd);
            ModelState.AddModelError(error.key, error.message);
            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> TwoFactor([FromBody] string model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _userService.TwoFactorCheckAsync(model);

            if (result is LoginResult.Success)
            {
#warning    JWT
                return Ok("JWT");
            }

            var error = result.LoginResultError();

            ModelState.AddModelError(error.key, error.message);

            return BadRequest(ModelState);
#warning check modelState is OK!!
        }
    }
}