using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.Authority.Bll.Interfaces;
using Microsoft.Practices.Unity;
using System.Web.Security;
using System.Web.Routing;

namespace THOK.Security
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TokenAclAuthorizeAttribute : AuthorizeAttribute
    {
        [Dependency]
        public  IUserService UserService { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool result = true;
            if (httpContext.Session["username"] != null && httpContext.Session["username"].ToString() != "")
            {
                string user = httpContext.Session["username"].ToString();
                string ipAdress = UserService.GetUserIp(user);
                string localip = UserService.GetLocalIp(user);
                if (ipAdress != localip)
                {
                    result = false;
                }
            }
            if (!result)
            {
                httpContext.Response.StatusCode = 403;
            } 
            return result;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.HttpContext.Response.StatusCode == 403)
            {
                filterContext.Result = new RedirectResult("/Home/Index");
            }
        }
    }
}