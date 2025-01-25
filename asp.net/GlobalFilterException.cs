using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GlobalFilterTemplate.Filter
{
    public class GlobalFilterExceptions : IExceptionFilter
    {
        private readonly ILogger<GlobalFilterExceptions> _logger;

        public GlobalFilterExceptions(ILogger<GlobalFilterExceptions> logger)
        {
            this._logger = logger;
        }

        public void OnException(ExceptionContext ctx)
        {
            this._logger.LogError(ctx.Exception, "Unhandled Exception occurred");
            
            var stackTrace = new StackTrace(context.Exception, true);
            var frame = stackTrace.GetFrame(0);
            var fileName = frame?.GetFileName();
            var lineNumber = frame?.GetFileLineNumber();
            
            var statusCode = ctx.Exception switch
            {
                // Add more status responses here.
                BadRequestException => 400,
                KeyNotFoundException => 404,
                UnauthorizedAccessException => 401,
                ConflictException => 409,
                _ => 500
            };

            var response = new ErrorResponse
            {
                StatusCode = statusCode,
                Message = statusCode switch
                {
                    400 => ctx.Exception.Message,
                    401 => ctx.Exception.Message,
                    404 => ctx.Exception.Message,
                    409 => ctx.Exception.Message,
                    _ => ctx.Exception.Message
                },
                Details = statusCode == 500 ?
                    ctx.Exception.InnerException?.Message : null
                FileName = fileName,
                LineNumber = lineNumber
            };
            ctx.Result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };
            ctx.ExceptionHandled = true;
        }

        public class ErrorResponse
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
            public string? Details { get; set; }
            public string? FileName { get; set; }
            public int? LineNumber { get; set; }
        }

    }
}
