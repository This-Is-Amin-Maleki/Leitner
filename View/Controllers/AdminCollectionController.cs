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
    [Authorize(Roles = nameof(UserType.Admin))]
    public class AdminCollectionController: Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CollectionService _collectionService;
        public AdminCollectionController(
            UserManager<ApplicationUser> userManager,
            CollectionService collectionService)
        {
            _userManager = userManager;
            _collectionService = collectionService;
        }

        // GET: CollectionController
        [Route("AdminCollection/{userId?}")]
        public async Task<ActionResult> Index(long userId = 0)
        {
            List<CollectionListDto> output;
            if (userId is 0)
            {
                output = await _collectionService.ReadAllAsync();
                return View(output);
            }
            output = await _collectionService.ReadAllAsync(userId);
            return View(output);
        }

        // GET: CollectionController/Details/5
        public async Task<ActionResult> Details(long id) =>
            View(await _collectionService.ReadCollectionAsync(id));

        // GET: CollectionController/Edit
        public async Task<ActionResult> Edit(long id) =>

            View(await _collectionService.ReadCollectionDataAsync(id));
            

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
