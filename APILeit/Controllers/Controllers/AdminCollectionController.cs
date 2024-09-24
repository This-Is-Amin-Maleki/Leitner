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

        // GET: CollectionController
        [HttpGet]
        [Route("api/[controller]/[action]/{userId}")]
        public async Task<ActionResult> GetAll(long userId = 0) =>
            Ok(userId is 0 ?
                await _collectionService.ReadAllAsync() :
                await _collectionService.ReadAllAsync(userId));

        // GET: CollectionController/Details/5
        [HttpGet]
        [Route("api/[controller]/[action]/{id}")]
        public async Task<ActionResult> GetDetails(long id) =>
            Ok(await _collectionService.ReadCollectionAsync(id));
    }
}
