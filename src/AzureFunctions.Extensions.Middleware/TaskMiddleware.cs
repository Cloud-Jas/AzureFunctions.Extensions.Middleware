using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;

namespace AzureFunctions.Extensions.Middleware
{
   /// <summary>
   /// Functions middleware to execute HTTP trigger method
   /// </summary>
   public class TaskMiddleware : ServerlessMiddleware
   {
      private readonly Func<Microsoft.Azure.WebJobs.ExecutionContext, Task> _execute;

      /// <summary>
      /// Initializes a new instance of the <see cref="ServerlessMiddleware"/> class.
      /// </summary>
      /// <param name="taskContext">The task to be executed.</param>
      public TaskMiddleware(Func<Microsoft.Azure.WebJobs.ExecutionContext, Task> taskContext)
      {
         _execute = taskContext;
      }      
      /// <summary>
      /// Executes the middleware and creates response from ActionContext
      /// </summary>
      /// <param name="context">The context.</param>
      /// <returns>Task</returns>
      public override async Task InvokeAsync(Microsoft.Azure.WebJobs.ExecutionContext context)
      {
         await _execute(context);
      }

      public override Task InvokeAsync(HttpContext context)
      {
         throw new NotImplementedException("HTTPContext is not available in non-http triggers");
      }
   }
}
