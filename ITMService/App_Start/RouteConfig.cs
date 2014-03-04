using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ITMService
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Two",
                url: "{controller}/{action}",
                defaults: new { controller = "Build", action = "media", id = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Three",
                url: "{controller}/BuildItem",
                defaults: new { controller = "BuildItem", action = "item"}
                );
            routes.MapRoute(
               name: "SingleBuild",
               url: "build/{id}",
               defaults: new { controller = "Build"}
               );

            routes.MapRoute(
                name: "User",
                url: "{controller}/User",
                defaults: new { controller = "User", action = "register" }
             );

            routes.MapRoute(
                name: "Active",
                url: "build/active:{id}",
                defaults: new { controller = "Build", action = "active"}
             );
            
            

        }

        
    }
}