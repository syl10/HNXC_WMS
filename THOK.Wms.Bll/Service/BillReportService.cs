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
                var printdata = stockin.ToArray().Select(i => new
                { 
                    i.MBILL_NO ,  //单号
                    BILL_DATE = i.BILL_DATE.ToString("yyyy-MM-dd"), //日期
                    i.BATCH_WEIGHT, //批次重量
                    i.BILL_METHOD, //单据方式
                    i.BTYPE_NAME , //单据类型
                    i.CATEGORY_NAME , //产品类别
                    CHECK_DATE = i.CHECK_DATE == null ? "" : ((DateTime)i.CHECK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.CHECKER , 
                    i.CIGARETTE_NAME,  //牌号
                    i.DBILL_NO, //
                    i.FORMULA_NAME , //配方
                    i.GRADE_NAME , //产品等级
                    ISMIX=i.IS_MIX =="1"?"是":"否" , //混装
                    i.ITEM_NO , //序号
                    i.LINE_NAME , //制丝线
                    i.NC_COUNT ,
                    OPERATE_DATE = i.OPERATE_DATE == null ? "" : ((DateTime)i.OPERATE_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.OPERATER ,
                    i.ORIGINAL_NAME , //产地
                    i.PACKAGE_COUNT ,
                    i.PRODUCT_NAME , //产品名称
                    i.REAL_WEIGHT , //实际重量
                    i.SOURCE_BILLNO ,
                    i.STATE , //状态
                    i.STATUS ,
                    i.STYLE_NAME , //形态 
                    i.TARGET_NAME , //
                    TASK_DATE = i.TASK_DATE == null ? "" : ((DateTime)i.TASK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.TASKER ,
                    i.WAREHOUSE_NAME ,
                    i.YEARS,
                    i.FPRODUCT_CODE
                }).OrderBy (I=>I.ITEM_NO );
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
        public bool StockoutPrint( string BILLNO, string BILLDATEFROM, string BILLDATETO, string BTYPECODE, string LINENO, string STATE, string CIGARETTECODE, string FORMULACODE)
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
                    i.PRODUCT_NAME, //产品名称
                    i.REAL_WEIGHT, //实际重量
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
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
