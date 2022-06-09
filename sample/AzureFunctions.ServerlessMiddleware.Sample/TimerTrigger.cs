using System;
using System.Threading.Tasks;
using AzureFunctions.Extensions.Middleware;
using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Middleware.Sample
{
   public class TimerTrigger
   {
      private readonly ILogger<TimerTrigger> _logger;
      private readonly IMiddlewareBuilder _middlewareBuilder;

      public TimerTrigger(ILogger<TimerTrigger> log, IMiddlewareBuilder middlewareBuilder)
      {
         _logger = log;
         _middlewareBuilder = middlewareBuilder;
      }
      [FunctionName("TimerTrigger")]
       public async Task Run([TimerTrigger("*/10 * * * * *")] TimerInfo myTimer, ILogger log)
      {
         await _middlewareBuilder.ExecuteAsync(new TaskMiddleware(async (httpContext) =>
         {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await Task.FromResult("test");
         }));
      }
   }
}
