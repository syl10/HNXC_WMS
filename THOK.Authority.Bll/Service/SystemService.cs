using System;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Authority.Bll.Interfaces;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;

namespace THOK.Authority.Bll.Service
{
    public class SystemService : ServiceBase<AUTH_SYSTEM>, ISystemService
    {
        [Dependency]
        public ISystemRepository SystemRepository { get; set; }
        [Dependency]
        public IModuleRepository ModuleRepository { get; set; }
        [Dependency]
        public IRoleSystemRepository RoleSystemRepository { get; set; }
        [Dependency]
        public IUserSystemRepository UserSystemRepository { get; set; }
        [Dependency]
        public IUserModuleRepository UserModuleRepository { get; set; }
        [Dependency]
        public IUserFunctionRepository UserFunctionRepository { get; set; }
        [Dependency]
        public ILoginLogRepository LoginLogRepository { get; set; }
        [Dependency]
        public IUserRepository UserRepository { get; set; }
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, string system_Name, string description, string status)
        {
            IQueryable<AUTH_SYSTEM> query = SystemRepository.GetQueryable();
            var systems = query.Where(i => i.SYSTEM_NAME.Contains(system_Name) && i.DESCRIPTION.Contains(description))
                .OrderBy(i => i.SYSTEM_ID)
                .Select(i => new { i.SYSTEM_ID, i.SYSTEM_NAME, i.DESCRIPTION, STATUS = i.STATUS == "1" ? "启用" : "禁用" });
            if (status != "")
            {
               // bool bStatus = Convert.ToBoolean(status);
                string bStatus = status;
                systems = query.Where(i => i.SYSTEM_NAME.Contains(system_Name) && i.DESCRIPTION.Contains(description) && i.STATUS == bStatus)
                    .OrderBy(i => i.SYSTEM_ID)
                    .Select(i => new { i.SYSTEM_ID, i.SYSTEM_NAME, i.DESCRIPTION, STATUS = i.STATUS == "1" ? "启用" : "禁用" });
            }
            int total = systems.Count();
            systems = systems.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = systems.ToArray() };
        }

        public bool Add(string systemName, string description, string status)
        {
            var system = new AUTH_SYSTEM()
            {
               // SYSTEM_ID = Guid.NewGuid().ToString(),
               SYSTEM_ID=SystemRepository.GetNewID("AUTH_SYSTEM","SYSTEM_ID"),
                SYSTEM_NAME = systemName,
                DESCRIPTION = description,
                STATUS = status
            };
            SystemRepository.Add(system);
            SystemRepository.SaveChanges();
            return true;
        }

        public bool Delete(string systemId)
        {
            //Guid gsystemId = new Guid(systemId);
            var system = SystemRepository.GetQueryable()
                .FirstOrDefault(i => i.SYSTEM_ID == systemId);
            if (system != null)
            {
                Del(ModuleRepository, system.AUTH_MODULE);
                Del(RoleSystemRepository, system.AUTH_ROLE_SYSTEM);
                Del(UserSystemRepository, system.AUTH_USER_SYSTEM);
                Del(LoginLogRepository, system.AUTH_LOGIN_LOG);
                SystemRepository.Delete(system);
                SystemRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(string systemId, string systemName, string description, string status)
        {
            //Guid sid = new Guid(systemId);
            var system = SystemRepository.GetQueryable()
                .FirstOrDefault(i => i.SYSTEM_ID == systemId);
            system.SYSTEM_NAME = systemName;
            system.DESCRIPTION = description;
            system.STATUS = status;
            SystemRepository.SaveChanges();
            return true;
        }

        public object GetSystemById(string systemID)
        {
            //Guid sid = new Guid(systemID);
            var sysytem = SystemRepository.GetQueryable().FirstOrDefault(s => s.SYSTEM_ID.Trim() == systemID);
            return sysytem.SYSTEM_NAME;
        }

        public object GetDetails(string userName, string systemID, string cityID)
        {
            var user = UserRepository.GetQueryable().FirstOrDefault(u => u.USER_NAME.ToLower() == userName.ToLower());
            string strSQL = "";
            if (user.IS_ADMIN == "1") {
                strSQL = string.Format("select * from auth_system system where system_id<>'{0}' ",systemID);
            }
            else 
            strSQL=string.Format("select * from auth_user_system system "+
                          "left join auth_system on system.system_system_id=auth_system.system_id " +
                         "where (is_active='1'or exists(select 1 from auth_user_module where (user_system_user_system_id=system.user_system_id and auth_user_module.is_active='1' "+
                         "or exists(select 1 from auth_user_function where auth_user_function.user_module_user_module_id=auth_user_module.module_module_id and auth_user_function.is_active='1')))) "+
                         "and user_user_id='{0}' and system_system_id<>'{1}' and city_city_id='001'",user.USER_ID,systemID);
            var pre = SystemRepository.RepositoryContext.DbContext.Database.SqlQuery<AUTH_SYSTEM>(strSQL);
            var qq = pre.Select(us => new { us.SYSTEM_ID, us.SYSTEM_NAME, us.DESCRIPTION, STATUS = us.STATUS == "1" ? "启用" : "未启用" });

            return qq.ToArray();
        }

    }
}
