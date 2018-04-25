using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RX.WXMember
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
               name: "pay1",
               url: "Pay/ResultNotify",
               defaults: new { controller = "Pay", action = "ResultNotify", id = UrlParameter.Optional },
               namespaces: new string[] { "RX.WXMember.Controllers" }
           );
            routes.MapRoute(
                name: "pay",
                url: "Pay/{id}",
                defaults: new { controller = "Pay", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "RX.WXMember.Controllers" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "RX.WXMember.Controllers" }
            );
        }
    }
}
