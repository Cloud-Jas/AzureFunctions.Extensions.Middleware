using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureFunctions.Extensions.Middleware.Infrastructure
{

   public class MiddlewareBuilder : IMiddlewareBuilder
   {
#if NET6_0
        private readonly List<ServerlessMiddleware> _middlewarePipeline = new();
#else
      private readonly List<ServerlessMiddleware> _middlewarePipeline = new List<ServerlessMiddleware>();
#endif

      private readonly IHttpContextAccessor _httpContextAccessor;

      private IExecutionContext _executionContext;

      public MiddlewareBuilder(IHttpContextAccessor httpContextAccessor, IExecutionContext executionContext)
      {
         _httpContextAccessor = httpContextAccessor;
         _executionContext = executionContext;
      }
      public MiddlewareBuilder(List<ServerlessMiddleware> middlewarePipeline)
      {
         _middlewarePipeline = new List<ServerlessMiddleware>();

         foreach (var serverlessMiddleware in middlewarePipeline)
         {
            Use(serverlessMiddleware);
         }
      }
      /// <inheritdoc>/>
      public async Task<dynamic> ExecuteAsync(ServerlessMiddleware middleware)
      {

         Use(middleware);

         var context = this._httpContextAccessor.HttpContext;

         if (context != null && _middlewarePipeline.Any())
         {
            await _middlewarePipeline.First().InvokeAsync(context);

            if (middleware is TaskMiddleware)
               return true;

            if (context.Response != null)
               return new MiddlewareResponse(context);

         }
         else if (_executionContext != null && _middlewarePipeline.Any())
         {
            await _middlewarePipeline.First().InvokeAsync(_executionContext as Microsoft.Azure.WebJobs.ExecutionContext);

            return true;

         }

         throw new Exception("No middleware configured");
      }
      /// <inheritdoc>/>
      public IMiddlewareBuilder Use(ServerlessMiddleware middleware)
      {
         if (_middlewarePipeline is null) throw new Exception("Middleware pipeline is not registerd");

         if (_middlewarePipeline?.Count() > 0)
         {
            _middlewarePipeline.Last().Next = middleware;
         }

         _middlewarePipeline.Add(middleware);

         return this;
      }
      /// <inheritdoc>/>
      public IMiddlewareBuilder UseWhen(Func<HttpContext, bool> condition, ServerlessMiddleware middleware)
      {
         var context = this._httpContextAccessor.HttpContext;

         if (condition(context))
         {
            this.Use(middleware);
         }

         return this;
      }
   }
}
