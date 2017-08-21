using THOK.Authority.DbModel;

namespace THOK.Authority.Bll.Interfaces
{
    public interface ILoginLogService : IService<AUTH_LOGIN_LOG>
    {
        bool Add(string UserName, string SystemID);
        object GetDetails(int page, int rows, string SystemID, string UserID, string LoginPC, string LoginTime);

        void UpdateValiateTime(string UserName);
    }
}
