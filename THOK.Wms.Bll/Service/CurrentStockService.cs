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
    public class CurrentStockService : ServiceBase<Storage>, ICurrentStockService
    {
        [Dependency]
        public IStorageRepository StorageRepository { get; set; }
        [Dependency]
        public IAreaRepository AreaRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region ICurrentStockService 成员

        public object GetCellDetails(int page, int rows, string productCode, string ware, string area, string unitType)
        {
            if (unitType == null || unitType == "")
            {
                unitType = "1";
            }
            IQueryable<Storage> storageQuery = StorageRepository.GetQueryable();
            var storages = storageQuery.Where(s => s.Quantity > 0);
            if (ware != null && ware != string.Empty || area != null && area != string.Empty)
            {
                if (ware != string.Empty)
                {
                    ware = ware.Substring(0, ware.Length - 1);
                }
                if (area != string.Empty)
                {
                    area = area.Substring(0, area.Length - 1);
                }

                storages = storages.Where(s => (ware.Contains(s.Cell.Shelf.Area.Warehouse.WarehouseCode) || area.Contains(s.Cell.Shelf.Area.AreaCode)));
            }
            if (productCode != string.Empty)
            {
                storages = storages.Where(s => s.ProductCode == productCode);
            }
            var storage = storages.GroupBy(s => s.Product.ProductCode)
                 .Select(s => new
                 {
                     ProductCode = s.Max(p => p.Product.ProductCode),
                     ProductName = s.Max(p => p.Product.ProductName),
                     Quantity = s.Sum(p => p.Quantity),
                     UnitName01 = s.Max(p => p.Product.UnitList.Unit01.UnitName),
                     UnitName02 = s.Max(p => p.Product.UnitList.Unit02.UnitName),
                     Count01 = s.Max(p => p.Product.UnitList.Unit01.Count),
                     Count02 = s.Max(p => p.Product.UnitList.Unit02.Count),
                 });
            int total = storage.Count();
            storage = storage.OrderBy(s => s.ProductCode);
            storage = storage.Skip((page - 1) * rows).Take(rows);
            if (unitType == "1")
            {
                string unitName1 = "标准件";
                decimal count1 = 10000;
                string unitName2 = "标准条";
                decimal count2 = 200;
                var currentstorage = storage.ToArray().Select(d => new
                {
                    ProductCode = d.ProductCode,
                    ProductName = d.ProductName,
                    UnitName1 = unitName1,
                    UnitName2 = unitName2,
                    Quantity1 = d.Quantity / count1,
                    Quantity2 = d.Quantity / count2,
                    Quantity3 = (d.Quantity % count1) / count2,
                    Quantity = d.Quantity
                });
                return new { total, rows = currentstorage.ToArray() };
            }
            if (unitType == "2")
            {
                var currentstorage = storage.ToArray().Select(d => new
                {
                    ProductCode = d.ProductCode,
                    ProductName = d.ProductName,
                    UnitName1 = d.UnitName01,
                    UnitName2 = d.UnitName02,
                    Quantity1 = d.Quantity / d.Count01,
                    Quantity2 = d.Quantity / d.Count02,
                    Quantity3 = (d.Quantity % d.Count01) / d.Count02,
                    Quantity = d.Quantity
                });
                return new { total, rows = currentstorage.ToArray() };
            }
            return new { total, rows = storage.ToArray() };
        }

        #endregion


        public System.Data.DataTable GetCurrentStock(int page, int rows, string productCode, string ware, string area, string unitType, out string areaName, bool isAbnormity)
        {
            areaName = string.Empty;
            if (unitType == null || unitType == "")
            {
                unitType = "1";
            }
            System.Data.DataTable dt = new System.Data.DataTable();
            IQueryable<Storage> storageQuery = StorageRepository.GetQueryable();
            IQueryable<Area> areaQuery = AreaRepository.GetQueryable();
            var areas = areaQuery.Where(a => area.Contains(a.AreaCode)).ToArray();
            foreach (var item in areas)
            {
                areaName += item.AreaName;
            }
            var storages = storageQuery.Where(s => s.Quantity > 0);
            if (ware != null && ware != string.Empty || area != null && area != string.Empty)
            {
                if (ware != string.Empty)
                {
                    ware = ware.Substring(0, ware.Length - 1);
                }
                if (area != string.Empty)
                {
                    area = area.Substring(0, area.Length - 1);
                }
                storages = storages.Where(s => (ware.Contains(s.Cell.Shelf.Area.Warehouse.WarehouseCode) || area.Contains(s.Cell.Shelf.Area.AreaCode)));
            }
            
            if (productCode != string.Empty)
            {
                storages = storages.Where(s => s.ProductCode == productCode);
            }

            if (isAbnormity == false)
                storages = storages.Where(s => s.Product.IsAbnormity != "1");

            var storage = storages.GroupBy(s => s.Product.ProductCode)
                 .Select(s => new
                 {
                     ProductCode = s.Max(p => p.Product.ProductCode),
                     ProductName = s.Max(p => p.Product.ProductName),
                     Quantity = s.Sum(p => p.Quantity),
                     UnitName01 = s.Max(p => p.Product.UnitList.Unit01.UnitName),
                     UnitName02 = s.Max(p => p.Product.UnitList.Unit02.UnitName),
                     Count01 = s.Max(p => p.Product.UnitList.Unit01.Count),
                     Count02 = s.Max(p => p.Product.UnitList.Unit02.Count),
                 });
            storage = storage.OrderBy(s => s.ProductCode);           
            if (unitType == "1")
            {
                string unitName1 = "标准件";
                decimal count1 = 10000;
                string unitName2 = "标准条";
                decimal count2 = 200;
                var currentstorage = storage.ToArray().Select(d => new
                {
                    ProductCode = d.ProductCode,
                    ProductName = d.ProductName,
                    UnitName1 = unitName1,
                    UnitName2 = unitName2,
                    Quantity1 = d.Quantity / count1,
                    Quantity2 = d.Quantity / count2,
                    Quantity3 = (d.Quantity % count1) / count2,
                    Quantity = d.Quantity
                });
                dt.Columns.Add("卷烟编码", typeof(string));
                dt.Columns.Add("卷烟名称", typeof(string));
                dt.Columns.Add("数量(件)", typeof(string));
                dt.Columns.Add("数量(条)", typeof(string));
                dt.Columns.Add("尾数(条)", typeof(string));
                foreach (var c in currentstorage)
                {
                    dt.Rows.Add
                        (
                            c.ProductCode,
                            c.ProductName,
                            c.Quantity1,
                            c.Quantity2,
                            c.Quantity3
                        );
                }
                if (currentstorage.Count() > 0)
                {
                    dt.Rows.Add(null, "总数：",
                        currentstorage.Sum(m => m.Quantity1),
                        currentstorage.Sum(m => m.Quantity2),
                        currentstorage.Sum(m => m.Quantity3));
                }
                return dt;
            }
            if (unitType == "2")
            {
                var currentstorage = storage.ToArray().Select(d => new
                {
                    ProductCode = d.ProductCode,
                    ProductName = d.ProductName,
                    UnitName1 = d.UnitName01,
                    UnitName2 = d.UnitName02,
                    Quantity1 = d.Quantity / d.Count01,
                    Quantity2 = d.Quantity / d.Count02,
                    Quantity3 = (d.Quantity % d.Count01) / d.Count02,
                    Quantity = d.Quantity
                });
                dt.Columns.Add("卷烟编码", typeof(string));
                dt.Columns.Add("卷烟名称", typeof(string));
                dt.Columns.Add("数量(件)", typeof(string));
                dt.Columns.Add("数量(条)", typeof(string));
                dt.Columns.Add("尾数(条)", typeof(string));
                foreach (var c in currentstorage)
                {
                    dt.Rows.Add
                        (
                            c.ProductCode,
                            c.ProductName,
                            c.Quantity1,
                            c.Quantity2,
                            c.Quantity3
                        );
                }
                if (currentstorage.Count() > 0)
                {
                    dt.Rows.Add(null, "总数：",
                        currentstorage.Sum(m => m.Quantity1),
                        currentstorage.Sum(m => m.Quantity2),
                        currentstorage.Sum(m => m.Quantity3));
                }
                return dt;
            }
            return dt;
        }
    }
}
