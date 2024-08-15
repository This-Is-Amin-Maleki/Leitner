using Microsoft.AspNetCore.Mvc;
using ModelsLeit.DTOs.Card;
using ModelsLeit.Entities;
using ServicesLeit.Services;
using SharedLeit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ViewLeit.Controllers
{
    [Authorize]
    public class CardController: Controller
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
        public async Task<ActionResult> Index(long id)
        {
            var userId = long.Parse(_userManager.GetUserId(User)!);
            List<CardDto> model = await _cardService.ReadCardsLimitedAsync(id, userId);
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
            ViewData["noEdit"] = status is CollectionStatus.Published or CollectionStatus.Submit;
            ViewData["CollectionId"] = id;
            return View(model);
        }

        // GET: CardController/Details/5
        public async Task<ActionResult> Details(long id)
        {
            var userId = long.Parse(_userManager.GetUserId(User)!);
            return View(await _cardService.ReadCardLimitedAsync(id, userId));
        }


        // GET: CardController/Create
        public ActionResult Create(long id)
        {
            ViewData["CollectionId"] = id;
            return View();
        }

        // POST: CardController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CardDto model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("XX", "Not Valid!");
                return View(model);
            }

            model.UserId = long.Parse(_userManager.GetUserId(User)!);
#warning catch!!
            try
            { 
               await _cardService.AddCardAsync(model);
               return RedirectToAction(nameof(Index), new { id = model.Collection.Id });
            }
            catch( Exception ex)
            {
                ModelState.AddModelError("xx", ex.Message);
                return View(model);
            }
        }

        // GET: CardController/Create
        [Route("Card/Upload/{id}")]
        public ActionResult Upload(long id)
        {
            ViewData["CollectionId"] = id;
            return View();
        }

        // POST: CardController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Card/Upload/{id}")]
        public async Task<ActionResult> Upload(long id, IFormFile sheet)
        {
            ViewData["CollectionId"] = id;
            if (sheet == null || sheet.Length == 0)
            {
                ViewData["Error"] = "No file uploaded!";
                return View(id);
            }

            int processed = 0;
            var extension = Path.GetExtension(sheet.FileName).ToLower();
            var tempFilePath = Path.GetTempFileName() + extension;

#warning catch!!
            try
            {
                // Upload to temp
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await sheet.CopyToAsync(stream);
                }
                // Processing data
                List<Card> list = _fileService.ReadCardsFromExcelFile(tempFilePath, id, 1000);

                //set userId
               var userId = long.Parse(_userManager.GetUserId(User)!);
                list.ForEach(x => x.UserId = userId);

                await _cardService.BulkAddCardsAsync(list);
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
            }
            finally
            {
                if (System.IO.File.Exists(tempFilePath))
                {
                    System.IO.File.Delete(tempFilePath);
                }
            }
            return RedirectToAction(nameof(Index), new { id });
        }

        // GET: CardController/Edit
        public async Task<ActionResult> Edit(long id)
        {
            var userId = long.Parse(_userManager.GetUserId(User)!);
            return View(await _cardService.ReadCardLimitedAsync(id, userId));
        } 

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
            model.UserId = long.Parse(_userManager.GetUserId(User)!);

#warning catch!!
            try
             {
                await _cardService.UpdateCardLimitedAsync(model);
                return RedirectToAction(nameof(Index), new { id = model.Collection.Id });
            }
             catch (Exception ex)
             {
                 ModelState.AddModelError("xx", ex.Message);
                 return View(model);
             }
        }

        // GET: CardController/Delete
        //public async Task<ActionResult> Delete(long id) =>
        //    ViewLeit(await _cardService.ReadCardAsync(id));

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
            model.UserId = long.Parse(_userManager.GetUserId(User)!);

            try
            {
                await _cardService.DeleteCardLimitedAsync(model);
                return RedirectToAction(nameof(Index), new { id = model.Collection.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("xx", ex.Message);
                return View(model);
            }
        }

    }
}
