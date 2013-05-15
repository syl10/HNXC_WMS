using System;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Authority.Bll.Interfaces;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;

namespace THOK.Authority.Bll.Service
{
    public class ServerService : ServiceBase<AUTH_SERVER>, IServerService
    {
        [Dependency]
        public IServerRepository ServerRepository { get; set; }
        [Dependency]
        public ICityRepository CityRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, string serverName, string description, string url, string isActive)
        {
            IQueryable<AUTH_SERVER> query = ServerRepository.GetQueryable();
            var servers = query.OrderBy(i => i.SERVER_ID).Select(i => new { i.SERVER_ID, i.SERVER_NAME, i.AUTH_CITY.CITY_ID, i.AUTH_CITY.CITY_NAME, i.DESCRIPTION, i.URL, IsActive = i.IS_ACTIVE == "1" ? "启用" : "禁用" });
            if (serverName != "" || description != "" || isActive != "")
            {
                servers = query.Where(i => i.SERVER_NAME.Contains(serverName)
                && i.DESCRIPTION.Contains(description)
                && i.URL.Contains(url))
                .OrderBy(i => i.SERVER_ID)
                .Select(i => new { i.SERVER_ID, i.SERVER_NAME, i.AUTH_CITY.CITY_ID, i.AUTH_CITY.CITY_NAME, i.DESCRIPTION, i.URL, IsActive = i.IS_ACTIVE == "1" ? "启用" : "禁用" });
            }
            int total = servers.Count();
            servers = servers.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = servers.ToArray() };
        }

        public bool Add(string serverName, string description, string url, bool isActive, string cityID)
        {
            // Guid gCityID = new Guid(cityID);
            var city = CityRepository.GetQueryable().Single(c => c.CITY_ID == cityID);
            var server = new AUTH_SERVER()
            {
                SERVER_ID = Guid.NewGuid().ToString(),
                SERVER_NAME = serverName,
                DESCRIPTION = description,
                URL = url,
                IS_ACTIVE = isActive.ToString(),
                AUTH_CITY = city
            };
            ServerRepository.Add(server);
            ServerRepository.SaveChanges();
            return true;
        }

        public bool Delete(string serverID)
        {
            // Guid gServerID = new Guid(serverID);
            var server = ServerRepository.GetQueryable()
                .FirstOrDefault(i => i.SERVER_ID == serverID);
            if (server != null)
            {
                ServerRepository.Delete(server);
                ServerRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(string serverID, string serverName, string description, string url, bool isActive, string cityID)
        {
            //Guid gServerID = new Guid(serverID);
            //Guid gCityID = new Guid(cityID);
            var city = CityRepository.GetQueryable().Single(c => c.CITY_ID == cityID);
            var server = ServerRepository.GetQueryable()
                .FirstOrDefault(i => i.SERVER_ID == serverID);
            server.SERVER_NAME = serverName;
            server.DESCRIPTION = description;
            server.URL = url;
            server.IS_ACTIVE = isActive.ToString();
            server.AUTH_CITY = city;
            ServerRepository.SaveChanges();
            return true;
        }

        public object GetServerById(string serverID)
        {
            //Guid sid = new Guid(serverID);
            var server = ServerRepository.GetQueryable().FirstOrDefault(s => s.SERVER_ID.Trim() == serverID);
            return server.SERVER_NAME;
        }

        public object GetDetails(string cityID, string serverID)
        {
            //Guid cityid=new Guid(cityID);
            //Guid serverid=new Guid(serverID);
            var server = ServerRepository.GetQueryable().Where(s => s.AUTH_CITY.CITY_ID == cityID && s.SERVER_ID == serverID).Select(s => s.SERVER_ID);
            var servers = ServerRepository.GetQueryable().Where(s => !server.Any(sv => sv == s.SERVER_ID) && s.AUTH_CITY.CITY_ID == cityID).Select(s => new { s.SERVER_ID, s.SERVER_NAME, s.DESCRIPTION, Status = bool.Parse(s.IS_ACTIVE) ? "启用" : "禁用" });
            return servers.ToArray();
        }

    }
}
