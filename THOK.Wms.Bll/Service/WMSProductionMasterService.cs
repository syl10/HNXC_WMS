using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using System.Data;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;

namespace THOK.Wms.Bll.Service
{
    class WMSProductionMasterService : ServiceBase<WMS_PRODUCTION_MASTER>, IWMSProductionMasterService 
    {
        [Dependency]
        public IWMSProductionMasterRepository  ProductionmasterRepository { get; set; }
        [Dependency]
        public IWMSProductionDetailRepository ProeductiondetailRepository { get; set; }
        [Dependency]
        public ISysTableStateRepository SysTableStateRepository { get; set; }
        [Dependency]
        public ICMDProuductRepository ProductRepository { get; set; }
        [Dependency]
        public IUserRepository UserRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }



        public object GetDetails(int page, int rows, string BILL_NO, string BILL_DATE,  string WAREHOUSE_CODE,  string CIGARETTE_CODE, string FORMULA_CODE, string STATE, string OPERATER, string OPERATE_DATE, string CHECKER, string CHECK_DATE, string BILL_DATEStar, string BILL_DATEEND,string BILLNOFROM,string BILLNOTO)
        {
            IQueryable<WMS_PRODUCTION_MASTER> query = ProductionmasterRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            IQueryable<AUTH_USER> userquery = UserRepository.GetQueryable();
            var detail = from a in query
                       join b in statequery on a.STATE equals b.STATE into bf from b in bf.DefaultIfEmpty ()
                       join d in userquery on a.OPERATER equals d.USER_ID into df from d in df.DefaultIfEmpty ()
                       join c in userquery on a.CHECKER equals c.USER_ID into e from c in e.DefaultIfEmpty ()
                       where b.TABLE_NAME == "WMS_PRODUCTION_MASTER" && b.FIELD_NAME == "STATE"
                       select new { 
                           a.BILL_NO ,
                           a.BILL_DATE,
                           a.SCHEDULE_NO ,
                           a.WAREHOUSE_CODE ,
                           a.CMD_WAREHOUSE .WAREHOUSE_NAME ,
                           a.CIGARETTE_CODE ,
                           a.CMD_CIGARETTE .CIGARETTE_NAME ,
                           a.FORMULA_CODE ,
                           a.WMS_FORMULA_MASTER .FORMULA_NAME,
                           a.BATCH_WEIGHT ,
                           a.IN_BILLNO ,
                           a.OUT_BILLNO ,
                           a.STATE ,
                           a.LINE_NO,
                           a.CMD_PRODUCTION_LINE.LINE_NAME,
                           STATENAME=b.STATE_DESC ,
                           OPERATER =d.USER_NAME,
                           a.OPERATE_DATE ,
                           a.CHECK_DATE ,
                           CHECKER=c.USER_NAME  
                       };
            if (!string.IsNullOrEmpty(BILL_NO))
            {
                detail = detail.Where(i => i.BILL_NO == BILL_NO);
            }
            if (!string.IsNullOrEmpty(BILL_DATE))
            {
                DateTime billdt = DateTime.Parse(BILL_DATE);
                detail = detail.Where(i => i.BILL_DATE.CompareTo(billdt) == 0);
            }
            if (!string.IsNullOrEmpty(WAREHOUSE_CODE))
            {
                detail = detail.Where(i => i.WAREHOUSE_CODE == WAREHOUSE_CODE);
            }
            if (!string.IsNullOrEmpty(CIGARETTE_CODE))
            {
                detail = detail.Where(i => i.CIGARETTE_CODE == CIGARETTE_CODE);
            }
            if (!string.IsNullOrEmpty(FORMULA_CODE))
            {
                detail = detail.Where(i => i.FORMULA_CODE == FORMULA_CODE);
            }
            if (!string.IsNullOrEmpty(STATE))
            {
                detail = detail.Where(i => i.STATE == STATE);
            }
            if (!string.IsNullOrEmpty(OPERATER))
            {
                detail = detail.Where(i => i.OPERATER.Contains(OPERATER));
            }
            if (!string.IsNullOrEmpty(OPERATE_DATE))
            {
                DateTime operatedt = DateTime.Parse(OPERATE_DATE);
                DateTime operatedt2 = operatedt.AddDays(1);
                detail = detail.Where(i => i.OPERATE_DATE.Value.CompareTo(operatedt) >= 0);
                detail = detail.Where(i => i.OPERATE_DATE.Value.CompareTo(operatedt2) < 0);
            }
            if (!string.IsNullOrEmpty(CHECKER))
            {
                detail = detail.Where(i => i.CHECKER.Contains(CHECKER));
            }
            if (!string.IsNullOrEmpty(CHECK_DATE))
            {
                DateTime checkdt = DateTime.Parse(CHECK_DATE);
                DateTime checkdt2 = checkdt.AddDays(1);
                detail = detail.Where(i => i.CHECK_DATE.Value.CompareTo(checkdt) >= 0);
                detail = detail.Where(i => i.CHECK_DATE.Value.CompareTo(checkdt2) < 0);
            }
            if (!string.IsNullOrEmpty(BILL_DATEStar))
            {
                DateTime datestare = DateTime.Parse(BILL_DATEStar);
                detail = detail.Where(i => i.BILL_DATE.CompareTo(datestare) >= 0);
            }
            if (!string.IsNullOrEmpty(BILL_DATEEND))
            {
                DateTime dateend = DateTime.Parse(BILL_DATEEND);
                detail = detail.Where(i => i.BILL_DATE.CompareTo(dateend) <= 0);
            }
            if (!string.IsNullOrEmpty(BILLNOFROM)) {
                detail = detail.Where(i => i.BILL_NO.CompareTo(BILLNOFROM )>=0);
            }
            if (!string.IsNullOrEmpty(BILLNOTO)) {
                detail = detail.Where(i => i.BILL_NO.CompareTo(BILLNOTO) <= 0);
            }
            if (THOK.Common.PrintHandle.issearch)
            {//用于单据查询中的打印
                THOK.Common.PrintHandle.searchdt = THOK.Common.ConvertData.LinqQueryToDataTable(detail);
                //THOK.Common.PrintHandle.issearch = false;
            }
            detail = detail.OrderByDescending(i => i.OPERATE_DATE);
            int total = detail.Count();
            detail = detail.Skip((page - 1) * rows).Take(rows);
            //total = detail.Count();
            var temp = detail.ToArray().Select(i => new
             {
                 i.BILL_NO,
                 BILL_DATE = i.BILL_DATE.ToString("yyyy-MM-dd"),
                 i.SCHEDULE_NO,
                 i.WAREHOUSE_CODE,
                 i.WAREHOUSE_NAME,
                 i.CIGARETTE_CODE,
                 i.CIGARETTE_NAME,
                 i.FORMULA_CODE,
                 i.FORMULA_NAME,
                 i.BATCH_WEIGHT,
                 i.IN_BILLNO,
                 i.OUT_BILLNO,
                 i.STATE,
                 i.LINE_NO,
                 i.LINE_NAME,
                 STATENAME = i.STATENAME,
                 i.OPERATER,
                 OPERATE_DATE = i.OPERATE_DATE == null ? "" : ((DateTime)i.OPERATE_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                 CHECK_DATE = i.CHECK_DATE == null ? "" : ((DateTime)i.CHECK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                 i.CHECKER

             });
           //int total = temp.Count();
           //temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp };
              
        }

        //审核
        public bool Audit(string checker, string billno)
        {
            var temp = ProductionmasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO == billno);
            if (temp != null)
            {
                temp.CHECK_DATE = DateTime.Now;
                temp.CHECKER = checker;
                temp.STATE = "2";
               int result= ProductionmasterRepository.SaveChanges();
               if (result == -1) return false;
            }
            else
                return false;
            return true;
        }
        //反审
        public bool Antitrial(string billno)
        {
            var temp = ProductionmasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO == billno);
            if (temp != null)
            {
                temp.CHECK_DATE = DateTime.Now;
                temp.CHECKER = "";
                temp.STATE = "1";
               int result=  ProductionmasterRepository.SaveChanges();
               if (result == -1) return false;
            }
            else
                return false;
            return true;
        }

