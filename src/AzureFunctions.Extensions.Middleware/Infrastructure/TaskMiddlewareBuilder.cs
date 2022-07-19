using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureFunctions.Extensions.Middleware.Infrastructure
{

   public class TaskMiddlewareBuilder : ITaskMiddlewareBuilder
   {
#if NET6_0
        private readonly List<TaskMiddleware> _middlewarePipeline = new();
#else
      private readonly List<TaskMiddleware> _middlewarePipeline = new List<TaskMiddleware>();
#endif            

      public TaskMiddlewareBuilder()
      {         
      }
      public TaskMiddlewareBuilder(List<TaskMiddleware> middlewarePipeline)
      {
         _middlewarePipeline = new List<TaskMiddleware>();

         foreach (var serverlessMiddleware in middlewarePipeline)
         {
            Use(serverlessMiddleware);
         }
      }
      /// <inheritdoc>/>
      public async Task ExecuteAsync(TaskMiddleware middleware)
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
      public ITaskMiddlewareBuilder Use(TaskMiddleware middleware)
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
