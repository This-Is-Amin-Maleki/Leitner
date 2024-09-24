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

        // GET: BoxController
        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<ActionResult> GetAll()
        {
            var userId = long.Parse(_userManager.GetUserId(User)!);
            List<BoxMiniDto> model = await _boxService.ReadAll4UserAsync(userId);
            return Ok(model);
        }

        // GET: BoxController/List/id
        [HttpGet]
        [Route("api/[controller]/[action]/{collectionId}")]
        public async Task<ActionResult> GetAllByCollection(long collectionId)
        {
            List<BoxMiniDto> model = await _boxService.ReadByCollectionAsync(collectionId);
            return Ok(model);
        }

        // POST: BoxController/Create
        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<ActionResult> Create([FromBody] BoxAddDto model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("XX", "Not Valid!");
                return BadRequest(ModelState);
            }

            long userId = long.Parse(_userManager.GetUserId(User)!);
            var collection = await _collectionService
                .CheckExistCollectionAsync(model.CollectionId, userId);
            if(collection is null)
            {
                ModelState.AddModelError("XX", "Not Allow!");
                return BadRequest(ModelState);
            }

            collection.CardPerDay= model.CardPerDay;
            collection.UserId = userId;
#warning catch!!
            try
            { 
               await _boxService.AddAsync(collection);
            }
            catch( Exception ex)
            {
                ModelState.AddModelError("xx", ex.Message);
                return BadRequest(ModelState);
            }

            return CreatedAtAction(
                nameof(GetAll),
                new { },
                new { message = $"{model.Name}'s Box created successfully!" }
            );
        }

        // POST: BoxController/Delete
        [HttpDelete]
        [Route("api/[controller]/[action]/{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var userId = long.Parse(_userManager.GetUserId(User)!);
                await _boxService.DeleteAsync(id, userId);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return CreatedAtAction(
                nameof(GetAll),
            new { },
                new { message = $"Box removed!" }
            );
        }
    }
}