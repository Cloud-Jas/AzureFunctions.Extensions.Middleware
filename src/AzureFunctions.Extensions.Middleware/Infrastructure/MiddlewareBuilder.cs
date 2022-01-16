using AzureFunctions.Extensions.Middleware.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureFunctions.Extensions.Middleware.Infrastructure
{
    
    public class MiddlewareBuilder : IMiddlewareBuilder
    {
#if NET6_0
        private readonly List<ServerlessMiddleware> _middlewarePipeline = new();
#else
        private readonly List<ServerlessMiddleware> _middlewarePipeline = new List<ServerlessMiddleware>();
#endif

        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public MiddlewareBuilder(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }        
        public MiddlewareBuilder(List<ServerlessMiddleware> middlewarePipeline)
        {
            _middlewarePipeline = new List<ServerlessMiddleware>();

            foreach (var serverlessMiddleware in middlewarePipeline)
            {
                Use(serverlessMiddleware);
            }
        }
        /// <inheritdoc>/>
        public async Task<IActionResult> ExecuteAsync(ServerlessMiddleware middleware)
        {

            Use(middleware);

            var context = this._httpContextAccessor.HttpContext;

            if (_middlewarePipeline.Any())
            {
               await _middlewarePipeline.First().InvokeAsync(context);

                if (context.Response != null)
                    return new MiddlewareResponse(context);
            }

            throw new Exception("No middleware configured");
        }
        /// <inheritdoc>/>
        public IMiddlewareBuilder Use(ServerlessMiddleware middleware)
        {
            if (_middlewarePipeline is null) throw new Exception("Middleware pipeline is not registerd");

            if (_middlewarePipeline?.Count() > 0)
            {
                _middlewarePipeline.Last().Next = middleware;
            }

            _middlewarePipeline.Add(middleware);

            return this;
        }
        /// <inheritdoc>/>
        public IMiddlewareBuilder UseWhen(Func<HttpContext, bool> condition,ServerlessMiddleware middleware)
        {
            var context = this._httpContextAccessor.HttpContext;

            if(condition(context))
            {
                this.Use(middleware);
            }

            return this;
        }
    }
}
