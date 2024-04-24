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
        public CollectionController(CollectionService collectionService, RawDbService raw)
        {
            _collectionService = collectionService;
        }

    }
}
