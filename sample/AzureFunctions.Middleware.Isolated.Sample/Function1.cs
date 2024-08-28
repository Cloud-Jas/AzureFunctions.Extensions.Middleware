using AzureFunctions.Extensions.Middleware;
using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctions.Middleware.Isolated.Sample
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly IHttpMiddlewareBuilder _middlewareBuilder;

        public Function1(ILogger<Function1> log, IHttpMiddlewareBuilder middlewareBuilder)
        {
            _logger = log;
            _middlewareBuilder = middlewareBuilder;
        }

        [Function("Function1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req,FunctionContext executionContext)
        {
            return await _middlewareBuilder.ExecuteAsync(new HttpMiddleware(async (httpContext) =>
            {
                _logger.LogInformation("C# HTTP trigger default function processed a request.");

                string name = httpContext.Request.Query["name"];

                string requestBody = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                name = name ?? data?.name;

                string responseMessage = string.IsNullOrEmpty(name)
                    ? "This HTTP triggered default function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                    : $"Hello, {name}. This HTTP triggered default function executed successfully.";

                return new OkObjectResult(responseMessage);
            }, executionContext));            
        }
    }
}
