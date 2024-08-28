using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace AzureFunctions.Extensions.Middleware.Abstractions
{
    /// <summary>
    /// Serverless middleware base class
    /// </summary>
    public abstract class HttpMiddlewareBase
    {
        /// <summary>
        /// Get Next instance in middleware pipeline
        /// </summary>
        public HttpMiddlewareBase Next { get; set; }
        /// <summary>
        /// Get Execution Context for in-process azure functions
        /// </summary>
        public Microsoft.Azure.WebJobs.ExecutionContext ExecutionContext { get; set; }

        /// <summary>
        /// Get Function Execution context for dotnet-isolated azure functions
        /// </summary>
        public FunctionContext FunctionExecutionContext { get; set; }
        /// <summary>
        /// ctor
        /// </summary>
        protected HttpMiddlewareBase()
        {
        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="next">Next middleware</param>
        protected HttpMiddlewareBase(HttpMiddlewareBase next)
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
