using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using THOK.Authority.Bll.Interfaces;

namespace THOK.Security
{
    [AttributeUsage(AttributeTargets.All,AllowMultiple = false, Inherited = true)]
    public class TokenAclAuthorizeAttribute :AuthorizeAttribute
    {
        ServiceFactory UserFactory = new ServiceFactory();
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool result = true;
            if (httpContext.Request.Cookies["username"] != null)
            {
                string user = httpContext.Request.Cookies["username"].Value;
                string ipAdress = UserFactory.GetService<IUserService>().GetUserIp(user);
                //string localip = httpContext.Request.UserHostAddress;
                string localip = System.Net.Dns.Resolve(System.Net.Dns.GetHostName()).AddressList[0].ToString();
                if (ipAdress != localip)
                {
                    result = false;
                }
            }
            return result;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {            
            if (!AuthorizeCore(filterContext.HttpContext))
            {
                FormsAuthentication.SignOut();
                filterContext.HttpContext.Response.Redirect("/Account/LogRegOff");
            }
        }
    }
}
