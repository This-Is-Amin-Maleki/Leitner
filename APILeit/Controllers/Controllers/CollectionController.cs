using Microsoft.AspNetCore.Mvc;
using ModelsLeit.DTOs.Collection;
using ServicesLeit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ModelsLeit.Entities;

namespace APILeit.Controllers
{
    [Authorize]
    [ApiController]
    public class CollectionController: ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CollectionService _collectionService;
        public CollectionController(
            UserManager<ApplicationUser> userManager,
            CollectionService collectionService)
        {
            _userManager = userManager;
            _collectionService = collectionService;
        }
    }
}
