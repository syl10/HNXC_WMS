using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;


namespace THOK.Wms.Bll.Service
{
    public class CMDCarService : ServiceBase<CMD_CAR>, ICMDCarService
    {
       [Dependency]
       public ICMDCarRepository CarRepository { get; set; }
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, string CarName, string MEMO, string isActive)
        {
            IQueryable<CMD_CAR> Query = CarRepository.GetQueryable();
            var Cranes = Query.OrderBy(i => i.CAR_NO).Select(i => new { i.CAR_NO, i.CAR_NAME, i.MEMO,IS_ACTIVECODE=i .IS_ACTIVE, IS_ACTIVE = i.IS_ACTIVE == "1" ? "启用" : "禁用" });
            if (!string.IsNullOrEmpty(CarName))
            {
                Cranes = Cranes.Where(i => i.CAR_NAME.Contains(CarName));
            }
            if (!string.IsNullOrEmpty(MEMO))
            {
                Cranes = Cranes.Where(i => i.MEMO.Contains(MEMO));
            }
            if (!string.IsNullOrEmpty(isActive))
            {
                Cranes = Cranes.Where(i => i.IS_ACTIVECODE  == isActive);
            }
            if (THOK.Common.PrintHandle.isbase)
            {
                THOK.Common.PrintHandle.baseinfoprint = THOK.Common.ConvertData.LinqQueryToDataTable(Cranes);
            }
            int total = Cranes.Count();
            Cranes = Cranes.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = Cranes.ToArray() };
        }

        public bool Add(string CraneName, string MEMO, string isActive)
        {
            var Crane = new CMD_CAR()
            {
                CAR_NO = CarRepository.GetNewID("CMD_CAR", "CAR_NO"),
                CAR_NAME = CraneName,
                MEMO = MEMO,
                IS_ACTIVE = isActive
            };

            CarRepository.Add(Crane);
            CarRepository.SaveChanges();
            return true;
        }

        public bool Delete(string CraneNo)
        {
            var Crane = CarRepository.GetQueryable().FirstOrDefault(i => i.CAR_NO == CraneNo);
            CarRepository.Delete(Crane);
           int rejust= CarRepository.SaveChanges();
           if (rejust == -1) return false;
           else return true;
        }

        public bool Save(string CraneNo, string CraneName, string MEMO, string isActive)
        {
            var Crane = CarRepository.GetQueryable().FirstOrDefault(i => i.CAR_NO == CraneNo);
            Crane.CAR_NAME = CraneName;
            Crane.MEMO = MEMO;
            Crane.IS_ACTIVE = isActive;
            CarRepository.SaveChanges();
            return true;
        }
    }
}
