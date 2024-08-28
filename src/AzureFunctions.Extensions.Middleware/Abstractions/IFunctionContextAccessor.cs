using Microsoft.Azure.Functions.Worker;

namespace AzureFunctions.Extensions.Middleware.Abstractions
{
    public interface IFunctionContextAccessor
    {
        FunctionContext FunctionContext { get; set; }
    }
}