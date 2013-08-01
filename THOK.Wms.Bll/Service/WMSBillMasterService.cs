using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using  THOK.Wms.Dal.Interfaces;
using System.Data;
using THOK.Wms.Bll.Models;

namespace THOK.Wms.Bll.Service
{
    class WMSBillMasterService:ServiceBase<WMS_BILL_MASTER>,IWMSBillMasterService
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public IWMSBillMasterRepository BillMasterRepository { get; set; }
        [Dependency]
        public ISysTableStateRepository SysTableStateRepository { get; set; }
        [Dependency]
        public IWMSBillDetailRepository BillDetailRepository { get; set; }
        [Dependency]
        public IWMSFormulaDetailRepository FormulaDetailRepository { get; set; }
        public object GetDetails(int page, int rows, string billtype)
        {
            IQueryable<WMS_BILL_MASTER > billquery = BillMasterRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            var billmaster = from a in billquery
                             join b in statequery on a.STATUS equals b.STATE
                             join c in statequery on a.STATE equals c.STATE
                             join d in statequery on a.BILL_METHOD equals d.STATE
                             where b.TABLE_NAME == "BILLMASTER" && b.FIELD_NAME == "STATUS" && c.TABLE_NAME == "BILLMASTER" && c.FIELD_NAME == "STATE" 
                             && d.TABLE_NAME == "BILLMASTER" && d.FIELD_NAME == "BILL_METHOD"&&a.CMD_BILL_TYPE .BILL_TYPE ==billtype 
                             select new {
                                  a.BILL_NO ,
                                  a.BILL_DATE ,
                                  a.BTYPE_CODE , //单据类型代码
                                  a.CMD_BILL_TYPE .BTYPE_NAME,  //单据类型名称
                                  a.SCHEDULE_NO,
                                  a.WAREHOUSE_CODE ,
                                  a.SYS_BILL_TARGET .TARGET_NAME , //目标位置名
                                  a.TARGET_CODE , //目标位置代码
                                  a.STATUS ,//单据来源代号
                                 STATUSNAME =  b.STATE_DESC ,//单据来源描述,手动,系统输入
                                  a.STATE ,//单据状态代号
                                  STATENAME= c.STATE_DESC ,//单据状态描述
                                  a.CIGARETTE_CODE,
                                  a.CMD_CIGARETTE .CIGARETTE_NAME ,//牌号名称
                                  a.FORMULA_CODE ,
                                  a.WMS_FORMULA_MASTER .FORMULA_NAME , //配方名称
                                  a.BATCH_WEIGHT ,
                                  a.SOURCE_BILLNO,
                                  a.OPERATER ,
                                  a.OPERATE_DATE ,
                                  a.CHECKER ,
                                  a.CHECK_DATE ,
                                  a.TASKER ,
                                  a.TASK_DATE ,
                                  a.BILL_METHOD ,//单据方式代码
                                 BILLMETHODNAME= d.STATE_DESC ,//单据方式描述
                                  a.SCHEDULE_ITEMNO ,
                                  a.LINE_NO ,//制丝线代码
                                  a.CMD_PRODUCTION_LINE .LINE_NAME //制丝线名
                             };
            var temp = billmaster.ToArray().OrderBy(i => i.BILL_DATE ).Select(i => new
            {
                 i.BILL_NO ,
                 BILL_DATE = i.BILL_DATE.ToString("yyyy-MM-dd HH:mm:ss"),
                 i.BTYPE_CODE ,
                 i.BTYPE_NAME ,
                 i.SCHEDULE_NO ,
                 i.WAREHOUSE_CODE ,
                 i.TARGET_CODE ,
                 i.TARGET_NAME ,
                 i.STATUS ,
                 i.STATUSNAME ,
                 i.STATE ,
                 i.STATENAME ,
                 i.SOURCE_BILLNO ,
                 i.CIGARETTE_CODE,
                 i.CIGARETTE_NAME,//牌号名称
                 i.FORMULA_CODE,
                 i.FORMULA_NAME, //配方名称
                 i.BATCH_WEIGHT,
                 i.OPERATER ,
                 OPERATE_DATE = i.OPERATE_DATE == null ? "" : ((DateTime)i.OPERATE_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                 i.CHECKER ,
                 CHECK_DATE = i.CHECK_DATE == null ? "" : ((DateTime)i.CHECK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                 i.TASKER ,
                 TASK_DATE = i.TASK_DATE == null ? "" : ((DateTime)i.TASK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                 i.BILL_METHOD ,
                 i.BILLMETHODNAME ,
                 i.SCHEDULE_ITEMNO ,
                 i.LINE_NO ,
                 i.LINE_NAME 
            });
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }
        //获取单据明细
        public object GetSubDetails(int page, int rows, string BillNo)
        {
            IQueryable <WMS_BILL_DETAIL > detailquery = BillDetailRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            var billdetail = from a in detailquery
                             join b in statequery on a.IS_MIX equals b.STATE
                             where b.TABLE_NAME == "BILLDETAIL" && b.FIELD_NAME == "IS_MIX"
                             select new { 
                                 a.ITEM_NO ,
                                 a.BILL_NO ,
                                 a.PRODUCT_CODE,
                                 a.WEIGHT ,
                                 a.REAL_WEIGHT ,
                                 a.PACKAGE_COUNT ,
                                 a.NC_COUNT ,
                                 TOTAL_WEIGHT=a.PACKAGE_COUNT*a .WEIGHT,
                                 a.IS_MIX,
                                 IS_MIXDESC=b .STATE_DESC,
                                 a.FPRODUCT_CODE 
                             };
            var temp = billdetail.ToArray().Where(i => i.BILL_NO == BillNo).OrderBy(i => i.ITEM_NO).Select (i=> i );
            int total = temp.Count(); 
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }

        //审核
        public bool Audit(string checker, string BillNo)
        {
            var billquery = BillMasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO  == BillNo );
            if (billquery != null)
            {
                billquery.CHECK_DATE = DateTime.Now;
                billquery.CHECKER = checker;
                billquery.STATE = "2";
                BillMasterRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        //反审
        public bool Antitrial(string BillNo)
        {
            var billquery = BillMasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO == BillNo);
            if (billquery != null)
            {
                billquery.CHECK_DATE = null ;
                billquery.CHECKER = "";
                billquery.STATE = "1";
                BillMasterRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }


        //获取单据编号
        public object GetBillNo(string userName, DateTime dt, string BILL_NO)
        {
            var strCode = BillMasterRepository.GetNewID("IS", dt, BILL_NO);
            var BillnoInfo =
                new
                {
                    userName = userName,
                    BillNo = strCode
                };
            return BillnoInfo;
        }


        public bool Add(WMS_BILL_MASTER mast, object detail)
        {
            bool rejust = false;
            try
            {
                mast.BILL_NO = BillMasterRepository.GetNewID("IS", mast.BILL_DATE , mast.BILL_NO);
                mast.OPERATE_DATE = DateTime.Now;
                //mast.BILL_DATE = DateTime.Now;
                mast.STATE = "1"; //默认保存状态
                mast.STATUS = "0"; //默认手工输入
                BillMasterRepository.Add(mast);

                DataTable dt = THOK.Common.JsonData.JsonToDataTable(((System.String[])detail)[0]);
                foreach (DataRow dr in dt.Rows)
                {
                    
                    WMS_BILL_DETAIL subdetail = new WMS_BILL_DETAIL();
                    THOK.Common.JsonData.DataBind(subdetail, dr);
                    subdetail.BILL_NO  = mast.BILL_NO ;
                    subdetail.IS_MIX = "0";
                    BillDetailRepository.Add(subdetail);
                }

                BillMasterRepository.SaveChanges();
                rejust = true;
            }
            catch (Exception ex)
            {

            }
            return rejust;
        }

        //批次入库时,载入配方.
        public object LoadFormulaDetail(int page, int rows, string Formulacode, decimal BATCH_WEIGHT)
        {
            int serial=1;
            IQueryable<WMS_FORMULA_DETAIL> formuladetail = FormulaDetailRepository.GetQueryable();
            var items = formuladetail.Where(i => i.FORMULA_CODE == Formulacode);
            List<FormulaDetail> list = new List<FormulaDetail>();
            foreach (WMS_FORMULA_DETAIL formula in items)
            {
                FormulaDetail item = new FormulaDetail();
                item.ITEM_NO = serial ;
                item.PRODUCT_CODE = formula.PRODUCT_CODE;
                item.WEIGHT = formula.CMD_PRODUCT.WEIGHT;
                item.PRODUCT_NAME = formula.CMD_PRODUCT.PRODUCT_NAME ;
                item.PACKAGE_COUNT = (int)(((formula.PERCENT * BATCH_WEIGHT) / 100) / formula.CMD_PRODUCT.WEIGHT);
                item.TOTAL_WEIGHT = item.WEIGHT * item.PACKAGE_COUNT;
                decimal lastweight = ((formula.PERCENT * BATCH_WEIGHT) / 100) - (item.PACKAGE_COUNT * formula.CMD_PRODUCT.WEIGHT); //不足一包的重量
                if (lastweight != 0)
                {
                    serial++;
                    FormulaDetail subitem = new FormulaDetail();
                    subitem.ITEM_NO = serial;
                    subitem.PRODUCT_CODE = formula.PRODUCT_CODE;
                    subitem.PRODUCT_NAME = formula.CMD_PRODUCT.PRODUCT_NAME;
                    subitem.WEIGHT = lastweight;
                    subitem.PACKAGE_COUNT = 1;
                    subitem.TOTAL_WEIGHT = lastweight;
                    list.Add(subitem);
                }
                if (item.PACKAGE_COUNT != 0)
                {
                    list.Add(item);
                }
                serial++;
            }

            var temp = list.OrderBy(i => i.PRODUCT_CODE).OrderBy (i=>i.ITEM_NO ).Select(i => i);
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }

        //修改
        public bool Edit(WMS_BILL_MASTER mast, object detail)
        {
            var billmast = BillMasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO == mast.BILL_NO);
            billmast.BILL_DATE = mast.BILL_DATE;
            billmast.WAREHOUSE_CODE = mast.WAREHOUSE_CODE;
            billmast.BTYPE_CODE = mast.BTYPE_CODE;
            if (mast.BILL_METHOD == "0")
            {
                billmast.CIGARETTE_CODE = mast.CIGARETTE_CODE;
                billmast.FORMULA_CODE = mast.FORMULA_CODE;
                billmast.BATCH_WEIGHT = mast.BATCH_WEIGHT;
            }
            var details = BillDetailRepository.GetQueryable().Where(i => i.BILL_NO == mast.BILL_NO);
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_BILL_DETAIL sub in tmp)
            {
                BillDetailRepository.Delete(sub);
            }

            DataTable dt = THOK.Common.JsonData.JsonToDataTable(((System.String[])detail)[0]); //修改
            if (dt != null)
            {
                dt.Columns.Remove("LINE_NAME");
                foreach (DataRow dr in dt.Rows)
                {
                    WMS_BILL_DETAIL subdetail = new WMS_BILL_DETAIL();
                    THOK.Common.JsonData.DataBind(subdetail, dr);
                    subdetail.BILL_NO = mast.BILL_NO;
                    BillDetailRepository.Add(subdetail);
                }
            }
            BillMasterRepository.SaveChanges();

            return true;
        }
    }
}
