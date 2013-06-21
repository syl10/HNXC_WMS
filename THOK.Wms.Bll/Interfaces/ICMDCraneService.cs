using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ICMDCraneService:IService<CMD_CRANE>
    {
        object GetDetails(int page, int rows, string CraneName, string MEMO, string isActive);

        bool Add(string CraneName, string MEMO, string isActive);

        bool Delete(string CraneNo);

        bool Save(string CraneNo, string CraneName, string MEMO, string isActive);

    }
}
