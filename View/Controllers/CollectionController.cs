using Microsoft.AspNetCore.Mvc;
using ModelsLeit.DTOs.Collection;
using ServicesLeit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ModelsLeit.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SharedLeit;

namespace ViewLeit.Controllers
{
    [Authorize]
    public class CollectionController: Controller
    {
        CollectionService _collectionService;
        public CollectionController(CollectionService collectionService)
        {
            _collectionService = collectionService;
        }

        // GET: CollectionController
        public async Task<ActionResult> Index() =>
            View(await _collectionService.ReadCollectionsAsync());

        // GET: CollectionController/Details/5
        public async Task<ActionResult> Details(long id) =>
            View(await _collectionService.ReadCollectionAsync(id));


        // GET: CollectionController/Create
        public ActionResult Create() => View();

        // POST: CollectionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CollectionAddDto model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("XX", "Not Valid!");
                return View(model);
            }

#warning catch!!
            try
            { 
               await _collectionService.AddCollectionAsync(model);
               return RedirectToAction(nameof(Index));
            }
            catch( Exception ex)
            {
                ModelState.AddModelError("xx", ex.Message);
                return View(model);
            }
        }

        // GET: CollectionController/Edit
        public async Task<ActionResult> Edit(long id) => 
            View(await _collectionService.ReadCollectionAsync(id));

        // POST: CollectionController/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CollectionDto model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("XX", "Not Valid!");
                return View(model);
            }

#warning catch!!
            try
             {
                await _collectionService.EditCollectionAsync(model);
                return RedirectToAction(nameof(Index));
             }
             catch (Exception ex)
             {
                 ModelState.AddModelError("xx", ex.Message);
                 return View(model);
             }
        }

        // GET: CollectionController/Delete
        //public async Task<ActionResult> Delete(long id) =>
        //    ViewLeit(await _collectionService.ReadCollectionAsync(id));

        // POST: CollectionController/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(CollectionDto model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("XX", "Not Valid!");
                return View(model);
            }

            try
            {
                await _collectionService.DeleteCollectionAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("xx", ex.Message);
                return View(model);
            }
        }

    }
}
