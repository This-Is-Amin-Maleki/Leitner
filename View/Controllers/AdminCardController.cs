using Microsoft.AspNetCore.Mvc;
using ModelsLeit.DTOs.Card;
using ModelsLeit.Entities;
using ServicesLeit.Services;
using SharedLeit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ModelsLeit.DTOs.Container;
using Newtonsoft.Json;
using ServicesLeit.Interfaces;
using System.Data;
using System.Runtime.InteropServices;

namespace ViewLeit.Controllers
{
    [Authorize(Roles = nameof(UserType.Admin))]
    public class AdminCardController: Controller
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
        public async Task<ActionResult> Index(long id, [Optional] CardStatus? state)
        {
            List<CardMiniUnlimitedDto> model = await _cardService.ReadCardsUnlimitedAsync(id, state);
            CollectionStatus? status;
            if (model.Count > 0)
            {
                var firstCard = model.First().Collection;
                ViewData["CollectionName"] = firstCard.Name;
                status = firstCard.Status;
            }
            else
            {
                var collectionData= await _collectionService.ReadCollectionNameAndStatusAsync(id);
                ViewData["CollectionName"] = collectionData.Name;
                status = collectionData.Status;
            }
            ViewData["CollectionId"] = id;
            ViewData["State"] = state;
            return View(model);
        }

        // GET: CardController/Details/5
        public async Task<ActionResult> Details(long id) =>
            View(await _cardService.ReadCardAsync(id));

        // GET: CardController/Edit
        public async Task<ActionResult> Edit(long id) => 
            View(await _cardService.ReadCardAsync(id));

        // POST: CardController/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CardDto model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("XX", "Not Valid!");
                return View(model);
            }

#warning catch!!
            try
             {
                await _cardService.UpdateCardAsync(model);
                return RedirectToAction(nameof(Index), new { id = model.Collection.Id });
            }
             catch (Exception ex)
             {
                 ModelState.AddModelError("xx", ex.Message);
                 return View(model);
             }
        }
        /*
        // POST: CardController/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(CardDto model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("XX", "Not Valid!");
                return View(model);
            }

            try
            {
                await _cardService.DeleteCardAsync(model);
                return RedirectToAction(nameof(Index), new { id = model.Collection.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("xx", ex.Message);
                return View(model);
            }
        }*/
        public async Task<ActionResult> CheckAll(long id)
        {
            try
            {
                var model = await _cardService.ReadCardsByStatusAsync(id);
                if (model is null && model.Submitted.Count is 0)
                {
                    ViewData["er"] = "No cards left to check";
                    return RedirectToAction(nameof(Index), new { id });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index), new { id });
            }
        }

        public async Task<ActionResult> Check(long id)
        {
            try
            {
                var model = await _cardService.ReadCardsByStatusAsync(id);
                if (model is null && model.Submitted.Count is 0)
                {
                    ViewData["er"] = "No cards left to check";
                    return RedirectToAction(nameof(Index), new { id });
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index), new {id});
            }
        }

        // POST: BoxController/Add
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Check(CardsArrayStatusDto model)
        {
            CardCheckedResultDto cardsUpdated = await _cardService.BulkUpdateCardsStatusAsync(model);
            ViewData["Collection"] = cardsUpdated.Collection.Name;
            ViewData["CardsCount"] = cardsUpdated.CheckedCards;
            return RedirectToAction(nameof(Index), new {id = model.Id});
        }
    }
}
