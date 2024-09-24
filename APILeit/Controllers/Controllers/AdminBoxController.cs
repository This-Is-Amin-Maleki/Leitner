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

        // GET: BoxController
        [HttpGet]
        [Route("api/[controller]/[action]/{userId}")]
        //[Route("AdminBox/User/{userId?}")]
        public async Task<ActionResult> GetAllByUser(long userId = 0)
        {
            List<BoxMiniDto> output;
            if (userId is 0)
            {
                output = await _boxService.ReadAllAsync();
                return Ok(output);
            }
            output = await _boxService.ReadAllAsync(userId);
            return Ok(output);
        }

        //[Route("AdminBox/{id?}")]
        [HttpGet]
        [Route("api/[controller]/[action]/{collectionId}")]
        public async Task<ActionResult> GetAll(long collectionId)
        {
            List<BoxMiniDto> output = await _boxService.ReadByCollectionAsync(collectionId);
            return Ok(output);
        }
    }
}