using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ModelsLeit.Entities;
using Services.Services;
using ViewLeit.Extensions;
using ServicesLeit.Services;
using ModelsLeit.ViewModels.User;

namespace View.Controllers
{
    public class PageController : Controller
    {
        /*
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender<ApplicationUser> _emailSender;
        private readonly IHttpContextAccessor _httpContext;
        private readonly RoleManager<UserRole> _roleManager;
        private readonly UserService _userService;
        private readonly ApplicationDbContext _db;  //need before repository
        private readonly ILogger<PageController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly PageRepository _pageRepository;
        private readonly LanguageRepository _languageRepository;
        */
        private readonly CollectionService _collectionService;

        public PageController(CollectionService collectionService)
        {
            _collectionService = collectionService;
        }

        public async Task<IActionResult> Index()
        {
            HomePageViewModel model = new();
            var collections = await _collectionService.ReadPublishedCollectionsAsync();
            model.Collections = collections;

            return View(model);
        }
    }
}