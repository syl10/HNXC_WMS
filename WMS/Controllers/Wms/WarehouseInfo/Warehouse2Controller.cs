using System;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using THOK.WebUtil;
using THOK.Wms.Bll.Service;
using THOK.Security;
using Wms.Security;
namespace Wms.Controllers.Wms.WarehouseInfo
{
    [TokenAclAuthorize]
    [SystemEventLog]
    public class Warehouse2Controller : Controller
    {
        [Dependency]
        public ICMDWarehouseService WarehouseService { get; set; }
        //[Dependency]
        //public ICargospaceService CargospaceService { get; set; }
        [Dependency]
        public ICMDCellService CellService { get; set; }
        //
        // GET: /Warehouse2/

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            //ViewBag.hasAdd = true;
            ViewBag.hasEdit = true;
            //ViewBag.hasDelete = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        //查询仓库信息表
        // POST: /Warehouse/Details
        [HttpPost]
        public ActionResult Details(int page, int rows, string type, string id)
        {
            var warehouse = CellService.GetDetail(page, rows, type, id);
            return Json(warehouse, "text/html", JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetSingleDetail(string type, string id)
        {
            var warehouse = CellService.GetSingleDetail(type, id);
            return Json(warehouse, "text/html", JsonRequestBehavior.AllowGet);
        }
        //查询仓库信息表
        // POST: /Warehouse/FindWarehouse
        [HttpPost]
        public ActionResult FindWarehouse(string parameter)
        {
            var warehouse = WarehouseService.FindWarehouse(parameter);
            return Json(warehouse, "text/html", JsonRequestBehavior.AllowGet);
        }

        //添加仓库信息表
        // POST: /Warehouse/WareCreate
        [HttpPost]
        public ActionResult WareCreate(CMD_WAREHOUSE warehouse)
        {
            bool bResult = WarehouseService.Add(warehouse);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        //编辑仓库表
        // GET: /Warehouse/Edit/
        public ActionResult Edit(CMD_WAREHOUSE warehouse)
        {
            bool bResult = WarehouseService.Save(warehouse);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        //删除仓库表
        // POST: /Warehouse/Delete/
        [HttpPost]
        public ActionResult Delete(string warehouseCode)
        {
            bool bResult = WarehouseService.Delete(warehouseCode);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        //获取生成的仓库编码
        // GET: /Warehouse/GetWareCode/
        public ActionResult GetWareCode()
        {
            var warehouseCode = WarehouseService.GetWareCode();
            return Json(warehouseCode, "text/html", JsonRequestBehavior.AllowGet);
        }

        #region /Warehouse2/CreateExcelToClient/
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;
            string type = Request.QueryString["type"];
            string id = Request.QueryString["id"];

            THOK.NPOI.Models.ExportParam ep = new THOK.NPOI.Models.ExportParam();
            ep.DT1 = CellService.GetCell(page, rows, type, id);
            ep.HeadTitle1 = "仓库信息";
            System.IO.MemoryStream ms = THOK.NPOI.Service.ExportExcel.ExportDT(ep);
            return new FileStreamResult(ms, "application/ms-excel");
        }
        #endregion
    }
}
