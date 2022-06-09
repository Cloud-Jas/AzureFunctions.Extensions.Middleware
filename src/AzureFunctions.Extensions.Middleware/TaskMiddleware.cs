using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;

namespace AzureFunctions.Extensions.Middleware
{
    /// <summary>
    /// Functions middleware to execute HTTP trigger method
    /// </summary>
    public class TaskMiddleware : ServerlessMiddleware
    {
        private readonly Func<HttpContext, Task> _execute;

      /// <summary>
      /// Initializes a new instance of the <see cref="ServerlessMiddleware"/> class.
      /// </summary>
      /// <param name="taskContext">The task to be executed.</param>
      public TaskMiddleware(Func<HttpContext, Task> taskContext)
        {
            _execute = taskContext;
        }

        /// <summary>
        /// Executes the middleware and creates response from ActionContext
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Task</returns>
        public override async Task InvokeAsync(HttpContext context)
        {
            await _execute(context);            
        }
    }
}
