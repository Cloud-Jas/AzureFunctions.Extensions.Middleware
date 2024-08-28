using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AzureFunctions.Middleware.InProcess.Sample.Middlewares
{
   public class TimerDataAccessMiddleware : NonHttpMiddlewareBase
   {
      private readonly ILogger<TimerDataAccessMiddleware> _logger;
      public TimerDataAccessMiddleware(ILogger<TimerDataAccessMiddleware> logger)
      {
         _logger = logger;
      }
      public override async Task InvokeAsync()
      {
         if (this.ExecutionContext.FunctionName.Equals("TimerTrigger"))
         {
            try
            {
               var timerData = this.Data as TimerInfo;
               _logger.LogInformation($"{this.ExecutionContext.FunctionName} Request triggered");
               await this.Next.InvokeAsync();
               _logger.LogInformation($"{this.ExecutionContext.FunctionName} Request processed without any exceptions");
            }
            catch (Exception ex)
            {
               _logger.LogError(ex.Message);
            }
         }
         await this.Next.InvokeAsync();
      }
   }
}
