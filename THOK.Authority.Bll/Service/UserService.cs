using System;
using System.Linq;
using System.Web.Script.Serialization;
using Microsoft.Practices.Unity;
using THOK.Authority.Bll.Interfaces;
using THOK.Authority.Bll.Models;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;
using THOK.Common;

namespace THOK.Authority.Bll.Service
{
    public class UserService : ServiceBase<AUTH_USER>, IUserService
    {
        [Dependency]
        public IUserRepository UserRepository { get; set; }
        [Dependency]
        public IRoleRepository RoleRepository { get; set; }
        [Dependency]
        public IUserRoleRepository UserRoleRepository { get; set; }
        [Dependency]
        public IUserSystemRepository UserSystemRepository { get; set; }
        [Dependency]
        public ICityRepository CityRepository { get; set; }
        [Dependency]
        public IServerRepository ServerRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        public bool UpdateUserInfo(string userName)
        {
            string ipaddress=System.Net.Dns.Resolve(System.Net.Dns.GetHostName()).AddressList[0].ToString();
            var user = UserRepository.GetSingle(i => i.USER_NAME == userName);
            if (user != null)
            {
                user.USER_NAME = userName;
                user.LOGIN_PC = ipaddress;
                UserRepository.SaveChanges();
                return true;
            }
            else { return false; }
        }
        public string GetLocalIp(string userName)
        {
            string ipaddress = System.Net.Dns.Resolve(System.Net.Dns.GetHostName()).AddressList[0].ToString();
            return ipaddress;
        }
        public string GetUserIp(string userName)
        {
            string loginPC = "";
            var user = UserRepository.GetQueryable().Where(i => i.USER_NAME == userName).ToArray();
            if (user.Count() > 0)
            {
                loginPC = user[0].LOGIN_PC;
                return loginPC;
            }
            else
            {
                return "";
            }
        }

        public bool DeleteUserIp(string userName)
        {
            var user = UserRepository.GetSingle(i => i.USER_NAME == userName);
            if (user != null)
            {
                user.LOGIN_PC = "";
                UserRepository.SaveChanges();
                return true;
            }
            else { return false; }
        }

        public object GetDetails(int page, int rows, string userName, string chineseName, string isLock, string isAdmin, string memo)
        {
            IQueryable<AUTH_USER> query = UserRepository.GetQueryable();
            var users = query.Where(i => i.USER_NAME.Contains(userName)
                && i.CHINESE_NAME.Contains(chineseName)
                && i.MEMO.Contains(memo))
                .OrderBy(i => i.USER_ID)
                .Select(i => new { i.USER_ID, i.USER_NAME, i.CHINESE_NAME, i.MEMO, IsLock =bool.Parse(i.IS_LOCK) ? "是" : "否", IsAdmin = bool.Parse(i.IS_ADMIN) ? "是" : "否" });
            int total = users.Count();
            users = users.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = users.ToArray() };
        }

        public bool Add(string userName, string pwd, string chineseName, bool isLock, bool isAdmin, string memo)
        {
            if (Check(userName))
            {
                var user = new AUTH_USER()
                {
                    USER_ID = Guid.NewGuid().ToString(),
                    USER_NAME = userName,
                    PWD = EncryptPassword(pwd),
                    CHINESE_NAME = chineseName,
                    IS_LOCK = isLock.ToString(),
                    IS_ADMIN = isAdmin.ToString(),
                    MEMO = memo
                };
                UserRepository.Add(user);
                UserRepository.SaveChanges();
                return true;
            }
            else { return false; }
        }

