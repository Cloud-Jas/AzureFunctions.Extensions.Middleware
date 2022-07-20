namespace AzureFunctions.Extensions.Middleware.Abstractions
{
   /// <summary>
   /// Creates NonHttp Middleware pipeline
   /// </summary>
   public interface INonHttpMiddlewareBuilder
   {
      /// <summary>
      /// Adds middleware to the pipeline
      /// </summary>
      /// <param name="middleware">ServerlessMiddleware</param>
      /// <returns>IMiddlewareBuilder</returns>
      INonHttpMiddlewareBuilder Use(NonHttpMiddlewareBase middleware);      
      /// <summary>
      /// Executes pipeline
      /// </summary>        
      /// <param name="middleware">ServerlessMiddleware</param>
      /// <returns>dynamic task</returns>
      Task ExecuteAsync(NonHttpMiddlewareBase middleware);
   }
}
