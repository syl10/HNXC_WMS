using THOK.Authority.DbModel;

namespace THOK.Authority.Bll.Interfaces
{
    public interface ISystemEventLogService : IService<AUTH_SYSTEM_EVENT_LOG>
    {
        object GetDetails(int page, int rows, string eventlogtime, string eventtype, string eventname, string frompc, string operateuser, string targetsystem);

    }
}
