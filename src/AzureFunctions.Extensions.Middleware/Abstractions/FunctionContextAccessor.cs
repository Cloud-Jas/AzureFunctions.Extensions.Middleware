using Microsoft.Azure.Functions.Worker;
using System.Threading;

namespace AzureFunctions.Extensions.Middleware.Abstractions
{
    public class FunctionContextAccessor : IFunctionContextAccessor
    {
        private static readonly AsyncLocal<FunctionContextHolder> _currentContext = new();

        public virtual FunctionContext FunctionContext
        {
            get => _currentContext.Value?.Context;
            set
            {
                if (_currentContext.Value != null)
                {
                    // Clear the previous context
                    _currentContext.Value.Context = null;
                }

                if (value != null)
                {
                    // Assign the new context
                    _currentContext.Value = new FunctionContextHolder { Context = value };
                }
            }
        }

        private class FunctionContextHolder
        {
            public FunctionContext Context { get; set; }
        }
    }
}
