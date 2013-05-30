using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ISRMService :IService<SRM>
    {
        object GetDetails(int page, int rows,SRM srm);

        bool Add(SRM srm);

        bool Save(SRM srm);

        bool Delete(int srmId);

        object GetSRM(int page, int rows, string queryString, string value);

        System.Data.DataTable GetSRM(int page, int rows, SRM srm);
    }
}
