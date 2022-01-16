﻿## Features

 * Able to add multiple custom middlewares to the pipeline
 * Able to access HTTP context inside the custom middleware
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

# 1. Add HttpContextAccessor to service collection

Inorder to access/modify HttpContext within custom middleware we need to add HttpContextAccessor in Startup.cs file.

```cs

builder.Services.AddHttpContextAccessor();

```

# 2. Add custom middlewares to the pipeline

One or more custom middlewares can be added to the execution pipeline using MiddlewareBuilder. 


```cs

builder.Services.AddTransient<IMiddlewareBuilder, MiddlewareBuilder>((serviceProvider) =>
            {
                var funcBuilder = new MiddlewareBuilder(serviceProvider.GetRequiredService<IHttpContextAccessor>());
                funcBuilder.Use(new ExceptionHandlingMiddleware(new LoggerFactory().CreateLogger(nameof(ExceptionHandlingMiddleware))));
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

# 4. Execute pipeline

Now we need to bind last middleware for our HttpTrigger method , to do that wrap our existing code inside Functionsmiddleware block "_middlewareBuilder.ExecuteAsync(new FunctionsMiddleware(async (httpContext) =>{HTTP trigger code})"

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