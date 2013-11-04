using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Data.Objects;
using FastReport;
using FastReport.Utils;
using FastReport.Web;
using System.Reflection;


namespace THOK.Wms.Bll.Service
{
    class WMSProductStateService : ServiceBase<WMS_PRODUCT_STATE>, IWMSProductStateService
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public IWMSProductStateRepository ProductStateRepository { get; set; }
        [Dependency]
        public ISysTableStateRepository SysTableStateRepository { get; set; }
        [Dependency]
        public ICMDCellRepository cellRepository { get; set; }
        [Dependency]
        public ICMDProuductRepository ProductRepository { get; set; }
        [Dependency]
        public IPrintReportRepository PrintReportRepository { get; set; }
        [Dependency]
        public IWorkSelectRepository WorkselectRepository { get; set; }

        //public bool test() {
        //    OracleConnection ora = new OracleConnection(); 
        //}

        public object Details(int page, int rows, string billno)
        {
            IQueryable<WMS_PRODUCT_STATE> query = ProductStateRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            IQueryable<CMD_PRODUCT> productquery = ProductRepository.GetQueryable();
            var details = from a in query
                          join b in statequery on a.IS_MIX equals b.STATE
                          join c in productquery on a.PRODUCT_CODE equals c.PRODUCT_CODE 
                          where b.TABLE_NAME == "WMS_PRODUCT_STATE" && b.FIELD_NAME == "IS_MIX"
                          select new
                          {
                              a.BILL_NO,
                              a.ITEM_NO,
                              a.SCHEDULE_NO,
                              a.PRODUCT_CODE,
                              c.PRODUCT_NAME ,
                              a.WEIGHT,
                              a.REAL_WEIGHT,
                              a.PACKAGE_COUNT,
                              a.OUT_BILLNO,
                              a.CELL_CODE,
                              a.NEWCELL_CODE,
                              a.PRODUCT_BARCODE,
                              a.PALLET_CODE,
                              a.IS_MIX, //是否混装代码
                              IS_MIXDESC = b.STATE_DESC  //是否混装,文字显示
                          };

