using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using TradeMap.Service.DTO.Response;
using TradeMap.Service.Exceptions;

namespace LocationBaseAdvertising.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error-local-development")]
        public IActionResult ErrorLocalDevelopment(
            [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.EnvironmentName != "Development")
            {
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments.");
            }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            return Problem(
                detail: context.Error.StackTrace,
                title: context.Error.Message);
        }

        [Route("/error")]
        public ErrorResponse Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context.Error; // Your exception
            var code = 500; // Internal Server Error by default
            string error = (exception as CrudException)?.Error;
            if (error == null) error = "";
            if (exception is KeyNotFoundException) code = 404; // Not Found
            else if (exception is UnauthorizedAccessException) code = 401; // Unauthorized
            else if (exception is FirebaseException) code = 403;
            else if (exception is NotImplementedException) code = 404;
            else if (exception is null) code = 400; // Bad Request
            else if (exception is CrudException) code = (int)(exception as CrudException).Status; // Bad Request

            Response.StatusCode = code; // You can use HttpStatusCode enum instead
            return new ErrorResponse(exception, error); // Your error model
        }
    }
}