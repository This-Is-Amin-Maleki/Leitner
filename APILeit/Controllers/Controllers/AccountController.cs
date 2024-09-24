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
    }
}