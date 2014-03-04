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
    public class ShelfController : Controller
    {
        [Dependency]
        public ICMDShelfService ShelfService { get; set; }
        //
        // GET: /Shelf/

        public ActionResult Index()
        {
            return View();
        }

        //查询货架信息表
        // POST: /Shelf/Details
        [HttpPost]
        public ActionResult Details(string warehouseCode, string areaCode, string shelfCode)
        {
            var shelf = ShelfService.GetDetails(warehouseCode, areaCode, shelfCode);
            return Json(shelf, "text/html", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Detail(string type, string id)
        {
            var shelf = ShelfService.GetDetail(type, id);
            return Json(shelf, "text/html", JsonRequestBehavior.AllowGet);
        }

        //查询货架信息表
        // POST: /Shelf/FindShelf
        [HttpPost]
        public ActionResult FindShelf(string parameter)
        {
            var shelf = ShelfService.FindShelf(parameter);
            return Json(shelf, "text/html", JsonRequestBehavior.AllowGet);
        }

        //添加货架信息表
        // POST: /Shelf/ShelfCreate
        [HttpPost]
        public ActionResult ShelfCreate(CMD_SHELF shelf)
        {
            bool bResult = ShelfService.Add(shelf);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        //编辑货架表
        // GET: /Shelf/Edit/
        public ActionResult Edit(CMD_SHELF shelf)
        {
            bool bResult = ShelfService.Save(shelf);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        //删除货架表
        // POST: /Shelf/Delete/
        [HttpPost]
        public ActionResult Delete(string shelfCode)
        {
            bool bResult = ShelfService.Delete(shelfCode);
            string msg = bResult ? "删除成功" : "删除失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        //获取生成的货架编码
        // GET: /Shelf/GetShelfCode/
        public ActionResult GetShelfCode(string areaCode)
        {
            var shelfCode = ShelfService.GetShelfCode(areaCode);
            return Json(shelfCode, "text/html", JsonRequestBehavior.AllowGet);
        }
    }
}
