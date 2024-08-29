using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AzureFunctions.Middleware.Isolated.Sample.Middlewares
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
         if (this.FunctionExecutionContext.FunctionDefinition.Name.Equals("TimerTrigger"))
         {
            try
            {
               var timerData = this.Data as TimerInfo;
               _logger.LogInformation($"{this.FunctionExecutionContext.FunctionDefinition.Name} Request triggered");
               await this.Next.InvokeAsync();
               _logger.LogInformation($"{this.FunctionExecutionContext.FunctionDefinition.Name} Request processed without any exceptions");
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