        public bool Delete(string userID)
        {
            //Guid gUserID = new Guid(userID);
            var user = UserRepository.GetQueryable()
                .FirstOrDefault(u => u.USER_ID == userID);
            if (user != null)
            {
                Del(UserRoleRepository, user.AUTH_USER_ROLE);
                Del(UserSystemRepository, user.AUTH_USER_SYSTEM);
                UserRepository.Delete(user);
                UserRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(string userID, string userName, string pwd, string chineseName, bool isLock, bool isAdmin, string memo)
        {
            //Guid gUserID = new Guid(userID);
            var user = UserRepository.GetQueryable()
                .FirstOrDefault(u => u.USER_ID == userID);
            user.USER_NAME = userName;
            user.PWD = !string.IsNullOrEmpty(pwd) ? EncryptPassword(pwd) : user.PWD;
            user.CHINESE_NAME = chineseName;
            user.IS_LOCK = isLock.ToString();
            user.IS_ADMIN = isAdmin.ToString();
            user.MEMO = memo;
            UserRepository.SaveChanges();
            return true;
        }

        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("值不能为NULL或为空。", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("值不能为NULL或为空。", "password");

            var adduser = new AUTH_USER();
            adduser.USER_ID = "001";
            adduser.USER_NAME = userName;
            adduser.CHINESE_NAME = "";
            adduser.LOGIN_PC = "";
            adduser.MEMO = "";
            adduser.PWD = EncryptPassword(password);
            adduser.IS_LOCK = "0";
            adduser.IS_ADMIN = "0";

            var user = UserRepository.GetSingle(i => i.USER_NAME == userName);
            if (user == null)
            {
                UserRepository.Add(adduser);
                UserRepository.SaveChanges();
            }
            return user != null && ComparePassword(password, user.PWD);
        }

        public bool ValidateUserPermission(string userName, string cityId, string systemId)
        {
            return true;
        }

        public string GetLogOnUrl(string userName, string password, string cityId, string systemId, string serverId)
        {
            string url = "";
            string logOnKey = "";
            if (string.IsNullOrEmpty(serverId))
            {
                url = GetUrlFromCity(cityId, out serverId);
            }

            if (string.IsNullOrEmpty(password))
            {
                IQueryable<AUTH_USER> queryCity = UserRepository.GetQueryable();
                var user = queryCity.Single(c => c.USER_NAME == userName);
                password = user.PWD;
            }

            if (!string.IsNullOrEmpty(serverId))
            {
                url = GetUrlFromServer(serverId);
            }
            var key = new UserLoginInfo()
                    {
                        CityID = cityId,
                        SystemID = systemId,
                        UserName = userName,
                        Password = password,
                        ServerID = serverId
                    };
            logOnKey = Des.EncryptDES((new JavaScriptSerializer()).Serialize(key), "12345678");
            url += @"/Account/LogOn/?LogOnKey=" + Uri.EscapeDataString(logOnKey);
            return url;
        }

        public bool ChangePassword(string userName, string password, string newPassword)
        {
            var user = UserRepository.GetQueryable().FirstOrDefault(u => u.USER_NAME == userName);
            if (ComparePassword(password, user.PWD))
            {
                user.PWD = EncryptPassword(newPassword);
                UserRepository.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 验证，用户名不能重复
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool Check(string userName)
        {
            var userNames = UserRepository.GetQueryable().Select(u => u.USER_NAME).ToArray();
            if(userNames.Contains(userName))
            {
                return false;
            }
            else{
            return true;}
        }
        public string FindUsersForFunction(string strFunctionID)
        {
            throw new NotImplementedException();
        }

        private bool ComparePassword(string password, string hash)
        {
            return EncryptPassword(password) == hash || password == hash;
        }

        private string EncryptPassword(string password)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
            data = x.ComputeHash(data);
            return System.Text.Encoding.ASCII.GetString(data);
        }

        private string GetUrlFromCity(string gCityID, out string serverId)
        {
            IQueryable<AUTH_CITY> queryCity = CityRepository.GetQueryable();
            var city = queryCity.Single(c => c.CITY_ID == gCityID);
            var server = city.AUTH_SERVER.OrderBy(s => s.SERVER_ID).First();
            serverId = server.SERVER_ID.ToString();
            return server.URL;
        }

        private string GetUrlFromServer(string gServerID)
        {
            IQueryable<AUTH_SERVER> query = ServerRepository.GetQueryable();
            var system = query.Single(s => s.SERVER_ID == gServerID);
            return system.URL;
        }

        public object GetUserRole(string userID)
        {
            //Guid uid = new Guid(userID);
            IQueryable<AUTH_USER> queryUser = UserRepository.GetQueryable();
            IQueryable<AUTH_ROLE> queryRole = RoleRepository.GetQueryable();
            var user = queryUser.FirstOrDefault(u => u.USER_ID == userID);
            var roles = user.AUTH_USER_ROLE.OrderBy(r => r.AUTH_ROLE.ROLE_ID).Select(r => new { r.USER_ROLE_ID, r.AUTH_USER.USER_ID, r.AUTH_USER.USER_NAME, r.AUTH_ROLE.ROLE_ID, r.AUTH_ROLE.ROLE_NAME });
            return roles.ToArray();
        }

        public object GetRoleInfo(string userID)
        {
            //Guid uid = new Guid(userID);
            IQueryable<AUTH_USER> queryUser = UserRepository.GetQueryable();
            IQueryable<AUTH_ROLE> queryRole = RoleRepository.GetQueryable();
            var user = queryUser.FirstOrDefault(u => u.USER_ID == userID);
            var roleIDs = user.AUTH_USER_ROLE.Select(ur => ur.AUTH_ROLE.ROLE_ID);
            var role = queryRole.Where(r => !roleIDs.Any(rid => rid == r.ROLE_ID))
                .Select(r => new { r.ROLE_ID, r.ROLE_NAME, Description = r.MEMO, Status = bool.Parse(r.IS_LOCK) ? "启用" : "禁用" });
            return role.ToArray();
        }

        public bool DeleteUserRole(string userRoleIdStr)
        {
            string[] userRoleIdList = userRoleIdStr.Split(',');
            for (int i = 0; i < userRoleIdList.Length - 1; i++)
            {
                //Guid userRoleId = new Guid(userRoleIdList[i]);
                string userRoleId = userRoleIdList[i].ToString();
                var UserRole = UserRoleRepository.GetQueryable().FirstOrDefault(ur => ur.USER_ROLE_ID == userRoleId);
                if (UserRole != null)
                {
                    UserRoleRepository.Delete(UserRole);
                    UserRoleRepository.SaveChanges();
                }
            }
            return true;
        }

        public bool AddUserRole(string userID, string roleIDStr)
        {
            try
            {
                var user = UserRepository.GetQueryable().FirstOrDefault(u => u.USER_ID == userID);
                string[] roleIdList = roleIDStr.Split(',');
                for (int i = 0; i < roleIdList.Length - 1; i++)
                {
                    //Guid rid = new Guid(roleIdList[i]);
                    string rid = roleIdList[i].ToString();
                    var role = RoleRepository.GetQueryable().FirstOrDefault(r => r.ROLE_ID == rid);
                    var userRole = new AUTH_USER_ROLE();
                    userRole.USER_ROLE_ID = Guid.NewGuid().ToString();
                    userRole.AUTH_USER = user;
                    userRole.AUTH_ROLE = role;
                    UserRoleRepository.Add(userRole);
                    UserRoleRepository.SaveChanges();
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public object GetUser(int page, int rows, string queryString, string value)
        {
            string userName = "", chineseName = "";

            if (queryString == "UserName")
            {
                userName = value;
            }
            else
            {
                chineseName = value;
            }
            IQueryable<AUTH_USER> query = UserRepository.GetQueryable();
            var users = query.Where(i => i.USER_NAME.Contains(userName) && i.CHINESE_NAME.Contains(chineseName) && bool.Parse(i.IS_ADMIN) == true)
                .OrderBy(i => i.USER_ID)
                .Select(i => new
                {
                    i.USER_ID,
                    i.USER_NAME,
                    i.CHINESE_NAME,
                    IsAdmin = bool.Parse(i.IS_ADMIN) ? "是" : "否"
                });
            int total = users.Count();
            users = users.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = users.ToArray() };
        }
    }
}

