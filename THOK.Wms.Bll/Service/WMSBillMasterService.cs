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
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;

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
        public object GetDetails(int page, int rows, string billtype, string flag, string BILL_NO, string BILL_DATE, string BTYPE_CODE, string WAREHOUSE_CODE, string BILL_METHOD, string CIGARETTE_CODE, string FORMULA_CODE, string STATE, string OPERATER, string OPERATE_DATE, string CHECKER, string CHECK_DATE, string STATUS, string BILL_DATEStar, string BILL_DATEEND, string SOURCE_BILLNO)
        {
            IQueryable<WMS_BILL_MASTER > billquery = BillMasterRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            IQueryable<AUTH_USER> userquery = UserRepository.GetQueryable();
            var billmaster = from a in billquery
                             join b in statequery on a.STATUS equals b.STATE
                             join c in statequery on a.STATE equals c.STATE
                             join d in statequery on a.BILL_METHOD equals d.STATE
                             join e in userquery on a.OPERATER equals e.USER_ID 
                             join f in userquery on a.CHECKER equals f.USER_ID  into fg from f in fg.DefaultIfEmpty ()
                             join g in userquery on a.TASKER equals g.USER_ID into hg from g in hg.DefaultIfEmpty ()
                             where b.TABLE_NAME == "WMS_BILL_MASTER" && b.FIELD_NAME == "STATUS" && c.TABLE_NAME == "WMS_BILL_MASTER" && c.FIELD_NAME == "STATE"
                             && d.TABLE_NAME == "WMS_BILL_MASTER" && d.FIELD_NAME == "BILL_METHOD" && a.CMD_BILL_TYPE.BILL_TYPE == billtype 
                             select new {
                                  a.BILL_NO ,
                                  a.BILL_DATE ,
                                  a.BTYPE_CODE , //单据类型代码
                                  a.CMD_BILL_TYPE .BTYPE_NAME,  //单据类型名称
                                  a.SCHEDULE_NO,
                                  a.WAREHOUSE_CODE ,
                                  a.CMD_WAREHOUSE .WAREHOUSE_NAME,
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
                                  OPERATER =e.USER_NAME ,
                                  a.OPERATE_DATE ,
                                  CHECKER=f.USER_NAME  ,
                                  a.CHECK_DATE ,
                                  TASKER =g.USER_NAME ,
                                  a.TASK_DATE ,
                                  a.BILL_METHOD ,//单据方式代码
                                 BILLMETHODNAME= d.STATE_DESC ,//单据方式描述
                                  a.SCHEDULE_ITEMNO ,
                                  a.LINE_NO ,//制丝线代码
                                  a.CMD_PRODUCTION_LINE .LINE_NAME //制丝线名
                             };
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
            if (!string.IsNullOrEmpty(STATUS)) {
                billmaster = billmaster.Where(i => i.STATUS == STATUS);
            }
            if (!string.IsNullOrEmpty(BILL_DATEStar)) {
                DateTime datestare = DateTime.Parse(BILL_DATEStar);
                billmaster = billmaster.Where(i => i.BILL_DATE.CompareTo(datestare) >= 0);
            }
            if (!string.IsNullOrEmpty(BILL_DATEEND)) {
                DateTime dateend = DateTime.Parse(BILL_DATEEND);
                billmaster = billmaster.Where(i => i.BILL_DATE.CompareTo(dateend) <= 0);
            }
            if (!string.IsNullOrEmpty(SOURCE_BILLNO)) {
                billmaster = billmaster.Where(i => i.SOURCE_BILLNO.Contains(SOURCE_BILLNO));
            }
            var temp = billmaster.ToArray().OrderByDescending(i => i.OPERATE_DATE ).Select(i => new
            {
                 i.BILL_NO ,
                 BILL_DATE = i.BILL_DATE.ToString("yyyy-MM-dd"),
                 i.BTYPE_CODE ,
                 i.BTYPE_NAME ,
                 i.SCHEDULE_NO ,
                 i.WAREHOUSE_CODE ,
                 i.WAREHOUSE_NAME ,
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
            if (flag == "2")
            {  //属于抽检补料入库单
                temp = temp.Where(i => "2,3".Contains(i.BILL_METHOD));
            }
            else
            {
                //temp = temp.Where(i => i.BILL_METHOD != "2");
                //temp = temp.Where(i => i.BILL_METHOD != "3");
                temp = temp.Where(i => !("2,3".Contains(i.BILL_METHOD)));
                if (flag == "1")
                {  //入库,出库作业  获取的记录即状态不是保存的
                    temp = temp.Where(i => i.STATE != "1");
                }
                if (flag == "4")
                {//紧急补料单
                    temp = temp.Where(i => i.BTYPE_CODE == "005");
                }
            }
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }
        //获取单据明细
        public object GetSubDetails(int page, int rows, string BillNo, int  flag)
        {
            IQueryable <WMS_BILL_DETAIL > detailquery = BillDetailRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            IQueryable<CMD_PRODUCT> productquery = ProductRepository.GetQueryable();
            var billdetail = from a in detailquery
                             join b in statequery on a.IS_MIX equals b.STATE
                             join c in productquery on a.PRODUCT_CODE equals c.PRODUCT_CODE
                             where b.TABLE_NAME == "WMS_BILL_DETAIL" && b.FIELD_NAME == "IS_MIX"
                             select new { 
                                 a.ITEM_NO ,
                                 a.BILL_NO ,
                                 a.PRODUCT_CODE,
                                 c.PRODUCT_NAME ,
                                 c.YEARS,
                                 c.CMD_PRODUCT_GRADE.GRADE_NAME,
                                 c.CMD_PRODUCT_STYLE.STYLE_NAME,
                                 c.CMD_PRODUCT_ORIGINAL.ORIGINAL_NAME,
                                 c.CMD_PRODUCT_CATEGORY.CATEGORY_NAME,
                                 a.WEIGHT ,
                                 a.REAL_WEIGHT ,
                                 a.PACKAGE_COUNT ,
                                 a.NC_COUNT ,
                                 TOTAL_WEIGHT=a.PACKAGE_COUNT*a .REAL_WEIGHT,
                                 a.IS_MIX,
                                 IS_MIXDESC=b .STATE_DESC,
                                 a.FPRODUCT_CODE 
                             };
            if (flag == 1) { //获取混装产品的信息.
                billdetail = billdetail.Where(i => i.WEIGHT != i.REAL_WEIGHT);
            }
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
               int result= BillMasterRepository.SaveChanges();
               if (result == -1) return false;
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
        public object GetBillNo(string userName, DateTime dt, string BILL_NO, string prefix)
        {
            var strCode = BillMasterRepository.GetNewID(prefix, dt, BILL_NO);
            var BillnoInfo =
                new
                {
                    userName = userName,
                    BillNo = strCode
                };
            return BillnoInfo;
        }


        public bool Add(WMS_BILL_MASTER mast, object detail, string prefix)
        {
            var targetcode = CmdBillTypeRepository.GetQueryable().FirstOrDefault(i => i.BTYPE_CODE == mast.BTYPE_CODE);
            bool rejust = false;
            int serial = 1;
            try
            {
                mast.BILL_NO = BillMasterRepository.GetNewID(prefix , mast.BILL_DATE , mast.BILL_NO);
                mast.OPERATE_DATE = DateTime.Now;
                //mast.BILL_DATE = DateTime.Now;
                mast.STATE = "1"; //默认保存状态
                mast.STATUS = "0"; //默认手工输入
                mast.TARGET_CODE = targetcode.TARGET_CODE;
                BillMasterRepository.Add(mast);

                DataTable dt = THOK.Common.ConvertData.JsonToDataTable(((System.String[])detail)[0]);
                foreach (DataRow dr in dt.Rows)
                {
                    
                    WMS_BILL_DETAIL subdetail = new WMS_BILL_DETAIL();
                    THOK.Common.ConvertData.DataBind(subdetail, dr);
                    subdetail.ITEM_NO = serial;
                    subdetail.BILL_NO  = mast.BILL_NO ;
                    subdetail.IS_MIX = "0";
                    subdetail.FPRODUCT_CODE = "";
                    BillDetailRepository.Add(subdetail);
                    serial++;
                }

              int brs=  BillMasterRepository.SaveChanges();
              if (brs == -1) rejust = false;
              else
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
                item.REAL_WEIGHT = formula.CMD_PRODUCT.WEIGHT;
                item.WEIGHT = formula.CMD_PRODUCT.WEIGHT;
                item.PRODUCT_NAME = formula.CMD_PRODUCT.PRODUCT_NAME ;
                item.PACKAGE_COUNT = (int)(((formula.PERCENT * BATCH_WEIGHT) / 100) / formula.CMD_PRODUCT.WEIGHT);
                item.TOTAL_WEIGHT = item.REAL_WEIGHT * item.PACKAGE_COUNT;
                item.IS_MIX = "0";
                item.FPRODUCT_CODE = "";
                decimal lastweight = ((formula.PERCENT * BATCH_WEIGHT) / 100) - (item.PACKAGE_COUNT * formula.CMD_PRODUCT.WEIGHT); //不足一包的重量
                if (lastweight != 0)//
                {
                    serial++;
                    FormulaDetail subitem = new FormulaDetail();
                    subitem.ITEM_NO = serial;
                    subitem.PRODUCT_CODE = formula.PRODUCT_CODE;
                    subitem.PRODUCT_NAME = formula.CMD_PRODUCT.PRODUCT_NAME;
                    subitem.REAL_WEIGHT  = lastweight;
                    subitem.WEIGHT = formula.CMD_PRODUCT.WEIGHT;
                    subitem.PACKAGE_COUNT = 1;
                    subitem.TOTAL_WEIGHT = lastweight;
                    subitem.IS_MIX = "0";
                    subitem.FPRODUCT_CODE = "";
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
                billmast.LINE_NO = mast.LINE_NO=="null"?"":mast .LINE_NO ;
            }
            var details = BillDetailRepository.GetQueryable().Where(i => i.BILL_NO == mast.BILL_NO);
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_BILL_DETAIL sub in tmp)
            {
                BillDetailRepository.Delete(sub);
            }

            DataTable dt = THOK.Common.ConvertData.JsonToDataTable(((System.String[])detail)[0]); //修改
            if (dt != null)
            {
                int serial = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    WMS_BILL_DETAIL subdetail = new WMS_BILL_DETAIL();
                    THOK.Common.ConvertData.DataBind(subdetail, dr);
                    subdetail.ITEM_NO = serial;
                    subdetail.BILL_NO = mast.BILL_NO;
                    if (subdetail.FPRODUCT_CODE == "null") subdetail.FPRODUCT_CODE = "";
                    //subdetail.IS_MIX = "0";
                    BillDetailRepository.Add(subdetail);
                    serial++;
                }
            }
          int result= BillMasterRepository.SaveChanges();
          if (result == -1) return false;
          else
              return true;
        }

        //获取序列号
        public object GetSerial(string BILLNO)
        {
            IQueryable<WMS_BILL_DETAIL> query = BillDetailRepository.GetQueryable();
            var Serial = query.OrderByDescending(i => i.ITEM_NO).FirstOrDefault(i => i.BILL_NO == BILLNO);
            if (Serial != null)
            {
                var newSerial = new
                {
                    Itemno = Serial.ITEM_NO
                };
                return newSerial;
            }
            else {
                var newSerial = new
                {
                    Itemno = 0
                };
                return newSerial;
            }
               
        }

        //设置混装
        public bool SetMIX(string BillNo, object detail)
        {
            try
            {
                IQueryable<WMS_BILL_DETAIL> query = BillDetailRepository.GetQueryable().Where(i => i.BILL_NO == BillNo);
                DataTable dt = THOK.Common.ConvertData.JsonToDataTable(((System.String[])detail)[0]); //修改
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        WMS_BILL_DETAIL subdetail = new WMS_BILL_DETAIL();
                        THOK.Common.ConvertData.DataBind(subdetail, dr);
                        var billdetail = query.FirstOrDefault(i => i.ITEM_NO == subdetail.ITEM_NO);
                        billdetail.IS_MIX = subdetail.IS_MIX;
                        billdetail.FPRODUCT_CODE = subdetail.FPRODUCT_CODE;
                    }
                }
             int result= BillDetailRepository.SaveChanges();
             if (result == -1) return false;
                return true;
            }
            catch (Exception ex) { return false; }
        }

        //删除
        public bool Delete(string BillNo)
        {
            var deletbillno = BillMasterRepository.GetQueryable().Where(i => i.BILL_NO  == BillNo ).FirstOrDefault();
            var details = BillDetailRepository.GetQueryable().Where(i => i.BILL_NO  == BillNo);
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_BILL_DETAIL  sub in tmp)
            {
                BillDetailRepository.Delete(sub);
            }
            BillMasterRepository.Delete(deletbillno);
           int result= BillMasterRepository.SaveChanges();
           if (result == -1) return false;
            return true;
        }

        //查询需要补料的单据
        public object billselect(int page, int rows, string billmethod, string billno)
        {
            string billtyp = "";
            //var billmaster=new object();
            IQueryable<WMS_BILL_MASTER> billquery = BillMasterRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            IQueryable<AUTH_USER> userquery = UserRepository.GetQueryable();
            IQueryable<CMD_CELL> cellquery = cellRepository.GetQueryable();
           if (billmethod == "2") {  //抽检单
               billtyp = "3"; 
            }
           if (billmethod == "3") {//补料的(即单据类型为入库的);
               billtyp = "1";
           }
          var  billmaster =(from a in billquery
                            //join h in cellquery on a.BILL_NO equals h.BILL_NO
                             join b in statequery on a.STATUS equals b.STATE
                             join c in statequery on a.STATE equals c.STATE
                             join d in statequery on a.BILL_METHOD equals d.STATE
                             join e in userquery on a.OPERATER equals e.USER_ID
                             join f in userquery on a.CHECKER equals f.USER_ID into fg
                             from f in fg.DefaultIfEmpty()
                             join g in userquery on a.TASKER equals g.USER_ID into hg
                             from g in hg.DefaultIfEmpty()
                             where b.TABLE_NAME == "WMS_BILL_MASTER" && b.FIELD_NAME == "STATUS" && c.TABLE_NAME == "WMS_BILL_MASTER" && c.FIELD_NAME == "STATE"
                             && d.TABLE_NAME == "WMS_BILL_MASTER" && d.FIELD_NAME == "BILL_METHOD" && a.CMD_BILL_TYPE.BILL_TYPE == billtyp
                             select new
                             {
                                 a.BILL_NO,
                                 a.BILL_DATE,
                                 a.BTYPE_CODE, //单据类型代码
                                 a.CMD_BILL_TYPE.BTYPE_NAME,  //单据类型名称
                                 a.SCHEDULE_NO,
                                 //a.WAREHOUSE_CODE,
                                 //a.CMD_WAREHOUSE.WAREHOUSE_NAME,
                                 //a.SYS_BILL_TARGET.TARGET_NAME, //目标位置名
                                 //a.TARGET_CODE, //目标位置代码
                                 a.STATUS,//单据来源代号
                                 STATUSNAME = b.STATE_DESC,//单据来源描述,手动,系统输入
                                 a.STATE,//单据状态代号
                                 //STATENAME = c.STATE_DESC,//单据状态描述
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
                             }).Distinct () ;
          if (billmethod == "2")
          {  //抽检单
              billmaster = billmaster.Where(i => (from b in cellquery  where b.BILL_NO ==i.SOURCE_BILLNO  select b.BILL_NO).Contains(i.SOURCE_BILLNO ));
          }
          if (billmethod == "3")
          {//补料的(即单据类型为入库的);
              billmaster = billmaster.Where(i => (from b in cellquery where b.BILL_NO ==i.BILL_NO  select b.BILL_NO).Contains(i.BILL_NO ));
          }
            if (!string.IsNullOrEmpty(billno)) {
                string info = billno.Split(':')[0];
                string val = billno.Split(':')[1];
                if (!string.IsNullOrEmpty(val))
                {
                    if (info == "billno")
                        billmaster = billmaster.Where(i => i.BILL_NO == val);
                    else if (info == "cigarate")
                        billmaster = billmaster.Where(i => i.CIGARETTE_CODE == val);
                    else
                        billmaster = billmaster.Where(i => i.FORMULA_CODE == val);
                }
            }
            var temp = billmaster.ToArray().OrderByDescending(i => i.OPERATE_DATE).Select(i => new
            {
                i.BILL_NO,
                BILL_DATE = i.BILL_DATE.ToString("yyyy-MM-dd"),
                i.BTYPE_CODE,
                i.BTYPE_NAME,
                i.SCHEDULE_NO,
                //i.WAREHOUSE_CODE,
                //i.WAREHOUSE_NAME,
                //i.TARGET_CODE,
                //i.TARGET_NAME,
                i.STATUS,
                i.STATUSNAME,
                i.STATE,
                //i.STATENAME,
                //i.SOURCE_BILLNO,
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
                i.SOURCE_BILLNO,
                i.LINE_NAME
            });
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }

        //抽检补料入库单添加
        public bool FillBillAdd(WMS_BILL_MASTER mast, object detail, string prefix)
        {
            var targetcode = CmdBillTypeRepository.GetQueryable().FirstOrDefault(i => i.BTYPE_CODE == mast.BTYPE_CODE);
            bool rejust = false;
            int serial = 1;
            try
            {
                mast.BILL_NO = BillMasterRepository.GetNewID(prefix, mast.BILL_DATE, mast.BILL_NO);
                mast.OPERATE_DATE = DateTime.Now;
                //mast.BILL_DATE = DateTime.Now;
                mast.STATE = "1"; //默认保存状态
                mast.STATUS = "0"; //默认手工输入
                mast.TARGET_CODE = targetcode.TARGET_CODE;
                BillMasterRepository.Add(mast);

                DataTable dt = THOK.Common.ConvertData.JsonToDataTable(((System.String[])detail)[0]);
                foreach (DataRow dr in dt.Rows)
                {

                    WMS_PRODUCT_STATE subdetail = new WMS_PRODUCT_STATE();
                    THOK.Common.ConvertData.DataBind(subdetail, dr);
                    subdetail.ITEM_NO = serial;
                    subdetail.BILL_NO = mast.BILL_NO;
                    //if (subdetail.SCHEDULE_NO == "null") subdetail.SCHEDULE_NO = "";
                    //if (subdetail.OUT_BILLNO == "null") subdetail.OUT_BILLNO = "";
                    ProductStateRepository.Add(subdetail);
                    serial++;
                }

                int brs = BillMasterRepository.SaveChanges();
                if (brs == -1) rejust = false;
                else
                    rejust = true;
            }
            catch (Exception ex)
            {

            }
            return rejust;
        }

        //抽检补料入库单修改
        public bool FillBillEdit(WMS_BILL_MASTER mast, object detail)
        {
            var billmast = BillMasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO == mast.BILL_NO);
            billmast.BILL_DATE = mast.BILL_DATE;
            billmast.SOURCE_BILLNO = mast.SOURCE_BILLNO;

            var details = ProductStateRepository.GetQueryable().Where(i => i.BILL_NO == mast.BILL_NO);
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_PRODUCT_STATE  sub in tmp)
            {
                ProductStateRepository.Delete(sub);
            }

            DataTable dt = THOK.Common.ConvertData.JsonToDataTable(((System.String[])detail)[0]); //修改
            if (dt != null)
            {
                int serial = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    WMS_PRODUCT_STATE subdetail = new WMS_PRODUCT_STATE();
                    THOK.Common.ConvertData.DataBind(subdetail, dr);
                    subdetail.ITEM_NO = serial;
                    subdetail.BILL_NO = mast.BILL_NO;
                    if (subdetail.SCHEDULE_NO == "null") subdetail.SCHEDULE_NO = "";
                    if (subdetail.OUT_BILLNO == "null") subdetail.OUT_BILLNO = "";
                    
                    ProductStateRepository.Add(subdetail);
                    serial++;
                }
            }
            int result = BillMasterRepository.SaveChanges();
            if (result == -1) return false;
            else
                return true;
        }

        //抽检补料入库单删除
        public bool FillBillDelete(string BillNo)
        {
            var deletbillno = BillMasterRepository.GetQueryable().Where(i => i.BILL_NO == BillNo).FirstOrDefault();
            var details = ProductStateRepository.GetQueryable().Where(i => i.BILL_NO == BillNo);
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_PRODUCT_STATE  sub in tmp)
            {
                ProductStateRepository.Delete(sub);
            }
            BillMasterRepository.Delete(deletbillno);
            int result = BillMasterRepository.SaveChanges();
            if (result == -1) return false;
            return true;
        }

        //抽检补料入库单作业
        public bool FillBillTask(string BillNo, string tasker)
        {
            var billmast = BillMasterRepository.GetQueryable().FirstOrDefault (i=>i.BILL_NO ==BillNo );
            var productstatequery = ProductStateRepository.GetQueryable().Where (i=>i.BILL_NO ==BillNo );
            var tmp = productstatequery.ToArray().AsEnumerable().Select(i => i);
            string soursebillno = billmast.SOURCE_BILLNO;
            if (billmast.BILL_METHOD == "2") //抽检
            {
                soursebillno = BillMasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO == billmast.SOURCE_BILLNO).SOURCE_BILLNO;
            }
            try
            {
                int serial = 1;
                foreach (WMS_PRODUCT_STATE item in tmp)
                {
                    WCS_TASK task = new WCS_TASK();
                    task.TASK_ID = billmast.BILL_NO + serial.ToString("00");
                    task.BILL_NO = billmast.BILL_NO;
                    task.TASK_TYPE = billmast.CMD_BILL_TYPE.TASK_TYPE;
                    task.TASK_LEVEL = decimal.Parse(billmast.CMD_BILL_TYPE.TASK_LEVEL);
                    task.PRODUCT_CODE = item.PRODUCT_CODE;
                    task.PRODUCT_BARCODE = item.PRODUCT_BARCODE;
                    task.REAL_WEIGHT = item.REAL_WEIGHT;
                    task.TARGET_CODE = billmast.TARGET_CODE;
                    task.STATE = "0";
                    task.TASK_DATE = DateTime.Now;
                    task.TASKER = tasker;
                    task.PRODUCT_TYPE = "1";
                    task.IS_MIX = item.IS_MIX;
                    task.SOURCE_BILLNO = soursebillno;
                    WcsTaskRepository.Add(task);
                    serial++;
                }
                billmast.TASK_DATE = DateTime.Now;
                billmast.TASKER = tasker;
                billmast.STATE = "3";
               int result= BillMasterRepository.SaveChanges();
               if (result == -1) return false;
               else  return true;
            }
            catch (Exception ex) {
                return false;
            }
        }

        //获取所有货位上不为空且处于解锁状态的启用的货位
        public object Cellselect(int page, int rows, string soursebill, string queryinfo, string selectedcellcodestr)
        {
            IQueryable<CMD_CELL> cellquery = cellRepository.GetQueryable();
            IQueryable<WMS_PRODUCT_STATE> productstate = ProductStateRepository.GetQueryable();
            var cells = (from a in cellquery
                         join b in productstate on a.PRODUCT_BARCODE equals b.PRODUCT_BARCODE
                        select new { 
                            a.CELL_CODE ,
                            a.CELL_NAME ,
                            a.PRODUCT_CODE,
                            a.PRODUCT_BARCODE,
                            a.CMD_PRODUCT .PRODUCT_NAME ,
                            a.CMD_PRODUCT .WEIGHT ,
                            a.PALLET_CODE ,
                            a.BILL_NO ,
                            a.REAL_WEIGHT ,
                            a.WAREHOUSE_CODE ,
                            a.IN_DATE,
                            PACKAGE_COUNT = 1,
                            a.IS_LOCK ,
                            a.IS_ACTIVE,
                            a.ERROR_FLAG,
                            b.IS_MIX 
                        }).Distinct ();
            var temp = cells.Where(i => i.PRODUCT_CODE != null&&i.IS_LOCK =="0"&&i.IS_ACTIVE=="1"&&i.ERROR_FLAG=="0").OrderBy(i => i.CELL_CODE ).Select(i => new
            {
                i.CELL_CODE,
                i.CELL_NAME,
                i.PRODUCT_CODE,
                i.PRODUCT_NAME,
                i.PRODUCT_BARCODE,
                i.WEIGHT,
                i.REAL_WEIGHT,
                i.PALLET_CODE,
                i.BILL_NO,
                i.WAREHOUSE_CODE,
                PACKAGE_COUNT = 1,
                i.IS_MIX,
                i.IN_DATE
            });
            if (!string.IsNullOrEmpty(soursebill)) {
                temp = temp.Where(i => i.BILL_NO==soursebill );
            }
            if (!string.IsNullOrEmpty(queryinfo)) {
                string info = queryinfo.Split(':')[0];
                string val = queryinfo.Split(':')[1];
                if (!string.IsNullOrEmpty(val)) {
                    if (info == "cellcode") { //货位号查询
                        temp =temp .Where (i=>i.CELL_CODE.Contains (val ));
                    }
                    else if (info == "productcode"){//产品编号查询
                        temp = temp.Where(i => i.PRODUCT_CODE.Contains(val));
                    }
                    else {  //产品条码查询
                        temp = temp.Where(i => i.PRODUCT_BARCODE.Contains(val));
                    }
                }
            }
            if (!string.IsNullOrEmpty(selectedcellcodestr))
            {
                temp = temp.Where(i => !selectedcellcodestr.Contains(i.CELL_CODE));
            }
            //temp = temp.OrderBy(i => i.IN_DATE);
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }

        //盘点单新增
        public bool InventoryAdd(WMS_BILL_MASTER mast, object detail, string prefix)
        {
            var targetcode = CmdBillTypeRepository.GetQueryable().FirstOrDefault(i => i.BTYPE_CODE == mast.BTYPE_CODE);
            List<WMS_BILL_DETAIL > detaillist = new List<WMS_BILL_DETAIL>();
            bool rejust = false;
            int serial = 1;
            try
            {
                mast.BILL_NO = BillMasterRepository.GetNewID(prefix, mast.BILL_DATE, mast.BILL_NO);
                mast.OPERATE_DATE = DateTime.Now;
                mast.BILL_METHOD = "0";
                mast.STATE = "1"; //默认保存状态
                mast.STATUS = "0"; //默认手工输入
                mast.TARGET_CODE = targetcode.TARGET_CODE;
                BillMasterRepository.Add(mast);

                DataTable dt = THOK.Common.ConvertData.JsonToDataTable(((System.String[])detail)[0]);
                foreach (DataRow dr in dt.Rows)
                {

                    WMS_PRODUCT_STATE subdetail = new WMS_PRODUCT_STATE();
                    WMS_BILL_DETAIL billdetail = new WMS_BILL_DETAIL();
                    THOK.Common.ConvertData.DataBind(subdetail, dr);
                    subdetail.ITEM_NO = serial;
                    subdetail.BILL_NO = mast.BILL_NO;

                    billdetail.BILL_NO = mast.BILL_NO;
                    billdetail.ITEM_NO = serial;
                    billdetail.PRODUCT_CODE = subdetail.PRODUCT_CODE;
                    billdetail.WEIGHT = subdetail.WEIGHT;
                    billdetail.REAL_WEIGHT = subdetail.REAL_WEIGHT;
                    billdetail.PACKAGE_COUNT = subdetail.PACKAGE_COUNT;
                    billdetail.IS_MIX = subdetail.IS_MIX;
                    WMS_BILL_DETAIL exits = detaillist.Find(i => i.PRODUCT_CODE == subdetail.PRODUCT_CODE);
                    if (exits != null) exits.PACKAGE_COUNT += 1;
                    else detaillist.Add(billdetail);
                    //锁定货位
                    var cell = cellRepository.GetQueryable().FirstOrDefault(i => i.CELL_CODE == subdetail.CELL_CODE);
                    cell.IS_LOCK = "1";

                    ProductStateRepository.Add(subdetail);
                    serial++;
                }
                serial = 1;
                foreach (WMS_BILL_DETAIL item in detaillist)
                {
                    item.ITEM_NO = serial ;
                    BillDetailRepository.Add(item);
                    serial++;
                }
                int brs = BillMasterRepository.SaveChanges();
                if (brs == -1) rejust = false;
                else
                    rejust = true;
            }
            catch (Exception ex)
            {

            }
            return rejust;
        }
        //盘点单修改
        public bool InventoryEdit(WMS_BILL_MASTER mast, object detail)
        {
            List<WMS_BILL_DETAIL> detaillist = new List<WMS_BILL_DETAIL>();
            var billmast = BillMasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO == mast.BILL_NO);
            billmast.BILL_DATE = mast.BILL_DATE;
            billmast.SOURCE_BILLNO = mast.SOURCE_BILLNO;

            var details = ProductStateRepository.GetQueryable().Where(i => i.BILL_NO == mast.BILL_NO);
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_PRODUCT_STATE sub in tmp)
            {
                //解锁原先的货位
                var oldcell = cellRepository.GetQueryable().FirstOrDefault(i => i.CELL_CODE == sub.CELL_CODE);
                oldcell.IS_LOCK = "0";

                ProductStateRepository.Delete(sub);
            }
            var billdetails = BillDetailRepository.GetQueryable().Where(i => i.BILL_NO == mast.BILL_NO);
            WMS_BILL_DETAIL[] billdetaillist = billdetails.ToArray();
            BillDetailRepository.Delete(billdetaillist);

            DataTable dt = THOK.Common.ConvertData.JsonToDataTable(((System.String[])detail)[0]); //修改
            if (dt != null)
            {
                int serial = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    WMS_PRODUCT_STATE subdetail = new WMS_PRODUCT_STATE();
                    WMS_BILL_DETAIL billdetail = new WMS_BILL_DETAIL();
                    THOK.Common.ConvertData.DataBind(subdetail, dr);
                    subdetail.ITEM_NO = serial;
                    subdetail.BILL_NO = mast.BILL_NO;
                    if (subdetail.SCHEDULE_NO == "null") subdetail.SCHEDULE_NO = "";
                    if (subdetail.OUT_BILLNO == "null") subdetail.OUT_BILLNO = "";

                    billdetail.BILL_NO = mast.BILL_NO;
                    billdetail.ITEM_NO = serial;
                    billdetail.PRODUCT_CODE = subdetail.PRODUCT_CODE;
                    billdetail.WEIGHT = subdetail.WEIGHT;
                    billdetail.REAL_WEIGHT = subdetail.REAL_WEIGHT;
                    billdetail.PACKAGE_COUNT = subdetail.PACKAGE_COUNT;
                    billdetail.IS_MIX = subdetail.IS_MIX;
                    WMS_BILL_DETAIL exits = detaillist.Find(i => i.PRODUCT_CODE == subdetail.PRODUCT_CODE);
                    if (exits != null) exits.PACKAGE_COUNT += 1;
                    else detaillist.Add(billdetail);
                    //锁定货位
                    var cell2 = cellRepository.GetQueryable().FirstOrDefault(i => i.CELL_CODE == subdetail.CELL_CODE);
                    cell2.IS_LOCK = "1";

                    ProductStateRepository.Add(subdetail);
                    serial++;
                }
                serial = 1;
                foreach (WMS_BILL_DETAIL item in detaillist)
                {
                    item.ITEM_NO = serial;
                    BillDetailRepository.Add(item);
                    serial++;
                }
            }
            int result = BillMasterRepository.SaveChanges();
            if (result == -1) return false;
            else
                return true;
        }

        //盘点单删除
        public bool InventoryDelete(string BillNo)
        {
            var deletbillno = BillMasterRepository.GetQueryable().Where(i => i.BILL_NO == BillNo).FirstOrDefault();
            var details = ProductStateRepository.GetQueryable().Where(i => i.BILL_NO == BillNo);
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_PRODUCT_STATE sub in tmp)
            {
                //解锁货位
                var cell = cellRepository.GetQueryable().FirstOrDefault(i => i.CELL_CODE == sub.CELL_CODE);
                cell.IS_LOCK = "0";
                ProductStateRepository.Delete(sub);
            }

            var billdetails = BillDetailRepository.GetQueryable().Where(i => i.BILL_NO == BillNo);
            WMS_BILL_DETAIL[] billdetaillist = billdetails.ToArray();
            BillDetailRepository.Delete(billdetaillist);

            BillMasterRepository.Delete(deletbillno);
            int result = BillMasterRepository.SaveChanges();
            if (result == -1) return false;
            return true;
        }

        //盘点单作业
        public bool InventoryTask(string BillNo, string tasker)
        {
            var billmast = BillMasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO == BillNo);
            var productstatequery = ProductStateRepository.GetQueryable().Where(i => i.BILL_NO == BillNo);
            var tmp = productstatequery.ToArray().AsEnumerable().Select(i => i);
            try
            {
                int serial = 1;
                foreach (WMS_PRODUCT_STATE item in tmp)
                {
                    WCS_TASK task = new WCS_TASK();
                    task.TASK_ID = billmast.BILL_NO + serial.ToString("00");
                    task.BILL_NO = billmast.BILL_NO;
                    task.CELL_CODE = item.CELL_CODE;
                    task.TASK_TYPE = billmast.CMD_BILL_TYPE.TASK_TYPE;
                    task.TASK_LEVEL = decimal.Parse(billmast.CMD_BILL_TYPE.TASK_LEVEL);
                    task.PRODUCT_CODE = item.PRODUCT_CODE;
                    task.PRODUCT_BARCODE = item.PRODUCT_BARCODE;
                    task.REAL_WEIGHT = item.REAL_WEIGHT;
                    task.TARGET_CODE = billmast.TARGET_CODE;
                    task.STATE = "0";
                    task.TASK_DATE = DateTime.Now;
                    task.TASKER = tasker;
                    task.PRODUCT_TYPE = "1";
                    task.IS_MIX = item.IS_MIX;
                    task.SOURCE_BILLNO = billmast .SOURCE_BILLNO ;
                    ////锁定货位
                    //var cell = cellRepository.GetQueryable().FirstOrDefault(i => i.CELL_CODE == item.CELL_CODE);
                    //cell.IS_LOCK = "1";

                    WcsTaskRepository.Add(task);
                    serial++;
                }
                billmast.TASK_DATE = DateTime.Now;
                billmast.TASKER = tasker;
                billmast.STATE = "3";
                int result = BillMasterRepository.SaveChanges();
                if (result == -1) return false;
                else return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //抽检单作业
        public bool SamplingTask(string BillNo, string tasker)
        {
            var billmast = BillMasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO == BillNo);
            var productstatequery = ProductStateRepository.GetQueryable().Where(i => i.BILL_NO == BillNo);
            var tmp = productstatequery.ToArray().AsEnumerable().Select(i => i);
            try
            {
                int serial = 1;
                foreach (WMS_PRODUCT_STATE item in tmp)
                {
                    WCS_TASK task = new WCS_TASK();
                    task.TASK_ID = billmast.BILL_NO + serial.ToString("00");
                    task.BILL_NO = billmast.BILL_NO;
                    task.CELL_CODE = item.CELL_CODE;
                    task.TASK_TYPE = billmast.CMD_BILL_TYPE.TASK_TYPE;
                    task.TASK_LEVEL = decimal.Parse(billmast.CMD_BILL_TYPE.TASK_LEVEL);
                    task.PRODUCT_CODE = item.PRODUCT_CODE;
                    task.PRODUCT_BARCODE = item.PRODUCT_BARCODE;
                    task.REAL_WEIGHT = item.REAL_WEIGHT;
                    task.TARGET_CODE = billmast.TARGET_CODE;
                    task.STATE = "0";
                    task.TASK_DATE = DateTime.Now;
                    task.TASKER = tasker;
                    task.PRODUCT_TYPE = "1";
                    task.IS_MIX = item.IS_MIX;
                    task.SOURCE_BILLNO = billmast.SOURCE_BILLNO;
                    ////锁定货位
                    //var cell = cellRepository.GetQueryable().FirstOrDefault(i => i.CELL_CODE == item.CELL_CODE);
                    //cell.IS_LOCK = "1";

                    WcsTaskRepository.Add(task);
                    serial++;
                }
                billmast.TASK_DATE = DateTime.Now;
                billmast.TASKER = tasker;
                billmast.STATE = "3";
                int result = BillMasterRepository.SaveChanges();
                if (result == -1) return false;
                else return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //找出空的货位
        public object GetNullCell(int page, int rows, string queryinfo)
        {
            IQueryable<CMD_CELL> cellquery = cellRepository.GetQueryable();
            var temp = cellquery.Where(i => i.PRODUCT_CODE ==null &&i.IS_ACTIVE=="1"&&i.ERROR_FLAG=="0").OrderBy(i => i.CELL_CODE).Select(i => new { 
                i.CELL_CODE ,
                i.CELL_COLUMN,
                i.CELL_NAME,
                i.CELL_ROW,
                i.AREA_CODE ,
                i.CMD_AREA .AREA_NAME,
                i.SHELF_CODE ,
                i.CMD_SHELF .SHELF_NAME,
                i.WAREHOUSE_CODE ,
                i.MEMO ,
                i.IS_LOCK,
                i.IS_ACTIVE
            });
            if (!string.IsNullOrEmpty(queryinfo)) {
                string info = queryinfo.Split(':')[0];
                string val = queryinfo.Split(':')[1];
                if (!string.IsNullOrEmpty(val))
                {
                    if (info == "cellcode")
                    { //货位号查询
                        temp = temp.Where(i => i.CELL_CODE.Contains(val));
                    }
                    else if (info == "cellcolumn")
                    { //列位查询
                        decimal colum = decimal.Parse(val);
                        temp = temp.Where(i => i.CELL_COLUMN==colum );
                    }
                    else if (info == "cellrow")
                    {//层位查询
                        decimal row = decimal.Parse(val);
                        temp = temp.Where(i => i.CELL_ROW == row);
                    }
                    else
                    {//货架查询
                        temp = temp.Where(i => i.SHELF_CODE.Contains(val));
                    }
                }
            }
            temp = temp.Where(i => i.IS_LOCK == "0");
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }

        //移库单新增
        public bool MoveStockAdd(WMS_BILL_MASTER mast, object detail, string prefix, out string error)
        {
            //string errormessage = "";
            var targetcode = CmdBillTypeRepository.GetQueryable().FirstOrDefault(i => i.BTYPE_CODE == mast.BTYPE_CODE);
            List<WMS_BILL_DETAIL> detaillist = new List<WMS_BILL_DETAIL>();
            bool rejust = false;
            int serial = 1;
            try
            {
                mast.BILL_NO = BillMasterRepository.GetNewID(prefix, mast.BILL_DATE, mast.BILL_NO);
                mast.OPERATE_DATE = DateTime.Now;
                mast.BILL_METHOD = "0";
                mast.STATE = "1"; //默认保存状态
                mast.STATUS = "0"; //默认手工输入
                mast.TARGET_CODE = targetcode.TARGET_CODE;
                BillMasterRepository.Add(mast);

                DataTable dt = THOK.Common.ConvertData.JsonToDataTable(((System.String[])detail)[0]);
                foreach (DataRow dr in dt.Rows)
                {

                    WMS_PRODUCT_STATE subdetail = new WMS_PRODUCT_STATE();
                    WMS_BILL_DETAIL billdetail = new WMS_BILL_DETAIL();
                    THOK.Common.ConvertData.DataBind(subdetail, dr);
                    subdetail.ITEM_NO = serial;
                    subdetail.BILL_NO = mast.BILL_NO;
                    //锁定所要移库的货位
                    var cell = cellRepository.GetQueryable().FirstOrDefault(i => i.CELL_CODE == subdetail.CELL_CODE);
                    cell.IS_LOCK = "1";
                    //锁定移库后的新货位
                    var newcell = cellRepository.GetQueryable().FirstOrDefault(i => i.CELL_CODE == subdetail.NEWCELL_CODE);
                    if (newcell.IS_LOCK == "1")
                    {
                        error= newcell.CELL_CODE + "货位已被锁定.";
                        return false;
                    }
                    else if (newcell .PRODUCT_CODE  !=null )
                    {
                        error = newcell.CELL_CODE + "该货位,已有货物,请选空的货位.";
                        return false;
                    }
                    else if (newcell.IS_ACTIVE == "0") {
                        error = newcell.CELL_CODE + "该货位,已被禁用.";
                        return false;
                    }
                    else if (newcell.ERROR_FLAG == "1") {
                        error = newcell.CELL_CODE + "该货位,目前有异常.";
                        return false;
                    }
                    else
                    {
                        newcell.IS_LOCK = "1";
                    }


                    billdetail.BILL_NO = mast.BILL_NO;
                    billdetail.ITEM_NO = serial;
                    billdetail.PRODUCT_CODE = subdetail.PRODUCT_CODE;
                    billdetail.WEIGHT = subdetail.WEIGHT;
                    billdetail.REAL_WEIGHT = subdetail.REAL_WEIGHT;
                    billdetail.PACKAGE_COUNT = subdetail.PACKAGE_COUNT;
                    billdetail.IS_MIX = subdetail.IS_MIX;
                    WMS_BILL_DETAIL exits = detaillist.Find(i => i.PRODUCT_CODE == subdetail.PRODUCT_CODE);
                    if (exits != null) exits.PACKAGE_COUNT += 1;
                    else detaillist.Add(billdetail);

                    ProductStateRepository.Add(subdetail);
                    serial++;
                }
                serial = 1;
                foreach (WMS_BILL_DETAIL item in detaillist)
                {
                    item.ITEM_NO = serial;
                    BillDetailRepository.Add(item);
                    serial++;
                }
                int brs = BillMasterRepository.SaveChanges();
                if (brs == -1) { error = ""; rejust = false; }
                else
                {
                    error = "";
                    rejust = true;
                }
            }
            catch (Exception ex)
            {
                error = "该货位不存.";
            }
            return rejust;
        }
        //移库单修改
        public bool MoveStockEdit(WMS_BILL_MASTER mast, object detail, out string error)
        {
            List<WMS_BILL_DETAIL> detaillist = new List<WMS_BILL_DETAIL>();
            var billmast = BillMasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO == mast.BILL_NO);
            billmast.BILL_DATE = mast.BILL_DATE;
            billmast.SOURCE_BILLNO = mast.SOURCE_BILLNO;

            var details = ProductStateRepository.GetQueryable().Where(i => i.BILL_NO == mast.BILL_NO);
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_PRODUCT_STATE sub in tmp)
            {
                //解锁原先所选的新货位
                var oldcell = cellRepository.GetQueryable().FirstOrDefault(i => i.CELL_CODE == sub.NEWCELL_CODE);
                oldcell.IS_LOCK = "0";
                //解锁原先要进行移库的货位
                var cell = cellRepository.GetQueryable().FirstOrDefault(i => i.CELL_CODE == sub.CELL_CODE);
                cell.IS_LOCK = "0";

                ProductStateRepository.Delete(sub);
            }
            var billdetails = BillDetailRepository.GetQueryable().Where(i => i.BILL_NO == mast.BILL_NO);
            WMS_BILL_DETAIL[] billdetaillist = billdetails.ToArray();
            BillDetailRepository.Delete(billdetaillist);

            DataTable dt = THOK.Common.ConvertData.JsonToDataTable(((System.String[])detail)[0]); //修改
            if (dt != null)
            {
                int serial = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    WMS_PRODUCT_STATE subdetail = new WMS_PRODUCT_STATE();
                    WMS_BILL_DETAIL billdetail = new WMS_BILL_DETAIL();
                    THOK.Common.ConvertData.DataBind(subdetail, dr);
                    subdetail.ITEM_NO = serial;
                    subdetail.BILL_NO = mast.BILL_NO;
                    if (subdetail.SCHEDULE_NO == "null") subdetail.SCHEDULE_NO = "";
                    if (subdetail.OUT_BILLNO == "null") subdetail.OUT_BILLNO = "";
                    //锁定要移库的货位
                    var cell2 = cellRepository.GetQueryable().FirstOrDefault(i => i.CELL_CODE == subdetail.CELL_CODE);
                    cell2.IS_LOCK = "1";

                    //锁定移库后的新货位
                    var newcell = cellRepository.GetQueryable().FirstOrDefault(i => i.CELL_CODE == subdetail.NEWCELL_CODE);
                    if (newcell.IS_LOCK == "1")
                    {
                        error = newcell.CELL_CODE + "货位已被锁定.";
                        return false;
                    }
                    else if (newcell.PRODUCT_CODE != null)
                    {
                        error = newcell.CELL_CODE + "货位上已我货物.";
                        return false;
                    }
                    else
                        newcell.IS_LOCK = "1";

                    billdetail.BILL_NO = mast.BILL_NO;
                    billdetail.ITEM_NO = serial;
                    billdetail.PRODUCT_CODE = subdetail.PRODUCT_CODE;
                    billdetail.WEIGHT = subdetail.WEIGHT;
                    billdetail.REAL_WEIGHT = subdetail.REAL_WEIGHT;
                    billdetail.PACKAGE_COUNT = subdetail.PACKAGE_COUNT;
                    billdetail.IS_MIX = subdetail.IS_MIX;
                    WMS_BILL_DETAIL exits = detaillist.Find(i => i.PRODUCT_CODE == subdetail.PRODUCT_CODE);
                    if (exits != null) exits.PACKAGE_COUNT += 1;
                    else detaillist.Add(billdetail);

                    ProductStateRepository.Add(subdetail);
                    serial++;
                }
                serial = 1;
                foreach (WMS_BILL_DETAIL item in detaillist)
                {
                    item.ITEM_NO = serial;
                    BillDetailRepository.Add(item);
                    serial++;
                }
            }
            int result = BillMasterRepository.SaveChanges();
            error = "";
            if (result == -1) return false;
            else
                return true;
        }

        //移库单作业
        public bool MoveStockTask(string BillNo, string tasker)
        {
            var billmast = BillMasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO == BillNo);
            var productstatequery = ProductStateRepository.GetQueryable().Where(i => i.BILL_NO == BillNo);
            var tmp = productstatequery.ToArray().AsEnumerable().Select(i => i);
            try
            {
                int serial = 1;
                foreach (WMS_PRODUCT_STATE item in tmp)
                {
                    WCS_TASK task = new WCS_TASK();
                    task.TASK_ID = billmast.BILL_NO + serial.ToString("00");
                    task.BILL_NO = billmast.BILL_NO;
                    task.CELL_CODE = item.CELL_CODE;
                    task.TASK_TYPE = billmast.CMD_BILL_TYPE.TASK_TYPE;
                    task.TASK_LEVEL = decimal.Parse(billmast.CMD_BILL_TYPE.TASK_LEVEL);
                    task.PRODUCT_CODE = item.PRODUCT_CODE;
                    task.PRODUCT_BARCODE = item.PRODUCT_BARCODE;
                    task.REAL_WEIGHT = item.REAL_WEIGHT;
                    task.TARGET_CODE = billmast.TARGET_CODE;
                    task.STATE = "0";
                    task.TASK_DATE = DateTime.Now;
                    task.TASKER = tasker;
                    task.PRODUCT_TYPE = "1";
                    task.IS_MIX = item.IS_MIX;
                    task.SOURCE_BILLNO = billmast.SOURCE_BILLNO;
                    task.NEWCELL_CODE = item.NEWCELL_CODE;//新货位
                    WcsTaskRepository.Add(task);
                    serial++;
                }
                billmast.TASK_DATE = DateTime.Now;
                billmast.TASKER = tasker;
                billmast.STATE = "3";
                int result = BillMasterRepository.SaveChanges();
                if (result == -1) return false;
                else return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //移库单删除
        public bool MoveStockDelete(string BillNo)
        {
            var deletbillno = BillMasterRepository.GetQueryable().Where(i => i.BILL_NO == BillNo).FirstOrDefault();
            var details = ProductStateRepository.GetQueryable().Where(i => i.BILL_NO == BillNo);
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_PRODUCT_STATE sub in tmp)
            {
                //移库后的货位进行解锁
                var newcell = cellRepository.GetQueryable().FirstOrDefault(i => i.CELL_CODE == sub.NEWCELL_CODE);
                newcell.IS_LOCK = "0";
                //要移库的货位进行解锁
                var cell = cellRepository.GetQueryable().FirstOrDefault(i => i.CELL_CODE == sub.CELL_CODE);
                cell.IS_LOCK = "0";

                ProductStateRepository.Delete(sub);
            }

            var billdetails = BillDetailRepository.GetQueryable().Where(i => i.BILL_NO == BillNo);
            WMS_BILL_DETAIL[] billdetaillist = billdetails.ToArray();
            BillDetailRepository.Delete(billdetaillist);

            BillMasterRepository.Delete(deletbillno);
            int result = BillMasterRepository.SaveChanges();
            if (result == -1) return false;
            return true;
        }

        //获取出库批次,用于紧急补料单.
        public object Outstockbill(int page, int rows, string queryinfo)
        {
            //var billmaster=new object();
            IQueryable<WMS_BILL_MASTER> billquery = BillMasterRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            IQueryable<AUTH_USER> userquery = UserRepository.GetQueryable();
            IQueryable<CMD_CELL> cellquery = cellRepository.GetQueryable();
            var billmaster = (from a in billquery
                              //join h in cellquery on a.BILL_NO equals h.BILL_NO
                              join b in statequery on a.STATUS equals b.STATE
                              join c in statequery on a.STATE equals c.STATE
                              join d in statequery on a.BILL_METHOD equals d.STATE
                              join e in userquery on a.OPERATER equals e.USER_ID
                              join f in userquery on a.CHECKER equals f.USER_ID into fg
                              from f in fg.DefaultIfEmpty()
                              join g in userquery on a.TASKER equals g.USER_ID into hg
                              from g in hg.DefaultIfEmpty()
                              where b.TABLE_NAME == "WMS_BILL_MASTER" && b.FIELD_NAME == "STATUS" && c.TABLE_NAME == "WMS_BILL_MASTER" && c.FIELD_NAME == "STATE"
                              && d.TABLE_NAME == "WMS_BILL_MASTER" && d.FIELD_NAME == "BILL_METHOD" && a.CMD_BILL_TYPE.BILL_TYPE == "2"
                              select new
                              {
                                  a.BILL_NO,
                                  a.BILL_DATE,
                                  a.BTYPE_CODE, //单据类型代码
                                  a.CMD_BILL_TYPE.BTYPE_NAME,  //单据类型名称
                                  a.SCHEDULE_NO,
                                  //a.WAREHOUSE_CODE,
                                  //a.CMD_WAREHOUSE.WAREHOUSE_NAME,
                                  //a.SYS_BILL_TARGET.TARGET_NAME, //目标位置名
                                  //a.TARGET_CODE, //目标位置代码
                                  a.STATUS,//单据来源代号
                                  STATUSNAME = b.STATE_DESC,//单据来源描述,手动,系统输入
                                  a.STATE,//单据状态代号
                                  //STATENAME = c.STATE_DESC,//单据状态描述
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
                              }).Distinct();
            if (!string.IsNullOrEmpty(queryinfo))
            {
                string info = queryinfo.Split(':')[0];
                string val = queryinfo.Split(':')[1];
                if (!string.IsNullOrEmpty(val))
                {
                    if (info == "billno")
                        billmaster = billmaster.Where(i => i.BILL_NO == val);
                    else if (info == "cigarate")
                        billmaster = billmaster.Where(i => i.CIGARETTE_CODE == val);
                    else
                        billmaster = billmaster.Where(i => i.FORMULA_CODE == val);
                }
            }
            billmaster =billmaster .Where (i=>!("1,2".Contains (i.STATE ))&&i.BTYPE_CODE !="005");//状态为作业以上的.
            var temp = billmaster.ToArray().OrderByDescending(i => i.OPERATE_DATE).Select(i => new
            {
                i.BILL_NO,
                BILL_DATE = i.BILL_DATE.ToString("yyyy-MM-dd"),
                i.BTYPE_CODE,
                i.BTYPE_NAME,
                i.SCHEDULE_NO,
                //i.WAREHOUSE_CODE,
                //i.WAREHOUSE_NAME,
                //i.TARGET_CODE,
                //i.TARGET_NAME,
                i.STATUS,
                i.STATUSNAME,
                i.STATE,
                //i.STATENAME,
                //i.SOURCE_BILLNO,
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
                i.SOURCE_BILLNO,
                i.LINE_NAME
            });
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }

        //紧急补料单新增
        public bool FeedingAdd(WMS_BILL_MASTER mast, object detail, string prefix)
        {
            var targetcode = CmdBillTypeRepository.GetQueryable().FirstOrDefault(i => i.BTYPE_CODE == mast.BTYPE_CODE);
            bool rejust = false;
            int serial = 1;
            try
            {
                mast.BILL_NO = BillMasterRepository.GetNewID(prefix, mast.BILL_DATE, mast.BILL_NO);
                mast.OPERATE_DATE = DateTime.Now;
                //mast.BILL_DATE = DateTime.Now;
                mast.BILL_METHOD = "0";
                mast.STATE = "1"; //默认保存状态
                mast.STATUS = "0"; //默认手工输入
                mast.TARGET_CODE = targetcode.TARGET_CODE;
                BillMasterRepository.Add(mast);

                DataTable dt = THOK.Common.ConvertData.JsonToDataTable(((System.String[])detail)[0]);
                foreach (DataRow dr in dt.Rows)
                {

                    WMS_BILL_DETAIL subdetail = new WMS_BILL_DETAIL();
                    THOK.Common.ConvertData.DataBind(subdetail, dr);
                    subdetail.ITEM_NO = serial;
                    subdetail.BILL_NO = mast.BILL_NO;
                    BillDetailRepository.Add(subdetail);
                    serial++;
                }

                int brs = BillMasterRepository.SaveChanges();
                if (brs == -1) rejust = false;
                else
                    rejust = true;
            }
            catch (Exception ex)
            {

            }
            return rejust;
        }

        //紧急补料单修改
        public bool FeedingEdit(WMS_BILL_MASTER mast, object detail)
        {
            var billmast = BillMasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO == mast.BILL_NO);
            billmast.BILL_DATE = mast.BILL_DATE;
            billmast.SOURCE_BILLNO = mast.SOURCE_BILLNO;

            var details = BillDetailRepository.GetQueryable().Where(i => i.BILL_NO == mast.BILL_NO);
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_BILL_DETAIL  sub in tmp)
            {
                BillDetailRepository.Delete(sub);
            }

            DataTable dt = THOK.Common.ConvertData.JsonToDataTable(((System.String[])detail)[0]); //修改
            if (dt != null)
            {
                int serial = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    WMS_BILL_DETAIL subdetail = new WMS_BILL_DETAIL();
                    THOK.Common.ConvertData.DataBind(subdetail, dr);
                    subdetail.ITEM_NO = serial;
                    subdetail.BILL_NO = mast.BILL_NO;
                    if (subdetail.FPRODUCT_CODE == "null") subdetail.FPRODUCT_CODE = "";
                    BillDetailRepository.Add(subdetail);
                    serial++;
                }
            }
            int result = BillMasterRepository.SaveChanges();
            if (result == -1) return false;
            else
                return true;
        }

        //获取单据下的明细,根据产品代码,消除重复的.
        public object GetSubDetails(int page, int rows, string BillNo)
        {
            IQueryable<WMS_BILL_DETAIL> detailquery = BillDetailRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            IQueryable<CMD_PRODUCT> productquery = ProductRepository.GetQueryable();
            var billdetail =from a in detailquery
                             join b in statequery on a.IS_MIX equals b.STATE
                             join c in productquery on a.PRODUCT_CODE equals c.PRODUCT_CODE
                             where b.TABLE_NAME == "WMS_BILL_DETAIL" && b.FIELD_NAME == "IS_MIX"
                             select new
                             {
                                 a.BILL_NO,
                                 a.ITEM_NO,
                                 a.PRODUCT_CODE,
                                 c.PRODUCT_NAME,
                                 c.YEARS,
                                 c.CMD_PRODUCT_GRADE.GRADE_NAME,
                                 c.CMD_PRODUCT_STYLE.STYLE_NAME,
                                 c.CMD_PRODUCT_ORIGINAL.ORIGINAL_NAME,
                                 c.CMD_PRODUCT_CATEGORY.CATEGORY_NAME,
                                 a.WEIGHT,
                                 a.REAL_WEIGHT,
                                 a.IS_MIX,
                                 a.PACKAGE_COUNT
                             };
            var temp = billdetail.ToArray().Where(i => i.BILL_NO == BillNo).OrderBy(i => i.ITEM_NO).Select(i => new {
                i.BILL_NO ,
                i.PRODUCT_CODE ,
                i.PRODUCT_NAME ,
                i.WEIGHT,
                REAL_WEIGHT ="",
                PACKAGE_COUNT="",
                IS_MIX="0"
            }).Distinct ();
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }
    }
}
