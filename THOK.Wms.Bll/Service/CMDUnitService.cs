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
    public class CMDUnitService:ServiceBase<CMD_UNIT>,ICMDUnitService
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public ICMDUnitRepository UnitRepository { get; set; }

        public object GetDetails(int page, int rows, string UNIT_NAME, string CATEGORY_CODE, string MEMO)
        {
            IQueryable<CMD_UNIT> Query = UnitRepository.GetQueryable();
            var Units = Query.OrderBy(i => i.UNIT_CODE).Select(i => new { i.UNIT_CODE, i.UNIT_NAME, i.CATEGORY_CODE,i.CMD_UNIT_CATEGORY.CATEGORY_NAME, i.MEMO });
            if (!string.IsNullOrEmpty(UNIT_NAME))
            {
                Units = Units.Where(i => i.UNIT_NAME.Contains(UNIT_NAME));
            }
            if (!string.IsNullOrEmpty(CATEGORY_CODE))
            {
                Units = Units.Where(i => i.CATEGORY_CODE == CATEGORY_CODE);
            }
            if (!string.IsNullOrEmpty(MEMO))
            {
                Units = Units.Where(i => i.MEMO.Contains(MEMO));
            }
            if (THOK.Common.PrintHandle.isbase)
            {
                THOK.Common.PrintHandle.baseinfoprint = THOK.Common.ConvertData.LinqQueryToDataTable(Units);
            }
            int total = Units.Count();
            Units = Units.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = Units.ToArray() };

        }

        public new bool Add(CMD_UNIT Unit)
        {
            Unit.UNIT_CODE = UnitRepository.GetNewID("CMD_UNIT", "UNIT_CODE");
            UnitRepository.Add(Unit);
            UnitRepository.SaveChanges();
            return true;
        }

        public bool Delete(string UNIT_CODE)
        {
            var unit = UnitRepository.GetQueryable().FirstOrDefault(i => i.UNIT_CODE == UNIT_CODE);
            UnitRepository.Delete(unit);
            int rejust= UnitRepository.SaveChanges();
            if (rejust == -1) return false;
            else return true;
        }

        public bool Save(CMD_UNIT Unit)
        {
            var SaveUnit = UnitRepository.GetQueryable().FirstOrDefault(i => i.UNIT_CODE == Unit.UNIT_CODE);
            SaveUnit.UNIT_NAME = Unit.UNIT_NAME;
            SaveUnit.CATEGORY_CODE = Unit.CATEGORY_CODE;
            SaveUnit.MEMO = Unit.MEMO;
            UnitRepository.SaveChanges();
            return true;
        }
    }
}
