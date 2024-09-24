using Microsoft.AspNetCore.Mvc;
using ModelsLeit.DTOs.User;
using ModelsLeit.Entities;
using Microsoft.AspNetCore.Identity;
using System.Text.Encodings.Web;
using ServicesLeit.Services;
using ModelsLeit.ViewModels.User;
using SharedLeit;
using Microsoft.AspNetCore.Authorization;

namespace APILeit.Controllers
{
    //[Authorize(Roles = nameof(UserType.Admin))]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminAccountController : ControllerBase
    {
        //private readonly ApplicationDbContext _db;
        private readonly ILogger<AdminAccountController> _logger;
        private readonly IConfiguration _configuration;
        private readonly int _pageSize;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserService _userService;
        // ApplicationDbContext db,

        public AdminAccountController(
            ILogger<AdminAccountController> logger,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
          UserService userService)
        {
            _configuration = configuration;
            _userManager = userManager;
            _logger = logger;
            _pageSize = _configuration.GetValue<int>("Configuration:ItemsInList");
            _userService = userService;
        }
    }
}