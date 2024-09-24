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

        [HttpGet]
        [Route("api/[controller]/All")]
        public async Task<IActionResult> GetAll(bool? active = null, UserType? type = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<UserListDto> user = await _userService.ReadAllAsync(active, type);
            return Ok(user);
        }

        [HttpGet]
        [Route("api/[controller]/[action]/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user is null)
            {
                return BadRequest(new { message = "User not valid!" });
            }

            var role = await _userManager.GetRolesAsync(user);
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

                Type = (UserType)Enum.Parse(typeof(UserType), role.First()),
            };
            return Ok(output);
        }

        [HttpPut]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> Update([FromBody] UserModifyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            var result = await _userService.ModifyAsync(model);
            if (!result)
            {
                ModelState.AddModelError("Fail", $"Failed to update {model.UserName} profile. Please try again later!");
                return BadRequest(ModelState);
            }

            return CreatedAtAction(
                nameof(GetAll),
                new { active = model.ParameterActive, type = model.ParameterType },
                new { message = $"{model.UserName} updated successfully!" }
            );

        }
    }
}