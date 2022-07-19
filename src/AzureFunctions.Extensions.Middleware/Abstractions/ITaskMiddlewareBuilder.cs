namespace AzureFunctions.Extensions.Middleware.Abstractions
{
   /// <summary>
   /// Creates NonHttp Middleware pipeline
   /// </summary>
   public interface ITaskMiddlewareBuilder
   {
      /// <summary>
      /// Adds middleware to the pipeline
      /// </summary>
      /// <param name="middleware">ServerlessMiddleware</param>
      /// <returns>IMiddlewareBuilder</returns>
      ITaskMiddlewareBuilder Use(TaskMiddleware middleware);      
      /// <summary>
      /// Executes pipeline
      /// </summary>        
      /// <param name="middleware">ServerlessMiddleware</param>
      /// <returns>dynamic task</returns>
      Task ExecuteAsync(TaskMiddleware middleware);
   }
}
