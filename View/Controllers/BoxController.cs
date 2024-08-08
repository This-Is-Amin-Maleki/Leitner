using Microsoft.AspNetCore.Mvc;
using ModelsLeit.DTOs.Card;
using ModelsLeit.DTOs.Box;
using ModelsLeit.DTOs.Collection;
using Newtonsoft.Json;
using Services.Services;
using ServicesLeit.Services;
using ModelsLeit.DTOs.Container;

namespace ViewLeit.Controllers
{
    public class BoxController : Controller
    {
        BoxService _boxService;
        CollectionService _collectionService;
        public BoxController(BoxService boxService,CollectionService collectionService)
        {
            _boxService = boxService;
            _collectionService = collectionService;
        }

        // GET: BoxController
        public async Task<ActionResult> Index()
        {
            List<BoxMiniDto> model = await _boxService.ReadAllAsync();
            return View(model);
        }

        // GET: BoxController/List/id
        public async Task<ActionResult> List(long id)
        {
            List<BoxMiniDto> model = await _boxService.ReadByCollectionAsync(id);
            if (model.Count is 0)
            {
                var collection = await _collectionService.GetCollectionNameAndStatusAsync(id);
                if (collection is null)
                {
                    return RedirectToAction(nameof(CollectionController.Index), "Collection");
                }
                ViewData["CollectionName"] = collection.Name;
            }
            else
            {
                ViewData["CollectionName"] = model.First().Collection.Name;
            }
            return View(model);
        }

        // GET: BoxController/Add
        public async Task<ActionResult> Add(long id)
        {
            var model = await _collectionService.CheckExistCollectionAsync(id);
            if(model is null)
            {
                ModelState.AddModelError("XX", "Not Allow!");
                return RedirectToAction(nameof(CollectionController.Index),"Collection");
            }
            return View(model);
        }

        // POST: BoxController/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(BoxAddDto model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("XX", "Not Valid!");
                return View(model);
            }
            var collection = await _collectionService.CheckExistCollectionAsync(model.CollectionId);
            if(collection is null)
            {
                ModelState.AddModelError("XX", "Not Allow!");
                return RedirectToAction(nameof(CollectionController.Index),"Collection");
            }
            collection.CardPerDay= model.CardPerDay;

#warning catch!!
            try
            { 
               await _boxService.AddAsync(collection);
               return RedirectToAction(nameof(Index));
            }
            catch( Exception ex)
            {
                ModelState.AddModelError("xx", ex.Message);
                return View(collection);
            }
        }

        // POST: BoxController/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(long id)
        {
                await _boxService.DeleteAsync(id);
            try
            {
                return Content("OK");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        // GET: BoxController/Add
        public async Task<ActionResult> Review(long id)
        {
            try
            {
            var model = await _boxService.ReviewAsync(id);
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: BoxController/Add
        public async Task<ActionResult> Study(long id)
        {
            try
            {
                var model = await _boxService.StudyAsync(id);
                if(/*model is null || */model.Approved.Count is 0 && model.Rejected.Count is 0)
                {
                    return RedirectToAction(nameof(Index));
                }
                return View(model);
            }
            catch(Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: BoxController/Add
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Study(ContainerStudiedDto model)
        {

            /*
            if (model.Approved != null) {
                foreach (var idc in model.Approved)
                {
                    model.Approved.Add(new Card() { Id = idc });
                }
            }
            if (model.Rejected != null)
            {
                foreach (var idc in model.Rejected)
                {
                    model.Rejected.Add(new Card() { Id = idc });
                }
            }*/
            //ContainerStudyDto j = JsonConvert.DeserializeObject<ContainerStudyDto>(json);
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("XX", "Not Valid!");
                return RejectViewForStudy(model);
            }
            #warning catch!!
            try
            {
                await _boxService.UpdateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("xx", ex.Message);
                return RejectViewForStudy(model);
            }
        }

        // GET: BoxController/Add
        [Route("Box/Next/{id}/{num}")]
        public async Task<ActionResult> Next(long id,int num)
        {
            
            CardDto card = new();
            try
            {
                card = await _boxService.ReadNextCardAsync(id,num);
            }
            catch (Exception ex)
            {
                card.Id = 0;
                card.Description = ex.Message;
            }
            var json = JsonConvert.SerializeObject(card);
            return Content(json);
        }
        ///////////////////////////////////////////

        private ActionResult RejectViewForStudy(ContainerStudiedDto model) =>
            View(_boxService.StudyFailAsync(model));
        
    }
}