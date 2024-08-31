using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ModelsLeit.DTOs;
using ModelsLeit.ViewModels;
using ServicesLeit.Interfaces;
using ServicesLeit.Services;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ViewLeit.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;
        private readonly FileService _fileService;

        public ErrorController(ILogger<ErrorController> logger, FileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            /*
            if(HttpContext.Response.StatusCode > 400)
            {
                var error = new ErrorDto { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
                if (error.ShowRequestId)
                {
                    var data = $"{DateTime.Now},{error.RequestId}";
                    await _fileService.WriteToFileAsync("Error.log", data);
                }
                return View("Index", error);
            }
            */
            return Redirect("/");
        }

    }
}
