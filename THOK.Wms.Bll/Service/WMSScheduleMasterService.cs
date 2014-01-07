using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using  THOK.Wms.Dal.Interfaces;
using System.Data;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;
using System.Reflection;

namespace THOK.Wms.Bll.Service
{
    class WMSScheduleMasterService : ServiceBase<WMS_SCHEDULE_MASTER>, IWMSScheduleMasterService 
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public IWMSScheduleMasterRepository  ScheduleMasterRepository { get; set; }
        [Dependency]
        public IWMSScheduleDetailRepository ScheduleDetailRepository { get; set; }
        [Dependency]
        public ISysTableStateRepository SysTableStateRepository { get; set; }
        [Dependency]
        public ICMDProductionLineRepository ProductionLineRepository { get; set; }
        [Dependency]
        public IUserRepository UserRepository { get; set; }
        [Dependency]
        public IWMSBillMasterRepository BillMasterRepository { get; set; }
        [Dependency]
        public IWMSBillDetailRepository BillDetailRepository { get; set; }
        [Dependency]
        public IWMSBillMasterService BillMasterService { get; set; }

        public object GetDetails(int page, int rows, string SCHEDULE_NO, string SCHEDULE_DATE, string STATE, string OPERATER, string OPERATE_DATE, string CHECKER, string CHECK_DATE, string BILLNOFROM, string BILLNOTO)
        {
            IQueryable<WMS_SCHEDULE_MASTER> ScheduleMaster = ScheduleMasterRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            IQueryable<AUTH_USER> userquery = UserRepository.GetQueryable();
            var schedule = from a in ScheduleMaster
                           join b in statequery on a.STATUS equals b.STATE 
                           join c in statequery on a.STATE equals c.STATE
                           join d in userquery on a.OPERATER equals d.USER_ID into df from d in df.DefaultIfEmpty ()
                           join e in userquery on a.CHECKER equals e.USER_ID into g from e in g.DefaultIfEmpty ()
                           where b.TABLE_NAME == "WMS_SCHEDULE_MASTER" && b.FIELD_NAME == "STATUS" && c.TABLE_NAME == "WMS_SCHEDULE_MASTER" && c.FIELD_NAME == "STATE"
                           select new
                           {
                               a.SCHEDULE_NO,
                               a.SCHEDULE_DATE,
                               a.SOURCE_BILLNO ,
                               OPERATER=d.USER_NAME ,
                               a.OPERATE_DATE,
                               a.CHECK_DATE,
                               CHECKER=e.USER_NAME ,
                               STATE = c.STATE_DESC,
                               STATUS = b.STATE_DESC,
                               STATECODE = a.STATE,
                               STATUSCODE = a.STATUS
                           };
            //schedule = schedule.OrderByDescending(i => i.SCHEDULE_NO).Select(i => i);
            if (!string.IsNullOrEmpty(SCHEDULE_NO))
            {
                schedule = schedule.Where(i => i.SCHEDULE_NO == SCHEDULE_NO);
            }
            if (!string.IsNullOrEmpty(SCHEDULE_DATE))
            {
                DateTime scheduledt = DateTime.Parse(SCHEDULE_DATE);
                schedule = schedule.Where(i => i.SCHEDULE_DATE.CompareTo (scheduledt )==0);
            }
            if (!string.IsNullOrEmpty(STATE))
            {
                schedule = schedule.Where(i => i.STATECODE == STATE);
            }
            if (!string.IsNullOrEmpty(OPERATER))
            {
                schedule = schedule.Where(i => i.OPERATER.Contains(OPERATER));
            }
            if (!string.IsNullOrEmpty(OPERATE_DATE))
            {
                DateTime operatedt = DateTime.Parse(OPERATE_DATE);
                DateTime operatedt2 = operatedt.AddDays(1);
                schedule = schedule.Where(i => i.OPERATE_DATE.Value .CompareTo (operatedt )>=0);
                schedule = schedule.Where(i => i.OPERATE_DATE.Value.CompareTo(operatedt2 ) <0);
            }
            if (!string.IsNullOrEmpty(CHECKER))
            {
                schedule = schedule.Where(i => i.CHECKER.Contains(CHECKER));
            }
            if (!string.IsNullOrEmpty(CHECK_DATE))
            {
                DateTime checkdt = DateTime.Parse(CHECK_DATE);
                DateTime checkdt2 = checkdt.AddDays(1);
                schedule = schedule.Where(i => i.CHECK_DATE.Value .CompareTo (checkdt )>=0);
                schedule = schedule.Where(i => i.CHECK_DATE.Value.CompareTo(checkdt2) < 0);
            }
            if (!string.IsNullOrEmpty(BILLNOFROM))
            {
                schedule = schedule.Where(i => i.SCHEDULE_NO.CompareTo(BILLNOFROM) >= 0);
            }
            if (!string.IsNullOrEmpty(BILLNOTO))
            {
                schedule = schedule.Where(i => i.SCHEDULE_NO.CompareTo(BILLNOTO) <= 0);
            }
            if (THOK.Common.PrintHandle.issearch)
            {//用于单据查询中的打印
                THOK.Common.PrintHandle.searchdt = THOK.Common.ConvertData.LinqQueryToDataTable(schedule);
                //THOK.Common.PrintHandle.issearch = false;
            }
            schedule = schedule.OrderByDescending(i => i.OPERATE_DATE);
            int total = schedule.Count();
            schedule = schedule.Skip((page - 1) * rows).Take(rows);
            var temp = schedule.ToArray().Select(i => new
            {
                i.SCHEDULE_NO,
                SCHEDULE_DATE = i.SCHEDULE_DATE.ToString("yyyy-MM-dd"),
                i.SOURCE_BILLNO ,
                i.OPERATER,
                OPERATE_DATE = i.OPERATE_DATE == null ? "" : ((DateTime)i.OPERATE_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                CHECK_DATE = i.CHECK_DATE == null ? "" : ((DateTime)i.CHECK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                i.CHECKER,
                STATE = i.STATE,
                STATUS = i.STATUS,
                STATECODE=i.STATECODE,
                STATUSCODE=i.STATUSCODE
            });
            return new { total, rows = temp};
        }


        public object GetSubDetails(int page, int rows, string SCHEDULE_NO)
        {
            IQueryable<WMS_SCHEDULE_DETAIL> ScheduleDetail = ScheduleDetailRepository.GetQueryable();
            var Subdetail = ScheduleDetail.OrderBy(i => i.ITEM_NO).Select(i => new { 
                i.SCHEDULE_NO ,
                i.ITEM_NO,
                i.FORMULA_CODE ,
                i.WMS_FORMULA_MASTER .FORMULA_NAME ,
                i.CIGARETTE_CODE,
                i.CMD_CIGARETTE .CIGARETTE_NAME ,
                i.BILL_NO,
                i.QUANTITY,
               LINE_NAME= i.CMD_PRODUCTION_LINE .LINE_NAME,
               i.LINE_NO
            });
            Subdetail = Subdetail.Where(i => i.SCHEDULE_NO == SCHEDULE_NO);
            int total = Subdetail.Count();
            Subdetail = Subdetail.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = Subdetail.ToArray() };
        }


