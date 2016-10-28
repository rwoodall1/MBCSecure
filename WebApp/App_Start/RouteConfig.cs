using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Web API Stateless Route Configurations
            //routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            routes.MapRoute(
                name: "AngularSPA",
                url: "{*id}",
                defaults: new { controller = "AngularAccess", action = "Index", id = UrlParameter.Optional }
            );

            routes.IgnoreRoute("Content/{*pathInfo}");
        }
    }
}
