using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AzureFunctions.Middleware.Sample.Middlewares
{
    public class ExceptionHandlingMiddleware : ServerlessMiddleware
    {
        private readonly ILogger _logger;
        public ExceptionHandlingMiddleware(ILogger logger)
        {
            _logger = logger;
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.LogInformation("Request triggered");

                await this.Next.InvokeAsync(context);

                _logger.LogInformation("Request processed without any exceptions");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                context.Response.StatusCode = 400;
                
                await context.Response.WriteAsync("Http Trigger request failed, Please try again");

            }
        }

      public override async Task InvokeAsync(ExecutionContext context)
      {
         try
         {
            _logger.LogInformation("Request triggered");

            await this.Next.InvokeAsync(context);

            _logger.LogInformation("Request processed without any exceptions");
         }
         catch (Exception ex)
         {
            _logger.LogError(ex.Message);           
         }
      }
   }
}
