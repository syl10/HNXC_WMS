using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
     public interface IRegionService:IService<Region>
    {
         object GetDetails(int page, int rows, string RegionName, string State);

         bool Add(Region region, out string strResult);

         bool Save(Region region, out string strResult);

         bool Delete(int regionId, out string strResult);

         object GetRegion(int page, int rows, string queryString, string value);

         System.Data.DataTable GetRegion(int page, int rows, string regionName, string state, string t);
    }
}
