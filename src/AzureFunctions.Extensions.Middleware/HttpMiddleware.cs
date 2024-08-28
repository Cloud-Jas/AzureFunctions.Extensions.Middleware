using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Functions.Worker;

namespace AzureFunctions.Extensions.Middleware
{
   /// <summary>
   /// Middleware for executing a function with HttpContext and returning an IActionResult.
   /// </summary>
   public class HttpMiddleware : HttpMiddlewareBase
   {
      private readonly Func<HttpContext, Task<IActionResult>> _execute;

      /// <summary>
      /// Initializes a new instance of the <see cref="HttpMiddleware"/> class.
      /// </summary>
      /// <param name="execute">The function to be executed.</param>
      /// <param name="executionContext">The unified execution context.</param>
      public HttpMiddleware(Func<HttpContext, Task<IActionResult>> execute, Microsoft.Azure.WebJobs.ExecutionContext executionContext)
      {
         _execute = execute ?? throw new ArgumentNullException(nameof(execute));
         base.ExecutionContext = executionContext;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="HttpMiddleware"/> class.
      /// </summary>
      /// <param name="execute">The function to be executed.</param>
      /// <param name="executionContext">The function execution context.</param>
      public HttpMiddleware(Func<HttpContext, Task<IActionResult>> execute, FunctionContext executionContext)
      {
         _execute = execute ?? throw new ArgumentNullException(nameof(execute));
         base.FunctionExecutionContext = executionContext;
      }

      /// <summary>
      /// Executes the middleware and creates a response from IActionResult.
      /// </summary>
      /// <param name="context">The HTTP context.</param>
      /// <returns>A task representing the asynchronous operation.</returns>
      public override async Task InvokeAsync(HttpContext context)
      {
         if (context == null) throw new ArgumentNullException(nameof(context));

         // Execute the provided function to get an IActionResult
         IActionResult result = await _execute(context);

         // Ensure that result is not null before attempting to execute it
         if (result == null)
            throw new InvalidOperationException("The IActionResult cannot be null.");

         // Create a new ActionContext and execute the result
         var actionContext = new ActionContext
         {
            HttpContext = context,
            RouteData = new RouteData(),
            ActionDescriptor = new ActionDescriptor()
         };

         await result.ExecuteResultAsync(actionContext);
      }
   }
}
