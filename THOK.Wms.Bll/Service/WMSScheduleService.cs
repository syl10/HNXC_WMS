using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using  THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    class WMSScheduleService : ServiceBase<WMS_SCHEDULE>, IWMSScheduleService 
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public   IWMSScheduleRepository Schedulerepository { set; get; }

        [Dependency]
        public IWMSFormulaMasterRepository MasterRepository { get; set; }
        [Dependency]
        public ISysTableStateRepository SysTableStateRepository { get; set; }



        public object GetDetails(int page, int rows, string SCHEDULE_NO, string SCHEDULE_DATE, string CIGARETTE, string FORMULA_CODE, string QUANTITY, string STATE, string OPERATER, string OPERATE_DATE, string CHECKER, string CHECK_DATE)
        {
            IQueryable<WMS_SCHEDULE>  schedulequery = Schedulerepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            var schedule = from a in schedulequery
                           join b in statequery on a.STATUS equals b.STATE
                           join c in statequery on a.STATE equals c.STATE
                           where b.TABLE_NAME == "WMS_SCHEDULE_MASTER" && b.FIELD_NAME == "STATUS" && c.TABLE_NAME == "WMS_SCHEDULE_MASTER" && c.FIELD_NAME == "STATE"
                           select new {
                               a.SCHEDULE_NO,
                               a.SCHEDULE_DATE,
                               a.QUANTITY,
                               a.OPERATER,
                               a.OPERATE_DATE,
                               a.FORMULA_CODE,
                               a.CHECK_DATE,
                               a.CHECKER,
                               a.CIGARETTE_CODE,
                               a.CIGARETTE_NAME,
                               STATE = c.STATE_DESC,
                               STATUS = b.STATE_DESC,
                               STATECODE=a .STATE ,
                               STATUSCODE=a.STATUS 
                           };
            schedule = schedule.OrderByDescending(i => i.SCHEDULE_NO).Select(i => i) ;
            if(!string .IsNullOrEmpty (SCHEDULE_NO ))
            {
                schedule = schedule.Where(i => i.SCHEDULE_NO == SCHEDULE_NO);
            }
            if (!string.IsNullOrEmpty(SCHEDULE_DATE))
            {
                schedule = schedule.Where(i => i.SCHEDULE_DATE.ToString().Contains(SCHEDULE_DATE));
            }
            if (!string.IsNullOrEmpty(CIGARETTE))
            {
                schedule = schedule.Where(i => i.CIGARETTE_CODE == CIGARETTE);
            }
            if (!string.IsNullOrEmpty(FORMULA_CODE))
            {
                schedule = schedule.Where(i => i.FORMULA_CODE == FORMULA_CODE);
            }
            if (!string.IsNullOrEmpty(QUANTITY))
            {
                schedule = schedule.Where(i => i.QUANTITY.ToString ()== QUANTITY);
            }
            if (!string.IsNullOrEmpty(STATE ))
            {
                schedule = schedule.Where(i => i.STATECODE  == STATE);
            }
            if (!string.IsNullOrEmpty(OPERATER ))
            {
                schedule = schedule.Where(i => i.OPERATER.Contains ( OPERATER));
            }
            if (!string.IsNullOrEmpty(OPERATE_DATE))
            {
                schedule = schedule.Where(i => i.OPERATE_DATE.Equals(OPERATE_DATE));
            }
            if (!string.IsNullOrEmpty(CHECKER))
            {
                schedule = schedule.Where(i => i.CHECKER.Contains(CHECKER));
            }
            if (!string.IsNullOrEmpty(CHECK_DATE))
            {
                schedule = schedule.Where(i => i.CHECK_DATE.Equals(CHECK_DATE));
            }
            int total = schedule.Count();
            schedule = schedule.Skip((page - 1) * rows).Take(rows);
            var temp = schedule.ToArray().AsEnumerable().Select(i => new
            {
                i.SCHEDULE_NO,
                SCHEDULE_DATE = i.SCHEDULE_DATE.ToString("yyyy-MM-dd"),
                i.QUANTITY,
                i.OPERATER,
                OPERATE_DATE =i.OPERATE_DATE ==null ?"": ((DateTime)i.OPERATE_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                i.FORMULA_CODE,
                CHECK_DATE =i.CHECK_DATE ==null ?"": ((DateTime)i.CHECK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                i.CHECKER,
                i.CIGARETTE_CODE,
                i.CIGARETTE_NAME,
                STATE = i.STATE ,
                STATUS = i.STATUS ,
            });
            return new { total, rows = temp.ToArray() };
        }


        public object GetSchedulno(string userName, DateTime dt, string SCHEDULE_NO)
        {
            var strCode = MasterRepository.GetNewID("SD", dt, SCHEDULE_NO);
            var ScheduleInfo =
                new
                {
                    userName = userName,
                    ScheduleNo = strCode
                };
            return ScheduleInfo;
        }


        public new bool Add(WMS_SCHEDULE schedule)
        {
            schedule.STATUS = "0";
            schedule.STATE = "1";
            schedule.OPERATE_DATE = DateTime.Now;
            Schedulerepository.Add(schedule);
            Schedulerepository.SaveChanges();
            return true;

        }


        public bool Save(WMS_SCHEDULE schedule)
        {
            var schedulequery = Schedulerepository.GetQueryable().FirstOrDefault(i => i.SCHEDULE_NO == schedule.SCHEDULE_NO);
            schedulequery.QUANTITY = schedule.QUANTITY;
            schedulequery.OPERATE_DATE = schedule.OPERATE_DATE;
            schedulequery.OPERATER = schedule.OPERATER;
            schedulequery.CIGARETTE_CODE = schedule.CIGARETTE_CODE;
            schedulequery.CIGARETTE_NAME = schedule.CIGARETTE_NAME;
            schedulequery.SCHEDULE_DATE = schedule.SCHEDULE_DATE;
            schedulequery.SOURCE_BILLNO = schedule.SOURCE_BILLNO;
            //schedulequery.STATE = schedule.STATE;
            //schedulequery.STATUS = schedule.STATUS;
            schedulequery.FORMULA_CODE = schedule.FORMULA_CODE;
            schedulequery.CHECK_DATE = schedule.CHECK_DATE;
            schedulequery.CHECKER = schedule.CHECKER;
            Schedulerepository.SaveChanges();
            return true;

        }


        public  bool Delete(string scheduleno)
        {
            var schedulequery = Schedulerepository.GetQueryable().FirstOrDefault(i => i.SCHEDULE_NO == scheduleno);
            if (schedulequery != null)
            {
                Schedulerepository.Delete(schedulequery);
                Schedulerepository.SaveChanges();
            }
            else
                return false;
            return true;
        }


        public bool Audit(string checker, string scheduleno)
        {
            var schedulequery = Schedulerepository.GetQueryable().FirstOrDefault(i => i.SCHEDULE_NO == scheduleno);
            if (schedulequery != null)
            {
                schedulequery.CHECK_DATE = DateTime.Now;
                schedulequery.CHECKER = checker;
                schedulequery.STATE = "2";
                Schedulerepository.SaveChanges();
            }
            else
                return false;
            return true;
                   
        }


        public bool Antitrial(string scheduleno)
        {
            var schedulequery = Schedulerepository.GetQueryable().FirstOrDefault(i => i.SCHEDULE_NO == scheduleno);
            if (schedulequery != null)
            {
                schedulequery.CHECK_DATE = null;
                schedulequery.CHECKER = "";
                schedulequery.STATE = "1";
                Schedulerepository.SaveChanges();
            }
            else
                return false;
            return true;
        }
    }
}
