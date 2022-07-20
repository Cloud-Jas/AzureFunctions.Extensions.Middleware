using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureFunctions.Extensions.Middleware.Abstractions
{
    /// <summary>
    /// Creates Functions Middleware pipeline
    /// </summary>
    public interface IHttpMiddlewareBuilder
    {
        /// <summary>
        /// Adds middleware to the pipeline
        /// </summary>
        /// <param name="middleware">ServerlessMiddleware</param>
        /// <returns>IMiddlewareBuilder</returns>
        IHttpMiddlewareBuilder Use(HttpMiddlewareBase middleware);
        /// <summary>
        /// Adds middleware to the pipeline based on the given condition
        /// </summary>
        /// <param name="condition">Condition on httpcontext</param>
        /// <param name="middleware">ServerlessMiddleware</param>
        /// <returns>IMiddlewareBuilder</returns>
        IHttpMiddlewareBuilder UseWhen(Func<HttpContext, bool> condition,HttpMiddlewareBase middleware);
        /// <summary>
        /// Executes pipeline
        /// </summary>        
        /// <param name="middleware">ServerlessMiddleware</param>
        /// <returns>dynamic task</returns>
        Task<dynamic> ExecuteAsync(HttpMiddlewareBase middleware);
    }
}
