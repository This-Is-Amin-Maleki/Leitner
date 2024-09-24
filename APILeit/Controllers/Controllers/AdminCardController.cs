using Microsoft.AspNetCore.Mvc;
using ModelsLeit.DTOs.Card;
using ModelsLeit.Entities;
using ServicesLeit.Services;
using SharedLeit;
using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;

namespace APILeit.Controllers
{
    [Authorize(Roles = nameof(UserType.Admin))]
    [ApiController]
    public class AdminCardController: ControllerBase
    {
        private readonly CardService _cardService;
        private readonly CollectionService _collectionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminCardController(
            CardService cardService,
            CollectionService collectionService,
            UserManager<ApplicationUser> userManager)
        {
            _cardService = cardService;
            _collectionService = collectionService;
            _userManager = userManager;
        }

        // GET: CardController
        [HttpGet]
        [Route("api/[controller]/[action]/{collectionId}")]
        public async Task<ActionResult> GetAll(long collectionId,[FromQuery] [Optional] CardStatus? state)
        {
            List<CardMiniUnlimitedDto> output = await _cardService.ReadCardsUnlimitedAsync(collectionId, state);
            return Ok(output);
        }

        // GET: CardController/Details/5
        [HttpGet]
        [Route("api/[controller]/[action]/{id}")]
        public async Task<ActionResult> Get(long id) =>
            Ok(await _cardService.ReadCardAsync(id));
    }
}
