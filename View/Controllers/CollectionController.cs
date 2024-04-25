using DataAccess.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Models.ViewModels;
using Services.Services;
using Shared;

namespace View.Controllers
{
    public class CollectionController: Controller
    {
        CollectionService _collectionService;
        public CollectionController(CollectionService collectionService)
        {
            _collectionService = collectionService;
        }

        // GET: CollectionController
        public async Task<ActionResult> Index() =>
            View(await _collectionService.GetCollectionsAsync());
            View(await _collectionService.ReadCollectionsAsync());

        // GET: CollectionController/Details/5
        public async Task<ActionResult> Details(long id) =>
            View(await _collectionService.ReadCollectionAsync(id));


        // GET: CollectionController/Create
        public ActionResult Create() => View();

        // POST: CollectionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CollectionViewModel model)
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

    }
}
