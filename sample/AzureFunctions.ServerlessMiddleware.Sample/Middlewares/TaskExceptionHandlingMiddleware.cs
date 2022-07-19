using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AzureFunctions.Middleware.Sample.Middlewares
{
    public class TaskExceptionHandlingMiddleware : TaskMiddleware
    {
        private readonly ILogger _logger;
        public TaskExceptionHandlingMiddleware(ILogger logger)
        {
            _logger = logger;
        }     
      public override async Task InvokeAsync()
      {
         try
         {
            _logger.LogInformation($"{this.ExecutionContext.FunctionName} Request triggered");            

            await this.Next.InvokeAsync();

            _logger.LogInformation($"{this.ExecutionContext.FunctionName} Request processed without any exceptions");
         }
         catch (Exception ex)
         {
            _logger.LogError(ex.Message);           
         }
      }
   }
}
