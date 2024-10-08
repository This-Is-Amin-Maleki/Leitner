using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelsLeit.Entities;
using Microsoft.AspNetCore.Identity;
using ServicesLeit.Services;
using ModelsLeit.ViewModels.User;

namespace APILeit.Controllers
{

    [Authorize]
    [ApiController]
    public class AccountController : ControllerBase
    {
        //private readonly ApplicationDbContext _db;
        private readonly ILogger<AccountController> _logger;
        //private readonly int _pageSize;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserService _userService;
        // ApplicationDbContext db,

        public AccountController(
            ILogger<AccountController> logger,
            // IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
          UserService userService)
        {
            _userManager = userManager;
            _logger = logger;
            //_pageSize = configuration.GetValue<int>("Configuration:ItemsInList");
            _userService = userService;
        }
        
        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> Get()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return BadRequest(new { message = "User not valid!" });
            }

            UserModifyProfileLimitedViewModel output = new()
            {
                Id = user.Id,
                Bio = user.Bio,
                Name = user.Name,
            };

            return Ok(output);
        }

        [HttpPut]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> Update([FromBody] UserModifyProfileLimitedViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            model.Id = Convert.ToInt64(
                _userManager.GetUserId(User));

            var result = await _userService.ModifyProfileLimitedAsync(model);

            if (result is null)
            {
                ModelState.AddModelError("XX", "User Not Valid!");
                return BadRequest(ModelState);
            }

            if (result is false)
            {
                ModelState.AddModelError("Fail", "Failed to update profile. Please try again later!");
                return BadRequest(ModelState);
            }

            return CreatedAtAction(
                nameof(Get),
                new { },
                new { message = "Profile updated successfully!" }
            );
        }

        [HttpPut]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordLimitedViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            model.Id = Convert.ToInt64(_userManager.GetUserId(User));

            var result = await _userService.ChangePasswordLimitedAsync(model);

            if (result is null)
            {
                ModelState.AddModelError("XX", "User Not Valid!");
                return BadRequest(ModelState);
            }

            if (result.Errors.Any())
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok(new { message = "Password updated successfully!" });
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> Enable2FA()
        {
            var result = await _userService.TwoFactorActivatorAsync(User);
            if (result is null)
            {
                ModelState.AddModelError("XX", "User Not Valid!");
                return BadRequest(ModelState);
            }

            return Ok(result);
        }

        [HttpPut]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> Enable2FASave([FromBody] string model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var input = new TFAConfirmViewModel()
            {
                Code = model,
                Principal = User,
            };
            var result = await _userService.TwoFactorConfirmAsync(input);
            if (!result)
            {
                ModelState.AddModelError("Confirm", "Your two factor authenticator code could not be validated.");
                return BadRequest(ModelState);
            }

            return Ok(new { message = "Two-factor authentication has been enabled successfully." });
        }

        [HttpPut]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> Deactivate2FA()
        {
            var result = await _userService.TwoFactorDeactivatorAsync(User);
            if (!result)
            {
                return BadRequest(new { message = "Failed to disable two-factor authentication! Please try again later or contact support." });
            }
            return Ok(new { message = "Two-factor authentication has been disabled successfully." });
        }

    }
}