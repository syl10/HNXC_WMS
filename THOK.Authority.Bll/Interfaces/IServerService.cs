using THOK.Authority.DbModel;

namespace THOK.Authority.Bll.Interfaces
{
    public interface IServerService : IService<AUTH_SERVER>
    {
        object GetDetails(int page, int rows, string serverName, string description, string url, string isActive, string CITY_CITY_ID);

        bool Add(string serverName, string description, string url, string isActive,string cityID);

        bool Delete(string serverID);

        bool Save(string serverID, string serverName, string description, string url, string isActive,string cityID);

        object GetServerById(string serverID);

        object GetDetails(string cityID,string serverID);
    }
}
