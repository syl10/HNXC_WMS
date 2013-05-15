using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.WebUtil;
using THOK.Authority.DbModel;
using THOK.Authority.Bll.Interfaces;

namespace Wms.Controllers.Authority.SystemParameterInfo
{
    public class ParameterSetController : Controller
    {
        //
        // GET: /ParameterSet/

        [Dependency]
        public ISystemParameterService SystemParameterService { get; set; }

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasAdd = true;
            ViewBag.hasEdit = true;
            ViewBag.hasDelete = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        // GET: /ParameterSet/GetSystemParameter/
        public ActionResult GetSystemParameter(int page, int rows, FormCollection collection)
        {
            string parameterName = collection["ParameterName"] ?? "";
            string parameterValue = collection["ParameterValue"] ?? "";
            string remark = collection["Remark"] ?? "";
            string userName = collection["UserName"] ?? "";
            AUTH_SYSTEM_PARAMETER sp = new AUTH_SYSTEM_PARAMETER();
            sp.PARAMETER_NAME = parameterName;
            sp.PARAMETER_VALUE = parameterValue;
            sp.REMARK = remark;
            sp.USER_NAME = userName;
            var result = SystemParameterService.GetSystemParameter(page, rows, sp);
            return Json(result, "text", JsonRequestBehavior.AllowGet);
        }
        // GET: /ParameterSet/AddSystemParameter/
        public ActionResult AddSystemParameter(AUTH_SYSTEM_PARAMETER systemParameter)
        {
            string error = string.Empty;
            string userName = this.User.Identity.Name.ToString();
            bool bResult = SystemParameterService.AddSystemParameter(systemParameter, userName, out error);
            string msg = bResult ? "添加成功" : "添加失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, error), "text", JsonRequestBehavior.AllowGet);
        }
        // GET: /ParameterSet/SetSystemParameter/
        public ActionResult SetSystemParameter(AUTH_SYSTEM_PARAMETER systemParameter)
        {
            string error = string.Empty;
            string userName=this.User.Identity.Name.ToString();
            bool bResult = SystemParameterService.SetSystemParameter(systemParameter,userName, out error);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, error), "text", JsonRequestBehavior.AllowGet);
        }
        // GET: /ParameterSet/DelSystemParameter/
        public ActionResult DelSystemParameter(int id)
        {
            string error = string.Empty;
            bool bResult = SystemParameterService.DelSystemParameter(id, out error);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, error), "text", JsonRequestBehavior.AllowGet);
        }
    }
}
