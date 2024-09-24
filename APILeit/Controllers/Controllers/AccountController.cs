using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace APILeit.Controllers
{

    [Authorize]
    [ApiController]
    public class AccountController : ControllerBase
    {
    }
}