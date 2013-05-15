using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;
using THOK.Common.Ef.EntityRepository;
namespace THOK.Authority.Dal.EntityRepository
{
    public class SystemEventLogRepository : RepositoryBase<AUTH_SYSTEM_EVENT_LOG>, ISystemEventLogRepository
    {
    }
}
