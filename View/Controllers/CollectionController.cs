using DataAccess.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Models.DTOs;
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


    }
}
