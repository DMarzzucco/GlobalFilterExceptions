public class GlobalFilterExceptions(ILogger<GlobalFilterExceptions> logger) : IExceptionFilter
{
        private readonly ILogger<GlobalFilterExceptions> _logger = logger;

        public void OnException(ExceptionContext context)
        {
            this._logger.LogError(ctx.Exception, "Unhandled Exception occurred");
            
            var statusCode = context.Exception switch
            {
                // Add more status responses here.
                BadRequestExceptions => 400,
                UnauthorizedAccessException => 401,
                ForbiddenExceptions => 403,
                KeyNotFoundException => 404,
                ConflictExceptions => 409,
                TooManyRequestsException => 429,
                _ => 500
            };
            var message = context.Exception switch
            {
                // Add more status responses here.    
                BadRequestExceptions ex => ex.Message,
                UnauthorizedAccessException ex => ex.Message,
                ForbiddenExceptions ex => ex.Message,
                KeyNotFoundException ex => ex.Message,
                ConflictExceptions ex => ex.Message,
                TooManyRequestsException ex => ex.Message,
                _ => context.Exception.Message
            };

            var response = new ErrorResponse
            {
                StatusCode = statusCode,
                Message = message,
                Details = statusCode == 500 ?
                    context.Exception.InnerException?.Message : null
            };

            context.Result = new ObjectResult(response)
            {
                StatusCode = statusCode
            };
            context.ExceptionHandled = true;
        }

        public class ErrorResponse
        {
            public int StatusCode { get; set; }
            public required string Message { get; set; }
            public string? Details { get; set; }
        }
    }
}

