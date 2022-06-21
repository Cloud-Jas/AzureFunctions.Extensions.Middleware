## Features

 * Able to add multiple custom middlewares to the pipeline
 * Able to access HTTP context inside the custom middleware
 * Able to access ExecutionContext inside non-http triggers
 * Able to inject middlewares in all the triggers available
 * Able to bypass middlewares and return response
 * Handle Crosscutting concerns of the application
	* Logging
	* Exception Handling
	* CORS 
	* Performance Monitoring
	* Caching
	* Security
 * Licenced under MIT - 100% free for personal and commercial use

## Supported Frameworks

 - NetCoreApp 3.1
 - NET 5.0
 - NET 6.0

## Installation

### Install with Package Manager Console

`PM> Install-Package AzureFunctions.Extensions.Middleware`

## Usage

### Getting Started

# 1. Add HttpContextAccessor and ExecutionContext to service collection

Inorder to access/modify HttpContext within custom middleware we need to add HttpContextAccessor in Startup.cs file and for accessing ExecutionContext we need to add FunctionExecutionContext concrete class in Startup.cs

```cs

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IExecutionContext, FunctionExecutionContext>();

```

# 2. Add custom middlewares to the pipeline

One or more custom middlewares can be added to the execution pipeline using MiddlewareBuilder. 


```cs

builder.Services.AddTransient<IMiddlewareBuilder, MiddlewareBuilder>((serviceProvider) =>
            {
				// added httpcontextaccessor and executioncontext to middlewarebuilder
                var funcBuilder = new MiddlewareBuilder(serviceProvider.GetRequiredService<IHttpContextAccessor>(),serviceProvider.GetRequiredService<IExecutionContext>());
				
				//add custom middlewares to the execution pipeline
                funcBuilder.Use(new ExceptionHandlingMiddleware(new LoggerFactory().CreateLogger(nameof(ExceptionHandlingMiddleware))));
				
				// add custom middleware based on condition (works in HTTP trigger)
                funcBuilder.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/api/Authorize"),
                    new AuthorizationMiddleware(new LoggerFactory().CreateLogger(nameof(AuthorizationMiddleware))));
                
				return funcBuilder;
            });

```

## 2.1 Use() 

 - Use() middleware takes custom middleware as parameter and will be applied to all the endpoints 

## 2.2 UseWhen()

 - UseWhen() takes Func<HttpContext, bool> and custom middleware as parameters. If the condition is satisfied then middleware will be added to the pipeline 
 of exectuion.


# 3. IMiddlewareBuilder dependency

We can now add IMiddlewareBuilder as a dependency to our HTTP trigger function class.

```cs 

        private readonly ILogger<Function1> _logger;
        private readonly IMiddlewareBuilder _middlewareBuilder;

        public Function1(ILogger<Function1> log, IMiddlewareBuilder middlewareBuilder)
        {
            _logger = log;            
            _middlewareBuilder = middlewareBuilder;
        }
```

# 4. Define Custom middlewares

If HTTP trigger is used try to implement the InvokeAsync(HttpContext) and for non-http triggers implement InvokeAsync(ExecutionContext), (If both http and non-http triggers are deployed in same
azure function try to implement both methods) 


```cs

public class ExceptionHandlingMiddleware : ServerlessMiddleware
    {
        private readonly ILogger _logger;
        public ExceptionHandlingMiddleware(ILogger logger)
        {
            _logger = logger;
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.LogInformation("Request triggered");

                await this.Next.InvokeAsync(context);

                _logger.LogInformation("Request processed without any exceptions");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                context.Response.StatusCode = 400;
                
                await context.Response.WriteAsync("Http Trigger request failed, Please try again");

            }
        }

      public override async Task InvokeAsync(ExecutionContext context)
      {
         try
         {
            _logger.LogInformation("Request triggered");

            await this.Next.InvokeAsync(context);

            _logger.LogInformation("Request processed without any exceptions");
         }
         catch (Exception ex)
         {
            _logger.LogError(ex.Message);           
         }
      }
   }

```

# 5. Execute pipeline

Now we need to bind last middleware for our HttpTrigger method , to do that wrap our existing code inside Functionsmiddleware block "_middlewareBuilder.ExecuteAsync(new FunctionsMiddleware(async (httpContext) =>{HTTP trigger code})"

For returning IActionResult use FunctionsMiddleware

```cs
 
return await _middlewareBuilder.ExecuteAsync(new FunctionsMiddleware(async (httpContext) =>
            {
                _logger.LogInformation("C# HTTP trigger function processed a request.");

                string name = httpContext.Request.Query["name"];                

                string requestBody = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                name = name ?? data?.name;

                string responseMessage = string.IsNullOrEmpty(name)
                    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                    : $"Hello, {name}. This HTTP triggered function executed successfully.";

                return new OkObjectResult(responseMessage);
            }));

```

For non-http triggers use TaskMiddleware

```cs

[FunctionName("TimerTrigger")]
       public async Task Run([TimerTrigger("*/10 * * * * *")] TimerInfo myTimer, ILogger log)
      {
         await _middlewareBuilder.ExecuteAsync(new TaskMiddleware(async (httpContext) =>
         {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await Task.FromResult("test");
         }));
      }	 	 

```

Based on the type of middleware(TaskMiddleware or FunctionsMiddleware) , respective InvokeAsync method will called with ExecutionContext or HttpContext

## Sample

You can find .NET 6 sample application [here](sample) . In this example we have registered Exception handling custom middleware to the exectuion order that
will handle any unhandled exceptions in the Http Trigger execution.


## Special Thanks

Thank you to the following people for their support and contributions!

- [@Sriram](https://www.linkedin.com/in/sriram-ganesan-it/) 

## Sponsor

Leave a ⭐ if this library helped you at handling cross-cutting concerns in serverless architecture.

<a href="https://www.buymeacoffee.com/divakarkumar" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/v2/default-yellow.png" alt="Buy Me A Coffee" style="height: 40px !important;width: 145 !important;" ></a>

[Website](//iamdivakarkumar.com) | [LinkedIn](https://www.linkedin.com/in/divakar-kumar/) | [Forum](https://github.com/Cloud-Jas/AzureFunctions.Extensions.Middleware/discussions) | [Contribution Guide](CONTRIBUTING.md) | [Donate](https://www.buymeacoffee.com/divakarkumar) | [License](LICENSE.txt)

&copy; [Divakar Kumar](//github.com/Divakar-Kumar)

For detailed documentation, please visit the [docs](docs/readme.md). 


## Contact

Divakar Kumar - [@Divakar-Kumar](https://www.linkedin.com/in/divakar-kumar/) - https://iamdivakarkumar.com

Project Link: [https://github.com/Cloud-Jas/AzureFunctions.Extensions.Middleware](https://github.com/Cloud-Jas/AzureFunctions.Extensions.Middleware)
