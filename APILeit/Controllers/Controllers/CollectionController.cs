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

        // GET: CollectionController
        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<ActionResult> GetAll()
        {
            var userId = long.Parse(_userManager.GetUserId(User)!);
            var output = await _collectionService.ReadAllAsync(userId);
            return Ok(output);
        }

        // GET: CollectionController/Details/5
        [HttpGet]
        [Route("api/[controller]/[action]/{id}")]
        public async Task<ActionResult> Get(long id)
        {
            var userId = long.Parse(_userManager.GetUserId(User)!);
            var output = await _collectionService.ReadUserCollectionAsync(id, userId);
            return Ok(output);
        }
    }
}
