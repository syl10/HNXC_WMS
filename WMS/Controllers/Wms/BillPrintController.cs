using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FastReport.Web;
using FastReport;
using THOK.Common;

namespace WMS.Controllers.Wms
{
    public class BillPrintController : Controller
    {
        //
        // GET: /BillPrint/
        private WebReport webReport = new WebReport();
        public ActionResult Index(string frxname, string dataname)
        {
            string Path = Server.MapPath("/");
            Path += @"ContentReport\Report\" + frxname;
            SetReport(Path, dataname);
            webReport.Width = 1020;
            webReport.Height = 390;
            webReport.ToolbarStyle = ToolbarStyle.Small;
            webReport.ToolbarIconsStyle = ToolbarIconsStyle.Blue;
           
            webReport.ToolbarBackgroundStyle = ToolbarBackgroundStyle.Light;
            ViewBag.WebReport = webReport;
            return View();
        }

        private void SetReport(string frxpath, string dataname)
        {
            webReport.Report.RegisterData(PrintHandle.dt.DefaultView, dataname);
            webReport.Report.Load(frxpath);
        }

    }
}
