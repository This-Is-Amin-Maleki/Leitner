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
    //[Authorize]
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
        private readonly RoleManager<UserRole> _roleManager;
        private readonly UserService _userService;
        // ApplicationDbContext db,

        public AccountController(
            ILogger<AccountController> logger,
            IConfiguration configuration,
            IHttpContextAccessor context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<UserRole> roleManager,
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

        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return RedirectToAction("Index", "Page");
        }

        public async Task<IActionResult> Profile()
        {            
            var user = await _userManager.GetUserAsync(User);
            if(user is null)
            {
                return RedirectToAction("Index", "Page");
            }
            PEditUserViewModel output = new()
            {
                //Bio = user.Bio,
                //Name = user.Name,
                Email = user.Email ?? string.Empty,
            };
            return View(output);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserModifyLimitedViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            model.Id = user.Id;
            var result = await _userService.ModifyLimitedAsync(model);
            if (result is null)
            {
                ModelState.AddModelError("Fail", "Failed to update profile. Please try again later!");
                return View(model);
            }

            if (result.Email is not null)
            {
                EmailTokenViewModel confirmModel = new()
                {
                    Identifier = result.Email,
                    Token = result.EmailToken,
                };
                var confirmLink = Url.Action("ConfirmEmail", "UserName", confirmModel, HttpContext.Request.Scheme);
                var siteDomain = _httpContext.HttpContext!.Request.Host.ToString();
                var sitenNme = _configuration.GetValue("SiteName", siteDomain);
                var confirmText = _configuration.GetValue("ConfirmRegisterEmail", "Hi Dear User<br> Please confirm your model by clicking on below link:<br><center><a href=\"{0}\">{0}</a></center>");
#warning x_Need Email Sender
                // var emailContent = await _emailSender.SendEmailAsync(model.Email, $"Confirm Password Reset - {sitenNme}", string.Format(confirmText, confirmLink));


                TempData["Email"] = "Please check your email and confirm your email address!";
            }

            if (result.PassChanged)
            {
                TempData["Password"] = "Password updated successfully!";
            }

            TempData["Result"] = "Profile updated successfully!";
            return View();
        }

        public IActionResult ConfirmAuthenticator() => View();

        public async Task<IActionResult> EnableAuthenticator()
        {
            var result = await _userService.TwoFactorActivatorAsync(User);
            if(result is null)
            {
                ModelState.AddModelError("AnyUser", "User not found.");
                return View();
            }
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(string model)
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

            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> DisableAuthenticator()
        {
            var result = await _userService.TwoFactorDeactivatorAsync(User);
            TempData["Twofactor"] = "Two-factor authentication has been disabled successfully.";
            if (!result)
            {
                TempData["Twofactor"] = "Failed to disable two-factor authentication! Please try again later or contact support.";
            }
            return RedirectToAction("Profile");
        }
    }
}