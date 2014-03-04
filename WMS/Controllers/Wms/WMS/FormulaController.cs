using System.Web.Mvc;
using System.Web.Routing;
using System.Text;
using Microsoft.Practices.Unity;
using THOK.WebUtil;
using THOK.Authority.Bll.Interfaces;
using System;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using THOK.Security;

namespace WMS.Controllers.Wms.WMS
{
    [TokenAclAuthorize]
    public class FormulaController : Controller
    {
        //
        // GET: /Formula/

        [Dependency]
        public IWMSFormulaService FormulaService { get; set; }

        // GET: /Formula/
        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasAdd = true;
            ViewBag.hasEdit = true;
            ViewBag.hasDelete = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        // GET: /Formula/Details/
        [HttpPost]
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string BTYPE_NAME = collection["BTYPE_NAME"] ?? "";
            string BILL_TYPE = collection["BILL_TYPE"] ?? "";
            string TASK_LEVEL = collection["TASK_LEVEL"] ?? "";
            string Memo = collection["MEMO"] ?? "";
            string TARGET_CODE = collection["TARGET_CODE"] ?? "";

            string FORMULA_CODE = collection["FORMULA_CODE"] ?? "";
            string FORMULA_NAME = collection["FORMULA_NAME"] ?? "";
            string CIGARETTE_CODE = collection["CIGARETTE_CODE"] ?? "";
            string ISACTIVE = collection["ISACTIVE"] ?? "";
            string FORMULADATE = collection["FORMULADATE"] ?? "";
            string OPERATER = collection["OPERATER"] ?? "";
            var users = FormulaService.GetDetails(page, rows, BTYPE_NAME, BILL_TYPE, TASK_LEVEL, Memo, TARGET_CODE,FORMULA_CODE ,FORMULA_NAME ,CIGARETTE_CODE ,ISACTIVE ,FORMULADATE ,OPERATER );
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }
        // GET: /Formula/Details/
        public ActionResult SubDetails(int page, int rows, string FORMULA_CODE)
        {
            var users = FormulaService.GetSubDetails(page, rows, FORMULA_CODE);
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /Formula/Create/
        [HttpPost]
        public ActionResult Create(WMS_FORMULA_MASTER master,object detail)
        {
            string userid = this.GetCookieValue("userid");
            master.OPERATER = userid;
            bool bResult = FormulaService.Add(master, detail);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        // POST: /Formula/Create/
        [HttpPost]
        public ActionResult Edit(WMS_FORMULA_MASTER master, object detail)
        {
            bool bResult = FormulaService.Edit(master, detail);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /Formula/Delete/
        [HttpPost]
        public ActionResult Delete(string FORMULA_CODE)
        {
            bool bResult = FormulaService.Delete(FORMULA_CODE);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, "失败原因可能是该配方被使用中"), "text/html", JsonRequestBehavior.AllowGet);
        }

        // POST: /Formula/GetFormulaCode/

        public ActionResult GetFormulaCode(System.DateTime dtime, string FORMULA_CODE)
        {
            string userName = this.GetCookieValue("username");
            var FormulaInfo = FormulaService.GetFormulaCode(userName, dtime, FORMULA_CODE);

            return Json(FormulaInfo, "text/html", JsonRequestBehavior.AllowGet);
        }
        //获取用户所选牌号的配方
        public ActionResult copy_detail(int page, int rows, string cigarettecode)
        {
            var users = FormulaService.GetSubDetailbyCigarettecode (page, rows, cigarettecode );
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }
        //获取用户所选牌号的已启用配方.
        public ActionResult Getuserfull(int page, int rows, string cigarettecode)
        {
            var users = FormulaService.Getusefull(page, rows, cigarettecode);
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }
        //验证配方编号
        public ActionResult Checkformulacode(string formulacode)
        {
            bool bResult = FormulaService.Checkformulacode(formulacode);
            string msg = bResult ? "1" : "-1";
            var just = new
            {
                 success=msg
            };
            return Json(just, "text/html", JsonRequestBehavior.AllowGet);

        }
        //打印
        public ActionResult Print(string FORMULACODE, string BILLDATEFROM, string BILLDATETO, string FORMULANAME, string ISACTIVE, string CIGARETTE_CODE)
        {
            //string Path = Server.MapPath("/");
            string userName = this.GetCookieValue("username");
            bool Result = FormulaService.FormulaPrint(FORMULACODE, BILLDATEFROM, BILLDATETO, FORMULANAME, ISACTIVE, CIGARETTE_CODE);
            string msg = Result ? "成功" : "失败";
            var just = new
            {
                success = msg
            };
            return Json(just, "text", JsonRequestBehavior.AllowGet);
        }
    }
}
