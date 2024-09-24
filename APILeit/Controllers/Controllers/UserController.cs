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
    }
}