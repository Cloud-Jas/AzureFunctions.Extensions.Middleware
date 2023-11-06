using AzureFunctions.Extensions.Middleware.Abstractions;
using AzureFunctions.Extensions.Middleware.Infrastructure;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AzureFunctions.Middleware.Sample.Middlewares;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(AzureFunctions.Middleware.Sample.Startup))]

namespace AzureFunctions.Middleware.Sample
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddLogging();
            builder.Services.AddTransient<IHttpMiddlewareBuilder, HttpMiddlewareBuilder>((serviceProvider) =>
            {
                var funcBuilder = new HttpMiddlewareBuilder(serviceProvider.GetRequiredService<IHttpContextAccessor>());
                funcBuilder.Use(new ExceptionHandlingMiddleware(serviceProvider.GetService<ILogger<ExceptionHandlingMiddleware>>()));
                funcBuilder.UseWhen(ctx => ctx != null && ctx.Request.Path.StartsWithSegments("/api/Authorize"),
                    new AuthorizationMiddleware(serviceProvider.GetService<ILogger<AuthorizationMiddleware>>()));
                return funcBuilder;
            });                        
            
            builder.Services.AddTransient<INonHttpMiddlewareBuilder, NonHttpMiddlewareBuilder>((serviceProvider) =>
         {
             var funcBuilder = new NonHttpMiddlewareBuilder();
             funcBuilder.Use(new TaskExceptionHandlingMiddleware(serviceProvider.GetService<ILogger<TaskExceptionHandlingMiddleware>>()));
             funcBuilder.Use(new ServiceBusMiddleware(serviceProvider.GetService<ILogger<ServiceBusMiddleware>>()));
             funcBuilder.Use(new TimerDataAccessMiddleware(serviceProvider.GetService<ILogger<TimerDataAccessMiddleware>>()));
             return funcBuilder;
         });
        }
    }
}
