<div id=top></div>
<h1 align="center"><a href="https://iamdivakarkumar.com/" target="blank"><img height="240" src="./icons/middleware.png"/><br/>AzureFunctions.Extensions.Middleware</a></h1>

<p align="center">
  <b>.NET library that allows developers to implement middleware pattern in Azure functions (In-Process Mode) </b>
</p>

<p align="center">  

<a href="https://github.com/Cloud-Jas/AzureFunctions.Extensions.Middleware/actions/workflows">
<img src="https://github.com/Cloud-Jas/AzureFunctions.Extensions.Middleware/actions/workflows/build.yml/badge.svg" alt="Build status"/>
</a>

<a href="https://www.nuget.org/packages/AzureFunctions.Extensions.Middleware">
<img src="https://img.shields.io/nuget/v/AzureFunctions.Extensions.Middleware.svg" alt="Middleware version"/>
</a>


<a href="https://www.nuget.org/packages/AzureFunctions.Extensions.Middleware">
<img src="https://img.shields.io/nuget/dt/AzureFunctions.Extensions.Middleware.svg" alt="Middleware downloads"/>
</a>

</p>

<p align="center">
  <a href="#features">Features</a> | <a href="#sample">Sample</a> | <a href="#installation">Installation</a> | <a href="#usage">Usage</a> | <a href="#special-thanks">Special Thanks</a> | <a href="https://github.com/cloud-jas/AzureFunctions.Extensions.Middleware/discussions">Forum</a>
</p>

<br/>

<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#features">Features</a>                  
    </li>
    <li><a href="#Supported-Frameworks">Supported frameworks</a></li>
    <li>
      <a href="#installation">Getting Started</a>      
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#sponsor">Sponsor</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>

## Updates 3.0

* Separated concerns of Http and non-http triggers
* bug-fixes in accessing executionContext
* cleaner approach to access data for non-http triggers 

> Note:  Breaking change of class name changes 
<br>
> FunctionsMiddleware => HttpMiddleware 
<br>
> TaskMiddleware => NonHttpMiddleware 
<br>
> IMiddlewareBuilder => IHttpMiddlewareBuilder
<br>
> ServerlessMiddleware => HttpMiddlewareBase


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

 <p align="right">(<a href="#top">back to top</a>)</p>

## Installation

### Install with Package Manager Console

`PM> Install-Package AzureFunctions.Extensions.Middleware`

## Usage

### Getting Started


# 1. HTTP Triggers

## 1.1 Add HttpContextAccessor in Startup.cs

Inorder to access/modify HttpContext within custom middleware we need to inject HttpContextAccessor to DI in Startup.cs file

```cs

builder.Services.AddHttpContextAccessor()

```

## 1.2. Add custom middlewares to the pipeline in Startup.cs

One or more custom middlewares can be added to the execution pipeline as below.

```cs

builder.Services.AddTransient<IHttpMiddlewareBuilder, HttpMiddlewareBuilder>((serviceProvider) =>
         {
            var funcBuilder = new HttpMiddlewareBuilder(serviceProvider.GetRequiredService<IHttpContextAccessor>());
            funcBuilder.Use(new ExceptionHandlingMiddleware(serviceProvider.GetService<ILogger<ExceptionHandlingMiddleware>>()));
            funcBuilder.UseWhen(ctx => ctx != null && ctx.Request.Path.StartsWithSegments("/api/Authorize"),
                   new AuthorizationMiddleware(serviceProvider.GetService<ILogger<AuthorizationMiddleware>>()));
            return funcBuilder;
         });

```

### 1.2.1 Use() 

 - Use() middleware takes custom middleware as parameter and will be applied to all the endpoints 

### 1.2.2 UseWhen()

 - UseWhen() takes Func<HttpContext, bool> and custom middleware as parameters. If the condition is satisfied then middleware will be added to the pipeline 
 of exectuion.


## 1.3. Pass IHttpMiddlewareBuilder in Http trigger class

Pass IHttpMiddlewareBuilder dependency to the constructor of Http trigger class

```cs 

        private readonly ILogger<FxDefault> _logger;
        private readonly IHttpMiddlewareBuilder _middlewareBuilder;

        public FxDefault(ILogger<FxDefault> log, IHttpMiddlewareBuilder middlewareBuilder)
        {
            _logger = log;
            _middlewareBuilder = middlewareBuilder;
        }
```

## 1.4. Modify http triggers methods

All of our custom middlewares are added in the Startup.cs file and now we need to bind last middleware for our HttpTrigger method , use "_middlewareBuilder.ExecuteAsync(new HttpMiddleware(async (httpContext) =>{HTTP trigger code},executionContext)" to wrap the code.

> NOTE:  pass optional parameter {executionContext} to use it in the custom middlewares , refer 1.5 to see how to make use of executionContext

```cs
 
public class FxDefault
    {
        private readonly ILogger<FxDefault> _logger;
        private readonly IHttpMiddlewareBuilder _middlewareBuilder;

        public FxDefault(ILogger<FxDefault> log, IHttpMiddlewareBuilder middlewareBuilder)
        {
            _logger = log;
            _middlewareBuilder = middlewareBuilder;
        }

        [FunctionName("Function1")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,ExecutionContext executionContext)
        {

           return await _middlewareBuilder.ExecuteAsync(new Extensions.Middleware.HttpMiddleware(async (httpContext) =>
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

```

In the above example we have passed executionContext as parameter to HttpMiddleware. This will be made available to all the custom middleware that are registered in startup.cs

## 1.5 How to define Custom middlewares for http triggers?

All custom middleware of Http triggers should inherit from HttpMiddlewareBase and override InvokeAsync method . You will be able to access both HttpContext and ExecutionContext

