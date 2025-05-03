using InventoryManagementSystem.Service.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace InventoryManagementSystem.API.Filters
{
    public class ApiExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ApiExceptionFilter> _logger;

        public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            _logger.LogError(exception, "Unhandled exception occurred");

            ObjectResult result;
            
            switch(exception)
            {
                case NotFoundException notFoundException:
                    result = new NotFoundObjectResult(new { message = exception.Message });
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                
                case UnauthorizedAccessException unauthorizedException:
                    result = new UnauthorizedObjectResult(new { message = exception.Message });
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;

                case InvalidOperationException invalidOpException:
                    result = new BadRequestObjectResult(new { message = exception.Message });
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                default:
                    result = new ObjectResult(new { message = "An internal server error occurred" })
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            context.Result = result;
            context.ExceptionHandled = true;
        }
    }
}
