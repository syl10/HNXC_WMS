using System;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Authority.Bll.Interfaces;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;
namespace THOK.Authority.Bll.Service
{
    public class LoginLogService : ServiceBase<AUTH_LOGIN_LOG>,ILoginLogService
    {
        [Dependency]
        public ILoginLogRepository LoginLogRepository { get; set; }
        [Dependency]
        public IUserRepository UserRepository { get; set; }
        [Dependency]
        public ISystemRepository SystemRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        public bool Add(string UserName, string SystemID)
        {
            AUTH_LOGIN_LOG LoginLog = new AUTH_LOGIN_LOG();
            LoginLog.LOG_ID = LoginLogRepository.GetNewID("AUTH_LOGIN_LOG", "LOG_ID");
            LoginLog.LOGIN_PC = System.Net.Dns.Resolve(System.Net.Dns.GetHostName()).AddressList[0].ToString();
            LoginLog.LOGIN_TIME = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            LoginLog.SYSTEM_SYSTEM_ID = SystemID;
            LoginLog.USER_USER_ID = UserRepository.GetSingle(i => i.USER_NAME.ToLower() == UserName.ToLower()).USER_ID;

            LoginLogRepository.Add(LoginLog);
            LoginLogRepository.SaveChanges();
            return true;
        }

        public object GetDetails(int page, int rows, string SystemID, string UserID, string LoginPC, string LoginTime)
        {
            IQueryable<AUTH_LOGIN_LOG> LOGINLOGQuery = LoginLogRepository.GetQueryable();
            var HelpContent = LOGINLOGQuery.Where(c => c.SYSTEM_SYSTEM_ID.Contains(SystemID) &&
                                                          c.USER_USER_ID.Contains(UserID) &&
                                                          c.LOGIN_PC.Contains(LoginPC) &&
                                                          c.LOGIN_TIME.Contains(LoginTime));

            HelpContent = HelpContent.OrderByDescending(h => h.LOG_ID);
            int total = HelpContent.Count();
            HelpContent = HelpContent.Skip((page - 1) * rows).Take(rows);

            var temp = HelpContent.ToArray().Select(c => new
            {
                ID = c.LOG_ID,
                LOGIN_PC = c.LOGIN_PC,
                LOGIN_TIME = c.LOGIN_TIME,
                LOGOUT_TIME = c.LOGOUT_TIME,
                SYSTEM_SYSTEM_ID= c.SYSTEM_SYSTEM_ID,
                SYSTEM_NAME = c.AUTH_SYSTEM.SYSTEM_NAME,
               USER_USER_ID= c.USER_USER_ID,
               CHINESE_NAME= c.AUTH_USER.CHINESE_NAME,
               USER_NAME= c.AUTH_USER.USER_NAME
            });
            if (THOK.Common.PrintHandle.isbase)
            {
                THOK.Common.PrintHandle.baseinfoprint = THOK.Common.ConvertData.LinqQueryToDataTable(HelpContent);
            }
            return new { total, rows = temp.ToArray() };
        }
        public void UpdateValiateTime(string UserName)
        {
            var LogID = LoginLogRepository.GetQueryable().Where(i => i.AUTH_USER.USER_NAME.ToLower() == UserName.ToLower()).Select(i => i.LOG_ID).Max();
            var LoginLog = LoginLogRepository.GetQueryable().Where(i => i.LOG_ID == LogID).FirstOrDefault();
            LoginLog.LOGOUT_TIME = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            LoginLogRepository.SaveChanges();
 
        }
    }
}
