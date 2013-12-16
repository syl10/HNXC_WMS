using System.Web.Mvc;
using System.Web.Routing;
using System.Text;
using Microsoft.Practices.Unity;
using THOK.WebUtil;
using THOK.Authority.Bll.Interfaces;
using System;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;

namespace WMS.Controllers.Wms.Base
{
    public class ProductController : Controller
    {
        [Dependency]
        public ICMDProductService ProductService { get; set; }

        [Dependency]
        public ICMDPorductStyleService Productstyle { get; set; }
        //
        // GET: /Product/

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasAdd = true;
            ViewBag.hasEdit = true;
            ViewBag.hasDelete = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.hasCopy = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        // GET: /Product/Details/
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            string PRODUCT_NAME = collection["PRODUCT_NAME"] ?? "";
            string ORIGINAL = collection["ORIGINAL"] ?? "";
            string YEARS = collection["YEARS"] ?? "";
            string GRADE = collection["GRADE"] ?? "";
            string STYLE = collection["STYLE"] ?? "";
            string WEIGHT = collection["WEIGHT"] ?? "";
            string CATEGORY_CODE = collection["CATEGORY_CODE"] ?? "";
            string MEMO = collection["MEMO"] ?? "";
            string print = collection["PRINT"] ?? "";
            if (print == "1")
            {
                THOK.Common.PrintHandle.isbase = true;
            }
            else
            {
                THOK.Common.PrintHandle.isbase = false;
            }
            //string username = collection["username"] ?? "";
            var Products = ProductService.GetDetails(page, rows, PRODUCT_NAME,ORIGINAL,YEARS,GRADE,STYLE,WEIGHT,MEMO,CATEGORY_CODE);
            return Json(Products, "text/html", JsonRequestBehavior.AllowGet);
        }
        // POST: /Product/Create/
        [HttpPost]
        public ActionResult Create(CMD_PRODUCT product)
        {
            bool bResult = ProductService.Add(product);
            string msg = bResult ? "新增成功" : "新增失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        // POST: /Product/Edit/
        [HttpPost]
        public ActionResult Edit(CMD_PRODUCT product)
        {
            bool bResult = ProductService.Save(product);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }
        // POST: /Product/Delete/
        [HttpPost]
        public ActionResult Delete(string PRODUCT_CODE)
        {
            bool bResult = ProductService.Delete(PRODUCT_CODE);
            string msg = bResult ? "删除成功" : "无法删除,可能原因是该产品正被使用中";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, null), "text/html", JsonRequestBehavior.AllowGet);
        }

        //获取形态列表
        public ActionResult Getstylelist(int page, int rows)
        {
            var users = Productstyle.GetDetails(page, rows);
            return Json(users, "text/html", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetProductDetails(int page, int rows,string QueryString, string Value)
        {
            var Products = ProductService.Selectprod (page, rows,QueryString ,Value);
            return Json(Products, "text/html", JsonRequestBehavior.AllowGet);
        }
    }
}


