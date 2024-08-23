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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CollectionService _collectionService;
        public CollectionController(
            UserManager<ApplicationUser> userManager,
            CollectionService collectionService)
        {
            _userManager = userManager;
            _collectionService = collectionService;
        }

        // GET: CollectionController
        public async Task<ActionResult> Index()
        {
            var userId = long.Parse(_userManager.GetUserId(User)!);
            var output = /*User.IsInRole(nameof(UserType.Admin)) ?
                await _collectionService.ReadCollectionsAsync() :*/
                await _collectionService.ReadAllAsync(userId);
            return View(output);
        }

        // GET: CollectionController/Details/5
        public async Task<ActionResult> Details(long id)
        {
            var userId = long.Parse(_userManager.GetUserId(User)!);
            return View(await _collectionService.ReadUserCollectionAsync(id, userId));
        }

        // GET: CollectionController/Create
        public ActionResult Create() => View();

        // POST: CollectionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CollectionCreationDto model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("XX", "Not Valid!");
                return View(model);
            }
            model.UserId =long.Parse(_userManager.GetUserId(User)!);
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
        public async Task<ActionResult> Edit(long id)
        {
            var userId = long.Parse(_userManager.GetUserId(User)!);

            return View(await _collectionService.ReadUserCollectionAsync(id, userId));
        }
            

        // POST: CollectionController/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CollectionModifyDto model)
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
                await _collectionService.EditCollectionLimitedAsync(model);
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
        public async Task<ActionResult> Delete(CollectionModifyDto model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("XX", "Not Valid!");
                return View(model);
            }
            model.UserId = long.Parse(_userManager.GetUserId(User)!);


            try
            {
                await _collectionService.DeleteCollectionLimitedAsync(model);
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
