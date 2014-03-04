using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net;

namespace ITMService.Filters
{
    public class ExcFilter : ExceptionFilterAttribute
    {

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            // run through several of these to handle the exceptions


            if (actionExecutedContext.Exception is NotImplementedException)
            {
                actionExecutedContext.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.NotImplemented);// return the response
            }
            if(actionExecutedContext.Exception is NotSupportedException)
            {
                actionExecutedContext.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.HttpVersionNotSupported);
            }
            if (actionExecutedContext.Exception is HttpRequestException)
            {
                actionExecutedContext.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.UnsupportedMediaType);
            }
            if (actionExecutedContext.Exception is System.Exception)
            {
                actionExecutedContext.Response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.NotAcceptable)
                {
                    ReasonPhrase = "unable to modify the data"
                };
                
            }
        }
    }
}