using Microsoft.AspNetCore.Mvc;
using ModelsLeit.DTOs.Card;
using ModelsLeit.Entities;
using ServicesLeit.Services;
using SharedLeit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        public async Task<ActionResult> TickAll(long id)
        {
            try
            {
                await _cardService.TickAllCardsAsync(id);
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction(nameof(Index), new { id });
        }

        [HttpGet]
        public async Task<ActionResult> Check(long id, CardStatus? status, int skip = 0)
        {
            CardCheckDto output;
            try
            {
                output = await _cardService.ReadCardCheck(id, status, skip);
                output.Skip = skip + 1;
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index), new { id });
            }
            return View(output);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CheckUp(CardCheckStatusDto model)
        {
            string content;
            try
            {
                var output = await _cardService.UpdateStatusAndReadNextCardCheck(model);
                if (output.Id is not 0)
                {
                    return Json(output);
                }

                content = Url.Action(
                nameof(Index),
                new
                {
                    id = model.CollectionId,
                    state = output.Status
                })!;
                return Json(content);

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            content = Url.Action(
            nameof(Index),
            new
            {
                id = model.CollectionId,
            })!;
            return Json(content);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CheckNext(CardCheckStatusDto model)
        {
            CardCheckDto output;
            try
            {
                output = await _cardService.ReadCardCheck(model.Id, model.Status, model.Skip);
                output.Skip = model.Skip + 1;
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                var content = Url.Action(
                nameof(Index),
                new
                {
                    id = model.CollectionId,
                })!;
                return Json(content);
            }
            return Json(output);
        }

    }
}
