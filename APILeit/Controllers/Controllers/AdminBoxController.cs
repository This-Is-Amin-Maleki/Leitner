using Microsoft.AspNetCore.Mvc;
using ModelsLeit.DTOs.Box;
using ServicesLeit.Services;
using Microsoft.AspNetCore.Identity;
using ModelsLeit.Entities;
using Microsoft.AspNetCore.Authorization;
using SharedLeit;

namespace APILeit.Controllers
{
    [Authorize(Roles = nameof(UserType.Admin))]
    [ApiController]
    public class AdminBoxController : ControllerBase
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
    }
}