using AzureFunctions.Extensions.Middleware.Abstractions;
using AzureFunctions.Extensions.Middleware.Infrastructure;
using AzureFunctions.Middleware.Isolated.Sample.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
       services.AddApplicationInsightsTelemetryWorkerService();
       services.ConfigureFunctionsApplicationInsights();
       services.AddHttpContextAccessor();
       services.AddTransient<IHttpMiddlewareBuilder, HttpMiddlewareBuilder>((serviceProvider) =>
       {
          var funcBuilder = new HttpMiddlewareBuilder(serviceProvider.GetRequiredService<IHttpContextAccessor>());
          funcBuilder.Use(new ExceptionHandlingMiddleware(serviceProvider.GetService<ILogger<ExceptionHandlingMiddleware>>()));
          funcBuilder.UseWhen(ctx => ctx != null && ctx.Request.Path.StartsWithSegments("/api/Authorize"),
               new AuthorizationMiddleware(serviceProvider.GetService<ILogger<AuthorizationMiddleware>>()));
          return funcBuilder;
       });
    })
    .Build();

host.Run();
