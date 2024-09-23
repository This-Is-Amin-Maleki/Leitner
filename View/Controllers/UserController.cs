using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ModelsLeit.Entities;
using ModelsLeit.DTOs.User;
using SharedLeit;
using ServicesLeit.Services;
using ModelsLeit.ViewModels.User;
using System.Text.Json;
using ServicesLeit.Extensions;

namespace View.Controllers
{
    [AllowAnonymous]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly NotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContext;
        private readonly RoleManager<IdentityRole<long>> _roleManager;
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
            RoleManager<IdentityRole<long>> roleManager,
            UserService userService,
            NotificationService notificationService)
        {
            _notificationService = notificationService;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _httpContext = httpContext;
            _roleManager = roleManager;
            _userService = userService;
        }

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

            //var preLoginFakeData = await _userService.PreLoginFakeAsync(model);
            //var res = await _userService.MyLoginAsync(preLoginFakeData, model.Password);
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
                var output = string.IsNullOrEmpty(model.ReturnUrl) ?
                    Content("/") :
                    Content(model.ReturnUrl);
                return output;
            }

            if (result is LoginResult.TwoFactorRequire)
            {
                return PartialView("Partial/User/_PartialTwoFactorForm");
            }
            var error = result.LoginResultError(preLoginData.User.LockoutEnd);
            ModelState.AddModelError(error.key, error.message);
            
            return PartialView("Partial/User/_PartialLoginForm", model);
        }

        [HttpPost]// p
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginFake(PreLoginViewModel model)

        {
            if (!ModelState.IsValid)
            {
                return PartialView("Partial/User/_PartialLoginForm", model);
            }
            //send lockOutTime To View
            TempData["LockedOutTime"] = 0;

            model.Mode = UserCheckMode.EmailAndUserName;
            var preLoginData = await _userService.PreLoginAsync(model);
            var result = await _userService.MyLoginAsync(preLoginData.User, model.Password);

            if ((int)result >= 9)
            {
                LoginResultViewModel outModel = new()
                {
                    Id = "Login",
                    Title = "Login Failed",
                    Message = "An error has occurred while processing your request. Please try again later.",
                    Button = "Reload",
                    Destination = ""
                };
                return PartialView("Partial/User/_PartialResultDialog", outModel);
            }

            if (result is LoginResult.Success)
            {
                var output = string.IsNullOrEmpty(model.ReturnUrl) ?
                    Content(Url.Action("Index", "Box")) :
                    Content(model.ReturnUrl);
                return output;
                return output;
            }

            if (result is LoginResult.TwoFactorRequire)
            {
                return PartialView("Partial/User/_PartialTwoFactorForm", new User2FAViewModel());
            }
            var error = result.LoginResultError(preLoginData.User.LockoutEnd);
            ModelState.AddModelError(error.key, error.message);
            
            return PartialView("Partial/User/_PartialLoginForm", model);
        }
        [HttpPost]// p
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("Partial/User/_PartialRegisterForm", model);
            }
            model.Domain = HttpContext.Request.Scheme +"://"+ HttpContext.Request.Host.Value;
            UserRegisterDto result = await _userService.RegisterAsync(model);
            if(result.Result is not RegisterResult.Success)
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
                return PartialView("Partial/User/_PartialRegisterForm", model);
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
                return PartialView("Partial/User/_PartialResultDialog", outModel);
            }
            return PartialView("Partial/User/_PartialResultDialog", outModel);
        }

        [HttpGet] //index
        [Route("Confirm/{input}")]
        public async Task<IActionResult> Confirmation(string input)
        {
            var result = await _userService.ConfirmationAsync(input);

            //error
            if (result.Status is ComfirmationStatus.Fail)
            {
                TempData["Error"] = "Invalid token. Please try again or request a new one!";
                return Redirect("/");
            }

            //email confirmed
            if (result.Status is ComfirmationStatus.SuccessEmail) //just email confirm
            {
                TempData["Message"] = "Email confirmed successfully. Now you can log in to your account.";
                return Redirect("/");
            }

            //reset password
            HomePageViewModel output = new()
            {
                PasswordReset = result.ResetPassword,
                Type = UserFormType.PasswordReset,
            };
            TempData["Model"] = JsonSerializer.Serialize(output);
            return Redirect("/");
        }

        [HttpPost]// p
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Forgot(string identifier)
        {
            LoginResultViewModel output = new()
            {
                Id = "ForgotSent",
                Title = "Failed to send the password reset email.",
                Message = "An error has occurred while processing your request! Please try again later.",
            };

            try{
                var model = new ResetPasswordRequestViewModel
                {
                    Mode = UserCheckMode.EmailOnly,
                    Identifier = identifier,
                    Domain = $"{_httpContext.HttpContext.Request.Scheme}://{_httpContext.HttpContext.Request.Host.Value}",
                };
                await _userService.SendPasswordResetTokenAsync(model);
            }
            catch
            {
                return PartialView("Partial/User/_PartialResultDialog", output);
            }

            output.Title = "Password Reset Email Sent";
            output.Message = "Password reset instructions have been sent to your email. Please check your inbox to proceed with resetting your password.";
            return PartialView("Partial/User/_PartialResultDialog", output);
        }

        [HttpPost]// p
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(UserResetPasswordViewModel model)
        {
            LoginResultViewModel outModel = new()
            {
                Id = "ForgotSent",
                Title = "Password Reset Failed",
                Message = "There was an issue resetting your password. Please try again or contact our support team if the problem persists.",
            };

            if (!ModelState.IsValid)
            {
                return PartialView("Partial/User/_PartialResultDialog", outModel);
            }

            model.Mode = UserCheckMode.EmailOnly;
            var result = await _userService.ResetPasswordConfirmAsync(model);

            if (!result)
            {
                outModel.Message = "There was an issue resetting your password! Please try again or contact our support team if the problem persists.";
                return PartialView("Partial/User/_PartialResultDialog", outModel);
            }

            outModel.Title = "Password Reset Successful";
            outModel.Message = "Your password has been successfully reset. You can now log in using your new password. Please ensure to keep your password secure and do not share it with anyone.";
            return PartialView("Partial/User/_PartialResultDialog", outModel);
        }

        [HttpPost]// p
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TwoFactor(string model)
        {
            var preLoginModel = new PreLoginViewModel();
            if (model is null)
            {
                return PartialView("Partial/User/_PartialLoginForm", preLoginModel);
            }

            var result = await _userService.TwoFactorCheckAsync(model);

            if (result is LoginResult.Success)
            {
                return RedirectToAction("Index", "Box");
            }

            var error = result.LoginResultError();

            ModelState.AddModelError(error.key, error.message);

            return PartialView("Partial/User/_PartialLoginForm", preLoginModel);
#warning check modelState is OK!!
        }

        [HttpPost]// p
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TwoFactorFake(User2FAViewModel model)
        {
            PreLoginViewModel preLoginModel = new()
            {
                Password = model.Password,
                Identifier = model.Identifier,
                Mode = UserCheckMode.EmailAndUserName
            };
            if (!ModelState.IsValid)//model is null)// 
            {
                return PartialView("Partial/User/_PartialLoginForm", preLoginModel);
            }

            model.User = await _userService.PreLoginFakeAsync(preLoginModel);

            var result = await _userService.MyTFA(model);

            if (result is LoginResult.Success)
            {
                return RedirectToAction("Index", "Box");
            }

            var error = result.LoginResultError();

            ModelState.AddModelError(error.key, error.message);

            return PartialView("Partial/User/_PartialLoginForm", preLoginModel);
#warning check modelState is OK!!
        }
    }
}