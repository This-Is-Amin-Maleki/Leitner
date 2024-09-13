using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ModelsLeit.Entities;
using ServicesLeit.Services;
using ModelsLeit.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using ModelsLeit.DTOs.Collection;
using System.Text.Json;

namespace View.Controllers
{
    [AllowAnonymous]
    public class PageController : Controller
    {
        private readonly CollectionService _collectionService;
        private readonly BoxService _boxService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public PageController(
            CollectionService collectionService,
            BoxService boxService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _boxService = boxService;
            _collectionService = collectionService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index(string? returnUrl)
        {
            HomePageViewModel model = new();
            //HomePageViewModel model = input ?? new();
            if (TempData["Message"] is not null)
            {
                ViewData["Message"] = TempData["Message"] as string;
            }
            if (TempData["Error"] is not null)
            {
                ViewData["ErrorTitle"] = "Error";
                ViewData["Error"] = TempData["Error"] as string;
            }
            if (TempData["Model"] is not null)
            {
                model = JsonSerializer.Deserialize<HomePageViewModel>(TempData["Model"] as string);
            }
            model.Login.ReturnUrl = returnUrl ?? string.Empty;
            model.Collections = await ReadCollections();
            return View(model);
        }

        [Route("Users/{username}")]
        public async Task<IActionResult> UserDetail(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user is null)
            {
                return Redirect("/");
            }

            List<CollectionShowDto> collections = await _collectionService.ReadPublishedCollectionsAsync(user.Id);

            if (collections is null || collections.Count is 0)
            {
                return Redirect("/");
            }

            var boxes = await _boxService.GetAllCollectionIdAsync(user.Id);

            CollectionOfUserDetailDto model = new()
            {
                Bio= user.Bio,
                Boxes= boxes,
                Collections = collections,
            };

            return View(model);
        }

        [Route("AccessDenied")]
        public async Task<IActionResult> Error(string? title, string? returnUrl)
        {
            HomePageViewModel model = new();
            model.Login.ReturnUrl = returnUrl ?? string.Empty;
            model.Collections = await ReadCollections();
            ViewData["ErrorTitle"] = "Access Denied";
            ViewData["Error"] = "You do not have permission to access!";
            return View("Index", model);
        }

        private async Task<IEnumerable<CollectionShowDto>> ReadCollections()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = long.Parse(_userManager.GetUserId(User)!);
                return await _collectionService.ReadUnusedPublishedCollectionsAsync(user);
            }
            return  _collectionService.ReadPublishedCollections(5);
        }
    }
}