using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ModelsLeit.DTOs.User;
using SharedLeit;
using ServicesLeit.Services;
using ModelsLeit.ViewModels.User;
using ServicesLeit.Interfaces;
using ServicesLeit.Extensions;

namespace APILeit.Controllers
{
    [AllowAnonymous]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserService _userService;
        private readonly ITokenService _tokenService;

        public UserController(IHttpContextAccessor httpContext,
            UserService userService,
            ITokenService token)
        {
            _httpContext = httpContext;
            _userService = userService;
            _tokenService = token;
        }
    }
}