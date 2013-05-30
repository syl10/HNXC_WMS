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
    public class UnitListService:ServiceBase<UnitList>,IUnitListService
    {
        [Dependency]
        public IUnitListRepository UnitListRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IUnitListService 成员

        public object GetDetails(int page, int rows, UnitList uls)
        {
            IQueryable<UnitList> unitListQuery = UnitListRepository.GetQueryable();
            
            var unitList = unitListQuery.Where(ul =>
                ul.UnitListCode.Contains(uls.UnitListCode)
                && ul.UnitListName.Contains(uls.UnitListName)
                && ul.UnitCode01.Contains(uls.UnitCode01)
                && ul.UnitCode02.Contains(uls.UnitCode02)
                && ul.UnitCode03.Contains(uls.UnitCode03)
                && ul.UnitCode04.Contains(uls.UnitCode04)
                && ul.IsActive.Contains(uls.IsActive)).OrderBy(ul => ul.UnitListCode);

            int total = unitList.Count();
            var unitLists = unitList.Skip((page - 1) * rows).Take(rows);
            var unit_List = unitLists.ToArray().Select(ul => new
            {
                ul.UnitListCode,
                ul.UniformCode,
                ul.UnitListName,
                ul.UnitCode01,
                ul.UnitCode02,
                ul.UnitCode03,
                ul.UnitCode04,
                UnitName01 = ul.Unit01.UnitName,
                UnitName02 = ul.Unit02.UnitName,
                UnitName03 = ul.Unit03.UnitName,
                UnitName04 = ul.Unit04.UnitName,
                Quantity01 = Convert.ToInt32(ul.Quantity01).ToString(),
                Quantity02 = Convert.ToInt32(ul.Quantity02).ToString(),
                Quantity03 = Convert.ToInt32(ul.Quantity03).ToString(),
                ul.IsActive,
                UpdateTime = ul.UpdateTime.ToString("yyyy-MM-dd")
            });
            return new { total, rows = unit_List.ToArray() };
        }

        public new bool Add(UnitList unitlist)
        {
            var ul = new UnitList();
            ul.UnitListCode = unitlist.UnitListCode;
            ul.UniformCode = unitlist.UniformCode;
            ul.UnitListName = unitlist.UnitListName;
            ul.UnitCode01 = unitlist.UnitCode01;
            ul.Quantity01 = Convert.ToInt32(unitlist.Quantity01);
            ul.UnitCode02 = unitlist.UnitCode02;
            ul.Quantity02 = Convert.ToInt32(unitlist.Quantity02);
            ul.UnitCode03 = unitlist.UnitCode03;
            ul.Quantity03 = Convert.ToInt32(unitlist.Quantity03);
            ul.UnitCode04 = unitlist.UnitCode04;
            ul.IsActive = unitlist.IsActive;
            ul.UpdateTime = DateTime.Now;
            UnitListRepository.Add(ul);
            UnitListRepository.SaveChanges();
            return true;
        }

        public bool Delete(string unitlistCode)
        {
            var unitlist = UnitListRepository.GetQueryable()
                .FirstOrDefault(b => b.UnitListCode == unitlistCode);
            if (unitlistCode != null)
            {
                UnitListRepository.Delete(unitlist);
                UnitListRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(UnitList unitlist)
        {
            var ul = UnitListRepository.GetQueryable().FirstOrDefault(u => u.UnitListCode == unitlist.UnitListCode);
            ul.UnitListCode = unitlist.UnitListCode;
            ul.UniformCode = unitlist.UniformCode;
            ul.UnitListName = unitlist.UnitListCode;
            ul.UnitCode01 = unitlist.UnitCode01;
            ul.Quantity01 = Convert.ToInt32(unitlist.Quantity01);
            ul.UnitCode02 = unitlist.UnitCode02;
            ul.Quantity02 = Convert.ToInt32(unitlist.Quantity02);
            ul.UnitCode03 = unitlist.UnitCode03;
            ul.Quantity03 = Convert.ToInt32(unitlist.Quantity03);
            ul.UnitCode04 = unitlist.UnitCode04;
            ul.IsActive = unitlist.IsActive;
            ul.UpdateTime = DateTime.Now;
            UnitListRepository.SaveChanges();
            return true;
        }
        #endregion

        public System.Data.DataTable GetUnitList(int page, int rows, UnitList uls)
        {
            IQueryable<UnitList> unitListQuery = UnitListRepository.GetQueryable();

            var unitList = unitListQuery.Where(ul =>
                   ul.UnitListCode.Contains(uls.UnitListCode)
                && ul.UnitListName.Contains(uls.UnitListName)
                && ul.UnitCode01.Contains(uls.UnitCode01)
                && ul.UnitCode02.Contains(uls.UnitCode02)
                && ul.UnitCode03.Contains(uls.UnitCode03)
                && ul.UnitCode04.Contains(uls.UnitCode04)
                && ul.IsActive.Contains(uls.IsActive)).OrderBy(ul => ul.UnitListCode);

            var unit_List = unitList.ToArray().Select(ul => new
            {
                ul.UnitListCode,
                ul.UniformCode,
                ul.UnitListName,
                ul.UnitCode01,
                ul.UnitCode02,
                ul.UnitCode03,
                ul.UnitCode04,
                UnitName01 = ul.Unit01.UnitName,
                UnitName02 = ul.Unit02.UnitName,
                UnitName03 = ul.Unit03.UnitName,
                UnitName04 = ul.Unit04.UnitName,
                Quantity01 = Convert.ToInt32(ul.Quantity01).ToString(),
                Quantity02 = Convert.ToInt32(ul.Quantity02).ToString(),
                Quantity03 = Convert.ToInt32(ul.Quantity03).ToString(),
                ul.IsActive,
                UpdateTime = ul.UpdateTime.ToString("yyyy-MM-dd")
            });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("计量单位系列编码", typeof(string));
            dt.Columns.Add("计量单位系列名称", typeof(string));
            dt.Columns.Add("件单位名称", typeof(string));
            dt.Columns.Add("转换比例1", typeof(string));
            dt.Columns.Add("条单位名称", typeof(string));
            dt.Columns.Add("转换比例2", typeof(string));
            dt.Columns.Add("包单位名称", typeof(string));
            dt.Columns.Add("转换比例3", typeof(string));
            dt.Columns.Add("支单位名称", typeof(string));
            dt.Columns.Add("是否启用", typeof(string));
            dt.Columns.Add("更新时间", typeof(string));
            foreach (var item in unit_List)
            {
                dt.Rows.Add
                    (
                        item.UnitListCode,
                        item.UnitListName,
                        item.UnitName01,
                        item.Quantity01,
                        item.UnitName02,
                        item.Quantity02,
                        item.UnitName03,
                        item.Quantity03,
                        item.UnitName04,
                        item.IsActive,
                        item.UpdateTime
                    );
            }
            return dt;
        }
    }
}
