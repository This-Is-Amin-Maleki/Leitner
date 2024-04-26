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

        // GET: CardController
        public async Task<ActionResult> Index(long id)
        {
            (List<CardViewModel> list, string collectionName) model = await _cardService.ReadCardsAsync(id);

            ViewData["CollectionId"] = id;
            ViewData["CollectionName"] = model.collectionName;
            return View(model.list);
        }

    }
}