        public object GetSchedulno(string userName, DateTime dt, string SCHEDULE_NO)
        {
            var strCode = ScheduleMasterRepository.GetNewID("SD", dt, SCHEDULE_NO);
            var ScheduleInfo =
                new
                {
                    userName = userName,
                    ScheduleNo = strCode
                };
            return ScheduleInfo;
        }


        public bool Add(WMS_SCHEDULE_MASTER mast, object detail)
        {
            bool rejust=false;
            try
            {
                mast.SCHEDULE_NO = ScheduleMasterRepository.GetNewID("SD", mast.SCHEDULE_DATE, mast.SCHEDULE_NO);
                mast.OPERATE_DATE = DateTime.Now;
                ScheduleMasterRepository.Add(mast);

                DataTable dt = THOK.Common.ConvertData.JsonToDataTable(((System.String[])detail)[0]);
                foreach (DataRow dr in dt.Rows)
                {
                    WMS_SCHEDULE_DETAIL subdetail = new WMS_SCHEDULE_DETAIL();
                    THOK.Common.ConvertData.DataBind(subdetail, dr);
                    subdetail.SCHEDULE_NO = mast.SCHEDULE_NO;
                    ScheduleDetailRepository.Add(subdetail);
                }
                ScheduleMasterRepository.SaveChanges();
                rejust = true;
            }
            catch (Exception ex) {
            }
            return rejust;
        }

