﻿using System;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Authority.Bll.Interfaces;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;

namespace THOK.Authority.Bll.Service
{
    public class CityService : ServiceBase<AUTH_CITY>, ICityService
    {
        [Dependency]
        public ICityRepository CityRepository { get; set; }
        [Dependency]
        public IUserSystemRepository UserSystemRepository { get; set; }
        [Dependency]
        public IUserRepository UserRepository { get; set; }
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, string cityName, string description, string isActive)
        {
            IQueryable<AUTH_CITY> query = CityRepository.GetQueryable();
            string isactive;
            var citys = query.OrderBy(i => i.CITY_ID).Select(i => new { i.CITY_ID, i.CITY_NAME, i.DESCRIPTION, IS_ACTIVE = i.IS_ACTIVE == "1" ? "启用" : "禁用" });
            if (cityName != "" || description != "" || isActive != "")
            {
                isactive = isActive;

                citys = query.Where(i => i.CITY_NAME.Contains(cityName)
                    && i.DESCRIPTION.Contains(description) && i.IS_ACTIVE == isactive)
                    .OrderBy(i => i.CITY_ID)
                    .Select(i => new { i.CITY_ID, i.CITY_NAME, i.DESCRIPTION, IS_ACTIVE = i.IS_ACTIVE == "1" ? "启用" : "禁用" });
            }

            int total = citys.Count();
            citys = citys.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = citys.ToArray() };
        }

        public bool Add(string cityName, string description, string isActive)
        {
            var city = new THOK.Authority.DbModel.AUTH_CITY()
            {
                //CITY_ID = Guid.NewGuid().ToString(),
                CITY_ID=CityRepository.GetNewID("AUTH_CITY","CITY_ID"),
                CITY_NAME = cityName,
                DESCRIPTION = description,
                IS_ACTIVE = isActive
            };
            CityRepository.Add(city);
            CityRepository.SaveChanges();
            return true;
        }

        public bool Delete(string cityID)
        {
            //Guid gCityID = new Guid(cityID);
            var city = CityRepository.GetQueryable()
                .FirstOrDefault(i => i.CITY_ID == cityID);
            if (city != null)
            {
                CityRepository.Delete(city);
                CityRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(string cityID, string cityName, string description, string isActive)
        {
            //Guid gCityID = new Guid(cityID);
            var city = CityRepository.GetQueryable()
                .FirstOrDefault(i => i.CITY_ID == cityID);
            city.CITY_NAME = cityName;
            city.DESCRIPTION = description;
            city.IS_ACTIVE = isActive;
            CityRepository.SaveChanges();
            return true;
        }

        public object GetCityByCityID(string cityID)
        {
            //var cs= CityRepository.GetQueryable().First(c => c.CITY_ID == cityID);
            //string name = cs.CITY_NAME;
            ////var city = CityRepository.GetQueryable().FirstOrDefault(c => c.CITY_ID == cityID);
            //return name;

            IQueryable<AUTH_CITY> query = CityRepository.GetQueryable();
            var citys = query.Select(i => new { i.CITY_ID, i.CITY_NAME });
            int cou = citys.Count();
            var city = citys.FirstOrDefault(c => c.CITY_ID.Trim() == cityID);
            return city.CITY_NAME;
        }

        public object GetDetails(string userName, string cityID, string systemID)
        {
            //Guid cityid = new Guid(cityID);
            //Guid systemid = new Guid(systemID);
            var user = UserRepository.GetQueryable().FirstOrDefault(u => u.USER_NAME.ToLower() == userName.ToLower());
            var userSystem = UserSystemRepository.GetQueryable().Where(us => us.USER_USER_ID == user.USER_ID && us.SYSTEM_SYSTEM_ID == systemID && us.CITY_CITY_ID == cityID).Select(us => us.USER_SYSTEM_ID);
            var usersystems = UserSystemRepository.GetQueryable().Where(us => !userSystem.Any(uid => uid == us.USER_SYSTEM_ID) && us.USER_USER_ID == user.USER_ID && us.SYSTEM_SYSTEM_ID == systemID).Select(us => new { us.AUTH_CITY.CITY_ID, us.AUTH_CITY.CITY_NAME, us.AUTH_CITY.DESCRIPTION, IS_ACTIVE = us.AUTH_CITY.IS_ACTIVE == "1" ? "启用" : "禁用" });
            return usersystems.ToArray();
        }

         public System.Data.DataTable GetCityExcel(int page,int rows,string CITY_NAME,string DESCRIPTION,string IS_ACTIVE)
        {
            IQueryable<AUTH_CITY> Cityquery = CityRepository.GetQueryable();
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("城市名称", typeof(string));
            dt.Columns.Add("描述", typeof(string));
            dt.Columns.Add("是否启用", typeof(string));
            foreach (var item in Cityquery)
            {
                dt.Rows.Add
                    (
                        item.CITY_NAME,
                        item.DESCRIPTION,
                        item.IS_ACTIVE=="1"?"启用":"禁用"
                    );
            }
            return dt;
        }
        
    }
}
