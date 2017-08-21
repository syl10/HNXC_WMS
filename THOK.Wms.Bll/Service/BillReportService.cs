using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using System.Data;
using FastReport;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;

namespace THOK.Wms.Bll.Service
{
    class BillReportService : ServiceBase<BILLREPORT >, IBillReportService
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public IBillReportRepository  BillReportRepository { get; set; }
        [Dependency]
        public IWMSBillMasterRepository BillMasterRepository { get; set; }
        [Dependency]
        public IWMSProductStateRepository ProductStateRepository { get; set; }
        [Dependency]
        public ISysTableStateRepository SysTableStateRepository { get; set; }
        [Dependency]
        public IUserRepository UserRepository { get; set; }
        //入库单
        public bool  StockinPrint(string flag, string username, string PrintCount, string BILLNO, string BILLDATEFROM, string BILLDATETO, string BTYPECODE, string BILLMETHOD, string STATE, string CIGARETTECODE, string FORMULACODE)
        {
            try
            {
                IQueryable<BILLREPORT> query = BillReportRepository.GetQueryable();
                var stockin = query.Where (i=>i.BILL_TYPE =="1") .Select(i => new {
                    i.MBILL_NO,  //单号
                    i.BILL_DATE, //日期
                    i.BATCH_WEIGHT, //批次重量
                    i.BILL_METHOD, //单据方式
                    i.BILLMETHODCODE,
                    i.BTYPE_NAME, //单据类型
                    i.BTYPE_CODE ,
                    i.CATEGORY_NAME, //产品类别
                     i.CHECK_DATE ,
                    i.CHECKER,
                    i.CIGARETTE_NAME,  //牌号
                    i.CIGARETTE_CODE,
                    i.DBILL_NO, //
                    i.FORMULA_NAME, //配方
                    i.FORMULA_CODE ,
                    i.GRADE_NAME, //产品等级
                    i.IS_MIX , //混装
                    i.ITEM_NO, //序号
                    i.LINE_NAME, //制丝线
                    i.NC_COUNT,
                    i.OPERATE_DATE,
                    i.OPERATER,
                    i.ORIGINAL_NAME, //产地
                    i.PACKAGE_COUNT,
                    i.PRODUCT_NAME, //产品名称
                    i.REAL_WEIGHT, //实际重量
                    i.SOURCE_BILLNO,
                    i.STATE, //状态
                    i.STATUS,
                    i.STYLE_NAME, //形态 
                    i.TARGET_NAME, //
                     i.TASK_DATE ,
                    i.TASKER,
                    i.WAREHOUSE_NAME,
                    i.YEARS,
                    i.FPRODUCT_CODE
                });
                if (!string.IsNullOrEmpty(BILLNO)) {
                    stockin = stockin.Where(i => i.MBILL_NO == BILLNO); 
                }
                if (!string.IsNullOrEmpty(BILLDATEFROM)) {
                    DateTime datestare = DateTime.Parse(BILLDATEFROM);
                    stockin = stockin.Where(i => i.BILL_DATE.CompareTo(datestare) >= 0);
                }
                if (!string.IsNullOrEmpty(BILLDATETO)) {
                    DateTime dateend = DateTime.Parse(BILLDATETO);
                    stockin = stockin.Where(i => i.BILL_DATE.CompareTo(dateend) <= 0);
                }
                if (!string.IsNullOrEmpty(BTYPECODE)) {
                    stockin = stockin.Where(i => i.BTYPE_CODE == BTYPECODE);
                }
                if (!string.IsNullOrEmpty(BILLMETHOD)) {
                    stockin = stockin.Where(i => i.BILLMETHODCODE == BILLMETHOD);
                }
                if (!string.IsNullOrEmpty(STATE)) {
                    stockin = stockin.Where(i => i.STATE == STATE);
                }
                if (!string.IsNullOrEmpty(CIGARETTECODE)) {
                    stockin = stockin.Where(i => i.CIGARETTE_CODE == CIGARETTECODE);
                }
                if (!string.IsNullOrEmpty(FORMULACODE)) {
                    stockin = stockin.Where(i => i.FORMULA_CODE == FORMULACODE);
                }
                if (flag == "2") { //抽检不料入库单
                    stockin = stockin.Where(i => "2,3".Contains(i.BILLMETHODCODE));
                }
                if (flag == "1") { //入库单
                    stockin = stockin.Where(i => "0,1".Contains(i.BILLMETHODCODE));
                }
                if (THOK.Common.PrintHandle.issearch)
                { //判断是否是综合查询里的打印
                    //var query1 = from a in dt.AsEnumerable() select a;
                    var query2 = from b in THOK.Common.PrintHandle.searchdt.AsEnumerable() select b.Field <string>("BILL_NO");
                    stockin = stockin.Where(i => query2.Contains(i.MBILL_NO));
                    THOK.Common.PrintHandle.issearch = false;
                    //dt = query1.CopyToDataTable();
                }
                var printdata = stockin.ToArray().Select(i => new
                {
                    i.MBILL_NO,  //单号
                    BILL_DATE = i.BILL_DATE.ToString("yyyy-MM-dd"), //日期
                    i.BATCH_WEIGHT, //批次重量
                    i.BILL_METHOD, //单据方式
                    i.BTYPE_NAME, //单据类型
                    i.CATEGORY_NAME, //产品类别
                    CHECK_DATE = i.CHECK_DATE == null ? "" : ((DateTime)i.CHECK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.CHECKER,
                    i.CIGARETTE_NAME,  //牌号
                    i.DBILL_NO, //
                    i.FORMULA_NAME, //配方
                    i.GRADE_NAME, //产品等级
                    ISMIX = i.IS_MIX == "1" ? "是" : "否", //混装
                    i.ITEM_NO, //序号
                    i.LINE_NAME, //制丝线
                    i.NC_COUNT,
                    OPERATE_DATE = i.OPERATE_DATE == null ? "" : ((DateTime)i.OPERATE_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.OPERATER,
                    i.ORIGINAL_NAME, //产地
                    i.PACKAGE_COUNT,
                    i.PRODUCT_NAME, //产品名称
                    i.REAL_WEIGHT, //实际重量
                    i.SOURCE_BILLNO,
                    i.STATE, //状态
                    i.STATUS,
                    i.STYLE_NAME, //形态 
                    i.TARGET_NAME, //
                    TASK_DATE = i.TASK_DATE == null ? "" : ((DateTime)i.TASK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.TASKER,
                    i.WAREHOUSE_NAME,
                    i.YEARS,
                    i.FPRODUCT_CODE
                }).OrderBy(I => I.ITEM_NO);
                DataTable dt = THOK.Common.ConvertData.LinqQueryToDataTable(printdata);
                THOK.Common.PrintHandle.dt = dt;
                //THOK.Common.PrintHandle.Onstockinprint(dt, "billreportprint");
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        }

        //出库单打印
        public bool StockoutPrint(string BILLNO, string BILLDATEFROM, string BILLDATETO, string BTYPECODE, string LINENO, string STATE, string CIGARETTECODE, string FORMULACODE, string SOURSEBILL, string SCHEDULENO)
        {
            try
            {
                IQueryable<BILLREPORT> query = BillReportRepository.GetQueryable();
                var stockin = query.Where(i => i.BILL_TYPE == "2").Select(i => new
                {
                    i.MBILL_NO,  //单号
                    i.BILL_DATE, //日期
                    i.BATCH_WEIGHT, //批次重量
                    i.BILL_METHOD, //单据方式
                    i.BILLMETHODCODE,
                    i.BTYPE_NAME, //单据类型
                    i.BTYPE_CODE,
                    i.CATEGORY_NAME, //产品类别
                    i.CHECK_DATE,
                    i.CHECKER,
                    i.CIGARETTE_NAME,  //牌号
                    i.CIGARETTE_CODE,
                    i.DBILL_NO, //
                    i.FORMULA_NAME, //配方
                    i.FORMULA_CODE,
                    i.GRADE_NAME, //产品等级
                    i.IS_MIX, //混装
                    i.ITEM_NO, //序号
                    i.LINE_NO ,//
                    i.LINE_NAME, //制丝线
                    i.NC_COUNT,
                    i.OPERATE_DATE,
                    i.OPERATER,
                    i.SCHEDULE_NO ,//计划单号
                    i.SCHEDULE_ITEMNO ,//计划单序号
                    i.ORIGINAL_NAME, //产地
                    i.PACKAGE_COUNT,
                    i.PRODUCT_CODE ,
                    i.PRODUCT_NAME, //产品名称
                    i.WEIGHT ,
                    i.REAL_WEIGHT, //实际重量
                    i.TOTALWEIGHT ,
                    i.SOURCE_BILLNO,
                    i.STATE, //状态
                    i.STATUS,
                    i.STYLE_NAME, //形态 
                    i.TARGET_NAME, //
                    i.TASK_DATE,
                    i.TASKER,
                    i.WAREHOUSE_NAME,
                    i.YEARS,
                    i.FPRODUCT_CODE
                });
                if (!string.IsNullOrEmpty(BILLNO))
                {
                    stockin = stockin.Where(i => i.MBILL_NO == BILLNO);
                }
                if (!string.IsNullOrEmpty(BILLDATEFROM))
                {
                    DateTime datestare = DateTime.Parse(BILLDATEFROM);
                    stockin = stockin.Where(i => i.BILL_DATE.CompareTo(datestare) >= 0);
                }
                if (!string.IsNullOrEmpty(BILLDATETO))
                {
                    DateTime dateend = DateTime.Parse(BILLDATETO);
                    stockin = stockin.Where(i => i.BILL_DATE.CompareTo(dateend) <= 0);
                }
                if (!string.IsNullOrEmpty(BTYPECODE))
                {
                    stockin = stockin.Where(i => i.BTYPE_CODE == BTYPECODE);
                }
                if (!string.IsNullOrEmpty(LINENO ))
                {
                    stockin = stockin.Where(i => i.LINE_NO == LINENO);
                }
                if (!string.IsNullOrEmpty(STATE))
                {
                    stockin = stockin.Where(i => i.STATE == STATE);
                }
                if (!string.IsNullOrEmpty(CIGARETTECODE))
                {
                    stockin = stockin.Where(i => i.CIGARETTE_CODE == CIGARETTECODE);
                }
                if (!string.IsNullOrEmpty(FORMULACODE))
                {
                    stockin = stockin.Where(i => i.FORMULA_CODE == FORMULACODE);
                }
                if (!string.IsNullOrEmpty(SOURSEBILL)) {
                    stockin = stockin.Where(i => i.SOURCE_BILLNO == SOURSEBILL);
                }
                if (!string.IsNullOrEmpty(SCHEDULENO)) {
                    stockin = stockin.Where(i => i.SCHEDULE_NO == SCHEDULENO);
                }
                if (THOK.Common.PrintHandle.issearch)
                { //判断是否是综合查询里的打印
                    //var query1 = from a in dt.AsEnumerable() select a;
                    var query2 = from b in THOK.Common.PrintHandle.searchdt.AsEnumerable() select b.Field<string>("BILL_NO");
                    stockin = stockin.Where(i => query2.Contains(i.MBILL_NO));
                    THOK.Common.PrintHandle.issearch = false;
                    //dt = query1.CopyToDataTable();
                }
                var printdata = stockin.ToArray().Select(i => new
                {
                    i.MBILL_NO,  //单号
                    BILL_DATE = i.BILL_DATE.ToString("yyyy-MM-dd"), //日期
                    i.BATCH_WEIGHT, //批次重量
                    i.BILL_METHOD, //单据方式
                    i.BTYPE_NAME, //单据类型
                    i.CATEGORY_NAME, //产品类别
                    CHECK_DATE = i.CHECK_DATE == null ? "" : ((DateTime)i.CHECK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.CHECKER,
                    i.CIGARETTE_NAME,  //牌号
                    i.DBILL_NO, //
                    i.FORMULA_NAME, //配方
                    i.GRADE_NAME, //产品等级
                    ISMIX = i.IS_MIX == "1" ? "是" : "否", //混装
                    i.ITEM_NO, //序号
                    i.LINE_NAME, //制丝线
                    i.NC_COUNT,
                    i.SCHEDULE_NO,//计划单号
                    i.SCHEDULE_ITEMNO,//计划单序号
                    OPERATE_DATE = i.OPERATE_DATE == null ? "" : ((DateTime)i.OPERATE_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.OPERATER,
                    i.ORIGINAL_NAME, //产地
                    i.PACKAGE_COUNT,
                    i.PRODUCT_CODE,
                    i.PRODUCT_NAME, //产品名称
                    i.WEIGHT,
                    i.REAL_WEIGHT, //实际重量
                    i.TOTALWEIGHT,
                    i.SOURCE_BILLNO,
                    i.STATE, //状态
                    i.STATUS,
                    i.STYLE_NAME, //形态 
                    i.TARGET_NAME, //
                    TASK_DATE = i.TASK_DATE == null ? "" : ((DateTime)i.TASK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.TASKER,
                    i.WAREHOUSE_NAME,
                    i.YEARS,
                    i.FPRODUCT_CODE
                }).OrderBy(I => I.ITEM_NO);
                DataTable dt = THOK.Common.ConvertData.LinqQueryToDataTable(printdata);
                THOK.Common.PrintHandle.dt = dt;
                //THOK.Common.PrintHandle.Onstockinprint(dt, "billreportprint");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //盘点单打印
        public bool InventoryPrint(string BILLNO, string BILLDATEFROM, string BILLDATETO, string STATE, string SOURSEBILL, string btypecode)
        {
            IQueryable<WMS_BILL_MASTER> billquery = BillMasterRepository.GetQueryable();
            IQueryable<WMS_PRODUCT_STATE> productquey = ProductStateRepository.GetQueryable();
            IQueryable <SYS_TABLE_STATE > statequey=SysTableStateRepository .GetQueryable ();
            IQueryable<AUTH_USER> userquery = UserRepository.GetQueryable();
            try
            {
                var inventory = from a in billquery
                                join b in productquey on a.BILL_NO equals b.BILL_NO
                                join c in statequey on a.STATUS equals c.STATE into cf
                                from c in cf.DefaultIfEmpty()
                                join d in statequey on a.STATE equals d.STATE into df
                                from d in df.DefaultIfEmpty()
                                join e in userquery on a.OPERATER equals e.USER_ID into ef
                                from e in ef.DefaultIfEmpty()
                                join f in userquery on a.CHECKER equals f.USER_ID into ff
                                from f in ff.DefaultIfEmpty()
                                join h in userquery on a.TASKER equals h.USER_ID into hf
                                from h in hf.DefaultIfEmpty()
                                where c.TABLE_NAME == "WMS_BILL_MASTER" && c.FIELD_NAME == "STATUS" && d.TABLE_NAME == "WMS_BILL_MASTER" &&
                                d.FIELD_NAME == "STATE" && a.BTYPE_CODE == btypecode
                                select new
                                {
                                    a.BILL_NO,
                                    a.BILL_DATE,
                                    a.WAREHOUSE_CODE,
                                    a.CMD_WAREHOUSE.WAREHOUSE_NAME,
                                    a.SYS_BILL_TARGET.TARGET_NAME,
                                    a.STATUS,
                                    STATUSNAME = c.STATE_DESC,
                                    a.STATE,
                                    STATENAME = d.STATE_DESC,
                                    a.SOURCE_BILLNO,
                                    OPERATER = e.USER_NAME,
                                    a.OPERATE_DATE,
                                    CHECKER = f.USER_NAME,
                                    a.CHECK_DATE,
                                    TASKER = h.USER_NAME,
                                    a.TASK_DATE,
                                    b.ITEM_NO,
                                    b.CELL_CODE,
                                    b.PRODUCT_CODE,
                                    b.WEIGHT,
                                    b.REAL_WEIGHT,
                                    b.PACKAGE_COUNT,
                                    b.PRODUCT_BARCODE,
                                    b.PALLET_CODE,
                                    b.NEWCELL_CODE ,
                                    b.IS_MIX,
                                    ISMIXNAME =b.IS_MIX =="1"?"是":"否"
                                };
                if (!string.IsNullOrEmpty(BILLNO))
                {
                    inventory = inventory.Where(i => i.BILL_NO == BILLNO);
                }
                if (!string.IsNullOrEmpty(BILLDATEFROM))
                {
                    DateTime datestare = DateTime.Parse(BILLDATEFROM);
                    inventory = inventory.Where(i => i.BILL_DATE.CompareTo(datestare) >= 0);
                }
                if (!string.IsNullOrEmpty(BILLDATETO))
                {
                    DateTime dateend = DateTime.Parse(BILLDATETO);
                    inventory = inventory.Where(i => i.BILL_DATE.CompareTo(dateend) <= 0);
                }
                if (!string.IsNullOrEmpty(STATE))
                {
                    inventory = inventory.Where(i => i.STATE == STATE);
                }
                if (!string.IsNullOrEmpty(SOURSEBILL))
                {
                    inventory = inventory.Where(i => i.SOURCE_BILLNO == SOURSEBILL);
                }
                if (THOK.Common.PrintHandle.issearch)
                { //判断是否是综合查询里的打印
                    //var query1 = from a in dt.AsEnumerable() select a;
                    var query2 = from b in THOK.Common.PrintHandle.searchdt.AsEnumerable() select b.Field<string>("BILL_NO");
                    inventory = inventory.Where(i => query2.Contains(i.BILL_NO));
                    THOK.Common.PrintHandle.issearch = false;
                    //dt = query1.CopyToDataTable();
                }
                var printdata = inventory.ToArray().OrderBy(i => i.BILL_NO).Select(i => new
                {
                    i.BILL_NO,
                    BILL_DATE = i.BILL_DATE.ToString("yyyy-MM-dd"),
                    i.WAREHOUSE_NAME,
                    i.TARGET_NAME,
                    i.STATUSNAME,
                    i.STATENAME,
                    i.SOURCE_BILLNO,
                    i.OPERATER,
                    OPERATE_DATE = i.OPERATE_DATE == null ? "" : ((DateTime)i.OPERATE_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.CHECKER,
                    CHECK_DATE = i.CHECK_DATE == null ? "" : ((DateTime)i.CHECK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.TASKER,
                    TASK_DATE = i.TASK_DATE == null ? "" : ((DateTime)i.TASK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.ITEM_NO,
                    i.CELL_CODE,
                    i.PRODUCT_CODE,
                    i.WEIGHT,
                    i.REAL_WEIGHT,
                    i.PACKAGE_COUNT,
                    i.PRODUCT_BARCODE,
                    i.PALLET_CODE,
                    i.NEWCELL_CODE,
                    i.ISMIXNAME
                });
                DataTable dt = THOK.Common.ConvertData.LinqQueryToDataTable(printdata);
                THOK.Common.PrintHandle.dt = dt;
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        }

        //损益单打印
        public bool StockdiffPrint(string BILLNO, string BILLDATEFROM, string BILLDATETO, string STATE, string SOURSEBILL, string btypecode)
        {
            try
            {
                IQueryable<BILLREPORT> query = BillReportRepository.GetQueryable();
                var stockin = query.Where(i => i.BILL_TYPE == "6").Select(i => new
                {
                    i.MBILL_NO,  //单号
                    i.BILL_DATE, //日期
                    i.BATCH_WEIGHT, //批次重量
                    i.BILL_METHOD, //单据方式
                    i.BILLMETHODCODE,
                    i.BTYPE_NAME, //单据类型
                    i.BTYPE_CODE,
                    i.CATEGORY_NAME, //产品类别
                    i.CHECK_DATE,
                    i.CHECKER,
                    i.CIGARETTE_NAME,  //牌号
                    i.CIGARETTE_CODE,
                    i.DBILL_NO, //
                    i.FORMULA_NAME, //配方
                    i.FORMULA_CODE,
                    i.GRADE_NAME, //产品等级
                    i.IS_MIX, //混装
                    i.ITEM_NO, //序号
                    i.LINE_NO,//
                    i.LINE_NAME, //制丝线
                    i.NC_COUNT,
                    i.OPERATE_DATE,
                    i.OPERATER,
                    i.SCHEDULE_NO,//计划单号
                    i.SCHEDULE_ITEMNO,//计划单序号
                    i.ORIGINAL_NAME, //产地
                    i.PACKAGE_COUNT,
                    i.PRODUCT_CODE,
                    i.PRODUCT_NAME, //产品名称
                    i.WEIGHT,
                    i.REAL_WEIGHT, //实际重量
                    i.TOTALWEIGHT,
                    i.SOURCE_BILLNO,
                    i.STATE, //状态
                    i.STATUS,
                    i.STYLE_NAME, //形态 
                    i.TARGET_NAME, //
                    i.TASK_DATE,
                    i.TASKER,
                    i.WAREHOUSE_NAME,
                    i.YEARS,
                    i.FPRODUCT_CODE
                });
                if (!string.IsNullOrEmpty(BILLNO))
                {
                    stockin = stockin.Where(i => i.MBILL_NO == BILLNO);
                }
                if (!string.IsNullOrEmpty(BILLDATEFROM))
                {
                    DateTime datestare = DateTime.Parse(BILLDATEFROM);
                    stockin = stockin.Where(i => i.BILL_DATE.CompareTo(datestare) >= 0);
                }
                if (!string.IsNullOrEmpty(BILLDATETO))
                {
                    DateTime dateend = DateTime.Parse(BILLDATETO);
                    stockin = stockin.Where(i => i.BILL_DATE.CompareTo(dateend) <= 0);
                }
                if (!string.IsNullOrEmpty(STATE))
                {
                    stockin = stockin.Where(i => i.STATE == STATE);
                }
                if (!string.IsNullOrEmpty(SOURSEBILL))
                {
                    stockin = stockin.Where(i => i.SOURCE_BILLNO == SOURSEBILL);
                }
                if (!string.IsNullOrEmpty(btypecode)) {
                    stockin = stockin.Where(i => i.BTYPE_CODE == btypecode);
                }
                if (THOK.Common.PrintHandle.issearch)
                { //判断是否是综合查询里的打印
                    //var query1 = from a in dt.AsEnumerable() select a;
                    var query2 = from b in THOK.Common.PrintHandle.searchdt.AsEnumerable() select b.Field<string>("BILL_NO");
                    stockin = stockin.Where(i => query2.Contains(i.MBILL_NO));
                    THOK.Common.PrintHandle.issearch = false;
                    //dt = query1.CopyToDataTable();
                }
                var printdata = stockin.ToArray().Select(i => new
                {
                    i.MBILL_NO,  //单号
                    BILL_DATE = i.BILL_DATE.ToString("yyyy-MM-dd"), //日期
                    i.BATCH_WEIGHT, //批次重量
                    i.BILL_METHOD, //单据方式
                    i.BTYPE_NAME, //单据类型
                    i.CATEGORY_NAME, //产品类别
                    CHECK_DATE = i.CHECK_DATE == null ? "" : ((DateTime)i.CHECK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.CHECKER,
                    i.CIGARETTE_NAME,  //牌号
                    i.DBILL_NO, //
                    i.FORMULA_NAME, //配方
                    i.GRADE_NAME, //产品等级
                    ISMIX = i.IS_MIX == "1" ? "是" : "否", //混装
                    i.ITEM_NO, //序号
                    i.LINE_NAME, //制丝线
                    i.NC_COUNT,
                    i.SCHEDULE_NO,//计划单号
                    i.SCHEDULE_ITEMNO,//计划单序号
                    OPERATE_DATE = i.OPERATE_DATE == null ? "" : ((DateTime)i.OPERATE_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.OPERATER,
                    i.ORIGINAL_NAME, //产地
                    i.PACKAGE_COUNT,
                    i.PRODUCT_CODE,
                    i.PRODUCT_NAME, //产品名称
                    i.WEIGHT,
                    i.REAL_WEIGHT, //实际重量
                    i.TOTALWEIGHT,
                    i.SOURCE_BILLNO,
                    i.STATE, //状态
                    i.STATUS,
                    i.STYLE_NAME, //形态 
                    i.TARGET_NAME, //
                    TASK_DATE = i.TASK_DATE == null ? "" : ((DateTime)i.TASK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.TASKER,
                    i.WAREHOUSE_NAME,
                    i.YEARS,
                    i.FPRODUCT_CODE
                }).OrderBy(I => I.ITEM_NO);
                DataTable dt = THOK.Common.ConvertData.LinqQueryToDataTable(printdata);
                THOK.Common.PrintHandle.dt = dt;
                //THOK.Common.PrintHandle.Onstockinprint(dt, "billreportprint");
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //抽检不料入库单打印
        public bool FillbillPrint(string BILLNO, string BILLDATEFROM, string BILLDATETO, string BILLMETHOD, string STATE, string CIGARETTECODE, string FORMULACODE)
        {
            IQueryable<WMS_BILL_MASTER> billquery = BillMasterRepository.GetQueryable();
            IQueryable<WMS_PRODUCT_STATE> productquey = ProductStateRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequey = SysTableStateRepository.GetQueryable();
            IQueryable<AUTH_USER> userquery = UserRepository.GetQueryable();
            try
            {
                var inventory = from a in billquery
                                join b in productquey on a.BILL_NO equals b.BILL_NO
                                join c in statequey on a.STATUS equals c.STATE into cf
                                from c in cf.DefaultIfEmpty()
                                join d in statequey on a.STATE equals d.STATE into df
                                from d in df.DefaultIfEmpty()
                                join g in statequey on a.BILL_METHOD equals g.STATE into gf from g in gf.DefaultIfEmpty ()
                                join e in userquery on a.OPERATER equals e.USER_ID into ef
                                from e in ef.DefaultIfEmpty()
                                join f in userquery on a.CHECKER equals f.USER_ID into ff
                                from f in ff.DefaultIfEmpty()
                                join h in userquery on a.TASKER equals h.USER_ID into hf
                                from h in hf.DefaultIfEmpty()
                                where c.TABLE_NAME == "WMS_BILL_MASTER" && c.FIELD_NAME == "STATUS" && d.TABLE_NAME == "WMS_BILL_MASTER" &&
                                d.FIELD_NAME == "STATE" && g.TABLE_NAME == "WMS_BILL_MASTER" && g.FIELD_NAME == "BILL_METHOD"
                                select new
                                {
                                    a.BILL_NO,
                                    a.BILL_DATE,
                                    a.WAREHOUSE_CODE,
                                    a.CMD_WAREHOUSE.WAREHOUSE_NAME,
                                    a.SYS_BILL_TARGET.TARGET_NAME,
                                    a.BILL_METHOD ,
                                    BILLMETHOD=g.STATE_DESC,
                                    a.FORMULA_CODE,
                                    a.WMS_FORMULA_MASTER .FORMULA_NAME ,
                                    a.CIGARETTE_CODE ,
                                    a.CMD_CIGARETTE .CIGARETTE_NAME ,
                                    a.BTYPE_CODE ,
                                    a.CMD_BILL_TYPE .BTYPE_NAME ,
                                    a.CMD_BILL_TYPE .BILL_TYPE ,
                                    a.BATCH_WEIGHT ,
                                    a.STATUS,
                                    STATUSNAME = c.STATE_DESC,
                                    a.STATE,
                                    STATENAME = d.STATE_DESC,
                                    a.SOURCE_BILLNO,
                                    OPERATER = e.USER_NAME,
                                    a.OPERATE_DATE,
                                    CHECKER = f.USER_NAME,
                                    a.CHECK_DATE,
                                    TASKER = h.USER_NAME,
                                    a.TASK_DATE,
                                    b.ITEM_NO,
                                    b.CELL_CODE,
                                    b.PRODUCT_CODE,
                                    b.WEIGHT,
                                    b.REAL_WEIGHT,
                                    b.PACKAGE_COUNT,
                                    b.PRODUCT_BARCODE,
                                    b.PALLET_CODE,
                                    b.NEWCELL_CODE,
                                    b.IS_MIX,
                                    ISMIXNAME = b.IS_MIX == "1" ? "是" : "否"
                                };
                inventory = inventory.Where(i => ("2,3").Contains(i.BILL_METHOD ));
                if (!string.IsNullOrEmpty(BILLNO))
                {
                    inventory = inventory.Where(i => i.BILL_NO == BILLNO);
                }
                if (!string.IsNullOrEmpty(BILLDATEFROM))
                {
                    DateTime datestare = DateTime.Parse(BILLDATEFROM);
                    inventory = inventory.Where(i => i.BILL_DATE.CompareTo(datestare) >= 0);
                }
                if (!string.IsNullOrEmpty(BILLDATETO))
                {
                    DateTime dateend = DateTime.Parse(BILLDATETO);
                    inventory = inventory.Where(i => i.BILL_DATE.CompareTo(dateend) <= 0);
                }
                if (!string.IsNullOrEmpty(STATE))
                {
                    inventory = inventory.Where(i => i.STATE == STATE);
                }
                if (!string.IsNullOrEmpty(CIGARETTECODE ))
                {
                    inventory = inventory.Where(i => i.CIGARETTE_CODE  == CIGARETTECODE);
                }
                if (!string.IsNullOrEmpty(FORMULACODE)) {
                    inventory = inventory.Where(i => i.FORMULA_CODE == FORMULACODE);
                }
                if (!string.IsNullOrEmpty(BILLMETHOD)) {
                    inventory = inventory.Where(i => i.BILL_METHOD == BILLMETHOD);
                }
                var printdata = inventory.ToArray().OrderBy(i => i.BILL_NO).Select(i => new
                {
                    i.BILL_NO,
                    BILL_DATE = i.BILL_DATE.ToString("yyyy-MM-dd"),
                    i.BILLMETHOD ,
                    i.BATCH_WEIGHT ,
                    i.CIGARETTE_NAME ,
                    i.FORMULA_NAME ,
                    i.BTYPE_NAME ,
                    i.WAREHOUSE_NAME,
                    i.TARGET_NAME,
                    i.STATUSNAME,
                    i.STATENAME,
                    i.SOURCE_BILLNO,
                    i.OPERATER,
                    OPERATE_DATE = i.OPERATE_DATE == null ? "" : ((DateTime)i.OPERATE_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.CHECKER,
                    CHECK_DATE = i.CHECK_DATE == null ? "" : ((DateTime)i.CHECK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.TASKER,
                    TASK_DATE = i.TASK_DATE == null ? "" : ((DateTime)i.TASK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.ITEM_NO,
                    i.CELL_CODE,
                    i.PRODUCT_CODE,
                    i.WEIGHT,
                    i.REAL_WEIGHT,
                    i.PACKAGE_COUNT,
                    i.PRODUCT_BARCODE,
                    i.PALLET_CODE,
                    i.NEWCELL_CODE,
                    i.ISMIXNAME
                });
                DataTable dt = THOK.Common.ConvertData.LinqQueryToDataTable(printdata);
                THOK.Common.PrintHandle.dt = dt;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
