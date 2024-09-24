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
    }
}
