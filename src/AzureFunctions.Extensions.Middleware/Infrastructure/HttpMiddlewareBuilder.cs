using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureFunctions.Extensions.Middleware.Infrastructure
{

   public class HttpMiddlewareBuilder : IHttpMiddlewareBuilder
   {
#if NET6_0
        private readonly List<Abstractions.HttpMiddlewareBase> _middlewarePipeline = new();
#else
      private readonly List<Abstractions.HttpMiddlewareBase> _middlewarePipeline = new List<Abstractions.HttpMiddlewareBase>();
#endif

      private readonly IHttpContextAccessor _httpContextAccessor;     

      public HttpMiddlewareBuilder(IHttpContextAccessor httpContextAccessor)
      {
         _httpContextAccessor = httpContextAccessor;         
      }
      public HttpMiddlewareBuilder(List<Abstractions.HttpMiddlewareBase> middlewarePipeline)
      {
         _middlewarePipeline = new List<Abstractions.HttpMiddlewareBase>();

         foreach (var serverlessMiddleware in middlewarePipeline)
         {
            Use(serverlessMiddleware);
         }
      }
      /// <inheritdoc>/>
      public async Task<dynamic> ExecuteAsync(Abstractions.HttpMiddlewareBase middleware)
      {

         Use(middleware);

         _middlewarePipeline.ForEach(x =>
         {          
            x.ExecutionContext = middleware.ExecutionContext;
         });

         var context = this._httpContextAccessor.HttpContext;

         if (context != null && _middlewarePipeline.Any())
         {
            await _middlewarePipeline.First().InvokeAsync(context);
            
            if (context.Response != null)
               return new MiddlewareResponse(context);

         }
         throw new Exception("No middleware configured");
      }
      /// <inheritdoc>/>
      public IHttpMiddlewareBuilder Use(Abstractions.HttpMiddlewareBase middleware)
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
      public IHttpMiddlewareBuilder UseWhen(Func<HttpContext, bool> condition, Abstractions.HttpMiddlewareBase middleware)
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
