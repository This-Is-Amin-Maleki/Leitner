using Microsoft.AspNetCore.Mvc;
using ModelsLeit.DTOs.Box;
using ServicesLeit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ModelsLeit.Entities;
using SharedLeit;

namespace ViewLeit.Controllers
{
    [Authorize(Roles = nameof(UserType.Admin))]
    public class AdminBoxController : Controller
    {
        private readonly BoxService _boxService;
        private readonly CollectionService _collectionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminBoxController(
            BoxService boxService,
            CollectionService collectionService,
            UserManager<ApplicationUser> userManager)
        {
            _boxService = boxService;
            _collectionService = collectionService;
            _userManager = userManager;
        }

        // GET: BoxController
        [Route("AdminBox/{userId?}")]
        public async Task<ActionResult> Index(long userId = 0)
        {
            ViewData["userId"] = userId;
            List<BoxMiniDto> output;
            if (userId is 0)
            {
                output = await _boxService.ReadAllAsync();
                return View(output);
            }
            output = await _boxService.ReadAllAsync(userId);
            return View(output);
        }

        [Route("AdminBox/{id?}")]
        public async Task<ActionResult> Index(long id)
        {
            List<BoxMiniDto> output = await _boxService.ReadByCollectionAsync(id);
            return View(output);
        }
    }
}