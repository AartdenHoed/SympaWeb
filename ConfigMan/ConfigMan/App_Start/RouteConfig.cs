using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ConfigMan {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default1",
                url: "{controller}/{action}/{id}/{filterstr}/{componentFilter}/{vendorFilter}/{authFilter}",
                defaults: new
                {
                    controller = "Component",
                    action = "Index",
                    id = UrlParameter.Optional,
                    filterstr = UrlParameter.Optional,
                    componentFilter = UrlParameter.Optional,
                    vendorFilter = UrlParameter.Optional,
                    authFilter = UrlParameter.Optional
                }


            );
            routes.MapRoute(
                name: "Default2",
                url: "{controller}/{action}/{message}/{msgLevel}/{filterstr}/{componentFilter}/{vendorFilter}/{authFilter}",
                defaults: new
                {
                    controller = "Component",
                    action = "Index",
                    message = UrlParameter.Optional,
                    msgLevel = UrlParameter.Optional,
                    filterstr = UrlParameter.Optional,
                    componentFilter = UrlParameter.Optional,
                    vendorFilter = UrlParameter.Optional,   
                    authFilter = UrlParameter.Optional
                }


            );
        }
    }
}
