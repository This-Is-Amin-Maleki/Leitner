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
using SharedLeit;

namespace View.Controllers
{
    [Authorize(Roles = nameof(UserType.Admin))]
    public class AdminAccountController : Controller
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

        public AdminAccountController(
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

        public async Task<IActionResult> Index(bool? active =null, UserType? type = null)
        {
            ViewBag.Active = active;
            ViewBag.UserType = type;

            ViewData["Active"] = active;
            ViewData["UserType"] =type;

            List<UserListDto>? user = await _userService.ReadAllAsync(active, type);
            return View(user);
        }
        public async Task<IActionResult> Modify(long id, bool? active = null, UserType? type = null)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var role = await _userManager.GetRolesAsync(user);
            if (user is null)
            {
                return RedirectToAction(
                    "Index",
                    "AdminAccount",
                    new { active, type });
            }
            UserModifyViewModel output = new()
            {
                Id = user.Id,
                Name = user.Name,
                Bio = user.Bio,
                UserName = user.UserName,
                Email = user.Email ?? string.Empty,
                Phone = user.PhoneNumber,
                TwoFactorAuthentication = user.TwoFactorEnabled,
                Active = user.Active,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                PhoneConfirmed = user.PhoneNumberConfirmed,
                ParameterActive = active ?? user.Active,
                ParameterType = type ?? (UserType)Enum.Parse(typeof(UserType), role.First()),

                Type = (UserType)Enum.Parse(typeof(UserType), role.First()),
            };
            return View(output);
        }
        public async Task<IActionResult> Details(long id)
        {            
            var user = await _userManager.FindByIdAsync(id.ToString());
            var role = await _userManager.GetRolesAsync(user);
            if(user is null)
            {
                return RedirectToAction(
                    "Index",
                    "AdminAccount");
            }
            UserModifyViewModel output = new()
            {
                Id = user.Id,
                Name = user.Name,
                Bio = user.Bio,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Phone = user.PhoneNumber,
                TwoFactorAuthentication = user.TwoFactorEnabled,
                Active = user.Active,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd ?? DateTime.Now,
                PhoneConfirmed = user.PhoneNumberConfirmed,

                Type = (UserType)Enum.Parse(typeof(UserType), role.First()),
            };
            return View(output);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(UserModifyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            var result = await _userService.ModifyAsync(model);
            if (!result)
            {
                ModelState.AddModelError("Fail", $"Failed to update {model.UserName} profile. Please try again later!");
                return View(model);
            }

            TempData["Result"] = $"{model.UserName} updated successfully!";
            
            return RedirectToAction(
                "Index",
                "AdminAccount",  new
                {
                    active = model.ParameterActive,
                    type = model.ParameterType
                });
        }
    }
}