        //删除
        public bool Delete(string billno)
        {
            var editmaster = ProductionmasterRepository.GetQueryable().Where(i => i.BILL_NO  == billno ).FirstOrDefault();


            var details = ProeductiondetailRepository.GetQueryable().Where(i => i.BILL_NO  == billno );
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_PRODUCTION_DETAIL  sub in tmp)
            {
                ProeductiondetailRepository.Delete(sub);
            }
            ProductionmasterRepository.Delete(editmaster);
           int result=  ProductionmasterRepository.SaveChanges();
           if (result == -1) return false;
            return true;
        }

        //单据编号
        public object GetBillNo(string userName, DateTime dt, string BILL_NO)
        {
            var strCode = ProductionmasterRepository.GetNewID("DP", dt, BILL_NO);
            var BillnoInfo =
                new
                {
                    userName = userName,
                    BillNo = strCode
                };
            return BillnoInfo;
        }

        //新增
        public bool Add(WMS_PRODUCTION_MASTER mast, object detail)
        {
            bool rejust = false;
            int serial = 1;
            try
            {
                mast.BILL_NO = ProductionmasterRepository.GetNewID("DP", mast.BILL_DATE, mast.BILL_NO);
                mast.OPERATE_DATE = DateTime.Now;
                //mast.BILL_DATE = DateTime.Now;
                mast.STATE = "1"; //默认保存状态
                mast.IN_BILLNO = " ";
                mast.OUT_BILLNO = " ";
                ProductionmasterRepository.Add(mast);

                DataTable dt = THOK.Common.ConvertData.JsonToDataTable(((System.String[])detail)[0]);
                foreach (DataRow dr in dt.Rows)
                {

                    WMS_PRODUCTION_DETAIL subdetail = new WMS_PRODUCTION_DETAIL();
                    THOK.Common.ConvertData.DataBind(subdetail, dr);
                    subdetail.ITEM_NO = serial;
                    subdetail.BILL_NO = mast.BILL_NO;
                    //subdetail.IS_MIX = "0";
                    //subdetail.FPRODUCT_CODE = "";
                    ProeductiondetailRepository.Add(subdetail);
                    serial++;
                }

              int rsb=  ProductionmasterRepository.SaveChanges();
              if (rsb == -1) rejust = false;
              else
                  rejust = true;
            }
            catch (Exception ex)
            {
                rejust = false;
            }
            return rejust;
        }

        //获取某个单据下明细
        public object GetSubDetails(int page, int rows, string BillNo)
        {
            IQueryable<WMS_PRODUCTION_DETAIL> detailquery = ProeductiondetailRepository.GetQueryable();
            //IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            IQueryable<CMD_PRODUCT> productquery = ProductRepository.GetQueryable();
            var billdetail = from a in detailquery
                             join c in productquery on a.PRODUCT_CODE equals c.PRODUCT_CODE
                             select new
                             {
                                 a.ITEM_NO,
                                 a.BILL_NO,
                                 a.PRODUCT_CODE,
                                 c.PRODUCT_NAME,
                                 c.YEARS ,
                                 c.CMD_PRODUCT_GRADE .GRADE_NAME ,
                                 c.CMD_PRODUCT_STYLE .STYLE_NAME ,
                                 c.CMD_PRODUCT_ORIGINAL .ORIGINAL_NAME ,
                                 c.CMD_PRODUCT_CATEGORY .CATEGORY_NAME ,
                                 a.WEIGHT,
                                 a.REAL_WEIGHT,
                                 a.PACKAGE_COUNT,
                                 a.NC_COUNT,
                                 TOTAL_WEIGHT = a.PACKAGE_COUNT * a.REAL_WEIGHT
                             };
            billdetail = billdetail.Where(i => i.BILL_NO == BillNo).OrderBy(i => i.ITEM_NO).Select(i => i);
            int total = billdetail.Count();
            billdetail = billdetail.Skip((page - 1) * rows).Take(rows);
            var temp = billdetail.ToArray();
            //int total = temp.Count();
            //temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp };
        }

        //修改
        public bool Edit(WMS_PRODUCTION_MASTER mast, object detail)
        {
            var billmast = ProductionmasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO == mast.BILL_NO);
            billmast.BILL_DATE = mast.BILL_DATE;
            billmast.WAREHOUSE_CODE = mast.WAREHOUSE_CODE;
                billmast.CIGARETTE_CODE = mast.CIGARETTE_CODE;
                billmast.FORMULA_CODE = mast.FORMULA_CODE;
                billmast.BATCH_WEIGHT = mast.BATCH_WEIGHT;
                billmast.LINE_NO = mast.LINE_NO;
                var details = ProeductiondetailRepository.GetQueryable().Where(i => i.BILL_NO == mast.BILL_NO);
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_PRODUCTION_DETAIL  sub in tmp)
            {
                ProeductiondetailRepository.Delete(sub);
            }

            DataTable dt = THOK.Common.ConvertData.JsonToDataTable(((System.String[])detail)[0]); //修改
            if (dt != null)
            {
                int serial = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    WMS_PRODUCTION_DETAIL subdetail = new WMS_PRODUCTION_DETAIL();
                    THOK.Common.ConvertData.DataBind(subdetail, dr);
                    subdetail.ITEM_NO = serial;
                    subdetail.BILL_NO = mast.BILL_NO;
                    ProeductiondetailRepository.Add(subdetail);
                    serial++;
                }
            }
          int result=  ProductionmasterRepository.SaveChanges();
          if (result == -1) return false;
            return true;
        }

        //打印
        public bool Print(string BILL_NO, string WAREHOUSE_CODE, string CIGARETTE_CODE, string FORMULA_CODE, string STATE, string BILL_DATEStar, string BILL_DATEEND, string SCHEDULENO, string IN_BILLNO, string OUT_BILLNO)
        {
            IQueryable<WMS_PRODUCTION_MASTER> query = ProductionmasterRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            IQueryable<AUTH_USER> userquery = UserRepository.GetQueryable();
            IQueryable<WMS_PRODUCTION_DETAIL> detailquery = ProeductiondetailRepository.GetQueryable();
            IQueryable<CMD_PRODUCT> productquery = ProductRepository.GetQueryable();
            try
            {
                var directproduct = from a in query
                                    join b in detailquery on a.BILL_NO equals b.BILL_NO
                                    join c in statequery on a.STATE equals c.STATE into cf
                                    from c in cf.DefaultIfEmpty()
                                    join d in userquery on a.OPERATER equals d.USER_ID into df
                                    from d in df.DefaultIfEmpty()
                                    join e in userquery on a.CHECKER equals e.USER_ID into ef
                                    from e in ef.DefaultIfEmpty()
                                    join f in productquery on b.PRODUCT_CODE equals f.PRODUCT_CODE
                                    where c.TABLE_NAME == "WMS_PRODUCTION_MASTER" && c.FIELD_NAME == "STATE"
                                    select new
                                    {
                                        a.BILL_NO,
                                        a.SCHEDULE_NO,
                                        a.WAREHOUSE_CODE,
                                        a.IN_BILLNO,
                                        a.OUT_BILLNO,
                                        a.CMD_WAREHOUSE.WAREHOUSE_NAME,
                                        a.BILL_DATE,
                                        a.CIGARETTE_CODE,
                                        a.CMD_CIGARETTE.CIGARETTE_NAME,
                                        a.FORMULA_CODE,
                                        a.WMS_FORMULA_MASTER.FORMULA_NAME,
                                        a.BATCH_WEIGHT,
                                        a.LINE_NO,
                                        a.CMD_PRODUCTION_LINE.LINE_NAME,
                                        a.STATE,
                                        c.STATE_DESC,
                                        OPERATER = d.USER_NAME,
                                        a.OPERATE_DATE,
                                        CHECKER = e.USER_NAME,
                                        a.CHECK_DATE,
                                        b.ITEM_NO,
                                        b.PRODUCT_CODE,
                                        f.PRODUCT_NAME,
                                        f.YEARS,
                                        f.CMD_PRODUCT_GRADE.GRADE_NAME,
                                        f.CMD_PRODUCT_CATEGORY.CATEGORY_NAME,
                                        f.CMD_PRODUCT_ORIGINAL.ORIGINAL_NAME,
                                        f.CMD_PRODUCT_STYLE.STYLE_NAME,
                                        b.WEIGHT,
                                        b.REAL_WEIGHT,
                                        b.PACKAGE_COUNT,
                                        b.NC_COUNT
                                    };
                if (!string.IsNullOrEmpty(BILL_NO))
                {
                    directproduct = directproduct.Where(i => i.BILL_NO == BILL_NO);
                }
                if (!string.IsNullOrEmpty(WAREHOUSE_CODE))
                {
                    directproduct = directproduct.Where(i => i.WAREHOUSE_CODE == WAREHOUSE_CODE);
                }
                if (!string.IsNullOrEmpty(CIGARETTE_CODE))
                {
                    directproduct = directproduct.Where(i => i.CIGARETTE_CODE == CIGARETTE_CODE);
                }
                if (!string.IsNullOrEmpty(FORMULA_CODE))
                {
                    directproduct = directproduct.Where(i => i.FORMULA_CODE == FORMULA_CODE);
                }
                if (!string.IsNullOrEmpty(STATE))
                {
                    directproduct = directproduct.Where(i => i.STATE == STATE);
                }
                if (!string.IsNullOrEmpty(SCHEDULENO))
                {
                    directproduct = directproduct.Where(i => i.SCHEDULE_NO == SCHEDULENO);
                }
                if (!string.IsNullOrEmpty(IN_BILLNO))
                {
                    directproduct = directproduct.Where(i => i.IN_BILLNO == IN_BILLNO);
                }
                if (!string.IsNullOrEmpty(OUT_BILLNO))
                {
                    directproduct = directproduct.Where(i => i.OUT_BILLNO == OUT_BILLNO);
                }
                if (!string.IsNullOrEmpty(BILL_DATEStar))
                {
                    DateTime datestare = DateTime.Parse(BILL_DATEStar);
                    directproduct = directproduct.Where(i => i.BILL_DATE.CompareTo(datestare) >= 0);
                }
                if (!string.IsNullOrEmpty(BILL_DATEEND))
                {
                    DateTime dateend = DateTime.Parse(BILL_DATEEND);
                    directproduct = directproduct.Where(i => i.BILL_DATE.CompareTo(dateend) <= 0);
                }
                //var temp = directproduct;
                DataTable dt;
                if (THOK.Common.PrintHandle.issearch)
                { //判断是否是综合查询里的打印
                    //var query1 = from a in dt.AsEnumerable() select a;
                    var query2 = from b in THOK.Common.PrintHandle.searchdt.AsEnumerable() select b.Field<string>("BILL_NO");
                    directproduct = directproduct.Where(i => query2.Contains(i.BILL_NO));
                    THOK.Common.PrintHandle.issearch = false;
                    //dt = THOK.Common.ConvertData.LinqQueryToDataTable(query1);
                }
                else {
                    //dt = THOK.Common.ConvertData.LinqQueryToDataTable(temp);
                }
                var temp = directproduct.ToArray().OrderBy(i => i.BILL_NO).Select(i => new
                {
                    i.BILL_NO,
                    i.WAREHOUSE_NAME,
                    BILL_DATE = i.BILL_DATE.ToString("yyyy-MM-dd"),
                    i.CIGARETTE_NAME,
                    i.FORMULA_NAME,
                    i.BATCH_WEIGHT,
                    i.LINE_NAME,
                    i.OPERATER,
                    i.STATE_DESC,
                    OPERATE_DATE = i.OPERATE_DATE == null ? "" : ((DateTime)i.OPERATE_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.CHECKER,
                    CHECK_DATE = i.CHECK_DATE == null ? "" : ((DateTime)i.CHECK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.ITEM_NO,
                    i.PRODUCT_CODE,
                    i.PRODUCT_NAME,
                    i.YEARS,
                    i.GRADE_NAME,
                    i.CATEGORY_NAME,
                    i.STYLE_NAME,
                    i.ORIGINAL_NAME,
                    i.WEIGHT,
                    i.REAL_WEIGHT,
                    i.PACKAGE_COUNT,
                    i.NC_COUNT
                });
                dt = THOK.Common.ConvertData.LinqQueryToDataTable(temp);
                THOK.Common.PrintHandle.dt = dt;
                return true;
            }
            catch (Exception ex) {
                THOK.Common.PrintHandle.dt = null;
                return false;
            }

        }
    }
}
