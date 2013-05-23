using THOK.Authority.DbModel;

namespace THOK.Authority.Bll.Interfaces
{
    public interface ICityService : IService<AUTH_CITY>
    {
        object GetDetails(int page, int rows, string cityName, string description, string isActive);

        bool Add(string cityName, string description, string isActive);        

        bool Delete(string cityID);

        bool Save(string cityID, string cityName, string description, string isActive);

        object GetCityByCityID(string cityID);

        object GetDetails(string userID, string cityID, string systemID);

       System.Data.DataTable GetCityExcel(int page,int rows,string CITY_NAME,string DESCRIPTION,string IS_ACTIVE);
    }
}
