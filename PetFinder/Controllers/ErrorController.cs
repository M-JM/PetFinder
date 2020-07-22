using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PetFinder.Models;

namespace PetFinder.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;
        
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
       
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            var ErrorMessages = new ErrorMessage();
            switch (statusCode)
            {
                case 404:
                    ErrorMessages.TheErrorMessage = "sorry this page does not exist";
                    ErrorMessages.Path = statusCodeResult.OriginalPath;
                    ErrorMessages.Qs = statusCodeResult.OriginalQueryString;
                    _logger.LogWarning($"404 Error Occured. Path = {statusCodeResult.OriginalPath} " +
                        $"and QueryString = {statusCodeResult.OriginalQueryString}");

                    break;
            }

            return View("NotFound", ErrorMessages);
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("Error")]
        public IActionResult Error()
        {
            var exceptiondetailsModel = new ExceptionDetail();
            var exceptiondetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();


            exceptiondetailsModel.ExceptionPath = exceptiondetails.Path;
            exceptiondetailsModel.ExceptionMessage = exceptiondetails.Error.Message;
            exceptiondetailsModel.Stacktrace = exceptiondetails.Error.StackTrace;

            _logger.LogError($"The path {exceptiondetails.Path} " +
               $"threw an exception {exceptiondetails.Error}");


            return View(exceptiondetailsModel);

        }

    }
}