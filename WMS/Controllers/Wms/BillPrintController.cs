using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FastReport.Web;
using FastReport;
using THOK.Common;
using THOK.Security;

namespace WMS.Controllers.Wms
{
    [TokenAclAuthorize]
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
           
            return View();
        }

        private void SetReport(string frxpath, string dataname)
        {
            try
            {
                if (PrintHandle.dt.DefaultView.Count > 0)
                {
                    webReport.Report.RegisterData(PrintHandle.dt.DefaultView, dataname);
                    webReport.Report.Load(frxpath);
                    webReport.Width =930;
                    webReport.Height = 390;
                    //webReport.ShowPageNumber = false;

                    webReport.ToolbarStyle = ToolbarStyle.Small;
                    webReport.ToolbarIconsStyle = ToolbarIconsStyle.Blue;

                    webReport.ToolbarBackgroundStyle = ToolbarBackgroundStyle.Light;
                    ViewBag.WebReport = webReport;
                }
            }
            catch (Exception ex) {
                string Path = Server.MapPath("/");
                Path += @"ContentReport\Report\errorreport.frx";
                webReport.Report.Load(Path);
            }
        }

    }
}
