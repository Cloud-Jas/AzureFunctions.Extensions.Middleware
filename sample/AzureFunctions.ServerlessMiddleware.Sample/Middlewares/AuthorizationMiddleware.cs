using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AzureFunctions.Middleware.Sample.Middlewares
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
            _logger.LogInformation($"{this.ExecutionContext.FunctionName} Authorization middleware triggered");

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
