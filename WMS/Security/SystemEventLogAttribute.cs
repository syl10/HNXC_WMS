using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.Security;
using THOK.Authority.Bll.Interfaces;

namespace Wms.Security
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class SystemEventLogAttribute : AuthorizeAttribute
    {
        ServiceFactory EventLogFactory = new ServiceFactory();
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            string eventName = httpContext.Request.RequestContext.RouteData.Values["action"].ToString();
            string eventDescription = httpContext.Request.RawUrl;
            if (httpContext.Request.Cookies["username"] != null && httpContext.Request.Cookies["systemid"] != null)
            {
                string operateUser = httpContext.Request.Cookies["username"].Value;
               string targetSystem= httpContext.Request.Cookies["systemid"].Value;
                string idAdress = httpContext.Request.UserHostAddress;
                if (operateUser != "" && operateUser != null)
                {
                   EventLogFactory.GetService<ISystemEventLogService>().CreateEventLog(eventName, eventDescription, operateUser, targetSystem, idAdress);
                }
            }
            return true;
        }
    }
}