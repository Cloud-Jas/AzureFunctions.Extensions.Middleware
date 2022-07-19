using System;
using System.Threading.Tasks;
using AzureFunctions.Extensions.Middleware;
using AzureFunctions.Extensions.Middleware.Abstractions;
using AzureFunctions.Middleware.Sample.Middlewares;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Middleware.Sample
{
   public class TimerTrigger
   {
      private readonly ILogger<TimerTrigger> _logger;
      private readonly ITaskMiddlewareBuilder _middlewareBuilder;

      public TimerTrigger(ILogger<TimerTrigger> log, ITaskMiddlewareBuilder middlewareBuilder)
      {
         _logger = log;
         _middlewareBuilder = middlewareBuilder;
      }
      [FunctionName("TimerTrigger")]
      public async Task Run([TimerTrigger("*/10 * * * * *")] TimerInfo myTimer, ILogger log,ExecutionContext context)
      {

         await _middlewareBuilder.ExecuteAsync(new NonHttpMiddleware(async () =>
            {
               log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
               await Task.FromResult("test");
            },context,myTimer));
      }
   }
}
