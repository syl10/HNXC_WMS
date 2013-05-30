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
    public class SortingLowerlimitService : ServiceBase<SortingLowerlimit>, ISortingLowerlimitService
    {
        [Dependency]
        public ISortingLowerlimitRepository SortingLowerlimitRepository { get; set; }

        [Dependency]
        public IUnitRepository UnitRepository { get; set; }
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region ISortingLowerlimitService 成员

        public object GetDetails(int page, int rows, string sortingLineCode, string sortingLineName, string productName, string productCode, string IsActive)
        {
            IQueryable<SortingLowerlimit> lowerLimitQuery = SortingLowerlimitRepository.GetQueryable();
            var lowerLimit = lowerLimitQuery.OrderBy(b => new { b.SortingLineCode,b.ProductCode }).Where(s => s.SortingLineCode == s.SortingLineCode);
            if (sortingLineCode != string.Empty && sortingLineCode != null)
            {
                lowerLimit = lowerLimit.Where(l => l.SortingLineCode.Contains(sortingLineCode));
            }
            if (sortingLineName != string.Empty && sortingLineName != null)
            {
                lowerLimit = lowerLimit.Where(l => l.SortingLine.SortingLineName.Contains(sortingLineName));
            }
            if (productCode != string.Empty && productCode != null)
            {
                lowerLimit = lowerLimit.Where(l => l.ProductCode.Contains(productCode));
            }
            if (productName != string.Empty && productName != null)
            {
                lowerLimit = lowerLimit.Where(l => l.Product.ProductName.Contains(productName));
            }
            if (IsActive != string.Empty && IsActive != null)
            {
                lowerLimit = lowerLimit.Where(l => l.IsActive == IsActive);
            }
            int total = lowerLimit.Count();
            lowerLimit = lowerLimit.Skip((page - 1) * rows).Take(rows);

            var temp = lowerLimit.ToArray().AsEnumerable().Select(b => new
            {
                b.ID,
                b.SortingLineCode,
                b.SortingLine.SortingLineName,
                b.ProductCode,
                b.Product.ProductName,
                b.UnitCode,
                b.Unit.UnitName,
                Quantity = b.Quantity / b.Unit.Count,
                IsActive = b.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = b.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            });

            return new { total, rows = temp.ToArray() };
        }

        public new bool Add(SortingLowerlimit sortLowerLimit)
        {
            var lowerLimit = new SortingLowerlimit();
            var unit = UnitRepository.GetQueryable().FirstOrDefault(u => u.UnitCode == sortLowerLimit.UnitCode);
            var lowerLimitList = SortingLowerlimitRepository.GetQueryable().FirstOrDefault(l => l.ProductCode == sortLowerLimit.ProductCode && l.SortingLineCode == sortLowerLimit.SortingLineCode);
            if (lowerLimitList == null)
            {
                lowerLimit.SortingLineCode = sortLowerLimit.SortingLineCode;
                lowerLimit.ProductCode = sortLowerLimit.ProductCode;
                lowerLimit.UnitCode = sortLowerLimit.UnitCode;
                lowerLimit.Quantity = sortLowerLimit.Quantity * unit.Count;
                lowerLimit.IsActive = sortLowerLimit.IsActive;
                lowerLimit.UpdateTime = DateTime.Now;

                SortingLowerlimitRepository.Add(lowerLimit);
                SortingLowerlimitRepository.SaveChanges();
            }
            else
            {
                lowerLimitList.Quantity = lowerLimitList.Quantity + (sortLowerLimit.Quantity * unit.Count);
                lowerLimitList.UpdateTime = DateTime.Now;
                SortingLowerlimitRepository.SaveChanges();
            }
            return true;
        }

        public bool Delete(string id)
        {
            var lowerLimit = SortingLowerlimitRepository.GetQueryable().ToArray().AsEnumerable().Where(s => id.Contains(s.ID.ToString()));
            if (lowerLimit.Count()>0)
            {
                foreach (var item in lowerLimit)
                {
                    SortingLowerlimitRepository.Delete(item);
                }
                SortingLowerlimitRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(SortingLowerlimit sortLowerLimit)
        {
            var lowerLimitSave = SortingLowerlimitRepository.GetQueryable().FirstOrDefault(s => s.ID == sortLowerLimit.ID);
            var unit = UnitRepository.GetQueryable().FirstOrDefault(u => u.UnitCode == sortLowerLimit.UnitCode);
            lowerLimitSave.SortingLineCode = sortLowerLimit.SortingLineCode;
            lowerLimitSave.ProductCode = sortLowerLimit.ProductCode;
            lowerLimitSave.UnitCode = sortLowerLimit.UnitCode;
            lowerLimitSave.Quantity = sortLowerLimit.Quantity * unit.Count;
            lowerLimitSave.IsActive = sortLowerLimit.IsActive;
            lowerLimitSave.UpdateTime = DateTime.Now;

            SortingLowerlimitRepository.SaveChanges();
            return true;
        }

        #endregion

        public System.Data.DataTable GetSortingLowerlimit(int page, int rows, string sortingLineCode, string sortingLineName, string productName, string productCode, string IsActive)
        {
            IQueryable<SortingLowerlimit> lowerLimitQuery = SortingLowerlimitRepository.GetQueryable();
            var lowerLimit = lowerLimitQuery.OrderBy(b => new { b.SortingLineCode, b.ProductCode }).Where(s => s.SortingLineCode == s.SortingLineCode);
            if (sortingLineCode != string.Empty && sortingLineCode != null)
            {
                lowerLimit = lowerLimit.Where(l => l.SortingLineCode.Contains(sortingLineCode));
            }
            if (sortingLineName != string.Empty && sortingLineName != null)
            {
                lowerLimit = lowerLimit.Where(l => l.SortingLine.SortingLineName.Contains(sortingLineName));
            }
            if (productCode != string.Empty && productCode != null)
            {
                lowerLimit = lowerLimit.Where(l => l.ProductCode.Contains(productCode));
            }
            if (productName != string.Empty && productName != null)
            {
                lowerLimit = lowerLimit.Where(l => l.Product.ProductName.Contains(productName));
            }
            if (IsActive != string.Empty && IsActive != null)
            {
                lowerLimit = lowerLimit.Where(l => l.IsActive == IsActive);
            }
            var temp = lowerLimit.ToArray().AsEnumerable().Select(b => new
            {
                b.ID,
                b.SortingLineCode,
                b.SortingLine.SortingLineName,
                b.ProductCode,
                b.Product.ProductName,
                b.UnitCode,
                b.Unit.UnitName,
                Quantity = b.Quantity / b.Unit.Count,
                IsActive = b.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = b.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("分拣线编码", typeof(string));
            dt.Columns.Add("分拣线名称", typeof(string));
            dt.Columns.Add("卷烟编码", typeof(string));
            dt.Columns.Add("卷烟名称", typeof(string));
            dt.Columns.Add("单位编码", typeof(string));
            dt.Columns.Add("单位名称", typeof(string));
            dt.Columns.Add("下限数量", typeof(decimal));
            dt.Columns.Add("是否可用", typeof(string));
            dt.Columns.Add("修改时间", typeof(string));
            foreach (var t in temp)
            {
                dt.Rows.Add
                    (
                        t.SortingLineCode,
                        t.SortingLineName,
                        t.ProductCode,
                        t.ProductName,
                        t.UnitCode,
                        t.UnitName,
                        t.Quantity,
                        t.IsActive,
                        t.UpdateTime
                    );
            }
            return dt;
        }
    }
}
