using Microsoft.AspNetCore.Mvc;
using ModelsLeit.DTOs.Card;
using ModelsLeit.Entities;
using ServicesLeit.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace APILeit.Controllers
{
    [Authorize]
    [ApiController]
    public class CardController: ControllerBase
    {
        private readonly CardService _cardService;
        private readonly FileService _fileService;
        private readonly CollectionService _collectionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CardController(
            CardService cardService,
            FileService fileService,
            CollectionService collectionService,
            UserManager<ApplicationUser> userManager)
        {
            _cardService = cardService;
            _fileService = fileService;
            _collectionService = collectionService;
            _userManager = userManager;
        }

        // GET: CardController
        [HttpGet]
        [Route("api/[controller]/[action]/{id}")]
        public async Task<ActionResult> GetAll(long id)
        {
            var userId = long.Parse(_userManager.GetUserId(User)!);
            List<CardMiniDto> model = await _cardService.ReadCardsLimitedAsync(id, userId);
            return Ok(model);
        }

        // GET: CardController/Details/5
        [HttpGet]
        [Route("api/[controller]/[action]/{id}")]
        public async Task<ActionResult> Get(long id)
        {
            var userId = long.Parse(_userManager.GetUserId(User)!);
            var model = await _cardService.ReadCardLimitedAsync(id, userId);
            return Ok(model);
        }

        // POST: CardController/Create
        [HttpPost]
        [Route("api/[controller]/[action]")]
        public async Task<ActionResult> Create([FromBody] CardDto model)
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
               await _cardService.AddCardAsync(model);
            }
            catch( Exception ex)
            {
                ModelState.AddModelError("xx", ex.Message);
                return BadRequest(ModelState);
            }

            return CreatedAtAction(
                nameof(GetAll),
                new { id = model.Id},
                new { message = "Card created!" }
            );
        }
    }
}
