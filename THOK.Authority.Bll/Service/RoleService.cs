using System;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Authority.Bll.Interfaces;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;

namespace THOK.Authority.Bll.Service
{
    public class RoleService : ServiceBase<AUTH_ROLE>, IRoleService
    {
        [Dependency]
        public IRoleRepository RoleRepository { get; set; }
        [Dependency]
        public IRoleSystemRepository RoleSystemRepository { get; set; }
        [Dependency]
        public IUserRoleRepository UserRoleRepository { get; set; }
        [Dependency]
        public IUserRepository UserRepository { get; set; }
        [Dependency]
        public IRoleFunctionRepository RoleFunctionRepository { get; set; }
        [Dependency]
        public IRoleModuleRepository RoleModuleRepository { get; set; }
        
        
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, string roleName, string memo,string islock)
        {
            IQueryable<AUTH_ROLE> queryRole = RoleRepository.GetQueryable();
            var roles = queryRole.Where(r => r.ROLE_NAME.Contains(roleName))
                    .OrderBy(r => r.ROLE_NAME)
                    .Select(r => new { r.ROLE_ID, r.ROLE_NAME, MEMO = r.MEMO, IS_LOCK = r.IS_LOCK == "1" ? "启用" : "禁用" });
            if (!String.IsNullOrEmpty(islock))
            {
                //bool bStatus = Convert.ToBoolean(islock);
                roles = queryRole.Where(r => r.ROLE_NAME.Contains(roleName)
                    && r.MEMO.Contains(memo)
                    && r.IS_LOCK == islock)
                    .OrderBy(r => r.ROLE_NAME)
                    .Select(r => new { r.ROLE_ID, r.ROLE_NAME, MEMO = r.MEMO, IS_LOCK = r.IS_LOCK == "1" ? "启用" : "禁用" });
            }
            if (THOK.Common.PrintHandle.isbase)
            {
                THOK.Common.PrintHandle.baseinfoprint = THOK.Common.ConvertData.LinqQueryToDataTable(roles);
            }
            int total = roles.Count();
            roles = roles.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = roles.ToArray() };
        }

        public bool Add(string roleName, string description, bool status)
        {
            var role = new AUTH_ROLE()
            {
                //ROLE_ID = Guid.NewGuid().ToString(),
                ROLE_ID = RoleRepository.GetNewID("AUTH_ROLE","ROLE_ID"),
                ROLE_NAME = roleName,
                MEMO = description,
                IS_LOCK = status==true?"1":"0"
            };
            RoleRepository.Add(role);
            RoleRepository.SaveChanges();
            return true;
        }
       
        public bool Delete(string roleID)
        {
            //Guid gRoleID = new Guid(roleID);
            var role = RoleRepository.GetQueryable()
                .FirstOrDefault(i => i.ROLE_ID == roleID);
            if (role != null)
            {
                while (role.AUTH_ROLE_SYSTEM.Count > 0)
                {
                    AUTH_ROLE_SYSTEM roleSystem = role.AUTH_ROLE_SYSTEM.First();
                    while (roleSystem.AUTH_ROLE_MODULE.Count > 0)
                    {
                        AUTH_ROLE_MODULE roleModule = roleSystem.AUTH_ROLE_MODULE.First();
                        while (roleModule.AUTH_ROLE_FUNCTION.Count > 0)
                        {
                            AUTH_ROLE_FUNCTION rolefunction = roleModule.AUTH_ROLE_FUNCTION.First();
                            RoleFunctionRepository.Delete(rolefunction);
                            RoleFunctionRepository.SaveChanges();
                        }
                        RoleModuleRepository.Delete(roleModule);
                        RoleModuleRepository.SaveChanges();
                    }
                    RoleSystemRepository.Delete(roleSystem);
                    RoleSystemRepository.SaveChanges();
                }

                while ( role.AUTH_USER_ROLE.Count>0)
                {
                    AUTH_USER_ROLE UserRole = role.AUTH_USER_ROLE.First();
                    UserRoleRepository.Delete(UserRole);
                    UserRoleRepository.SaveChanges();
                }

                //Del(RoleSystemRepository, role.AUTH_ROLE_SYSTEM);
                //Del(UserRoleRepository, role.AUTH_USER_ROLE);
                RoleRepository.Delete(role);
                RoleRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(string roleID, string roleName, string description, bool status)
        {
            //Guid gRoleID = new Guid(roleID);
            var role = RoleRepository.GetQueryable()
                .FirstOrDefault(i => i.ROLE_ID == roleID);
            role.ROLE_NAME = roleName;
            role.MEMO = description;
            role.IS_LOCK = status == true ? "1" : "0";
            RoleRepository.SaveChanges();
            return true;
        }

        public string FindRolesForFunction(string strFunctionID)
        {
            throw new NotImplementedException();
        }
        
        #region IRoleService 成员

        public object GetRoleUser(string roleID)
        {
            //Guid rid = new Guid(roleID);
            IQueryable<AUTH_USER> queryUser = UserRepository.GetQueryable();
            IQueryable<AUTH_ROLE> queryRole = RoleRepository.GetQueryable();
            var role = queryRole.FirstOrDefault(r => r.ROLE_ID == roleID);
            var users = role.AUTH_USER_ROLE.OrderBy(u => u.AUTH_USER.USER_ID).Select(u => new { u.USER_ROLE_ID, u.AUTH_ROLE.ROLE_ID, u.AUTH_ROLE.ROLE_NAME, u.AUTH_USER.USER_ID, u.AUTH_USER.USER_NAME });
            return users.ToArray();
        }

        public object GetUserInfo(string roleID)
        {
            //Guid rid = new Guid(roleID);
            IQueryable<AUTH_USER> queryUser = UserRepository.GetQueryable();
            IQueryable<AUTH_ROLE> queryRole = RoleRepository.GetQueryable();
            var role = queryRole.FirstOrDefault(r => r.ROLE_ID == roleID);
            var userIDs = role.AUTH_USER_ROLE.Select(ru => ru.AUTH_USER.USER_ID);
            var user = queryUser.Where(u => !userIDs.Any(uid => uid == u.USER_ID))
                .Select(u => new { u.USER_ID, u.USER_NAME, MEMO = u.MEMO, IS_LOCK = u.IS_LOCK == "1" ? "启用" : "禁用" });
            return user.ToArray();
        }

        public bool DeleteRoleUser(string roleUserIdStr)
        {
            string[] roleUserIdList = roleUserIdStr.Split(',');
            for (int i = 0; i < roleUserIdList.Length - 1; i++)
            {
                //Guid roleUserId = new Guid(roleUserIdList[i]);
                string roleUserId = roleUserIdList[i].ToString();
                var RoleUser = UserRoleRepository.GetQueryable().FirstOrDefault(ur => ur.USER_ROLE_ID == roleUserId);
                if (RoleUser != null)
                {
                    UserRoleRepository.Delete(RoleUser);
                    UserRoleRepository.SaveChanges();
                }
            }
            return true;
        }

        public bool AddRoleUser(string roleID, string userIDStr)
        {
            try
            {
                var role = RoleRepository.GetQueryable().FirstOrDefault(r => r.ROLE_ID== roleID);
                string[] userIdList = userIDStr.Split(',');
                for (int i = 0; i < userIdList.Length - 1; i++)
                {
                    //Guid uid = new Guid(userIdList[i]);
                    string uid = userIdList[i].ToString();
                    var user = UserRepository.GetQueryable().FirstOrDefault(u => u.USER_ID == uid);
                    var roleUser = new AUTH_USER_ROLE();
                    roleUser.USER_ROLE_ID = UserRoleRepository.GetNewID("AUTH_USER_ROLE", "USER_ROLE_ID");
                    roleUser.AUTH_USER = user;
                    roleUser.AUTH_ROLE = role;
                    UserRoleRepository.Add(roleUser);
                    UserRoleRepository.SaveChanges();
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
