using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using System.Data;
namespace THOK.Wms.Bll.Service
{
    public class CargospaceService : ServiceBase<Storage>, ICargospaceService
    {
        [Dependency]
        public IStorageRepository StorageRepository { get; set; }
        [Dependency]
        public ICellRepository CellRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region ICargospaceService 成员

        public object GetCellDetails(int page, int rows, string type, string id)
        {
            try
            {
                IQueryable<Storage> storageQuery = StorageRepository.GetQueryable();
                var storages = storageQuery.Where(s => s.StorageCode != null);
                if (type == "ware")
                {
                    storages = storages.Where(s => s.Cell.Shelf.Area.Warehouse.WarehouseCode == id);
                }
                else if (type == "area")
                {
                    storages = storageQuery.Where(s => s.Cell.Shelf.Area.AreaCode == id);
                }
                else if (type == "shelf")
                {
                    storages = storageQuery.Where(s => s.Cell.Shelf.ShelfCode == id);
                }
                else if (type == "cell")
                {
                    storages = storageQuery.Where(s => s.Cell.CellCode == id);
                }

                var temp = storages.OrderBy(s => s.Product.ProductName).Where(s => s.Quantity > 0);
                int total = temp.Count();
                temp = temp.Skip((page - 1) * rows).Take(rows);
                var Storage = temp.ToArray().ToArray().Select(s => new
                {
                    s.StorageCode,
                    s.Cell.CellCode,
                    s.Cell.CellName,
                    s.Product.ProductCode,
                    s.Product.ProductName,
                    s.Product.Unit.UnitCode,
                    s.Product.Unit.UnitName,
                    Quantity = s.Quantity / s.Product.Unit.Count,
                    IsActive = s.IsActive == "1" ? "可用" : "不可用",
                    StorageTime = s.StorageTime.ToString("yyyy-MM-dd"),
                    UpdateTime = s.UpdateTime.ToString("yyyy-MM-dd")
                });
                return new { total, rows = Storage.ToArray() };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public object GetCellDetails(string type, string id)
        {
            try
            {
                IQueryable<Cell> cellQuery = CellRepository.GetQueryable();
                IQueryable<Storage> storageQuery = StorageRepository.GetQueryable();
                var storages = storageQuery;
                var cells = cellQuery.Where(s => s.MaxPalletQuantity==0);
                if (type == "ware")
                {
                    cells = cellQuery.Where(c => c.Shelf.Area.Warehouse.WarehouseCode == id && c.MaxPalletQuantity == 0);
                }
                else if (type == "area")
                {
                    cells = cellQuery.Where(c => c.Shelf.Area.AreaCode == id && c.MaxPalletQuantity == 0);
                }
                else if (type == "shelf")
                {
                    cells = cellQuery.Where(c => c.Shelf.ShelfCode == id && c.MaxPalletQuantity == 0);
                }
                else if (type == "cell")
                {
                    cells = cellQuery.Where(c => c.CellCode == id && c.MaxPalletQuantity == 0);
                }
                var sCells = cells.Join(storages, c => c.CellCode, s => s.CellCode, (c, s) => new { cells = c, storages = s }).ToArray();
                if (sCells.Count() > 0)
                {
                    var Cell = sCells.Select(c => new
                    {
                        cellCode = c.cells.CellCode,
                        cellName = c.cells.CellName,
                        productCode = string.IsNullOrEmpty(c.storages.ProductCode) == true ? "" : c.storages.ProductCode,
                        productName = c.storages.Product == null ? "" : c.storages.Product.ProductName,
                        shelfCode = c.cells.ShelfCode,
                        unitCode = c.storages.Product == null ? "" : c.storages.Product.Unit.UnitCode,
                        unitName = c.storages.Product == null ? "" : c.storages.Product.Unit.UnitName,
                        maxQuantity = c.cells.MaxQuantity,
                        asdd1 = c.cells.CellCode.Substring(4, 2),//区
                        asdd2 = c.cells.CellCode.Substring(8, 2),//货架
                        asdd3 = c.cells.CellCode.Substring(14, 1),//层
                        asdd4 = c.cells.CellCode.Substring(11, 2),//列
                        Quantity = c.storages.Product == null ? 0 : c.storages.Quantity / c.storages.Product.Unit.Count,
                        EmptyQuantity = c.storages.Product == null ? c.cells.MaxQuantity : c.cells.MaxQuantity - c.storages.Quantity / c.storages.Product.Unit.Count,
                        IsActive = c.cells.IsActive == "1" ? "可用" : "不可用",
                        UpdateTime = c.storages.UpdateTime.ToString("yyyy-MM-dd")
                    }).OrderByDescending(c => c.asdd4).OrderByDescending(c => c.asdd3).OrderByDescending(c => c.asdd2).OrderByDescending(c => c.asdd1);

                    return Cell.ToArray();
                }
                else
                {
                   var s="".ToArray();
                    return s;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 入库可用货位选择
        public object GetInCellDetail(string type, string id)
        {
            try
            {
                IQueryable<Cell> cellQuery = CellRepository.GetQueryable();
                IQueryable<Storage> storageQuery = StorageRepository.GetQueryable().Where(s=>s.InFrozenQuantity==0&&s.OutFrozenQuantity==0&&s.IsActive=="1"&&s.LockTag=="");
                var storages = storageQuery;
                var cells = cellQuery.Where(s => s.MaxPalletQuantity == 0);
                if (type == "ware")
                {
                    cells = cellQuery.Where(c => c.Shelf.Area.Warehouse.WarehouseCode == id && c.MaxPalletQuantity==0);
                }
                else if (type == "area")
                {
                    cells = cellQuery.Where(c => c.Shelf.Area.AreaCode == id && c.MaxPalletQuantity == 0);
                }
                else if (type == "shelf")
                {
                    cells = cellQuery.Where(c => c.Shelf.ShelfCode == id && c.MaxPalletQuantity == 0);
                }
                else if (type == "cell")
                {
                    cells = cellQuery.Where(c => c.CellCode == id && c.MaxPalletQuantity == 0);
                }
                var sCells = cells.Join(storages, c => c.CellCode, s => s.CellCode, (c, s) => new { cells = c, storages = s }).ToArray();
                if (sCells.Count() > 0)
                {
                    var Cell = sCells.Select(c => new
                    {
                        cellCode = c.cells.CellCode,
                        cellName = c.cells.CellName,
                        productCode = string.IsNullOrEmpty(c.storages.ProductCode) == true ? "" : c.storages.ProductCode,
                        productName = c.storages.Product == null ? "" : c.storages.Product.ProductName,
                        shelfCode = c.cells.ShelfCode,
                        unitCode = c.storages.Product == null ? "" : c.storages.Product.Unit.UnitCode,
                        unitName = c.storages.Product == null ? "" : c.storages.Product.Unit.UnitName,
                        maxQuantity = c.cells.MaxQuantity,
                        asdd1 = c.cells.CellCode.Substring(4, 2),//区
                        asdd2 = c.cells.CellCode.Substring(8, 2),//货架
                        asdd3 = c.cells.CellCode.Substring(14, 1),//层
                        asdd4 = c.cells.CellCode.Substring(11, 2),//列
                        Quantity = c.storages.Product == null ? 0 : c.storages.Quantity / c.storages.Product.Unit.Count,
                        EmptyQuantity = c.storages.Product == null ? c.cells.MaxQuantity : c.cells.MaxQuantity - c.storages.Quantity / c.storages.Product.Unit.Count,
                        IsActive = c.cells.IsActive == "1" ? "可用" : "不可用",
                        UpdateTime = c.storages.UpdateTime.ToString("yyyy-MM-dd")
                    }).OrderByDescending(c => c.asdd4).OrderByDescending(c => c.asdd3).OrderByDescending(c => c.asdd2).OrderByDescending(c => c.asdd1);
                    var Cells = Cell.Where(c=>c.EmptyQuantity>0);
                    if (Cells.Count() > 0)
                    {
                        return Cells.ToArray();
                    }
                    else
                    {
                        var s = "".ToArray();
                        return s;
                    }
                }
                else
                {
                    var s = "".ToArray();
                    return s;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 出库可用货位选择
        public object GetOutCellDetail(string type, string id, string productCode)
        {
            try
            {
                IQueryable<Cell> cellQuery = CellRepository.GetQueryable();
                IQueryable<Storage> storageQuery = StorageRepository.GetQueryable().Where(s => s.InFrozenQuantity == 0 && s.OutFrozenQuantity == 0 && s.IsActive == "1" && s.LockTag == ""&&s.Quantity>0&&s.ProductCode.Contains(productCode));
                var storages = storageQuery;
                var cells = cellQuery.Where(s => s.MaxPalletQuantity == 0);
                if (type == "ware")
                {
                    cells = cellQuery.Where(c => c.Shelf.Area.Warehouse.WarehouseCode == id && c.MaxPalletQuantity == 0);
                }
                else if (type == "area")
                {
                    cells = cellQuery.Where(c => c.Shelf.Area.AreaCode == id && c.MaxPalletQuantity == 0);
                }
                else if (type == "shelf")
                {
                    cells = cellQuery.Where(c => c.Shelf.ShelfCode == id && c.MaxPalletQuantity == 0);
                }
                else if (type == "cell")
                {
                    cells = cellQuery.Where(c => c.CellCode == id && c.MaxPalletQuantity == 0);
                }
                var sCells = cells.Join(storages, c => c.CellCode, s => s.CellCode, (c, s) => new { cells = c, storages = s }).ToArray();
                if (sCells.Count() > 0)
                {
                    var Cell = sCells.Select(c => new
                    {
                        cellCode = c.cells.CellCode,
                        cellName = c.cells.CellName,
                        productCode = string.IsNullOrEmpty(c.storages.ProductCode) == true ? "" : c.storages.ProductCode,
                        productName = c.storages.Product == null ? "" : c.storages.Product.ProductName,
                        shelfCode = c.cells.ShelfCode,
                        unitCode = c.storages.Product == null ? "" : c.storages.Product.Unit.UnitCode,
                        unitName = c.storages.Product == null ? "" : c.storages.Product.Unit.UnitName,
                        maxQuantity = c.cells.MaxQuantity,
                        asdd1 = c.cells.CellCode.Substring(4, 2),//区
                        asdd2 = c.cells.CellCode.Substring(8, 2),//货架
                        asdd3 = c.cells.CellCode.Substring(14, 1),//层
                        asdd4 = c.cells.CellCode.Substring(11, 2),//列
                        Quantity = c.storages.Product == null ? 0 : c.storages.Quantity / c.storages.Product.Unit.Count,
                        EmptyQuantity = c.storages.Product == null ? c.cells.MaxQuantity : c.cells.MaxQuantity - c.storages.Quantity / c.storages.Product.Unit.Count,
                        IsActive = c.cells.IsActive == "1" ? "可用" : "不可用",
                        UpdateTime = c.storages.UpdateTime.ToString("yyyy-MM-dd")
                    }).OrderByDescending(c => c.asdd4).OrderByDescending(c => c.asdd3).OrderByDescending(c => c.asdd2).OrderByDescending(c => c.asdd1);
                    return Cell.ToArray();
                }
                else
                {
                    var s = "".ToArray();
                    return s;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public System.Data.DataTable GetCargospace(int page, int rows, string type, string id)
        {
            try
            {
                IQueryable<Storage> storageQuery = StorageRepository.GetQueryable();
                var storages = storageQuery.Where(s => s.StorageCode != null);
                if (type == "ware")
                {
                    storages = storages.Where(s => s.Cell.Shelf.Area.Warehouse.WarehouseCode == id);
                }
                else if (type == "area")
                {
                    storages = storageQuery.Where(s => s.Cell.Shelf.Area.AreaCode == id);
                }
                else if (type == "shelf")
                {
                    storages = storageQuery.Where(s => s.Cell.Shelf.ShelfCode == id);
                }
                else if (type == "cell")
                {
                    storages = storageQuery.Where(s => s.Cell.CellCode == id);
                }

                var temp = storages.OrderBy(s => s.Product.ProductName).Where(s => s.Quantity > 0);
                
                var Storage = temp.ToArray().ToArray().Select(s => new
                {
                    s.StorageCode,
                    s.Cell.CellCode,
                    s.Cell.CellName,
                    s.Product.ProductCode,
                    s.Product.ProductName,
                    s.Product.Unit.UnitCode,
                    s.Product.Unit.UnitName,
                    Quantity = s.Quantity / s.Product.Unit.Count,
                    IsActive = s.IsActive == "1" ? "可用" : "不可用",
                    StorageTime = s.StorageTime.ToString("yyyy-MM-dd"),
                    UpdateTime = s.UpdateTime.ToString("yyyy-MM-dd")
                });
                System.Data.DataTable dt = new System.Data.DataTable();
                dt.Columns.Add("货位编码", typeof(string));
                dt.Columns.Add("货位名称", typeof(string));
                dt.Columns.Add("商品编码", typeof(string));
                dt.Columns.Add("商品名称", typeof(string));
                dt.Columns.Add("单位编码", typeof(string));
                dt.Columns.Add("单位名称", typeof(string));
                dt.Columns.Add("数量", typeof(string));
                dt.Columns.Add("储存时间", typeof(string));
                dt.Columns.Add("更新时间", typeof(string));
                foreach (var item in Storage)
                {
                    dt.Rows.Add
                        (
                            item.CellCode,
                            item.CellName,
                            item.ProductCode,
                            item.ProductName,
                            item.UnitCode,
                            item.UnitName,
                            item.Quantity,
                            item.StorageTime,
                            item.UpdateTime
                        );
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
