using System;
using System.Threading.Tasks;
using AzureFunctions.Extensions.Middleware;
using AzureFunctions.Extensions.Middleware.Abstractions;
using AzureFunctions.Middleware.InProcess.Sample.Middlewares;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Middleware.InProcess.Sample
{
   public class TimerTrigger
   {
      private readonly ILogger<TimerTrigger> _logger;
      private readonly INonHttpMiddlewareBuilder _middlewareBuilder;

      public TimerTrigger(ILogger<TimerTrigger> log, INonHttpMiddlewareBuilder middlewareBuilder)
      {
         _logger = log;
         _middlewareBuilder = middlewareBuilder;
      }
      [FunctionName("TimerTrigger")]
      public async Task Run([TimerTrigger("*/60 * * * * *")] TimerInfo myTimer, ILogger log,ExecutionContext context)
      {

         await _middlewareBuilder.ExecuteAsync(new NonHttpMiddleware(async () =>
            {
               _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
               await Task.FromResult(true);
            },context,myTimer));
      }
   }
}
