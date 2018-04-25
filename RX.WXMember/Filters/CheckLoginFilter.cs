using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RX.WXMember.Filters
{
    public class CheckLoginFilter : FilterAttribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if(HttpContext.Current.Session["user"]==null)
            {
                filterContext.Result = new RedirectResult("/login");
            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["user"] == null)
            {
                filterContext.Result = new RedirectResult("/login");
            }
        }
    }
}