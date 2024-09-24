using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelsLeit.DTOs.Collection;
using ServicesLeit.Services;
using SharedLeit;

namespace APILeit.Controllers
{
    [Authorize(Roles = nameof(UserType.Admin))]
    [ApiController]
    public class AdminCollectionController : ControllerBase
    {
        private readonly CollectionService _collectionService;
        public AdminCollectionController(
            CollectionService collectionService)
        {
            _collectionService = collectionService;
        }
    }
}
