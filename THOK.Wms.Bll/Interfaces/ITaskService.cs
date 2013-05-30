using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ITaskService : IService<Task>
    {
        bool InBIllTask(string billNo, out string errInfo);
        bool MoveBIllTask(string billNo, out string errInfo);
    }
}
