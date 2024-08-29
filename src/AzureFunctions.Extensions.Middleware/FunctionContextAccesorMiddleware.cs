using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Text;
using AzureFunctions.Extensions.Middleware.Abstractions;

namespace AzureFunctions.Extensions.Middleware
{
    public class FunctionContextAccessorMiddleware : IFunctionsWorkerMiddleware
    {
        private IFunctionContextAccessor FunctionContextAccessor { get; }

        public FunctionContextAccessorMiddleware(IFunctionContextAccessor accessor)
        {
            FunctionContextAccessor = accessor;
        }

        public Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            if (FunctionContextAccessor.FunctionContext != null)
            {
                // This should never happen because the context should be localized to the current Task chain.
                // But if it does happen (perhaps the implementation is bugged), then we need to know immediately so it can be fixed.
                throw new InvalidOperationException($"Unable to initalize {nameof(IFunctionContextAccessor)}: context has already been initialized.");
            }

            FunctionContextAccessor.FunctionContext = context;

            return next(context);
        }
    }
}
