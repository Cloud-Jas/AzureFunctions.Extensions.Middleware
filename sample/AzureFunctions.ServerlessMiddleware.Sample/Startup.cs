using AzureFunctions.Extensions.Middleware.Abstractions;
using AzureFunctions.Extensions.Middleware.Infrastructure;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AzureFunctions.Middleware.Sample.Middlewares;

[assembly: FunctionsStartup(typeof(AzureFunctions.Middleware.Sample.Startup))]

namespace AzureFunctions.Middleware.Sample
{
   internal class Startup : FunctionsStartup
   {
      public override void Configure(IFunctionsHostBuilder builder)
      {
         builder.Services.AddHttpContextAccessor();         
         builder.Services.AddLogging();
         builder.Services.AddTransient<IMiddlewareBuilder, MiddlewareBuilder>((serviceProvider) =>
         {
            var funcBuilder = new MiddlewareBuilder(serviceProvider.GetRequiredService<IHttpContextAccessor>());
            funcBuilder.Use(new ExceptionHandlingMiddleware(new LoggerFactory().CreateLogger(nameof(ExceptionHandlingMiddleware))));
            funcBuilder.UseWhen(ctx => ctx != null && ctx.Request.Path.StartsWithSegments("/api/Authorize"),
                   new AuthorizationMiddleware(new LoggerFactory().CreateLogger(nameof(AuthorizationMiddleware))));
            return funcBuilder;
         });
         builder.Services.AddTransient<ITaskMiddlewareBuilder, TaskMiddlewareBuilder>((serviceProvider) =>
         {
            var funcBuilder = new TaskMiddlewareBuilder();
            funcBuilder.Use(new TaskExceptionHandlingMiddleware(new LoggerFactory().CreateLogger(nameof(ExceptionHandlingMiddleware))));
            funcBuilder.Use( new TimerDataAccessMiddleware(new LoggerFactory().CreateLogger(nameof(TimerDataAccessMiddleware))));
            return funcBuilder;
         });
      }
   }
}
