using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ModelsLeit.Entities;
using ServicesLeit.Services;
using ModelsLeit.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using ModelsLeit.DTOs.Collection;
using System.Text.Json;
using ViewLeit.Constants;

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

            if (TempData["Model"] is not null)
            {
                model = JsonSerializer.Deserialize<HomePageViewModel>(TempData["Model"] as string);
            }

            if (HttpContext.Response.StatusCode is 302 or >= 400)
            {
                model.ErrorTitle =  "Error " + HttpContext.Response.StatusCode;
                model.Error = HttpStatus
                    .FromCode(HttpContext.Response.StatusCode)
                    .Description;
            }
            if (TempData["Error"] is not null)
            {
                model.ErrorTitle = (TempData["ErrorTitle"] ?? "Error").ToString();
                model.Error = (TempData["Error"]).ToString();
            }

            if (TempData["Message"] is not null)
            {
                model.Message = TempData["Message"] as string;
            }
            model.Login.ReturnUrl = returnUrl ?? string.Empty;
            model.Collections = await ReadCollections();
            return View(model);
        }

        [Route("Users/{username}")]
        public async Task<IActionResult> UserDetails(string username)
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

            long[]? boxes = [];
            if (isSignedIn)
            {
                var userId = long.Parse(_userManager.GetUserId(User)!);
                boxes = await _boxService.GetAllCollectionIdAsync(userId);
            }
                
            CollectionOfUserDetailDto model = new()
            {
                Bio = user.Bio,
                Boxes = boxes,
                Collections = collections,
            };

            return View(model);
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