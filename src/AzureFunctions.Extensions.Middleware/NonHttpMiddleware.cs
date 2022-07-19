using AzureFunctions.Extensions.Middleware.Abstractions;

namespace AzureFunctions.Extensions.Middleware
{
   /// <summary>
   /// NonHttpMiddleware to execute non-HTTP trigger method
   /// </summary>
   public class NonHttpMiddleware : TaskMiddleware
   {
      private readonly Func<Task> _execute;

      /// <summary>
      /// Initializes a new instance of the <see cref="NonHttpMiddleware"/> class.
      /// </summary>
      /// <param name="taskContext">The task to be executed.</param>
      public NonHttpMiddleware(Func<Task> taskContext,Microsoft.Azure.WebJobs.ExecutionContext? context=default,object? data=default)
      {
         _execute = taskContext;
         base.ExecutionContext= context;
         base.Data = data;
      }

      /// <summary>
      /// Executes the middleware 
      /// </summary>      
      /// <returns>Task</returns>
      public override async Task InvokeAsync()
      {
         await _execute();
      }
   }
}