        //审核
        public bool Audit(string checker, string scheduleno)
        {
            var schedulequery = ScheduleMasterRepository.GetQueryable().FirstOrDefault(i => i.SCHEDULE_NO == scheduleno);
            if (schedulequery != null)
            {
                schedulequery.CHECK_DATE = DateTime.Now;
                schedulequery.CHECKER = checker;
                schedulequery.STATE = "2";
                ScheduleMasterRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        //反审核
        public bool Antitrial(string scheduleno)
        {
            var schedulequery = ScheduleMasterRepository.GetQueryable().FirstOrDefault(i => i.SCHEDULE_NO == scheduleno);
            if (schedulequery != null)
            {
                schedulequery.CHECK_DATE = null;
                schedulequery.CHECKER = "";
                schedulequery.STATE = "1";
                ScheduleMasterRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        //制丝线
        public object GetProductLine(int page,int rows)
        {
            IQueryable<CMD_PRODUCTION_LINE> query = ProductionLineRepository.GetQueryable();
            var linequery = query.OrderBy(i => i.LINE_NO).Select(i => new { 
                i.LINE_NO ,
                i.LINE_NAME ,
                i.MEMO 
            });
            int total = linequery.Count();
            linequery = linequery.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = linequery.ToArray() };
        }


        public bool Edit(WMS_SCHEDULE_MASTER mast, object detail)
        {
            var schedulemast = ScheduleMasterRepository.GetQueryable().FirstOrDefault (i => i.SCHEDULE_NO == mast.SCHEDULE_NO);
            schedulemast.SCHEDULE_DATE = mast.SCHEDULE_DATE;

            var details = ScheduleDetailRepository.GetQueryable().Where(i => i.SCHEDULE_NO  == mast.SCHEDULE_NO );
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_SCHEDULE_DETAIL  sub in tmp)
            {
                ScheduleDetailRepository.Delete(sub);
            }

            DataTable dt = THOK.Common.ConvertData.JsonToDataTable(((System.String[])detail)[0]); //修改
            if (dt != null)
            {
                dt.Columns.Remove("LINE_NAME");
                foreach (DataRow dr in dt.Rows)
                {
                    WMS_SCHEDULE_DETAIL subdetail = new WMS_SCHEDULE_DETAIL();
                    THOK.Common.ConvertData.DataBind(subdetail, dr);
                    subdetail.SCHEDULE_NO  = mast.SCHEDULE_NO;
                    subdetail.BILL_NO = "";
                    ScheduleDetailRepository.Add(subdetail);
                }
            }

            ScheduleMasterRepository.SaveChanges();


            return true;
        }


        public  bool Delete(string scheduleno)
        {
            var editmaster = ScheduleMasterRepository.GetQueryable().Where(i => i.SCHEDULE_NO == scheduleno ).FirstOrDefault();


            var details = ScheduleDetailRepository.GetQueryable().Where(i => i.SCHEDULE_NO  == scheduleno );
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_SCHEDULE_DETAIL sub in tmp)
            {
                ScheduleDetailRepository.Delete(sub);
            }
            ScheduleMasterRepository.Delete(editmaster);
            ScheduleMasterRepository.SaveChanges();
            return true;
        }

        //计划单打印
        public bool SchedulePrint(string SCHEDULENO, string BILLDATEFROM, string BILLDATETO, string STATE)
        {
            IQueryable<WMS_SCHEDULE_MASTER> ScheduleMaster = ScheduleMasterRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            IQueryable<AUTH_USER> userquery = UserRepository.GetQueryable();
            IQueryable<WMS_SCHEDULE_DETAIL> ScheduleDetail = ScheduleDetailRepository.GetQueryable();
            try
            {
                var schedule = from a in ScheduleMaster
                               join b in ScheduleDetail on a.SCHEDULE_NO equals b.SCHEDULE_NO
                               join c in userquery on a.OPERATER equals c.USER_ID into cf
                               from c in cf.DefaultIfEmpty ()
                               join d in userquery on a.CHECKER equals d.USER_ID into df
                               from d in df.DefaultIfEmpty ()
                               join e in statequery on a.STATUS equals e.STATE
                               join f in statequery on a.STATE equals f.STATE
                               where e.TABLE_NAME == "WMS_SCHEDULE_MASTER" && e.FIELD_NAME == "STATUS" && f.TABLE_NAME == "WMS_SCHEDULE_MASTER" && f.FIELD_NAME == "STATE"
                               select new
                               {
                                   a.SCHEDULE_NO,
                                   a.SCHEDULE_DATE,
                                   STATUNAME = e.STATE_DESC,
                                   STATU= a.STATUS,
                                   STATENAME = f.STATE_DESC,
                                   STATE = a.STATE,
                                   OPERATER = c.USER_NAME,
                                   a.OPERATE_DATE,
                                   CHECKER = d.USER_NAME,
                                   a.CHECK_DATE,
                                   b.ITEM_NO,
                                   b.CIGARETTE_CODE,
                                   b.CMD_CIGARETTE.CIGARETTE_NAME,
                                   b.FORMULA_CODE,
                                   b.WMS_FORMULA_MASTER.FORMULA_NAME,
                                   b.BILL_NO,
                                   b.QUANTITY,
                                   b.CMD_PRODUCTION_LINE.LINE_NAME
                               };
                if (!string.IsNullOrEmpty(SCHEDULENO))
                {
                    schedule = schedule.Where(i => i.SCHEDULE_NO == SCHEDULENO);
                }
                if (!string.IsNullOrEmpty(BILLDATEFROM))
                {
                    DateTime datestare = DateTime.Parse(BILLDATEFROM);
                    schedule = schedule.Where(i => i.SCHEDULE_DATE.CompareTo(datestare) >= 0);
                }
                if (!string.IsNullOrEmpty(BILLDATETO))
                {
                    DateTime dateend = DateTime.Parse(BILLDATETO);
                    schedule = schedule.Where(i => i.SCHEDULE_DATE.CompareTo(dateend) <= 0);
                }
                if (!string.IsNullOrEmpty(STATE))
                {
                    schedule = schedule.Where(i => i.STATE== STATE);
                }
                if (THOK.Common.PrintHandle.issearch)
                { //判断是否是综合查询里的打印
                    //var query1 = from a in dt.AsEnumerable() select a;
                    var query2 = from b in THOK.Common.PrintHandle.searchdt.AsEnumerable() select b.Field<string>("SCHEDULE_NO");
                    schedule = schedule.Where(i => query2.Contains(i.SCHEDULE_NO));
                    THOK.Common.PrintHandle.issearch = false;
                    //dt = query1.CopyToDataTable();
                }
                var temp = schedule.ToArray().OrderBy(i => i.SCHEDULE_NO).Select(i => new
                {
                    i.SCHEDULE_NO,
                    SCHEDULE_DATE = i.SCHEDULE_DATE.ToString("yyyy-MM-dd"),
                    i.STATUNAME,
                    i.STATENAME,
                    i.OPERATER,
                    OPERATE_DATE = i.OPERATE_DATE == null ? "" : ((DateTime)i.OPERATE_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.CHECKER,
                    CHECK_DATE = i.CHECK_DATE == null ? "" : ((DateTime)i.CHECK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                    i.ITEM_NO,
                    i.CIGARETTE_CODE,
                    i.CIGARETTE_NAME,
                    i.FORMULA_CODE,
                    i.FORMULA_NAME,
                    i.BILL_NO,
                    i.QUANTITY,
                    i.LINE_NAME
                });
                DataTable dt = THOK.Common.ConvertData.LinqQueryToDataTable(temp);
                THOK.Common.PrintHandle.dt = dt;
                return true;
            }
            catch (Exception ex) {
                THOK.Common.PrintHandle.dt = null;
                return false;
            }
        }

        //生成出库单
        public bool CreateOutBill(string Scheduleno,string userid)
        {
            bool rejust = false;
            string billno = "";
            IQueryable<WMS_SCHEDULE_DETAIL> ScheduleDetail = ScheduleDetailRepository.GetQueryable();
            var Schedulemast = ScheduleMasterRepository.GetQueryable().FirstOrDefault(i => i.SCHEDULE_NO == Scheduleno);
            Schedulemast.STATE = "3";
            var formules = ScheduleDetail.Where(i => i.SCHEDULE_NO == Scheduleno).OrderBy (i=>i.ITEM_NO );
            foreach (WMS_SCHEDULE_DETAIL item in formules) {
                WMS_BILL_MASTER mast = new WMS_BILL_MASTER();
                mast.BILL_NO = BillMasterRepository.GetNewID("OS", DateTime .Now.Date ,billno);
                mast.BILL_DATE = DateTime.Now;
                mast.BTYPE_CODE = "002";
                mast.SCHEDULE_NO = Scheduleno;
                mast.SCHEDULE_ITEMNO = item.ITEM_NO;
                mast.TARGET_CODE = "001";
                mast.WAREHOUSE_CODE = "001";
                mast.STATUS = "0";
                mast.STATE = "2";
                mast.OPERATER = userid;
                mast.OPERATE_DATE = DateTime.Now;
                mast.CHECK_DATE = DateTime.Now;
                mast.CHECKER = userid;
                mast.BILL_METHOD = "0";
                mast.LINE_NO = item.LINE_NO;
                mast.CIGARETTE_CODE = item.CIGARETTE_CODE;
                mast.FORMULA_CODE = item.FORMULA_CODE;
                mast.BATCH_WEIGHT = item.QUANTITY;
                BillMasterRepository.Add(mast);
                billno ="OS"+ (double .Parse ( mast.BILL_NO.Substring(2))+1).ToString ();
                item.BILL_NO = mast.BILL_NO;

                var  formulobj = BillMasterService.LoadFormulaDetail(1, 1000, item.FORMULA_CODE, item.QUANTITY);
                THOK.Wms.Bll.Models.FormulaDetail[] items;
                Type detailtype = formulobj.GetType();
                try
                {
                    PropertyInfo[] aa = detailtype.GetProperties();
                    items = (THOK.Wms.Bll.Models.FormulaDetail[])aa[1].GetValue(formulobj, null);
                    foreach (THOK.Wms.Bll.Models.FormulaDetail  obj in items) {
                        WMS_BILL_DETAIL detail = new WMS_BILL_DETAIL();
                        detail.BILL_NO = mast.BILL_NO;
                        detail.ITEM_NO = obj.ITEM_NO;
                        detail.PRODUCT_CODE = obj.PRODUCT_CODE;
                        detail.WEIGHT = obj.WEIGHT;
                        detail.REAL_WEIGHT = obj.REAL_WEIGHT;
                        detail.PACKAGE_COUNT = obj.PACKAGE_COUNT;
                        detail.IS_MIX = obj.IS_MIX;
                        detail.FPRODUCT_CODE = obj.FPRODUCT_CODE;
                        detail.FORDER = obj.FORDER;
                        BillDetailRepository.Add(detail);
                    }
                }
                catch (Exception ex) { }
            }
            int brs = BillMasterRepository.SaveChanges();
            if (brs == -1) rejust = false;
            else
            {
               ScheduleDetailRepository.SaveChanges();
               ScheduleMasterRepository.SaveChanges();
                rejust = true;
            }
            return rejust;
        }
    }
}
