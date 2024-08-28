using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.MiddlewareV8.Sample.Middlewares
{
    public class AuthorizationMiddleware : HttpMiddlewareBase
   {
        private readonly ILogger<AuthorizationMiddleware> _logger;
        public AuthorizationMiddleware(ILogger<AuthorizationMiddleware> logger)
        {
            _logger = logger;
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation($"{this.FunctionExecutionContext.FunctionDefinition.Name} Authorization middleware triggered");

            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = 401;
                
                await context.Response.WriteAsync("Authorization header is not provided");
                
                return;
            }            

            await this.Next.InvokeAsync(context);
        }
   }
}
