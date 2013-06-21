using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ICMDCarService:IService<CMD_CAR>
    {
        object GetDetails(int page, int rows, string CarName, string MEMO, string isActive);

        bool Add(string CarName, string MEMO, string isActive);

        bool Delete(string CarNo);

        bool Save(string CarNo, string CarName, string MEMO, string isActive);

    }
}