            var temp = details.Where(i => i.BILL_NO == billno).OrderBy(i => i.ITEM_NO).Select(i => i);
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };

        }

        //入库单据拆分为作业
        public bool Task(string billno, string btypecode, string tasker, out string error)
        {
            string sqlstr = "begin stockinwork('" + billno + "','" + btypecode + "','" + tasker + "');end;";
            int result = ProductStateRepository.Exeprocedure(sqlstr, out  error);
            //return ((ObjectContext)RepositoryContext).ExecuteStoreCommand("","");
            if (result < 0)
                return false;
            else
                return true;
            //((ObjectContext)RepositoryContext).ExecuteStoreCommand("", "");
        }

        //出库作业.
        public bool Task(string billno, string cigarettecode, string formulacode, string batchweight, string tasker, out string error)
        {
            string sqlstr = "begin STOCKOUTWORK('" + billno + "','" + cigarettecode + "','" + formulacode + "'," + batchweight + ",'" + tasker + "');end;";
            //string sqlstr = "update WMS_BILL_MASTER set state='2' where bill_no='"+billno+"' ";
            int result = ProductStateRepository.Exeprocedure(sqlstr, out error);
            //return ((ObjectContext)RepositoryContext).ExecuteStoreCommand("","");
            if (result < 0)
                return false;
            else
                return true;
        }

        //托盘入库作业
        public bool Task(string billno, string tasker, out string error)
        {
            string sqlstr = "begin PALLETSTOCKINWORK('" + billno + "','" + tasker + "');end;";
            int result = ProductStateRepository.Exeprocedure(sqlstr, out  error);
            //return ((ObjectContext)RepositoryContext).ExecuteStoreCommand("","");
            if (result < 0)
                return false;
            else
                return true;
        }

        //获取要补料的单据下的产品条码(只获取可以补料的条码)
        public object Barcodeselect(int page, int rows, string soursebillno)
        {
            IQueryable<WMS_PRODUCT_STATE> query = ProductStateRepository.GetQueryable();
            IQueryable<CMD_CELL> cellquery = cellRepository.GetQueryable();
            IQueryable<CMD_PRODUCT> productquery = ProductRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            var barcode = from a in query
                          join c in productquery on a.PRODUCT_CODE equals c.PRODUCT_CODE
                          join d in statequery on a.IS_MIX equals d.STATE
                          where a.BILL_NO == soursebillno && d.TABLE_NAME == "WMS_PRODUCT_STATE" && d.FIELD_NAME == "IS_MIX"
                          && !(from b in cellquery where b.BILL_NO == soursebillno select b.PRODUCT_BARCODE).Contains(a.PRODUCT_BARCODE)
                          select new
                          {
                              a.ITEM_NO,
                              a.PRODUCT_CODE,
                              a.PRODUCT_BARCODE,
                              c.PRODUCT_NAME,
                              a.SCHEDULE_NO,
                              a.WEIGHT,
                              a.REAL_WEIGHT,
                              a.OUT_BILLNO,
                              a.IS_MIX,
                              IS_MIXDESC = d.STATE_DESC,  //是否混装,文字显示
                              a.PACKAGE_COUNT
                          };
            var temp = barcode.OrderBy(i => i.ITEM_NO).Select(i => i);
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }
        
        public string GetPdfName(string Path, string username, string barcodes, string billno,string PrintCount)
        {
            string FileName = "";
            try
            {
                IQueryable<PRINTREPORT> query = PrintReportRepository.GetQueryable();
                var que2 = query.Where(i => i.BILL_NO == billno && barcodes.Contains(i.PRODUCT_BARCODE)).Select(i => new
                {
                    i.BILL_NO,
                    i.PRODUCT_BARCODE,
                    i.BILL_DATE,
                    i.CATEGORY_NAME,
                    i.CIGARETTE_NAME,
                    i.FORMULA_NAME,
                    i.GRADE_NAME,
                    i.ORIGINAL_NAME,
                    i.PRODUCT_CODE,
                    i.PRODUCT_NAME,
                    i.REAL_WEIGHT,
                    i.STYLE_NAME,
                    i.YEARS
                });
                var que = que2.ToArray().Select(i => new
                {
                    i.BILL_NO,
                    PRODUCT_BARCODE = i.PRODUCT_BARCODE + "00000000000000",
                    BILL_DATE = i.BILL_DATE.Value.ToString("yyyy-MM-dd"),
                    i.CATEGORY_NAME,
                    i.CIGARETTE_NAME,
                    i.FORMULA_NAME,
                    i.GRADE_NAME,
                    i.ORIGINAL_NAME,
                    i.PRODUCT_CODE,
                    i.PRODUCT_NAME,
                    i.REAL_WEIGHT,
                    i.STYLE_NAME,
                    i.YEARS
                });
                DataTable dt = THOK.Common.ConvertData.LinqQueryToDataTable(que);
                using (Report report = new Report())
                {
                    report.Load(Path + @"ContentReport\Report\barcodeprint.frx");

                    
                    report.RegisterData(dt.DefaultView, "printreport");

                    report.Prepare();
                    report.FinishReport += new EventHandler(report_FinishReport);
                    FileName = Path + @"ContentReport\PDF\barcodeprint_" + username + "_" + PrintCount + ".pdf";
                    FastReport.Export.Pdf.PDFExport pdfExport = new FastReport.Export.Pdf.PDFExport();
                    //if (System.IO.File.Exists(FileName))
                    //    System.IO.File.Delete(FileName);
                    report.Export(pdfExport, FileName);
                    //string st = report.FinishReportEvent;
                    //report.OnFinishReport(new EventArgs());
                    FileName = "barcodeprint_" + username + "_" + PrintCount + ".pdf";
                }
            }
            catch (Exception ex)
            {

            }
            return FileName;

        }

        void report_FinishReport(object sender, EventArgs e)
        {
            Report obj = (Report)sender;
            if (obj.Operation == ReportOperation.Exporting) { }
            //throw new NotImplementedException();
        }


        //紧急补料作业
        public bool FeedingTask(string billno, string tasker, out string error)
        {
            string sqlstr = "begin FEEDINGWORK('" + billno + "','" + tasker + "');end;";
            int result = ProductStateRepository.Exeprocedure(sqlstr, out  error);
            //return ((ObjectContext)RepositoryContext).ExecuteStoreCommand("","");
            if (result < 0)
                return false;
            else
                return true;
        }

       // 作业查询
        public object Worksearch(int page, int rows, string BILL_NO, string TASK_DATE, string BTYPE_CODE, string BILLMETHOD, string CIGARETTE_CODE, string FORMULA_CODE, string PRODUCT_BARCODE)
        {
            IQueryable<WORKSELECT> query = WorkselectRepository.GetQueryable();
            var work = query.OrderBy(i => i.TASK_DATE).Select(i => new
            {
                i.BILL_NO,
                i.PRODUCT_CODE,
                i.PRODUCT_BARCODE,
                i.REAL_WEIGHT,
                i.TARGET_CODE,
                i.STATE,
                i.TASK_DATE,
                i.TASKER,
                i.MIXNAME,
                i.PRODUCT_NAME,
                i.CATEGORY_NAME,
                i.ORIGINAL_NAME,
                i.GRADE_NAME,
                i.STYLE_NAME,
                i.BTYPE_CODE,
                i.BTYPE_NAME,
                i.BILL_METHOD,
                i.BILLMETHOD,
                i.CIGARETTE_CODE,
                i.CIGARETTE_NAME,
                i.FORMULA_CODE,
                i.FORMULA_NAME
            });
            if (!string.IsNullOrEmpty(BILL_NO))
            {
                work = work.Where(i => i.BILL_NO == BILL_NO);
            }
            if (!string.IsNullOrEmpty(TASK_DATE))
            {
                DateTime date = DateTime.Parse(TASK_DATE);
                DateTime date2 = date.AddDays(1);
                work = work.Where(i => i.TASK_DATE.Value.CompareTo(date) >= 0);
                work = work.Where(i => i.TASK_DATE.Value.CompareTo(date2) < 0);
            }
            if (!string.IsNullOrEmpty(BTYPE_CODE)) {
                work = work.Where(i => i.BTYPE_CODE == BTYPE_CODE);
            }
            if (!string.IsNullOrEmpty(BILLMETHOD)) {
                work = work.Where(i => i.BILL_METHOD == BILLMETHOD);
            }
            if (!string.IsNullOrEmpty(CIGARETTE_CODE)) {
                work = work.Where(i => i.CIGARETTE_CODE == CIGARETTE_CODE);
            }
            if (!string.IsNullOrEmpty(FORMULA_CODE)) {
                work = work.Where(i => i.FORMULA_CODE == FORMULA_CODE);
            }
            if (!string.IsNullOrEmpty(PRODUCT_BARCODE)) {
                work = work.Where(i => i.PRODUCT_BARCODE == PRODUCT_BARCODE);
            }
            var temp = work.ToArray().Select(i => new {
                i.BILL_NO,
                i.PRODUCT_CODE,
                i.PRODUCT_BARCODE,
                i.REAL_WEIGHT,
                i.TARGET_CODE,
                i.STATE,
                TASK_DATE = i.TASK_DATE == null ? "" : ((DateTime)i.TASK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                i.TASKER,
                i.MIXNAME,
                i.PRODUCT_NAME,
                i.CATEGORY_NAME,
                i.ORIGINAL_NAME,
                i.GRADE_NAME,
                i.STYLE_NAME,
                i.BTYPE_CODE,
                i.BTYPE_NAME,
                i.BILL_METHOD,
                i.BILLMETHOD,
                i.CIGARETTE_CODE,
                i.CIGARETTE_NAME,
                i.FORMULA_CODE,
                i.FORMULA_NAME
            });
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }


        //public object Worksearch(int page, int rows, string BILL_NO, string TASK_DATE, string BTYPE_CODE, string BILLMETHOD, string CIGARETTE_CODE, string FORMULA_CODE, string PRODUCT_BARCODE)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
