﻿using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureFunctions.Extensions.Middleware.Infrastructure
{
    public class HttpMiddlewareBuilder : IHttpMiddlewareBuilder
    {
#if NET8_0 || NET6_0
        private readonly List<HttpMiddlewareBase> _middlewarePipeline = new();
#else
        private readonly List<HttpMiddlewareBase> _middlewarePipeline = new List<HttpMiddlewareBase>();
#endif

        private readonly IHttpContextAccessor _httpContextAccessor;
        private IFunctionContextAccessor FunctionContextAccessor { get; }

        public HttpMiddlewareBuilder(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        public HttpMiddlewareBuilder(IFunctionContextAccessor functionContextAccessor)
        {
            FunctionContextAccessor = functionContextAccessor ?? throw new ArgumentNullException(nameof(functionContextAccessor));
        }

        public HttpMiddlewareBuilder(List<HttpMiddlewareBase> middlewarePipeline)
        {
            if (middlewarePipeline == null)
                throw new ArgumentNullException(nameof(middlewarePipeline));

            _middlewarePipeline = new List<HttpMiddlewareBase>(middlewarePipeline);
        }

        /// <summary>
        /// Executes the middleware pipeline with the provided middleware.
        /// </summary>
        /// <param name="middleware">The middleware to execute.</param>
        /// <returns>The result of the middleware execution.</returns>
        public async Task<dynamic> ExecuteAsync(HttpMiddlewareBase middleware)
        {
            HttpContext httpContext = null;

            if (middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            Use(middleware);

            // Set the execution context for all middleware in the pipeline
            foreach (var pipeMiddleware in _middlewarePipeline)
            {
                if (middleware.ExecutionContext != null)
                {
                    pipeMiddleware.ExecutionContext = middleware.ExecutionContext;
                    httpContext = _httpContextAccessor.HttpContext;
                }
                else if (middleware.FunctionExecutionContext != null)
                {
                    pipeMiddleware.FunctionExecutionContext = middleware.FunctionExecutionContext;
#if NET8_0
                    httpContext = middleware.FunctionExecutionContext.GetHttpContext();
#endif
                }
            }

            if (httpContext == null)
                throw new InvalidOperationException("HttpContext is not available.");

            if (_middlewarePipeline.Any())
            {
                // Start executing the middleware pipeline
                await _middlewarePipeline.First().InvokeAsync(httpContext);

                // Return the middleware response if available
                return httpContext.Response != null ? new MiddlewareResponse(httpContext) : null;
            }

            throw new InvalidOperationException("No middleware configured.");
        }

        /// <summary>
        /// Adds a middleware to the pipeline.
        /// </summary>
        /// <param name="middleware">The middleware to add.</param>
        /// <returns>The current instance of <see cref="HttpMiddlewareBuilder"/>.</returns>
        public IHttpMiddlewareBuilder Use(HttpMiddlewareBase middleware)
        {
            if (middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            if (_middlewarePipeline.Count > 0)
            {
                _middlewarePipeline.Last().Next = middleware;
            }

            _middlewarePipeline.Add(middleware);

            return this;
        }

        /// <summary>
        /// Conditionally adds a middleware to the pipeline based on the provided condition.
        /// </summary>
        /// <param name="condition">A function to determine whether to add the middleware.</param>
        /// <param name="middleware">The middleware to add if the condition is true.</param>
        /// <returns>The current instance of <see cref="HttpMiddlewareBuilder"/>.</returns>
        public IHttpMiddlewareBuilder UseWhen(Func<HttpContext, bool> condition, HttpMiddlewareBase middleware)
        {
            HttpContext context = null;
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            if (middleware == null)
                throw new ArgumentNullException(nameof(middleware));

#if NET8_0
            if (FunctionContextAccessor != null && FunctionContextAccessor.FunctionContext != null)
                context = FunctionContextAccessor.FunctionContext.GetHttpContext();
#endif            
            if (context == null)
                context = _httpContextAccessor.HttpContext;

            if (context != null && condition(context))
            {
                Use(middleware);
            }

            return this;
        }
    }
}
