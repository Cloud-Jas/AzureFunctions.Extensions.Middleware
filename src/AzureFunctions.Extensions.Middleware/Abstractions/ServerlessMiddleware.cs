using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureFunctions.Extensions.Middleware.Abstractions
{
    /// <summary>
    /// Serverless middleware base class
    /// </summary>
    public abstract class ServerlessMiddleware
    {
        /// <summary>
        /// Get Next instance in middleware pipeline
        /// </summary>
        public ServerlessMiddleware Next { get; set; }
        /// <summary>
        /// ctor
        /// </summary>
        protected ServerlessMiddleware()
        {            
        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="next">Next middleware</param>
        protected ServerlessMiddleware(ServerlessMiddleware next)
        {
            this.Next = next;
        }
        /// <summary>
        /// Invoke middleware
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>Task</returns>
        public abstract Task InvokeAsync(HttpContext context);

    }
}
