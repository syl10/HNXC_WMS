using System;
using System.Web;
using System.Web.Mvc;

namespace Wms.Security
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class NonAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Request.RequestContext.RouteData.Values["action"].ToString() == "LogOff")
            {
                httpContext.SkipAuthorization = true;
            }
            return true; 
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.HttpContext.Response.StatusCode == 403)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}