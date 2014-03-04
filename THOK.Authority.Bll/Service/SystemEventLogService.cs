using System;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Authority.Bll.Interfaces;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;

namespace THOK.Authority.Bll.Service
{
    public class SystemEventLogService : ServiceBase<AUTH_SYSTEM_EVENT_LOG>, ISystemEventLogService
    {
        [Dependency]
        public ISystemEventLogRepository SystemEventLogRepository { get; set; }
        [Dependency]
        public ISystemRepository SystemRepository { get; set; }
        public object GetDetails(int page, int rows, string eventlogtime, string eventtype, string eventname, string frompc, string operateuser, string targetsystem)
        {
            IQueryable<AUTH_SYSTEM_EVENT_LOG> query = SystemEventLogRepository.GetQueryable();
            IQueryable <AUTH_SYSTEM > systemquery=SystemRepository .GetQueryable ();
            var query2 = from a in query
                         join b in systemquery on a.TARGET_SYSTEM equals b.SYSTEM_ID into bf
                         from b in bf.DefaultIfEmpty()
                         select new { 
                             a.EVENT_LOG_ID ,
                             a.EVENT_LOG_TIME,
                             a.EVENT_TYPE ,
                             a.EVENT_NAME ,
                             a.EVENT_DESCRIPTION ,
                             a.FROM_PC,
                             a.OPERATE_USER ,
                             a.TARGET_SYSTEM ,
                             b.SYSTEM_NAME 
                         };

            var systemeventlogs = query2.Where(i => i.EVENT_LOG_TIME.Contains(eventlogtime)
                    && i.EVENT_TYPE.Contains(eventtype) && i.EVENT_NAME.Contains(eventname) && i.FROM_PC.Contains(frompc) && i.OPERATE_USER.Contains(operateuser) && i.TARGET_SYSTEM.Contains(targetsystem))
                    .OrderBy(i => i.EVENT_LOG_ID)
                    .Select(i => new { i.EVENT_LOG_ID, i.EVENT_NAME, i.EVENT_TYPE, i.FROM_PC, i.EVENT_LOG_TIME, i.OPERATE_USER, i.EVENT_DESCRIPTION, i.TARGET_SYSTEM,i.SYSTEM_NAME });

            if (THOK.Common.PrintHandle.isbase)
            {
                THOK.Common.PrintHandle.baseinfoprint = THOK.Common.ConvertData.LinqQueryToDataTable(systemeventlogs);
            }
            int total = systemeventlogs.Count();
            systemeventlogs = systemeventlogs.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = systemeventlogs.ToArray() };
        }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        public void CreateEventLog(string eventName, string eventDescription, string operateUser, string targetSystem, string idAdress)
        {

            var SYSTEM_EVENT = new AUTH_SYSTEM_EVENT_LOG()
            {
                //  USER_ID = Guid.NewGuid().ToString(),
                EVENT_LOG_ID = SystemEventLogRepository.GetNewID("AUTH_SYSTEM_EVENT_LOG", "EVENT_LOG_ID"),
                EVENT_NAME = eventName,
                EVENT_DESCRIPTION = eventDescription,
                OPERATE_USER = operateUser,
                TARGET_SYSTEM = targetSystem,
                FROM_PC = idAdress,
                EVENT_LOG_TIME = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                EVENT_TYPE = eventName
            };
            SystemEventLogRepository.Add(SYSTEM_EVENT);
            SystemEventLogRepository.SaveChanges();

        }
    }
}
