﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunctions.Extensions.Middleware.Abstractions
{
   public interface IExecutionContext
   {
      Guid InvocationId { get; set; }
      string FunctionName { get; set; }
      string FunctionDirectory { get; set; }
      string FunctionAppDirectory { get; set; }
   }
}
