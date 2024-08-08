using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ModelsLeit.Entities;
using ModelsLeit.DTOs.User;
using SharedLeit;
using ViewLeit.Extensions;
using Services.Services;
using ModelsLeit.ViewModels;
using System.Web.WebPages;
using Microsoft.IdentityModel.Tokens;
using ModelsLeit.ViewModels.User;

namespace View.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender<ApplicationUser> _emailSender;
        private readonly IHttpContextAccessor _httpContext;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly UserService _userService;

        /*
        private readonly ApplicationDbContext _db;  //need before repository
        private readonly ILogger<PageController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly PageRepository _pageRepository;
        private readonly LanguageRepository _languageRepository;
        */

        public UserController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IHttpContextAccessor httpContext,
            RoleManager<UserRole> roleManager,
            UserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _httpContext = httpContext;
            _emailSender = new EmailSender();
            _roleManager = roleManager;
            _userService = userService;
        }
        public IActionResult Index() => View();

        [HttpPost]// p
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(PreLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Partial/User/_PartialLoginForm", model);
            }
            //send lockOutTime To View
            TempData["LockedOutTime"] = 0;

            model.Mode = UserCheckMode.EmailAndUserName;
            var preLoginData = await _userService.PreLoginAsync(model);
            var result = await _userService.LoginAsync(preLoginData);

            if ((int)result >= 9 )
            {
                LoginResultViewModel outModel = new()
                {
                    Id = "Login",
                    Title = "Login Failed",                    
                    Message = "An error has occurred while processing your request. Please try again later.",
                    Button = "Reload",
                    Destination  = ""
                };
                return PartialView("Partial/User/_PartialResultDialog", outModel);
            }

            if (result is LoginResult.Success)
            {
                var output= Content(Url.Action("Index", "Box"));
                return output;
            }

            if (result is LoginResult.TwoFactorRequire)
            {
                return PartialView("Partial/User/_PartialTwoFactorForm", model);
            }
            var error = result.LoginResultError(preLoginData.User.LockoutEnd);
            ModelState.AddModelError(error.key, error.message);

            if(error.key is "NotConfirmed")
            {
                ViewData["NotConfirmed"] = 1;
            }

            return PartialView("Partial/User/_PartialLoginForm", model);
        }
    }
}