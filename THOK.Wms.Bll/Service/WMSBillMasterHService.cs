using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using THOK.Authority.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Authority.Dal.Interfaces;
using THOK.Wms.Bll.Models;

namespace THOK.Wms.Bll.Service
{
    class WMSBillMasterHService : ServiceBase<WMS_BILL_MASTERH>, IWMSBillMasterHService
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public IWMSBillMasterHRepository BillMasterHRepository { get; set; }
        [Dependency]
        public IWMSBillMasterRepository BillMasterRepository { get; set; }
        [Dependency]
        public ISysTableStateRepository SysTableStateRepository { get; set; }
        [Dependency]
        public IWMSBillDetailHRepository BillDetailHRepository { get; set; }
        [Dependency]
        public IWMSBillDetailRepository BillDetailRepository { get; set; }
        [Dependency]
        public IWMSFormulaDetailRepository FormulaDetailRepository { get; set; }
        [Dependency]
        public ICMDProuductRepository ProductRepository { get; set; }
        [Dependency]
        public ICmdBillTypeRepository CmdBillTypeRepository { get; set; }
        [Dependency]
        public IUserRepository UserRepository { get; set; }
        [Dependency]
        public ICMDCellRepository cellRepository { get; set; }
        [Dependency]
        public IWMSProductStateRepository ProductStateRepository { get; set; }
        [Dependency]
        public IWCSTaskRepository WcsTaskRepository { get; set; }
        public object GetDetails(int page, int rows, string billtype, string flag, string BILL_NO, string BILL_DATE, string BTYPE_CODE, string WAREHOUSE_CODE, string BILL_METHOD, string CIGARETTE_CODE, string FORMULA_CODE, string STATE, string OPERATER, string OPERATE_DATE, string CHECKER, string CHECK_DATE, string STATUS, string BILL_DATEStar, string BILL_DATEEND, string SOURCE_BILLNO, string LINENO, string BILLNOFROM, string BILLNOTO)
        {
            try
            {
                //IQueryable<WMS_BILL_MASTERH> billquery = BillMasterHRepository.Exesqlstr("select *from( select ROWNUM  rw,bill_no,bill_date,btype_code,schedule_no,warehouse_code,target_code,status,state,source_billno,operater,operate_date,checker,check_date,tasker,task_date,bill_method,schedule_itemno,line_no,cigarette_code,formula_code,batch_weight from WMS_BILL_MASTERH order by bill_date) where RW>=50 and RW<=100");
                IQueryable<WMS_BILL_MASTERH> billquery = BillMasterHRepository.GetQueryable();
                IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
                IQueryable<AUTH_USER> userquery = UserRepository.GetQueryable();
                IQueryable<WMS_BILL_MASTER> billquery2 = BillMasterRepository.GetQueryable();
                var billmaster =( from a in billquery
                                 join b in statequery on a.STATUS equals b.STATE
                                 join c in statequery on a.STATE equals c.STATE
                                 join d in statequery on a.BILL_METHOD equals d.STATE
                                 join e in userquery on a.OPERATER equals e.USER_ID
                                 join f in userquery on a.CHECKER equals f.USER_ID into fg
                                 from f in fg.DefaultIfEmpty()
                                 join g in userquery on a.TASKER equals g.USER_ID into hg
                                 from g in hg.DefaultIfEmpty()
                                 where b.TABLE_NAME == "WMS_BILL_MASTER" && b.FIELD_NAME == "STATUS" && c.TABLE_NAME == "WMS_BILL_MASTER" && c.FIELD_NAME == "STATE"
                                && d.TABLE_NAME == "WMS_BILL_MASTER" && d.FIELD_NAME == "BILL_METHOD" && a.CMD_BILL_TYPE.BILL_TYPE == billtype
                                 select new
                                 {
                                     a.BILL_NO,
                                     a.BILL_DATE,
                                     a.BTYPE_CODE, //单据类型代码
                                     a.CMD_BILL_TYPE.BTYPE_NAME,  //单据类型名称
                                     a.SCHEDULE_NO,
                                     a.WAREHOUSE_CODE,
                                     a.CMD_WAREHOUSE.WAREHOUSE_NAME,
                                     a.SYS_BILL_TARGET.TARGET_NAME, //目标位置名
                                     a.TARGET_CODE, //目标位置代码
                                     a.STATUS,//单据来源代号
                                     STATUSNAME = b.STATE_DESC,//单据来源描述,手动,系统输入
                                     a.STATE,//单据状态代号
                                     STATENAME = c.STATE_DESC,//单据状态描述
                                     a.CIGARETTE_CODE,
                                     a.CMD_CIGARETTE.CIGARETTE_NAME,//牌号名称
                                     a.FORMULA_CODE,
                                     a.WMS_FORMULA_MASTER.FORMULA_NAME, //配方名称
                                     a.BATCH_WEIGHT,
                                     a.SOURCE_BILLNO,
                                     OPERATER = e.USER_NAME,
                                     a.OPERATE_DATE,
                                     CHECKER = f.USER_NAME,
                                     a.CHECK_DATE,
                                     TASKER = g.USER_NAME,
                                     a.TASK_DATE,
                                     a.BILL_METHOD,//单据方式代码
                                     BILLMETHODNAME = d.STATE_DESC,//单据方式描述
                                     a.SCHEDULE_ITEMNO,
                                     a.LINE_NO,//制丝线代码
                                     a.CMD_PRODUCTION_LINE.LINE_NAME //制丝线名
                                 }).Concat(from a in billquery2
                                           join b in statequery on a.STATUS equals b.STATE
                                           join c in statequery on a.STATE equals c.STATE
                                           join d in statequery on a.BILL_METHOD equals d.STATE
                                           join e in userquery on a.OPERATER equals e.USER_ID
                                           join f in userquery on a.CHECKER equals f.USER_ID into fg
                                           from f in fg.DefaultIfEmpty()
                                           join g in userquery on a.TASKER equals g.USER_ID into hg
                                           from g in hg.DefaultIfEmpty()
                                           where b.TABLE_NAME == "WMS_BILL_MASTER" && b.FIELD_NAME == "STATUS" && c.TABLE_NAME == "WMS_BILL_MASTER" && c.FIELD_NAME == "STATE"
                                          && d.TABLE_NAME == "WMS_BILL_MASTER" && d.FIELD_NAME == "BILL_METHOD" && a.CMD_BILL_TYPE.BILL_TYPE == billtype
                                           select new
                                           {
                                               a.BILL_NO,
                                               a.BILL_DATE,
                                               a.BTYPE_CODE, //单据类型代码
                                               a.CMD_BILL_TYPE.BTYPE_NAME,  //单据类型名称
                                               a.SCHEDULE_NO,
                                               a.WAREHOUSE_CODE,
                                               a.CMD_WAREHOUSE.WAREHOUSE_NAME,
                                               a.SYS_BILL_TARGET.TARGET_NAME, //目标位置名
                                               a.TARGET_CODE, //目标位置代码
                                               a.STATUS,//单据来源代号
                                               STATUSNAME = b.STATE_DESC,//单据来源描述,手动,系统输入
                                               a.STATE,//单据状态代号
                                               STATENAME = c.STATE_DESC,//单据状态描述
                                               a.CIGARETTE_CODE,
                                               a.CMD_CIGARETTE.CIGARETTE_NAME,//牌号名称
                                               a.FORMULA_CODE,
                                               a.WMS_FORMULA_MASTER.FORMULA_NAME, //配方名称
                                               a.BATCH_WEIGHT,
                                               a.SOURCE_BILLNO,
                                               OPERATER = e.USER_NAME,
                                               a.OPERATE_DATE,
                                               CHECKER = f.USER_NAME,
                                               a.CHECK_DATE,
                                               TASKER = g.USER_NAME,
                                               a.TASK_DATE,
                                               a.BILL_METHOD,//单据方式代码
                                               BILLMETHODNAME = d.STATE_DESC,//单据方式描述
                                               a.SCHEDULE_ITEMNO,
                                               a.LINE_NO,//制丝线代码
                                               a.CMD_PRODUCTION_LINE.LINE_NAME //制丝线名
                                           });
                //billmaster = billmaster.Where(i => i.BILL_NO.CompareTo("IS20131211") >= 0);
                //billmaster = billmaster.Where(i => i.BILL_NO.CompareTo("IS20131218") <= 0);
                if (!string.IsNullOrEmpty(BILL_NO))
                {
                    billmaster = billmaster.Where(i => i.BILL_NO == BILL_NO);
                   
                }
                if (!string.IsNullOrEmpty(BILL_DATE))
                {
                    DateTime billdt = DateTime.Parse(BILL_DATE);
                    billmaster = billmaster.Where(i => i.BILL_DATE.CompareTo(billdt) == 0);
                }
                if (!string.IsNullOrEmpty(BTYPE_CODE))
                {
                    billmaster = billmaster.Where(i => i.BTYPE_CODE == BTYPE_CODE);
                }
                if (!string.IsNullOrEmpty(WAREHOUSE_CODE))
                {
                    billmaster = billmaster.Where(i => i.WAREHOUSE_CODE == WAREHOUSE_CODE);
                }
                if (!string.IsNullOrEmpty(BILL_METHOD))
                {
                    billmaster = billmaster.Where(i => i.BILL_METHOD == BILL_METHOD);
                }
                if (!string.IsNullOrEmpty(CIGARETTE_CODE))
                {
                    billmaster = billmaster.Where(i => i.CIGARETTE_CODE == CIGARETTE_CODE);
                }
                if (!string.IsNullOrEmpty(FORMULA_CODE))
                {
                    billmaster = billmaster.Where(i => i.FORMULA_CODE == FORMULA_CODE);
                }
                if (!string.IsNullOrEmpty(STATE))
                {
                    billmaster = billmaster.Where(i => i.STATE == STATE);
                }
                if (!string.IsNullOrEmpty(OPERATER))
                {
                    billmaster = billmaster.Where(i => i.OPERATER.Contains(OPERATER));
                }
                if (!string.IsNullOrEmpty(OPERATE_DATE))
                {
                    DateTime operatedt = DateTime.Parse(OPERATE_DATE);
                    DateTime operatedt2 = operatedt.AddDays(1);
                    billmaster = billmaster.Where(i => i.OPERATE_DATE.Value.CompareTo(operatedt) >= 0);
                    billmaster = billmaster.Where(i => i.OPERATE_DATE.Value.CompareTo(operatedt2) < 0);
                }
                if (!string.IsNullOrEmpty(CHECKER))
                {
                    billmaster = billmaster.Where(i => i.CHECKER.Contains(CHECKER));
                }
                if (!string.IsNullOrEmpty(CHECK_DATE))
                {
                    DateTime checkdt = DateTime.Parse(CHECK_DATE);
                    DateTime checkdt2 = checkdt.AddDays(1);
                    billmaster = billmaster.Where(i => i.CHECK_DATE.Value.CompareTo(checkdt) >= 0);
                    billmaster = billmaster.Where(i => i.CHECK_DATE.Value.CompareTo(checkdt2) < 0);
                }
                if (!string.IsNullOrEmpty(STATUS))
                {
                    billmaster = billmaster.Where(i => i.STATUS == STATUS);
                }
                if (!string.IsNullOrEmpty(BILL_DATEStar))
                {
                    DateTime datestare = DateTime.Parse(BILL_DATEStar);
                    billmaster = billmaster.Where(i => i.BILL_DATE.CompareTo(datestare) >= 0);
                }
                if (!string.IsNullOrEmpty(BILL_DATEEND))
                {
                    DateTime dateend = DateTime.Parse(BILL_DATEEND);
                    billmaster = billmaster.Where(i => i.BILL_DATE.CompareTo(dateend) <= 0);
                }
                if (!string.IsNullOrEmpty(SOURCE_BILLNO))
                {
                    billmaster = billmaster.Where(i => i.SOURCE_BILLNO.Contains(SOURCE_BILLNO));
                }
                if (!string.IsNullOrEmpty(LINENO))
                {
                    billmaster = billmaster.Where(i => i.LINE_NO == LINENO);
                }
                if (!string.IsNullOrEmpty(BILLNOFROM))
                {
                    billmaster = billmaster.Where(i => i.BILL_NO.CompareTo(BILLNOFROM) >= 0);
                }
                if (!string.IsNullOrEmpty(BILLNOTO))
                {
                    billmaster = billmaster.Where(i => i.BILL_NO.CompareTo(BILLNOTO) <= 0);
                }
                if (flag == "2")
                {  //属于抽检补料入库单
                    billmaster = billmaster.Where(i => "2,3".Contains(i.BILL_METHOD));
                }
                else
                {
                    //temp = temp.Where(i => i.BILL_METHOD != "2");
                    //temp = temp.Where(i => i.BILL_METHOD != "3");
                    billmaster = billmaster.Where(i => !("2,3".Contains(i.BILL_METHOD)));
                    if (flag == "1")
                    {  //入库,出库作业  获取的记录即状态不是保存的
                        billmaster = billmaster.Where(i => i.STATE != "1");
                    }
                    if (flag == "4")
                    {//紧急补料单
                        billmaster = billmaster.Where(i => i.BTYPE_CODE == "005");
                    }
                    if (flag == "3")
                    {//倒库单
                        billmaster = billmaster.Where(i => i.BTYPE_CODE == "006");
                    }

                }
                if (THOK.Common.PrintHandle.issearch)
                {//用于单据查询中的打印
                    THOK.Common.PrintHandle.searchdt = THOK.Common.ConvertData.LinqQueryToDataTable(billmaster);
                    //THOK.Common.PrintHandle.issearch = false;
                }
                billmaster = billmaster.OrderByDescending(i => i.BILL_NO );
                int total = billmaster.Count();
                billmaster = billmaster.Skip((page - 1) * rows).Take(rows);
                //var temp = billmaster.ToArray();
                var temp = billmaster.ToArray().Select(i => new
                {
                    i.BILL_NO,
                    BILL_DATE = i.BILL_DATE.ToString("yyyy-MM-dd"),
                    i.BTYPE_CODE,
                    i.BTYPE_NAME,
                    i.SCHEDULE_NO,
                    i.WAREHOUSE_CODE,
                    i.WAREHOUSE_NAME,
                    i.TARGET_CODE,
                    i.TARGET_NAME,
                    i.STATUS,
                    i.STATUSNAME,
                    i.STATE,
                    i.STATENAME,
                    i.SOURCE_BILLNO,
                    i.CIGARETTE_CODE,
                    i.CIGARETTE_NAME,//牌号名称
                    i.FORMULA_CODE,
                    i.FORMULA_NAME, //配方名称
                    i.BATCH_WEIGHT,
                    i.OPERATER,
                    OPERATE_DATE = i.OPERATE_DATE == null ? "" : ((DateTime)i.OPERATE_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.CHECKER,
                    CHECK_DATE = i.CHECK_DATE == null ? "" : ((DateTime)i.CHECK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.TASKER,
                    TASK_DATE = i.TASK_DATE == null ? "" : ((DateTime)i.TASK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.BILL_METHOD,
                    i.BILLMETHODNAME,
                    i.SCHEDULE_ITEMNO,
                    i.LINE_NO,
                    i.LINE_NAME
                });
                return new { total, rows = temp };
            }
            catch (Exception ex) {
                return null;
            }
        }
        //获取单据明细
        public object GetSubDetails(int page, int rows, string BillNo, int flag)
        {
            IQueryable<WMS_BILL_DETAIL> detailquery = BillDetailRepository.GetQueryable();
            IQueryable<WMS_BILL_DETAILH> detailquery2 = BillDetailHRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            IQueryable<CMD_PRODUCT> productquery = ProductRepository.GetQueryable();
            var billdetail = (from a in detailquery
                              join b in statequery on a.IS_MIX equals b.STATE
                              join c in productquery on a.PRODUCT_CODE equals c.PRODUCT_CODE
                              where b.TABLE_NAME == "WMS_BILL_DETAIL" && b.FIELD_NAME == "IS_MIX"
                              select new
                              {
                                  a.ITEM_NO,
                                  a.BILL_NO,
                                  a.PRODUCT_CODE,
                                  c.PRODUCT_NAME,
                                  c.YEARS,
                                  c.CMD_PRODUCT_GRADE.GRADE_NAME,
                                  c.CMD_PRODUCT_STYLE.STYLE_NAME,
                                  c.CMD_PRODUCT_ORIGINAL.ORIGINAL_NAME,
                                  c.CMD_PRODUCT_CATEGORY.CATEGORY_NAME,
                                  a.WEIGHT,
                                  a.REAL_WEIGHT,
                                  a.PACKAGE_COUNT,
                                  a.NC_COUNT,
                                  TOTAL_WEIGHT = a.PACKAGE_COUNT * a.REAL_WEIGHT,
                                  a.IS_MIX,
                                  IS_MIXDESC = b.STATE_DESC,
                                  a.FPRODUCT_CODE
                              }).Concat(from a in detailquery2
                                        join b in statequery on a.IS_MIX equals b.STATE
                                        join c in productquery on a.PRODUCT_CODE equals c.PRODUCT_CODE
                                        where b.TABLE_NAME == "WMS_BILL_DETAIL" && b.FIELD_NAME == "IS_MIX"
                                        select new
                                        {
                                            a.ITEM_NO,
                                            a.BILL_NO,
                                            a.PRODUCT_CODE,
                                            c.PRODUCT_NAME,
                                            c.YEARS,
                                            c.CMD_PRODUCT_GRADE.GRADE_NAME,
                                            c.CMD_PRODUCT_STYLE.STYLE_NAME,
                                            c.CMD_PRODUCT_ORIGINAL.ORIGINAL_NAME,
                                            c.CMD_PRODUCT_CATEGORY.CATEGORY_NAME,
                                            a.WEIGHT,
                                            a.REAL_WEIGHT,
                                            a.PACKAGE_COUNT,
                                            a.NC_COUNT,
                                            TOTAL_WEIGHT = a.PACKAGE_COUNT * a.REAL_WEIGHT,
                                            a.IS_MIX,
                                            IS_MIXDESC = b.STATE_DESC,
                                            a.FPRODUCT_CODE
                                        });
            if (flag == 1)
            { //获取混装产品的信息.
                billdetail = billdetail.Where(i => i.WEIGHT != i.REAL_WEIGHT);
            }
            billdetail = billdetail.Where(i => i.BILL_NO == BillNo).OrderBy(i => i.ITEM_NO);
            int total = billdetail.Count();
            billdetail = billdetail.Skip((page - 1) * rows).Take(rows);
            try
            {
                var temp = billdetail.ToArray().Select(i => i);
            }
            catch (Exception ex) { }
            return new { total, rows = billdetail };
        }
    }
}
