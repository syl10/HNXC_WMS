using THOK.Authority.DbModel;

namespace THOK.Authority.Bll.Interfaces
{
    public interface ISystemParameterService : IService<AUTH_SYSTEM_PARAMETER>
    {
        object GetSystemParameter(int page, int rows, AUTH_SYSTEM_PARAMETER systemParameter);
        bool SetSystemParameter(AUTH_SYSTEM_PARAMETER systemParameter, string userName, out string error);
        bool AddSystemParameter(AUTH_SYSTEM_PARAMETER systemParameter, string userName, out string error);
        bool DelSystemParameter(int id, out string error);
        bool SetSystemParameter();
    }
}
