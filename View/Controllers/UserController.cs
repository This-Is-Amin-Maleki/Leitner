using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ModelsLeit.Entities;
using ModelsLeit.DTOs.User;
using SharedLeit;
using ViewLeit.Extensions;
using Services.Services;
using System.Web.WebPages;
using ModelsLeit.ViewModels.User;

namespace View.Controllers
{
    [AllowAnonymous]
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

        [HttpPost]// p
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            UserRegisterViewModel registerationData = new()
            {
                Email = model.Email,
                Password = model.Password,
                UserName = model.UserName,
                Type = UserType.User,
                Name = model.UserName,
            };

            UserRegisterDto result = await _userService.RegisterAsync(registerationData);
            if(result.Result is not RegisterResult.Success)
            {
                var error = result.Result.RegisterResultError();
                ModelState.AddModelError(error.key, error.message);
                return PartialView("Partial/User/_PartialRegisterForm", model);
            }

            if(result.Email is not null)
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
            }

            LoginResultViewModel outModel = new()
            {
                Id = "Register",
                Title = "Registration Email Sent",
                Message = "Registration successful! Please check your email to confirm your account.",
            };
            return PartialView("Partial/User/_PartialResultDialog", outModel);
        }

        [HttpGet] //index
        public async Task<IActionResult> ConfirmEmail(EmailTokenViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "An error has occurred!";
                return View("Index");
            }

            var result = await _userService.EmailConfirmAsync(model);

            TempData["Message"] = result.IsEmpty() ?
                "Invalid token. Please try again or request a new one." :
                result ?? "Email confirmed successfully. Now you can log in to your account.";

            return View("Index");
        }
        
        [HttpPost]// p
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendEmail/*Comfirm*/(ResendEmailViewModel model)
        {
            LoginResultViewModel outModel = new()
            {
                Id = "ResendEmail",
                Title = "Confirmation Email Resent",
                Message = "Please check your email to confirm your account.",
            };
            var result = await _userService.EmailTokenGeneratorAsync(model.Email);
            if (result is null)
            {
                //  email not valid!!
                outModel.Title = "Confirmation Email Resending Failed";
                outModel.Message = "An error has occurred while processing your request. Please try again later.";
                return PartialView("Partial/User/_PartialResultDialog", outModel);
            }

            //sendMail
            var confirmLink = Url.Action("ConfirmEmail", "UserName", protocol: HttpContext.Request.Scheme,
            values: new EmailTokenViewModel
            {
                Identifier = result.Identifier,
                Token = result.Token
            });
            var sitenNme = _configuration.GetValue("SiteName", _httpContext.HttpContext!.Request.Host.ToString());
            var confirmText = _configuration.GetValue("ResendConfirmRegisterEmail", "Hi Dear User,<br> Please confirm your model by clicking on below link:<br><center><a href=\"{0}\">{0}</a></center>");

#warning x_Need Email Sender
            // var emailContent = await _emailSender.SendEmailAsync(model.Email, $"Confirm Password Reset - {sitenNme}", string.Format(confirmText, confirmLink));

            return PartialView("Partial/User/_PartialResultDialog", outModel);
        }



        [HttpPost]// p
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Forgot(ResetPasswordRequestViewModel model)
        {
            LoginResultViewModel outModel = new()
            {
                Id = "ForgotSent",
                Title = "Password Reset Email Sending Failed",
                Message = "An error has occurred while processing your request. Please try again later.",
            };
            if (!ModelState.IsValid)
            {
                return PartialView("Partial/User/_PartialResultDialog", outModel);
            }
            model.Mode = UserCheckMode.EmailOnly;
            var token = await _userService.ResetPasswordTokenGeneratorAsync(model);
            if (token is null)
            {
                outModel.Message = "An error has occurred while processing your request! Please try again later.";
                return PartialView("Partial/User/_PartialResultDialog", outModel);
            }

            EmailTokenViewModel confirmModel = new()
            {
                Identifier = model.Identifier,
                Token = token,
            };
            var confirmLink = Url.Action("ConfirmEmail", "UserName", confirmModel, HttpContext.Request.Scheme);
            var siteDomain = _httpContext.HttpContext!.Request.Host.ToString();
            var sitenNme = _configuration.GetValue("SiteName", siteDomain);
            var confirmText = _configuration.GetValue("ConfirmRegisterEmail", "Hi Dear User<br> Please confirm your model by clicking on below link:<br><center><a href=\"{0}\">{0}</a></center>");
#warning x_Need Email Sender
            // var emailContent = await _emailSender.SendEmailAsync(model.Email, $"Confirm Password Reset - {sitenNme}", string.Format(confirmText, confirmLink));

            outModel.Title = "Password Reset Email Sent";
            outModel.Message = "Password reset instructions have been sent to your email. Please check your inbox to proceed with resetting your password.";
            return PartialView("Partial/User/_PartialResultDialog", outModel);
        }

        [HttpGet] //index
        public async Task<IActionResult> ResetPassword(UserConfirmViewModel model)
        {
            model.Mode = UserCheckMode.EmailOnly;
            TryValidateModel(model);

            if (ModelState.IsValid)
            {
                TempData["Message"] = "Invalid token. Please try again or request a new one.";
                return View("Index");
            }
            UserResetPasswordViewModel checkData = new()
            {
                Identifier = model.Identifier,
                Token = model.Token,
            };

            var check = await _userService.ResetPasswordCheckTokenAsync(checkData);
            if (!check)
            {
                TempData["Message"] = "Invalid token. Please try again or request a new one!";
                return View("Index");
            }

            HomePageViewModel outModel = new()
            {
                PasswordReset = new UserResetPasswordViewModel()
                {
                    Identifier = model.Identifier,
                    Token = model.Token,
                },
                Type = UserFormType.PasswordReset,
            };
            return View("Index", outModel);
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
            var loginModel = new PreLoginViewModel();
            if (!ModelState.IsValid)
            {
                return PartialView("Partial/User/_PartialLoginForm", loginModel);
            }

            var result = await _userService.TwoFactorCheckAsync(model);

            if (result is LoginResult.Success)
            {
                return RedirectToAction("Index", "Box");
            }

            var error = result.LoginResultError();

            ModelState.AddModelError(error.key, error.message);

            return PartialView("Partial/User/_PartialLoginForm", loginModel);
#warning check modelState is OK!!
        }

    }
}