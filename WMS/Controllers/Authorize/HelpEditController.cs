using System.Web.Mvc;
using System.Text;
using System.Web.Routing;
using System.Web.Script.Serialization; 
using Microsoft.Practices.Unity;
using THOK.WebUtil;
using THOK.Authority.DbModel;
using THOK.Authority.Bll.Interfaces;
using System;

namespace WMS.Controllers.Authority
{
    public class HelpEditController : Controller
    {
        //
        // GET: /HelpEdit/
        [Dependency]
        public IHelpContentService HelpContentService { get; set; }

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        
        //帮助tree结构
        // GET: /HelpEdit/GetHelpContent/
        public ActionResult GetHelpContent(string sysId)
        {
            var helpTree = HelpContentService.GetHelpContentTree(sysId);
            return Json(helpTree, "text", JsonRequestBehavior.AllowGet);
        }
        // GET: /HelpEdit/SaveHelpEdit/
        [ValidateInput(false)]
        public ActionResult SaveHelpEdit(string helpId01, string editor01)
        {
            string strResult = string.Empty;
            bool bResult = HelpContentService.EditSave(helpId01, editor01, out strResult);
            string msg = bResult ? "更新成功" : "更新失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text", JsonRequestBehavior.AllowGet);
        }
        //依据ID获取帮助文档内容
        // GET: /HelpEdit/GetContentTxt/
        public ActionResult GetContentTxt(string helpId)
        {
            var helpContent = HelpContentService.GetContentTxt(helpId);
            return Json(helpContent, "text", JsonRequestBehavior.AllowGet);
        }
        //依据ID获取帮助文档内容
        // GET: /HelpEdit/GetContentTxt_02/
        public ActionResult GetSingleContentTxt(string helpId)
        {
            var helpContent = HelpContentService.GetSingleContentTxt(helpId);
            return Json(helpContent, "text", JsonRequestBehavior.AllowGet);
        }
    }
}
