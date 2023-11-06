using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using AzureFunctions.Extensions.Middleware;
using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using static AzureFunctions.Middleware.Sample.Middlewares.ServiceBusMiddleware;

namespace AzureFunctions.Middleware.Sample
{
    public class FxServiceBusTrigger
    {
        private readonly INonHttpMiddlewareBuilder _middlewareBuilder;
        public FxServiceBusTrigger(INonHttpMiddlewareBuilder middlewareBuilder)
        {
            _middlewareBuilder = middlewareBuilder;
        }
        [FunctionName("FxServiceBusTrigger")]
        public async Task Run([ServiceBusTrigger("sbq-azfunc-middleware", Connection = "SbConnString")] ServiceBusReceivedMessage myQueueItem,ServiceBusMessageActions messageActions, ILogger log, ExecutionContext context)
        {
            await _middlewareBuilder.ExecuteAsync(new NonHttpMiddleware(async () =>
            {             
                log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem.Body}");                
                await Task.FromResult(true);
            }, context, new ServiceBusData
            {
                Message = myQueueItem,
                MessageActions = messageActions
            }));
        }
    }
}
