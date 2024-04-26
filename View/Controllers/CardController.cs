using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using Services.Services;
using System.Collections;

namespace View.Controllers
{
    public class CardController: Controller
    {
        CardService _cardService;
        public CardController(CardService cardService)
        {
            _cardService = cardService;
        }

    }
}
