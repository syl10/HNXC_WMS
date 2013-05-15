using THOK.Authority.DbModel;

namespace THOK.Authority.Bll.Interfaces
{
    public interface IUserService : IService<AUTH_USER>
    {
        string GetUserIp(string userName);

        bool UpdateUserInfo(string userName);

        string GetLocalIp(string userName);

        bool DeleteUserIp(string userName);

        object GetDetails(int page, int rows, string userName, string chineseName, string isLock, string isAdmin, string memo);

        bool Add(string userName, string pwd, string ChineseName, bool isLock, bool isAdmin,string memo);

        bool Delete(string userID);

        bool Save(string userID, string userName, string pwd, string chineseName, bool isLock, bool isAdmin,string memo);

        bool ValidateUser(string userName, string password);

        bool ChangePassword(string userName, string password, string newPassword);

        bool ValidateUserPermission(string userName, string cityId, string systemId);

        string GetLogOnUrl(string userName, string password, string cityId, string systemId, string serverId);

        string FindUsersForFunction(string strFunctionID);

        object GetUserRole(string userID);

        object GetRoleInfo(string userID);

        bool DeleteUserRole(string userRoleIdStr);

        bool AddUserRole(string userID, string roleIDStr);

        object GetUser(int page, int rows, string queryString, string value);

        bool Check(string userName);

    }
}
