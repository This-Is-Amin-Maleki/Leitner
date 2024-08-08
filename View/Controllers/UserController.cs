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
    }
}