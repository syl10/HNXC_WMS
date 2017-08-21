using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using THOK.WebUtil;
using THOK.Security;
using Wms.Security;

namespace Authority.Controllers.Wms.WarehouseInfo
{
    [TokenAclAuthorize]
    [SystemEventLog]
    public class CellController : Controller
    {
        [Dependency]
        public ICMDCellService CellService { get; set; }

        //
        // GET: /Cell/

        public ActionResult Index()
        {
            return View();
        }

        //查询货位信息表
        // POST: /Cell/CellDetails
        [HttpPost]
        public ActionResult CellDetails(int page, int rows, string cellCode)
        {
            var cell = CellService.GetDetails(page, rows, cellCode);
            return Json(cell, "text/html", JsonRequestBehavior.AllowGet);
        }

        //查询货位信息表
        // POST: /Cell/FindCell
        [HttpPost]
        public ActionResult FindCell(string parameter)
        {
            var cell = CellService.FindCell(parameter);
            return Json(cell, "text/html", JsonRequestBehavior.AllowGet);
        }

        //查询仓库数据treegrid结构，用于修改
        // GET: /Cell/Details/
        public ActionResult Details(string wareCode)
        {
            var wareCell = CellService.GetSearch(wareCode);
            return Json(wareCell, "text/html", JsonRequestBehavior.AllowGet);
        }

        //查询仓库数据treegrid结构，用于分拣设置货位
        // GET: /Cell/GetSortCell/
        public ActionResult GetSortCell(string areaType)
        {
            var wareCell = CellService.GetSortCell(areaType);
            return Json(wareCell, "text/html", JsonRequestBehavior.AllowGet);
        }

        //查询仓库数据tree结构,用于盘点
        // GET: /Cell/GetCheckDetails/
        public ActionResult GetCheckDetails(string id)
        {
            var wareCell = CellService.GetWareCheck(id);
            return Json(wareCell, "text/html", JsonRequestBehavior.AllowGet);
        }

        //添加货位信息表
        // POST: /Cell/CellCreate
        [HttpPost]
        public ActionResult CellCreate(CMD_CELL cell)
        {
            string errorInfo = string.Empty;
            bool bResult = CellService.Add(cell, out errorInfo);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, errorInfo), "text/html", JsonRequestBehavior.AllowGet);
        }

        //编辑货位表
        // GET: /Cell/Edit/
        public ActionResult Edit(CMD_CELL cell)
        {
            bool bResult = CellService.Save(cell);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        //删除货位表
        // POST: /Cell/Delete/
        [HttpPost]
        public ActionResult Delete(string cellCode)
        {
            bool bResult = CellService.Delete(cellCode);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /Cell/GetCell/
        public ActionResult GetCell(string shelfCode)
        {
            var wareCell = CellService.GetCell(shelfCode); ;
            return Json(wareCell, "text/html", JsonRequestBehavior.AllowGet);
        }

        ////根据条件查询仓库数据tree结构,用于移库
        //// GET: /Cell/GetMoveCellDetails/
        //public ActionResult GetMoveCellDetails(string id, string inOrOut, string productCode)
        //{
        //    var wareCell = CellService.GetMoveCellDetails(id,inOrOut,productCode);
        //    return Json(wareCell, "text", JsonRequestBehavior.AllowGet);
        //}

        //获取生成的货位编码
        // GET: /Cell/GetCellCode/
        public ActionResult GetCellCode(string shelfCode)
        {
            var cellCode = CellService.GetCellCode(shelfCode);
            return Json(cellCode, "text/html", JsonRequestBehavior.AllowGet);
        }
    }
}
