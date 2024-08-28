using AzureFunctions.Extensions.Middleware.Abstractions;
using AzureFunctions.Extensions.Middleware.Infrastructure;
using AzureFunctions.Middleware.Isolated.Sample.Middlewares;
using Functions.Worker.ContextAccessor;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(applicationBuilder =>
    {
        applicationBuilder.UseFunctionContextAccessor();
        applicationBuilder.UseNewtonsoftJson();
    })
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddHttpContextAccessor();
        services.AddFunctionContextAccessor();
        services.AddTransient<IHttpMiddlewareBuilder, HttpMiddlewareBuilder>((serviceProvider) =>
        {
            var funcBuilder = new HttpMiddlewareBuilder(serviceProvider.GetRequiredService<IHttpContextAccessor>(), serviceProvider.GetRequiredService<IFunctionContextAccessor>());
            funcBuilder.Use(new ExceptionHandlingMiddleware(serviceProvider.GetService<ILogger<ExceptionHandlingMiddleware>>()));
            funcBuilder.UseWhen(ctx =>
            {
                if (ctx != null && ctx.Request.Path.StartsWithSegments("/api/Authorize"))
                {
                    return true;
                }
                return false;
            }, new AuthorizationMiddleware(serviceProvider.GetService<ILogger<AuthorizationMiddleware>>()));
            return funcBuilder;
        });
    })
    .Build();

host.Run();
