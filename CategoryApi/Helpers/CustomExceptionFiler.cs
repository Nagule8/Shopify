using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ShopApi.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomExceptionFiler : ExceptionFilterAttribute
    {

        public override void OnException(ExceptionContext exceptionContext)
        {
            // Customize this object to fit your needs
            var result = new ObjectResult(new
            {
                exceptionContext.Exception.Message, // Or a different generic message
                exceptionContext.Exception.Source,
                ExceptionType = exceptionContext.Exception.GetType().FullName,
            })
            {
                StatusCode = exceptionContext.Exception.HResult //(int)HttpStatusCode.InternalServerError
            };

            // Log the exception
            Log.Error(exceptionContext.Exception, exceptionContext.Exception.Message);
            //Set the result
            exceptionContext.Result = result;

            base.OnException(exceptionContext);
        }
    }
}
