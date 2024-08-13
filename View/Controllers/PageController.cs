using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ModelsLeit.Entities;
using ServicesLeit.Services;
using ViewLeit.Extensions;
using ServicesLeit.Services;
using ModelsLeit.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using ModelsLeit.DTOs.Collection;

namespace View.Controllers
{
    [AllowAnonymous]
    public class PageController : Controller
    {
        private readonly CollectionService _collectionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public PageController(
            CollectionService collectionService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _collectionService = collectionService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            HomePageViewModel model = new();
            model.Collections = await ReadCollections();
            return View(model);
        }

        private async Task<IEnumerable<CollectionDto>> ReadCollections()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = long.Parse(_userManager.GetUserId(User)!);
                return await _collectionService.ReadUnusedPublishedCollectionsAsync(user);
            }
            return await _collectionService.ReadPublishedCollectionsAsync();
        }
    }
}