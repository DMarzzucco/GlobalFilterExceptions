using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace SAAS_task.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "An unhandled exception ocurred.");
            var statusCode = context.Exception is KeyNotFoundException ? 404 : 500;

            var response = new ErrorResponse
            {
                StatusCode = statusCode,
                Message = statusCode == 500
                    ? "Internal Error Server, Please try again later"
                    : "Resource not found",

                Details = statusCode == 500 ? null : context.Exception.Message
            };
            context.Result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };
            context.ExceptionHandled = true;
        }
    }
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? Details { get; set; }
    }

}
