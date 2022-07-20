using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureFunctions.Extensions.Middleware.Infrastructure
{

   public class NonHttpMiddlewareBuilder : INonHttpMiddlewareBuilder
   {
#if NET6_0
        private readonly List<NonHttpMiddlewareBase> _middlewarePipeline = new();
#else
      private readonly List<NonHttpMiddlewareBase> _middlewarePipeline = new List<NonHttpMiddlewareBase>();
#endif            

      public NonHttpMiddlewareBuilder()
      {         
      }
      public NonHttpMiddlewareBuilder(List<NonHttpMiddlewareBase> middlewarePipeline)
      {
         _middlewarePipeline = new List<NonHttpMiddlewareBase>();

         foreach (var serverlessMiddleware in middlewarePipeline)
         {
            Use(serverlessMiddleware);
         }
      }
      /// <inheritdoc>/>
      public async Task ExecuteAsync(NonHttpMiddlewareBase middleware)
      {

         Use(middleware);         

         _middlewarePipeline.ForEach(x =>
         {
            x.Data = middleware.Data;
            x.ExecutionContext = middleware.ExecutionContext;
         });

         if (_middlewarePipeline.Any())
         {
            await _middlewarePipeline.First().InvokeAsync();            
         }         
      }
      /// <inheritdoc>/>
      public INonHttpMiddlewareBuilder Use(NonHttpMiddlewareBase middleware)
      {         

         if (_middlewarePipeline is null) throw new Exception("Middleware pipeline is not registerd");

         if (_middlewarePipeline?.Count() > 0)
         {
            _middlewarePipeline.Last().Next = middleware;
         }

         _middlewarePipeline?.Add(middleware);

         return this;
      }
   }
}
