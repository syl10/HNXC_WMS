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
   public  class CMDCraneService : ServiceBase<CMD_CRANE>, ICMDCraneService
    {
       [Dependency]
       public ICMDCraneRepository CraneRepository { get; set; }
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, string CraneName, string MEMO, string isActive)
        {
            IQueryable<CMD_CRANE> Query = CraneRepository.GetQueryable();
            var Cranes = Query.OrderBy(i => i.CRANE_NO).Select(i => new { i.CRANE_NO, i.CRANE_NAME, i.MEMO, IS_ACTIVE = i.IS_ACTIVE == "1" ? "启用" : "禁用" });
            if (!string.IsNullOrEmpty(CraneName))
            {
                Cranes = Cranes.Where(i => i.CRANE_NAME.Contains(CraneName));
            }
            if (!string.IsNullOrEmpty(MEMO))
            {
                Cranes = Cranes.Where(i => i.MEMO.Contains(MEMO));
            }
            if (!string.IsNullOrEmpty(isActive))
            {
                Cranes = Cranes.Where(i => i.IS_ACTIVE == isActive);
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
            var Crane = new CMD_CRANE()
            {
                CRANE_NO = CraneRepository.GetNewID("CMD_CRANE", "CRANE_NO"),
                CRANE_NAME = CraneName,
                MEMO = MEMO,
                IS_ACTIVE = isActive
            };

            CraneRepository.Add(Crane);
            CraneRepository.SaveChanges();
            return true;
        }

        public bool Delete(string CraneNo)
        {
            var Crane = CraneRepository.GetQueryable().FirstOrDefault(i => i.CRANE_NO == CraneNo);
            CraneRepository.Delete(Crane);
           int rejust= CraneRepository.SaveChanges();
           if (rejust == -1) return false;
           else
               return true;
        }

        public bool Save(string CraneNo, string CraneName, string MEMO, string isActive)
        {
            var Crane = CraneRepository.GetQueryable().FirstOrDefault(i => i.CRANE_NO == CraneNo);
            Crane.CRANE_NAME = CraneName;
            Crane.MEMO = MEMO;
            Crane.IS_ACTIVE = isActive;
            CraneRepository.SaveChanges();
            return true;
        }
    }
}
