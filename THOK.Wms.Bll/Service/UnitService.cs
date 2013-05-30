using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    public class UnitService : ServiceBase<Unit>, IUnitService
    {
        [Dependency]
        public IUnitRepository UnitRepository { get; set; }

        [Dependency]
        public IUnitListRepository UnitListRepository { get; set; }

        [Dependency]
        public IProductRepository ProductRepository { get; set; }
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IUnitService 成员

        public object GetDetails(int page, int rows, string UnitCode, string UnitName, string IsActive)
        {
            IQueryable<Unit> unitQuery = UnitRepository.GetQueryable();
            var unit = unitQuery.Where(u => u.UnitCode.Contains(UnitCode) && u.UnitName.Contains(UnitName))
                .OrderBy(u => u.UnitCode)
                .Select(u => u);
            if (!IsActive.Equals(""))
            {
                unit = unit.Where(u => u.IsActive.Contains(IsActive));
            }
            int total = unit.Count();
            unit = unit.Skip((page - 1) * rows).Take(rows);

            var temp = unit.ToArray().Select(u => new
            {
                u.UnitCode,
                u.UnitName,
                COUNT = u.Count,
                IsActive = u.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = u.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss"),
                RowVersion = Convert.ToBase64String(u.RowVersion)
            });
            return new { total, rows = temp.ToArray() };
        }

        public object GetDetails(int page, int rows, string QueryString, string Value)
        {
            string UnitCode = "";
            string UnitName = "";
            if (QueryString == "UnitCode")
            {
                UnitCode = Value;
            }
            else
            {
                UnitName = Value;
            }
            IQueryable<Unit> UnitQuery = UnitRepository.GetQueryable();
            var Unit = UnitQuery.Where(c => c.UnitName.Contains(UnitName) && c.UnitCode.Contains(UnitCode) && c.IsActive == "1")
                .OrderBy(c => c.UnitCode).AsEnumerable()
                .Select(c => new
                {

                    IsActive = c.IsActive == "1" ? "可用" : "不可用",
                    c.UnitCode,
                    c.UnitName,
                    UpdateTime = c.UpdateTime.ToString("yyyy-MM-dd")
                });
            int total = Unit.Count();
            Unit = Unit.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = Unit.ToArray() };
        }

        public new bool Add(Unit unit)
        {
            var un = new Unit();
            un.UnitCode = unit.UnitCode;
            un.UnitName = unit.UnitName;
            un.Count = unit.Count;
            un.IsActive = unit.IsActive;
            un.UpdateTime = DateTime.Now;

            UnitRepository.Add(un);
            UnitRepository.SaveChanges();
            return true;
        }

        public bool Delete(string UnitCode)
        {
            var unit = UnitRepository.GetQueryable()
                .FirstOrDefault(b => b.UnitCode == UnitCode);
            if (UnitCode != null)
            {
                UnitRepository.Delete(unit);
                UnitRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(Unit unit)
        {
            unit.UpdateTime = DateTime.Now;
            UnitRepository.Attach(unit);
            UnitRepository.SaveChanges();
            return true;
        }

        /// <summary>
        /// 根据卷烟编码查询系列单位
        /// </summary>
        /// <param name="productCode">卷烟编码</param>
        /// <returns></returns>
        public object FindUnit(string productCode)
        {
            IQueryable<Unit> unitQuery = UnitRepository.GetQueryable();
            var product = ProductRepository.GetQueryable().FirstOrDefault(p => p.ProductCode == productCode);
            var unitList = product.UnitList;
            var r = (new Unit[] { unitList.Unit01, unitList.Unit02, unitList.Unit03, unitList.Unit04 }).Select(u => new { UnitCode = u.UnitCode, UnitName = u.UnitName });
            return r;
        }

        #endregion

        public bool DownUnit()
        {
            throw new NotImplementedException();
        }

        public System.Data.DataTable GetUnit(int page, int rows, string UnitCode, string UnitName, string IsActive)
        {
            IQueryable<Unit> unitQuery = UnitRepository.GetQueryable();
            var unit = unitQuery.Where(u => u.UnitCode.Contains(UnitCode) && u.UnitName.Contains(UnitName))
                .OrderBy(u => u.UnitCode)
                .Select(u => u);
            if (!IsActive.Equals(""))
            {
                unit = unit.Where(u => u.IsActive.Contains(IsActive));
            }
            var temp = unit.ToArray().Select(u => new
            {
                u.UnitCode,
                u.UnitName,
                COUNT = u.Count,
                IsActive = u.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = u.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss"),
                RowVersion = Convert.ToBase64String(u.RowVersion)
            });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("计量单位编码", typeof(string));
            dt.Columns.Add("计量单位名称", typeof(string));
            dt.Columns.Add("支数", typeof(string));
            dt.Columns.Add("是否启用", typeof(string));
            dt.Columns.Add("更新时间", typeof(string));
            foreach (var item in temp)
            {
                dt.Rows.Add
                    (
                        item.UnitCode,
                        item.UnitName,
                        item.COUNT,
                        item.IsActive,
                        item.UpdateTime
                    );
            }
            return dt;
        }
    }
}
