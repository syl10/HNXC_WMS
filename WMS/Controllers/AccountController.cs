﻿using System.Web.Mvc;
using System.Web.Routing;
using THOK.WebUtil;
using Microsoft.Practices.Unity;
using THOK.Security;
using System.Web.Script.Serialization;
using THOK.Common;
using THOK.Authority.Bll.Interfaces;
using THOK.Authority.Bll.Models;
using System.Collections;
using System;

namespace WMS.Controllers
{
    [TokenAclAuthorize]
    public class AccountController : Controller
    {
        [Dependency]
        public IFormsAuthenticationService FormsService { get; set; }
        [Dependency]
        public IUserService UserService { get; set; }
        [Dependency]
        public ILoginLogService LoginLogService { get; set; }

        [HttpPost]
        public ActionResult LogOn(string userName, string password, string cityId, string systemId, string serverId)
        {
            bool bResult = false;
            if (string.IsNullOrEmpty(cityId)) //默认城市为001，shj 2013/06/13
                cityId = "001";
            string msg = "";
            if(UserService.ValidateUser(userName, password))
            {
                if (UserService.ValidateUserPermission(userName, cityId, systemId))
                {
                    bResult = true;
                    msg = "登录成功!";
                }
                else
                {
                    msg = "登录失败:当前用户没有访问请求的系统服务器的权限!";
                }
            }
            else
            {
                msg = "登录失败:用户名或密码错误！";
            }
            string url = bResult ? UserService.GetLogOnUrl(userName,password,cityId, systemId, serverId) : "";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, url), "text/html");
        }

        public ActionResult LogOn(string logOnKey)
        {
            UserLoginInfo userLoginInfo = (new JavaScriptSerializer()).Deserialize<UserLoginInfo>(Des.DecryptDES(logOnKey, "12345678"));
            string userName = userLoginInfo.UserName; 
            string password = userLoginInfo.Password;
            string cityId = userLoginInfo.CityID;
            string systemId = userLoginInfo.SystemID;
            string serverId = userLoginInfo.ServerID;
            string userid=userLoginInfo .UserID ;
            bool bResult = UserService.ValidateUser(userName, password)
                && UserService.ValidateUserPermission(userName, cityId, systemId);
            if (bResult)
            {
                FormsService.SignIn(userName, false);
                this.AddCookie("userid", userid);
                this.AddCookie("cityid", cityId);
                this.AddCookie("systemid", systemId);
                this.AddCookie("serverid", serverId);
                this.AddCookie("username", userName);
                LoginLogService.Add(userName, systemId);
            }
            return new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" } });
        }

        public ActionResult LogOff()
        {
            FormsService.SignOut();
            return RedirectToAction("Index","Home");
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(string userName, string password,string newPassword)
        {
            bool bResult = UserService.ChangePassword(userName, password, newPassword); 
            string msg = bResult ? "修改密码成功" : "修改密码失败,请确认用户名与密码输入正确！";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg), "text/html");
        }

        [Authorize]
        public ActionResult ChangeServer(string cityId, string systemId, string serverId)
        {
            bool bResult = false;
            string msg = "";
            string userName = this.User.Identity.Name;
            cityId = cityId ?? this.GetCookieValue("cityid");
            systemId = systemId ?? this.GetCookieValue("systemid");

            if (UserService.ValidateUserPermission(userName,cityId, systemId))
            {
                bResult = true;                
                msg = "切换成功!";
            }
            else
            {
                msg = "切换失败:当前用户没有访问请求的系统服务器的权限!";
            }

            this.AddCookie("c", cityId ?? "NULL");
            this.AddCookie("s", systemId ?? "NULL");
            this.AddCookie("ss", serverId ?? "NULL");

            string url = bResult ? UserService.GetLogOnUrl(userName,null, cityId, systemId, serverId) : "";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, url), "text/html", JsonRequestBehavior.AllowGet);
        }

        public void LogRegOff()
        {
            ViewBag.userName = "";
            ViewBag.LogReg = "aa";
            HttpContext.Response.Write("<script>alert('该账号已在另一个地方登录');window.parent.location.reload();</script>");
        }
    }
}
