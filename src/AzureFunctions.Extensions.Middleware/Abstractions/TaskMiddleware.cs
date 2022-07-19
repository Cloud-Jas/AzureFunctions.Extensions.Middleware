namespace AzureFunctions.Extensions.Middleware.Abstractions
{
   /// <summary>
   /// Serverless middleware base class
   /// </summary>
   public abstract class TaskMiddleware
   {
      /// <summary>
      /// Get Next instance in middleware pipeline
      /// </summary>
      public TaskMiddleware Next { get; set; }
      /// <summary>
      /// Generic data
      /// </summary>
      public object Data { get; set; }
      /// <summary>
      /// Get Execution Context
      /// </summary>
      public Microsoft.Azure.WebJobs.ExecutionContext ExecutionContext { get; set; }
      /// <summary>
      /// ctor
      /// </summary>
      protected TaskMiddleware()
      {
      }
      /// <summary>
      /// ctor
      /// </summary>
      /// <param name="next">Next middleware</param>
      protected TaskMiddleware(TaskMiddleware next)
      {
         this.Next = next;
      }
      /// <summary>
      /// Invoke middleware 
      /// </summary>      
      /// <returns>Task</returns>
      public abstract Task InvokeAsync();
   }
}
