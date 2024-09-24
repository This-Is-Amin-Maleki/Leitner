using Microsoft.AspNetCore.Mvc;
using ModelsLeit.DTOs.Card;
using ModelsLeit.DTOs.Box;
using ServicesLeit.Services;
using ModelsLeit.DTOs.Container;
using Microsoft.AspNetCore.Identity;
using ModelsLeit.Entities;
using Microsoft.AspNetCore.Authorization;

namespace APILeit.Controllers
{
    [Authorize]
    [ApiController]
    public class BoxController : ControllerBase
    {
        private readonly BoxService _boxService;
        private readonly CollectionService _collectionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BoxController(
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