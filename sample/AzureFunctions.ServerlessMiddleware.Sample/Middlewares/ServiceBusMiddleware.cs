using Azure.Messaging.ServiceBus;
using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AzureFunctions.Middleware.Sample.Middlewares
{
    public class ServiceBusMiddleware : NonHttpMiddlewareBase
    {
        private readonly ILogger<ServiceBusMiddleware> _logger;        

        public ServiceBusMiddleware(ILogger<ServiceBusMiddleware> logger)
        {
            _logger = logger;            
        }
        public class ServiceBusData
        {
            public ServiceBusReceivedMessage Message { get; set; }
            public ServiceBusMessageActions MessageActions { get; set; }

        }
        public override async Task InvokeAsync()
        {
            if (this.ExecutionContext.FunctionName.EndsWith("ServiceBusTrigger"))
            {
                try
                {                    
                    var serviceBusData = this.Data as ServiceBusData;

                    if (serviceBusData != null)
                    {
                        _logger.LogInformation($"{this.ExecutionContext.FunctionName} Request triggered");

                        if (this.ExecutionContext.FunctionName.Equals("FxServiceBusTrigger")) // consider this is the disabled function
                        {
                            long sequenceNumber = serviceBusData.Message.SequenceNumber; // add sequenceNumber to cache for later retrieval of deferred messages
                            await serviceBusData.MessageActions.DeferMessageAsync(serviceBusData.Message);
                        }
                        else
                        {
                            await this.Next.InvokeAsync();
                            await serviceBusData.MessageActions.CompleteMessageAsync(serviceBusData.Message);

                        }
                    }
                    else
                    {
                        await this.Next.InvokeAsync();
                    }

                    _logger.LogInformation($"{this.ExecutionContext.FunctionName} Request processed without any exceptions");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
            }
            else
            {
                await this.Next.InvokeAsync();
            }
        }
    }
}
