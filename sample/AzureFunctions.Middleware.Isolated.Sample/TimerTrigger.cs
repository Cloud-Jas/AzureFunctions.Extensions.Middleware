using System;
using AzureFunctions.Extensions.Middleware;
using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Middleware.Isolated.Sample
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

        [Function("TimerTrigger")]
        public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, FunctionContext context)
        {

            await _middlewareBuilder.ExecuteAsync(new NonHttpMiddleware(async () =>
            {
                _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

                if (myTimer.ScheduleStatus is not null)
                {
                    _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
                }
                await Task.FromResult(true);
            }, context, myTimer));


        }
    }
}
