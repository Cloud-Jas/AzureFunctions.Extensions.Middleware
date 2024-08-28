using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AzureFunctions.Extensions.Middleware;
using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace AzureFunctions.Middleware.Isolated.Sample
{
    public class FxAuthorize
    {
        private readonly ILogger<FxAuthorize> _logger;
        private readonly IHttpMiddlewareBuilder _middlewareBuilder;

        public FxAuthorize(ILogger<FxAuthorize> log, IHttpMiddlewareBuilder middlewareBuilder)
        {
            _logger = log;
            _middlewareBuilder = middlewareBuilder;
        }

        [Function("Authorize")]
        [OpenApiOperation(operationId: "authorize", tags: new[] { "authorize" }, Summary = "authorize", Description = "This uses authorize middleware", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Summary = "The response", Description = "This returns the response")]
        public async Task<IActionResult> Authorize(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Authorize")] HttpRequest req,FunctionContext executionContext)
        {

            return await _middlewareBuilder.ExecuteAsync(new HttpMiddleware(async (httpContext) =>
            {
               _logger.LogInformation("C# HTTP trigger authorize function processed a request.");

                string name = httpContext.Request.Query["name"];                

                string requestBody = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                name = name ?? data?.name;

                string responseMessage = string.IsNullOrEmpty(name)
                    ? "This HTTP triggered authorize function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                    : $"Hello, {name}. This HTTP triggered authorize function executed successfully.";

                return new OkObjectResult(responseMessage);
            }, executionContext));            
            
        }        
    }
}

