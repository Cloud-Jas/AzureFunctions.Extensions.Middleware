using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AzureFunctions.Extensions.Middleware;
using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace AzureFunctions.Middleware.Sample
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

        [FunctionName("Authorize")]
        [OpenApiOperation(operationId: "Authorize", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Authorize(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Authorize")] HttpRequest req,ExecutionContext executionContext)
        {

            return await _middlewareBuilder.ExecuteAsync(new Extensions.Middleware.HttpMiddleware(async (httpContext) =>
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

