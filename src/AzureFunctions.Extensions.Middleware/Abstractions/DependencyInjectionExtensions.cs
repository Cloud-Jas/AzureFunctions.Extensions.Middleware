using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunctions.Extensions.Middleware.Abstractions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddFunctionContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IFunctionContextAccessor, FunctionContextAccessor>();           
            return services;
        }
        public static IFunctionsWorkerApplicationBuilder UseFunctionContextAccessor(this IFunctionsWorkerApplicationBuilder builder)
        {
            builder.UseMiddleware<FunctionContextAccessorMiddleware>();
            return builder;
        }
    }
}
