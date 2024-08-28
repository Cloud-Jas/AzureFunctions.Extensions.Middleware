using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.Azure.Functions.Worker;

namespace AzureFunctions.Extensions.Middleware
{
   /// <summary>
   /// Middleware for executing non-HTTP trigger methods.
   /// </summary>
   public class NonHttpMiddleware : NonHttpMiddlewareBase
   {
      private readonly Func<Task> _execute;

      /// <summary>
      /// Initializes a new instance of the <see cref="NonHttpMiddleware"/> class.
      /// </summary>
      /// <param name="execute">The task to be executed.</param>
      /// <param name="context">The optional execution context (for WebJobs SDK).</param>
      /// <param name="data">Optional data to be associated with the middleware.</param>
      public NonHttpMiddleware(
          Func<Task> execute,
          Microsoft.Azure.WebJobs.ExecutionContext? context = null,
          object? data = null)
      {
         _execute = execute ?? throw new ArgumentNullException(nameof(execute));
         base.ExecutionContext = context;
         base.Data = data;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="NonHttpMiddleware"/> class.
      /// </summary>
      /// <param name="execute">The task to be executed.</param>
      /// <param name="functionContext">The optional function execution context (for Azure Functions Worker).</param>
      /// <param name="data">Optional data to be associated with the middleware.</param>
      public NonHttpMiddleware(
          Func<Task> execute,
          FunctionContext? functionContext = null,
          object? data = null)
      {
         _execute = execute ?? throw new ArgumentNullException(nameof(execute));
         base.FunctionExecutionContext = functionContext;
         base.Data = data;
      }

      /// <summary>
      /// Executes the middleware by running the provided task.
      /// </summary>
      /// <returns>A task representing the asynchronous operation.</returns>
      public override async Task InvokeAsync()
      {
         await _execute();
      }
   }
}
