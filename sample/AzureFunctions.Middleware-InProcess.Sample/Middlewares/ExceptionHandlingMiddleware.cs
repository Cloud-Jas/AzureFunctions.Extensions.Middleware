using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AzureFunctions.Middleware.Sample.Middlewares
{
    public class ExceptionHandlingMiddleware : HttpMiddlewareBase
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.LogInformation($"{this.ExecutionContext.FunctionName} Request triggered");

                await this.Next.InvokeAsync(context);

                _logger.LogInformation($"{this.ExecutionContext.FunctionName} Request processed without any exceptions");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                context.Response.StatusCode = 400;
                
                await context.Response.WriteAsync($"{this.ExecutionContext.FunctionName} request failed, Please try again");

            }
        }
   }
}
