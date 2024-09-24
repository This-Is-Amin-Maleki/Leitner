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

        // POST: CollectionController/Create
        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<ActionResult> Create([FromBody] CollectionCreationDto model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("XX", "Not Valid!");
                return BadRequest(ModelState);
            }

            model.UserId =long.Parse(_userManager.GetUserId(User)!);
#warning catch!!
            try
            { 
               await _collectionService.AddCollectionAsync(model);
            }
            catch( Exception ex)
            {
                ModelState.AddModelError("xx", ex.Message);
                return BadRequest(ModelState);
            }

            return CreatedAtAction(
                nameof(Index),
                new { },
                new { message = $"{model.Name} created successfully!" }
            );
        }

        // POST: CollectionController/Edit
        [HttpPut]
        [Route("api/[controller]/[action]")]
        public async Task<ActionResult> Edit([FromBody] CollectionModifyDto model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("XX", "Not Valid!");
                return BadRequest(ModelState);
            }

            model.UserId = long.Parse(_userManager.GetUserId(User)!);
#warning catch!!
            try
            {
                await _collectionService.EditCollectionLimitedAsync(model);
             }
             catch (Exception ex)
             {
                 ModelState.AddModelError("xx", ex.Message);
                 return BadRequest(ModelState);
            }

            return CreatedAtAction(
                nameof(Index),
                new { },
                new { message = $"{model.Name} modified successfully!" }
            );
        }
    }
}
