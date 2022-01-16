using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AzureFunctions.Middleware.Sample.Middlewares
{
    public class AuthorizationMiddleware : ServerlessMiddleware
    {
        private readonly ILogger _logger;
        public AuthorizationMiddleware(ILogger logger)
        {
            _logger = logger;
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("Authorization middleware triggered");

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
