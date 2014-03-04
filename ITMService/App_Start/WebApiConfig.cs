using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ITMService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "Preview Image",
                routeTemplate: "api/item/{action}/{id}",
                defaults: new { controller = "Item", action = "preview", id = RouteParameter.Optional}
                );
            // force authorization on all calls to the API
            config.Filters.Add(new AuthorizeAttribute());

            
        }
    }
}
