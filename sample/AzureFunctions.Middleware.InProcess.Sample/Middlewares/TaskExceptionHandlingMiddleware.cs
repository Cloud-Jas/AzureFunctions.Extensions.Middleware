using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace AzureFunctions.Middleware.InProcess.Sample.Middlewares
{
    public class TaskExceptionHandlingMiddleware : NonHttpMiddlewareBase
    {
        private readonly ILogger<TaskExceptionHandlingMiddleware> _logger;
        public TaskExceptionHandlingMiddleware(ILogger<TaskExceptionHandlingMiddleware> logger)
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
