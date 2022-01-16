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
    public class FunctionsMiddleware : ServerlessMiddleware
    {
        private readonly Func<HttpContext, Task<IActionResult>> _execute;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerlessMiddleware"/> class.
        /// </summary>
        /// <param name="funcContext">The task to be executed.</param>
        public FunctionsMiddleware(Func<HttpContext, Task<IActionResult>> funcContext)
        {
            _execute = funcContext;
        }

        /// <summary>
        /// Executes the middleware and creates response from ActionContext
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Task</returns>
        public override async Task InvokeAsync(HttpContext context)
        {
            var result = await _execute(context);

            await result.ExecuteResultAsync(new ActionContext(context, new RouteData(), new ActionDescriptor()));
        }
    }
}