> Note
> You have access to execution context in all the custom middlewares , only if you pass the executionContext as 2nd parameter in the HttpMiddleware wrapper (refer 1.4)
> To access it use {this.ExecutionContext} , refer below

```cs

    public class ExceptionHandlingMiddleware : HttpMiddlewareBase
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }
        public override async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.LogInformation($"{this.ExecutionContext.FunctionName} Request triggered");

                await this.Next.InvokeAsync(context);

                _logger.LogInformation($"{this.ExecutionContext.FunctionName} Request processed without any exceptions");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                context.Response.StatusCode = 400;
                
                await context.Response.WriteAsync($"{this.ExecutionContext.FunctionName} request failed, Please try again");

            }
        }
   }

```


# 2. Non-HTTP Triggers


## 2.1 Add custom middlewares to the pipeline in Startup.cs

One or more custom middlewares can be added to the execution pipeline as below for non-http triggers

```cs

builder.Services.AddTransient<INonHttpMiddlewareBuilder, NonHttpMiddlewareBuilder>((serviceProvider) =>
         {
            var funcBuilder = new NonHttpMiddlewareBuilder();
            funcBuilder.Use(new TaskExceptionHandlingMiddleware(serviceProvider.GetService<ILogger<TaskExceptionHandlingMiddleware>>()));
            funcBuilder.Use(new TimerDataAccessMiddleware(serviceProvider.GetService<ILogger<TimerDataAccessMiddleware>>()));
            return funcBuilder;
         });

```

### 2.1.1 Use() 

 - Use() middleware takes custom middleware as parameter and will be applied to all the endpoints 
 
 > NOTE: UseWhen is not available in non-http triggers
 
 <br>
 
 > However you could use ExecutionContext in each custom middleware to perform similar logic :). Refer the examples given below


## 2.2. Pass INonHttpMiddlewareBuilder in Non-Http trigger class

Pass INonHttpMiddlewareBuilder dependency to the constructor of Non-Http trigger class

```cs 

      private readonly ILogger<TimerTrigger> _logger;
      private readonly INonHttpMiddlewareBuilder _middlewareBuilder;

      public TimerTrigger(ILogger<TimerTrigger> log, INonHttpMiddlewareBuilder middlewareBuilder)
      {
         _logger = log;
         _middlewareBuilder = middlewareBuilder;
      }
```

## 2.3. Modify non-http triggers methods

All of our custom middlewares are added in the Startup.cs file and now we need to bind last middleware for our HttpTrigger method , use "_middlewareBuilder.ExecuteAsync(new NonHttpMiddleware(async (httpContext) =>{HTTP trigger code},executionContext,data)" to wrap the code.

> NOTE:  pass optional parameters {executionContext,data} to use it in the custom middlewares , refer 1.5 to see how to make use of executionContext


```cs
 
 public class TimerTrigger
   {
      private readonly ILogger<TimerTrigger> _logger;
      private readonly INonHttpMiddlewareBuilder _middlewareBuilder;

      public TimerTrigger(ILogger<TimerTrigger> log, INonHttpMiddlewareBuilder middlewareBuilder)
      {
         _logger = log;
         _middlewareBuilder = middlewareBuilder;
      }
      [FunctionName("TimerTrigger")]
      public async Task Run([TimerTrigger("*/10 * * * * *")] TimerInfo myTimer, ILogger log,ExecutionContext context)
      {

         await _middlewareBuilder.ExecuteAsync(new NonHttpMiddleware(async () =>
            {
               _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
               await Task.FromResult(true);
            },context,myTimer));
      }
   }

```

In the above example we have passed both executionContext and timerinfo data as parameters to NonHttpMiddleware. This will be made available to all the custom middleware that are registered in startup.cs

## 2.4 How to define Custom middlewares for non-http triggers?

All custom middleware of Non-Http triggers should inherit from TaskMiddleware and override InvokeAsync method . You will be able to access both ExecutionContext and Data

> Note
> You have access to execution context and data in all the custom middlewares , only if you pass the executionContext and data as 2nd,3rd parameter in the NonHttpMiddleware wrapper respectively (refer 1.4)
> To access it use {this.ExecutionContext}/{this.Data} , refer below

```cs

    public class TimerDataAccessMiddleware : NonHttpMiddlewareBase
   {
      private readonly ILogger<TimerDataAccessMiddleware> _logger;
      public TimerDataAccessMiddleware(ILogger<TimerDataAccessMiddleware> logger)
      {
         _logger = logger;
      }
      public override async Task InvokeAsync()
      {
         if (this.ExecutionContext.FunctionName.Equals("TimerTrigger"))
         {
            try
            {
               var timerData = this.Data as TimerInfo;
               _logger.LogInformation($"{this.ExecutionContext.FunctionName} Request triggered");
               await this.Next.InvokeAsync();
               _logger.LogInformation($"{this.ExecutionContext.FunctionName} Request processed without any exceptions");
            }
            catch (Exception ex)
            {
               _logger.LogError(ex.Message);
            }
         }
         await this.Next.InvokeAsync();
      }
   }

```


<p align="right">(<a href="#top">back to top</a>)</p>

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

[![Nuget library screenshot][product-screenshot]](https://iamdivakarkumar.com)

Divakar Kumar - [@Divakar-Kumar](https://www.linkedin.com/in/divakar-kumar/) - https://iamdivakarkumar.com

Project Link: [https://github.com/Cloud-Jas/AzureFunctions.Extensions.Middleware](https://github.com/Cloud-Jas/AzureFunctions.Extensions.Middleware)

<p align="right">(<a href="#top">back to top</a>)</p>

[product-screenshot]: https://dev-to-uploads.s3.amazonaws.com/uploads/articles/3x47dpp54ok0w1g6vkp7.png
