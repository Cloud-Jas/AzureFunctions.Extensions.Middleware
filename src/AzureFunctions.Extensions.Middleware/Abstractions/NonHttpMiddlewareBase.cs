﻿using Microsoft.Azure.Functions.Worker;

namespace AzureFunctions.Extensions.Middleware.Abstractions
{
    /// <summary>
    /// Serverless middleware base class
    /// </summary>
    public abstract class NonHttpMiddlewareBase
    {
        /// <summary>
        /// Get Next instance in middleware pipeline
        /// </summary>
        public NonHttpMiddlewareBase Next { get; set; }
        /// <summary>
        /// Generic data
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// Get Execution Context for in-process azure functions
        /// </summary>
        public Microsoft.Azure.WebJobs.ExecutionContext ExecutionContext { get; set; }
        /// <summary>
        /// Get Function Execution Context for dotnet-isolated azure functions
        /// </summary>
        public FunctionContext FunctionExecutionContext { get; set; }
        /// <summary>
        /// ctor
        /// </summary>
        protected NonHttpMiddlewareBase()
        {
        }
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="next">Next middleware</param>
        protected NonHttpMiddlewareBase(NonHttpMiddlewareBase next)
        {
            this.Next = next;
        }
        /// <summary>
        /// Invoke middleware 
        /// </summary>      
        /// <returns>Task</returns>
        public abstract Task InvokeAsync();
    }
}
