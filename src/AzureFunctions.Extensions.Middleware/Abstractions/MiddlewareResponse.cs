using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureFunctions.Extensions.Middleware.Abstractions
{
    /// <summary>
    /// Middleware Response object
    /// </summary>
    public class MiddlewareResponse : IActionResult
    {
        private readonly HttpContext httpContext;
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">HttpContext</param>
        public MiddlewareResponse(HttpContext context)
        {
            httpContext = context;
        }
        /// <summary>
        /// Get HttpContext
        /// </summary>
        public HttpContext HttpContext => this.httpContext;
        /// <summary>
        /// Execute Result Async
        /// </summary>
        /// <param name="context">ActionContext</param>
        /// <returns></returns>
        public async Task ExecuteResultAsync(ActionContext context)
        {
            await Task.CompletedTask;
            context.HttpContext = this.httpContext;
        }
    }
}
