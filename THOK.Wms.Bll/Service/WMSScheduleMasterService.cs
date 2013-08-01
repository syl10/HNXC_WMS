using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using  THOK.Wms.Dal.Interfaces;
using System.Data;

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

        public object GetDetails(int page, int rows, string SCHEDULE_NO, string SCHEDULE_DATE, string STATE, string OPERATER, string OPERATE_DATE, string CHECKER, string CHECK_DATE)
        {
            IQueryable<WMS_SCHEDULE_MASTER> ScheduleMaster = ScheduleMasterRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            var schedule = from a in ScheduleMaster
                           join b in statequery on a.STATUS equals b.STATE 
                           join c in statequery on a.STATE equals c.STATE
                           where b.TABLE_NAME == "WMS_SCHEDULE_MASTER" && b.FIELD_NAME == "STATUS" && c.TABLE_NAME == "WMS_SCHEDULE_MASTER" && c.FIELD_NAME == "STATE"
                           select new
                           {
                               a.SCHEDULE_NO,
                               a.SCHEDULE_DATE,
                               a.SOURCE_BILLNO ,
                               a.OPERATER,
                               a.OPERATE_DATE,
                               a.CHECK_DATE,
                               a.CHECKER,
                               STATE = c.STATE_DESC,
                               STATUS = b.STATE_DESC,
                               STATECODE = a.STATE,
                               STATUSCODE = a.STATUS
                           };
            schedule = schedule.OrderByDescending(i => i.SCHEDULE_NO).Select(i => i);
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
            var temp = schedule.ToArray().OrderBy(i=>i.SCHEDULE_DATE).Select(i => new
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
            int total = schedule.Count();
            schedule = schedule.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }


        public object GetSubDetails(int page, int rows, string SCHEDULE_NO)
        {
            IQueryable<WMS_SCHEDULE_DETAIL> ScheduleDetail = ScheduleDetailRepository.GetQueryable();
            var Subdetail = ScheduleDetail.OrderBy(i => i.ITEM_NO).Select(i => new { 
                i.SCHEDULE_NO ,
                i.ITEM_NO,
                i.FORMULA_CODE ,
                i.CIGARETTE_CODE,
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

                DataTable dt = THOK.Common.JsonData.JsonToDataTable(((System.String[])detail)[0]);
                foreach (DataRow dr in dt.Rows)
                {
                    WMS_SCHEDULE_DETAIL subdetail = new WMS_SCHEDULE_DETAIL();
                    THOK.Common.JsonData.DataBind(subdetail, dr);
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

            DataTable dt = THOK.Common.JsonData.JsonToDataTable(((System.String[])detail)[0]); //修改
            if (dt != null)
            {
                dt.Columns.Remove("LINE_NAME");
                foreach (DataRow dr in dt.Rows)
                {
                    WMS_SCHEDULE_DETAIL subdetail = new WMS_SCHEDULE_DETAIL();
                    THOK.Common.JsonData.DataBind(subdetail, dr);
                    subdetail.SCHEDULE_NO  = mast.SCHEDULE_NO;
                    subdetail.BILL_NO = "";
                    ScheduleDetailRepository.Add(subdetail);
                }
            }

            ScheduleMasterRepository.SaveChanges();


            return true;
        }


        public new bool Delete(string scheduleno)
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
    }
}
