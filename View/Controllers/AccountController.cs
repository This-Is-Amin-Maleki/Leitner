using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelsLeit.DTOs.User;
using ModelsLeit.Entities;
using Microsoft.AspNetCore.Identity;
using ModelsLeit.ViewModels;
using System.Text.Encodings.Web;
using ServicesLeit.Services;
using System.Data;
using ModelsLeit.ViewModels.User;

namespace View.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //private readonly ApplicationDbContext _db;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly int _pageSize;
        private readonly UrlEncoder _urlEncoder;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<long>> _roleManager;
        private readonly UserService _userService;
        // ApplicationDbContext db,

        public AccountController(
            ILogger<AccountController> logger,
            IConfiguration configuration,
            IHttpContextAccessor context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole<long>> roleManager,
            UrlEncoder urlEncoder,
          UserService userService)
        {
            _urlEncoder = urlEncoder;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
            _pageSize = _configuration.GetValue<int>("Configuration:ItemsInList");
            _httpContext = context;
            _roleManager = roleManager;
            _userService = userService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return Redirect("/");
        }

        /*
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {   
                return Redirect("/");
            }

            UserModifyLimitedViewModel output = new()
            {
                Bio = user.Bio,
                Name = user.Name,
            };

            return View(output);
        }
        */

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return Redirect("/");
            }

            UserModifyProfileLimitedViewModel output = new()
            {   
                Id = user.Id,
                Bio = user.Bio,
                Name = user.Name,
            };

            return View(output);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserModifyProfileLimitedViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Id = Convert.ToInt64(
                _userManager.GetUserId(User));
            
            var result = await _userService.ModifyProfileLimitedAsync(model);

            if (result is null)
            {
                return Redirect("/");
            }

            if(result is false) { 
                ModelState.AddModelError("Fail", "Failed to update profile. Please try again later!");
                return View(model);
            }

            TempData["Message"] = "Profile updated successfully!";
            return Redirect("/");
        }

        public IActionResult ChangePassword()
        {
            ViewData["userId"] = _userManager.GetUserId(User);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(UserChangePasswordLimitedViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Id = Convert.ToInt64(
                _userManager.GetUserId(User));
            
            var result = await _userService.ChangePasswordLimitedAsync(model);

            if (result is null)
            {
                return Redirect("/");
            }

            if(result is false) { 
                ModelState.AddModelError("Fail", "Failed to update password. Please try again later!");
                return View(model);
            }

            ViewData["userId"] = "Password updated successfully!";
            return Redirect("/");
        }

        public async Task<IActionResult> Enable2FA()
        {
            var result = await _userService.TwoFactorActivatorAsync(User);
            if(result is null)
            {
                return Redirect("/");
            }

            return View(result);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enable2FA(string model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var input = new TFAConfirmViewModel() {
                Code = model,
                Principal = User,
            };
            var result = await _userService.TwoFactorConfirmAsync(input);
            if (!result)
            {
                ModelState.AddModelError("Confirm", "Your two factor authenticator code could not be validated.");
                return View();
            }

            return View("Confirm2FA");
        }
        public IActionResult ConfirmAuthenticator() => View();

        public IActionResult Disable2FA() => View();
        public async Task<IActionResult> Deactivate2FA()
        {
            var result = await _userService.TwoFactorDeactivatorAsync(User);
            TempData["Twofactor"] = "Two-factor authentication has been disabled successfully.";
            if (!result)
            {
                TempData["Twofactor"] = "Failed to disable two-factor authentication! Please try again later or contact support.";
                return View("Disable2FA");
            }
            return View();
        }
        
    }
}