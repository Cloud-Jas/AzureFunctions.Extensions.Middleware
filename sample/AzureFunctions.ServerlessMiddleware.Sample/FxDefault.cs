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
    public class FxDefault
    {
        private readonly ILogger<FxDefault> _logger;
        private readonly IMiddlewareBuilder _middlewareBuilder;

        public FxDefault(ILogger<FxDefault> log, IMiddlewareBuilder middlewareBuilder)
        {
            _logger = log;
            _middlewareBuilder = middlewareBuilder;
        }

        [FunctionName("Function1")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {

            return await _middlewareBuilder.ExecuteAsync(new FunctionsMiddleware(async (httpContext) =>
            {
                _logger.LogInformation("C# HTTP trigger default function processed a request.");

                throw new Exception("custom exception");

                string name = httpContext.Request.Query["name"];                

                string requestBody = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                name = name ?? data?.name;

                string responseMessage = string.IsNullOrEmpty(name)
                    ? "This HTTP triggered default function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                    : $"Hello, {name}. This HTTP triggered default function executed successfully.";

                return new OkObjectResult(responseMessage);
            }));            
            
        }
    }
}

