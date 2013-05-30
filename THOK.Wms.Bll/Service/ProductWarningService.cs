using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using System.Data;

namespace THOK.Wms.Bll.Service
{
     public class ProductWarningService:ServiceBase<ProductWarning>,IProductWarningService
    {
         [Dependency]
         public IProductWarningRepository ProductWarningRepository { get; set; }
         [Dependency]
         public IStorageRepository StorageRepository { get; set; }
         [Dependency]
         public IProductRepository ProductRepository { get; set; }
         [Dependency]
         public IUnitRepository UnitRepository { get; set; }
         [Dependency]
         public IDailyBalanceRepository DailyBalanceRepository { get; set; }
         [Dependency]
         public ICellRepository CellRepository { get; set; }
         [Dependency]
         public IAreaRepository AreaRepository { get; set; }

         protected override Type LogPrefix
         {
             get { return this.GetType(); }
         }
        #region  卷烟预警设置的增，删，改，查方法
         public object GetDetail(int page, int rows, string productCode, decimal minLimited, decimal maxLimited, decimal assemblyTime)
         {
             IQueryable<ProductWarning> productWarningQuery = ProductWarningRepository.GetQueryable();
             var productWarning = productWarningQuery.Where(p => p.ProductCode.Contains(productCode)).OrderBy(p => p.ProductCode).Select(p => p).ToArray();
             if (productCode != "")
             {
                 productWarning = productWarning.Where(p => p.ProductCode == productCode).ToArray();
             }
             if (minLimited != 100000)
             {
                 productWarning = productWarning.Where(p => p.MinLimited <= minLimited).ToArray();
             }
             if (maxLimited != 100000)
             {
                 productWarning = productWarning.Where(p => p.MaxLimited >= maxLimited).ToArray();
             }
             if (assemblyTime != 3600)
             {
                 productWarning = productWarning.Where(p => p.AssemblyTime >= assemblyTime).ToArray();
             }
             var productWarn = productWarning
                 .Select(p => new
                 {
                     p.ProductCode,
                     ProductName = ProductRepository.GetQueryable().FirstOrDefault(q => q.ProductCode==p.ProductCode).ProductName,
                     p.UnitCode,
                     p.Unit.UnitName,
                     MinLimited = p.MinLimited / p.Unit.Count,
                     MaxLimited = p.MaxLimited / p.Unit.Count,
                     p.AssemblyTime,
                     p.Memo
                 });
             int total = productWarn.Count();
             productWarn = productWarn.Skip((page - 1) * rows).Take(rows);
             return new { total, rows = productWarn.ToArray() };
         }

         public bool Add(ProductWarning productWarning)
         {
             var unit = UnitRepository.GetQueryable().FirstOrDefault(u => u.UnitCode ==productWarning.UnitCode);
             var productWarn = new ProductWarning();
             productWarn.ProductCode = productWarning.ProductCode;
             productWarn.UnitCode = productWarning.UnitCode;
             productWarn.MinLimited = productWarning.MinLimited * unit.Count;
             productWarn.MaxLimited = productWarning.MaxLimited * unit.Count;
             productWarn.AssemblyTime = productWarning.AssemblyTime;
             productWarn.Memo = productWarning.Memo;

             ProductWarningRepository.Add(productWarn);
             ProductWarningRepository.SaveChanges();
             return true;
         }

         public bool Delete(string productCode)
         {
             var productWarn = ProductWarningRepository.GetQueryable()
                 .FirstOrDefault(p => p.ProductCode == productCode);

             if (productWarn != null)
             {
                 ProductWarningRepository.Delete(productWarn);
                 ProductWarningRepository.SaveChanges();
             }
             else 
             {
                 return false;
             }
             return true;
         }

         public bool Save(ProductWarning productWarning)
         {
             var productWarn = ProductWarningRepository.GetQueryable()
                 .FirstOrDefault(p => p.ProductCode == productWarning.ProductCode);
             var unit = UnitRepository.GetQueryable().FirstOrDefault(u => u.UnitCode == productWarning.UnitCode);
             productWarn.ProductCode = productWarning.ProductCode;
             productWarn.UnitCode = productWarning.UnitCode;
             productWarn.MinLimited = productWarning.MinLimited * unit.Count;
             productWarn.MaxLimited = productWarning.MaxLimited * unit.Count;
             productWarn.AssemblyTime = productWarning.AssemblyTime;
             productWarn.Memo = productWarning.Memo;

             ProductWarningRepository.SaveChanges();
             return true;
         }
        #endregion

        #region 产品短缺、超储查询
         public object GetQtyLimitsDetail(int page, int rows, string productCode, decimal minLimited, decimal maxLimited, string unitCode)
         {
             IQueryable<ProductWarning> ProductWarningQuery = ProductWarningRepository.GetQueryable();
             var unit = UnitRepository.GetQueryable().FirstOrDefault(u => u.UnitCode == unitCode);
             var ProductWarning = ProductWarningQuery.Where(p => p.ProductCode.Contains(productCode)).ToArray();
             if(productCode!="")
             {
                 ProductWarning = ProductWarning.Where(p=>p.ProductCode == productCode).ToArray();
             }
              if (minLimited != 100000)
             {
                 ProductWarning = ProductWarning.Where(p=>p.MinLimited<=minLimited * unit.Count).ToArray();
             }
             if (maxLimited != 100000)
             {
                 ProductWarning = ProductWarning.Where(p => p.MaxLimited >= maxLimited * unit.Count).ToArray();
             }
             var productWarning = ProductWarning.Select(t => new 
                 {
                     ProductCode =t.ProductCode,
                     ProductName = ProductRepository.GetQueryable().FirstOrDefault(q => q.ProductCode == t.ProductCode).ProductName,
                     UnitCode=t.UnitCode,
                     UnitName = t.Unit.UnitName,
                     Quantity = StorageRepository.GetQueryable().AsEnumerable().Where(s=>s.ProductCode==t.ProductCode).Sum(s=>s.Quantity)/t.Unit.Count,
                     MinLimited =t.MinLimited/t.Unit.Count,
                     MaxLimited = t.MaxLimited / t.Unit.Count
                 });
             productWarning = productWarning.Where(p=>p.Quantity>=p.MaxLimited ||p.Quantity<=p.MinLimited);
             int total = productWarning.Count();
             productWarning = productWarning.OrderBy(q => q.ProductCode);
             productWarning = productWarning.Skip((page - 1) * rows).Take(rows);
             return new { total, rows = productWarning.ToArray() };
         }
        #endregion

        #region 积压产品查询
         public object GetProductDetails(int page, int rows, string productCode, decimal assemblyTime)
         {
             IQueryable<Storage> StorageQuery = StorageRepository.GetQueryable();
             IQueryable<ProductWarning> ProductWarningQuery = ProductWarningRepository.GetQueryable();
             var ProductWarning = ProductWarningQuery.Where(p => p.ProductCode.Contains(productCode));
             var storage = StorageQuery.Where(s => s.ProductCode.Contains(productCode));
             var Storages = storage.Join(ProductWarning, s => s.ProductCode, p => p.ProductCode, (s, p) => new { storage = s, ProductWarning = p }).ToArray();

             Storages = Storages.Where(s => !string.IsNullOrEmpty(s.ProductWarning.AssemblyTime.ToString())).ToArray();
             if (Storages.Count() > 0)
             {
                 if (productCode != "")
                 {
                     Storages = Storages.Where(s => s.storage.ProductCode == productCode).ToArray();
                 }
                 if (assemblyTime != 360)
                 {
                     Storages = Storages.Where(s => s.ProductWarning.AssemblyTime >= assemblyTime).ToArray();
                 }
                 else
                 {
                     Storages = Storages.Where(s =>s.ProductWarning.AssemblyTime<=(DateTime.Now - s.storage.StorageTime).Days).ToArray();
                 }
             }
             var ProductTimeOut = Storages.AsEnumerable()
                 .Select(s => new
                 {
                     ProductCode = s.storage.ProductCode,
                     ProductName = s.storage.Product.ProductName,
                     cellCode = s.storage.CellCode,
                     cellName = s.storage.Cell.CellName,
                     quantity = s.storage.Quantity / s.storage.Product.Unit.Count,
                     storageTime = s.storage.StorageTime.ToString("yyyy-MM-dd hh:mm:ss"),
                     days = (DateTime.Now - s.storage.StorageTime).Days
                 });
             int total = ProductTimeOut.Count();
             ProductTimeOut = ProductTimeOut.OrderBy(p => p.ProductCode);
             ProductTimeOut = ProductTimeOut.Skip((page - 1) * rows).Take(rows);
             return new { total, rows = ProductTimeOut.ToArray() };
         }
        #endregion

        #region 预警提示信息
         public object GetWarningPrompt()
         {
             var ProductWarning= ProductWarningRepository.GetQueryable();
             var StorageQuery = StorageRepository.GetQueryable();
             var Storages = StorageQuery.Join(ProductWarning, s => s.ProductCode, p => p.ProductCode, (s, p) => new { storage = s, ProductWarning = p }).ToArray();
             var TimeOutWarning = Storages.Where(s => !string.IsNullOrEmpty(s.ProductWarning.AssemblyTime.ToString())).ToArray();
             var QuantityLimitsWarning = Storages.Where(s => !string.IsNullOrEmpty(s.ProductWarning.MinLimited.ToString()))
                                         .GroupBy(s=>s.storage.ProductCode)
                                         .Select(s=>new{
                                            productCode= s.Max(t=>t.storage.ProductCode),
                                            quantityTotal=s.Sum(t=>t.storage.Quantity),
                                            minlimits=s.Max(t=>t.ProductWarning.MinLimited)
                                         }).ToArray();
             var QuantityLimitsWarnings = Storages.Where(s =>! string.IsNullOrEmpty(s.ProductWarning.MaxLimited.ToString()))
                                        .GroupBy(s => s.storage.ProductCode)
                                        .Select(s => new
                                        {
                                            productCode = s.Max(t => t.storage.ProductCode),
                                            quantityTotal = s.Sum(t => t.storage.Quantity),
                                            maxlimits = s.Max(t => t.ProductWarning.MaxLimited)
                                        }).ToArray();
             if (TimeOutWarning.Count() > 0)
             {
                TimeOutWarning=TimeOutWarning.Where(t=>(DateTime.Now-t.storage.StorageTime).Days>=t.ProductWarning.AssemblyTime).ToArray();
             }
             if (QuantityLimitsWarning.Count() >= 0)
             {
                 QuantityLimitsWarning = QuantityLimitsWarning.Where(q=>q.quantityTotal<=q.minlimits).ToArray();
             }
             if (QuantityLimitsWarnings.Count() >= 0)
             {
                 QuantityLimitsWarnings = QuantityLimitsWarnings.Where(q => q.quantityTotal >= q.maxlimits).ToArray();
             }
             int total=TimeOutWarning.Count() + QuantityLimitsWarning.Count() + QuantityLimitsWarnings.Count();
             return total;
         }
         #endregion

        #region 库存分析数据
         public object GetStorageByTime()
         {
             IQueryable<DailyBalance> EndQuantity = DailyBalanceRepository.GetQueryable();
             //标准时间戳：621355968000000000
             var storageQuantity = EndQuantity.OrderBy(e=>e.SettleDate).GroupBy(e=>e.SettleDate).ToArray().Select(e => new 
             {
                 TimeInter = decimal.Parse(((e.Max(m => m.SettleDate).ToUniversalTime().Ticks - 621355680000000000) / 10000).ToString()),
                 TotalQuantity=e.Sum(s=>s.Ending/10000)
             });
             return storageQuantity;
         }
        #endregion

        #region  库区占有率分析数据
         public object GetCellInfo()
         {
             IQueryable<Area> AreaQuery = AreaRepository.GetQueryable();
             IQueryable<Storage> StorageQuery = StorageRepository.GetQueryable();
             var storageQuantity = StorageQuery.Join(AreaQuery, s => s.Cell.AreaCode, a => a.AreaCode, (s, a) => new { storage = s, area = a });
             var total = StorageQuery.Where(s => s.Quantity > 0).Sum(s => s.Quantity);
             var storage = storageQuantity.GroupBy(s => s.area.AreaCode).Where(s => s.Sum(t => t.storage.Quantity) > 0)
                                .Select(s =>new
                                {
                                    areaName=s.Max(t=>t.area.AreaName),
                                    totalQuantity=Decimal.Round(s.Sum(t=>t.storage.Quantity)/total,5)*100
                                }).ToArray();
             return storage;
         }
        #endregion

        #region 货位分析数据
         public object GetCell()
         {
             DataTable dt =new DataTable();
             IQueryable<Area> AreaQuery = AreaRepository.GetQueryable();
             IQueryable<Storage> StorageQuery = StorageRepository.GetQueryable();
             IQueryable<Cell> CellQuery = CellRepository.GetQueryable();
             var storageQuantity = StorageQuery.Join(AreaQuery, s => s.Cell.AreaCode, a => a.AreaCode, (s, a) => new { storage = s, area = a });
             var CellInfo = AreaQuery.Join(CellQuery, s => s.AreaCode, c => c.AreaCode, (s, c) => new { Storage = s, cell = c });
             var CellInfos =CellInfo.GroupBy(s=>s.cell.AreaCode)
                                .Select(s => new
                                {
                                    areaName =s.Max(t=>t.cell.Area.AreaName),
                                    enableQty=storageQuantity.AsEnumerable().Where(t=>t.area.AreaCode==s.Max(m=>m.cell.AreaCode)).Count(t=>t.storage.Quantity>0 ||t.storage.InFrozenQuantity>0 ||t.storage.OutFrozenQuantity>0),
                                    totalQty =s.Where(t => t.cell.IsActive == "0" && t.cell.MaxPalletQuantity >0).Sum(t=>t.cell.MaxPalletQuantity)>0?s.Where(t => t.cell.IsActive == "0" && t.cell.MaxPalletQuantity >0).Sum(t=>t.cell.MaxPalletQuantity):0+s.Where(t => t.cell.IsActive == "0" && t.cell.MaxPalletQuantity == 0).Count(t => t.cell.IsActive == "0") ,
                                    totalQtys = s.Where(t => t.cell.IsActive == "1" && t.cell.MaxPalletQuantity > 0).Sum(t => t.cell.MaxPalletQuantity) > 0 ? s.Where(t => t.cell.IsActive == "1" && t.cell.MaxPalletQuantity > 0).Sum(t => t.cell.MaxPalletQuantity) : 0 + s.Where(t => t.cell.IsActive == "1" && t.cell.MaxPalletQuantity == 0).Count(t => t.cell.IsActive == "1")
                                });
             return CellInfos;
         }
        #endregion

         public System.Data.DataTable GetProductWarning(int page, int rows, string productCode, decimal minLimited, decimal maxLimited, decimal assemblyTime)
         {
             System.Data.DataTable dt = new System.Data.DataTable();
             IQueryable<ProductWarning> productWarningQuery = ProductWarningRepository.GetQueryable();
             var productWarning = productWarningQuery.Where(p => p.ProductCode.Contains(productCode)).OrderBy(p => p.ProductCode).Select(p => p).ToArray();
             if (productCode != "")
             {
                 productWarning = productWarning.Where(p => p.ProductCode == productCode).ToArray();
             }
             if (minLimited != 100000)
             {
                 productWarning = productWarning.Where(p => p.MinLimited <= minLimited).ToArray();
             }
             if (maxLimited != 100000)
             {
                 productWarning = productWarning.Where(p => p.MaxLimited >= maxLimited).ToArray();
             }
             if (assemblyTime != 3600)
             {
                 productWarning = productWarning.Where(p => p.AssemblyTime >= assemblyTime).ToArray();
             }
             var productWarn = productWarning
                 .Select(p => new
                 {
                     p.ProductCode,
                     ProductName = ProductRepository.GetQueryable().FirstOrDefault(q => q.ProductCode == p.ProductCode).ProductName,
                     p.UnitCode,
                     p.Unit.UnitName,
                     MinLimited = p.MinLimited / p.Unit.Count,
                     MaxLimited = p.MaxLimited / p.Unit.Count,
                     p.AssemblyTime,
                     p.Memo
                 });
             dt.Columns.Add("商品编码", typeof(string));
             dt.Columns.Add("商品名称", typeof(string));
             dt.Columns.Add("单位编码", typeof(string));
             dt.Columns.Add("单位名称", typeof(string));
             dt.Columns.Add("数量下限", typeof(string));
             dt.Columns.Add("数量上限", typeof(string));
             dt.Columns.Add("积压时间", typeof(string));
             dt.Columns.Add("备注", typeof(string));
             foreach (var p in productWarn)
             {
                 dt.Rows.Add
                     (
                        p.ProductCode,
                        p.ProductName,
                        p.UnitCode,
                        p.UnitName,
                        p.MinLimited,
                        p.MaxLimited,
                        p.AssemblyTime,
                        p.Memo
                     );
             }
             return dt;
         }

         public System.Data.DataTable GetQuantityLimitsDetail(int page, int rows, string productCode, decimal minLimited, decimal maxLimited, string unitCode)
         {
             System.Data.DataTable dt=new System.Data.DataTable();
             IQueryable<ProductWarning> ProductWarningQuery = ProductWarningRepository.GetQueryable();
             var unit = UnitRepository.GetQueryable().FirstOrDefault(u => u.UnitCode == unitCode);
             var ProductWarning = ProductWarningQuery.Where(p => p.ProductCode.Contains(productCode)).ToArray();
             if (productCode != "")
             {
                 ProductWarning = ProductWarning.Where(p => p.ProductCode == productCode).ToArray();
             }
             if (minLimited != 100000)
             {
                 ProductWarning = ProductWarning.Where(p => p.MinLimited <= minLimited * unit.Count).ToArray();
             }
             if (maxLimited != 100000)
             {
                 ProductWarning = ProductWarning.Where(p => p.MaxLimited >= maxLimited * unit.Count).ToArray();
             }
             var productWarning = ProductWarning.Select(t => new
             {
                 ProductCode = t.ProductCode,
                 ProductName = ProductRepository.GetQueryable().FirstOrDefault(q => q.ProductCode == t.ProductCode).ProductName,
                 UnitCode = t.UnitCode,
                 UnitName = t.Unit.UnitName,
                 Quantity = StorageRepository.GetQueryable().AsEnumerable().Where(s => s.ProductCode == t.ProductCode).Sum(s => s.Quantity) / t.Unit.Count,
                 MinLimited = t.MinLimited / t.Unit.Count,
                 MaxLimited = t.MaxLimited / t.Unit.Count
             });
             productWarning = productWarning.Where(p => p.Quantity >= p.MaxLimited || p.Quantity <= p.MinLimited);
             dt.Columns.Add("商品代码", typeof(string));
             dt.Columns.Add("商品名称", typeof(string));
             dt.Columns.Add("单位编码", typeof(string));
             dt.Columns.Add("单位名称", typeof(string));
             dt.Columns.Add("商品总数量", typeof(string));
             dt.Columns.Add("商品数量上限", typeof(string));
             dt.Columns.Add("商品数量下限", typeof(string));
             foreach (var p in productWarning)
             {
                 dt.Rows.Add
                     (
                        p.ProductCode,
                        p.ProductName,
                        p.UnitCode,
                        p.UnitName,
                        p.Quantity,
                        p.MaxLimited,
                        p.MinLimited
                     );
             }
             return dt;
         }

         public System.Data.DataTable GetProductTimeOut(int page, int rows, string productCode, decimal assemblyTime)
         {
             System.Data.DataTable dt = new System.Data.DataTable();
             IQueryable<Storage> StorageQuery = StorageRepository.GetQueryable();
             IQueryable<ProductWarning> ProductWarningQuery = ProductWarningRepository.GetQueryable();
             var ProductWarning = ProductWarningQuery.Where(p => p.ProductCode.Contains(productCode));
             var storage = StorageQuery.Where(s => s.ProductCode.Contains(productCode));
             var Storages = storage.Join(ProductWarning, s => s.ProductCode, p => p.ProductCode, (s, p) => new { storage = s, ProductWarning = p }).ToArray();

             Storages = Storages.Where(s => !string.IsNullOrEmpty(s.ProductWarning.AssemblyTime.ToString())).ToArray();
             if (Storages.Count() > 0)
             {
                 if (productCode != "")
                 {
                     Storages = Storages.Where(s => s.storage.ProductCode == productCode).ToArray();
                 }
                 if (assemblyTime != 360)
                 {
                     Storages = Storages.Where(s => s.ProductWarning.AssemblyTime >= assemblyTime).ToArray();
                 }
                 else
                 {
                     Storages = Storages.Where(s => s.ProductWarning.AssemblyTime <= (DateTime.Now - s.storage.StorageTime).Days).ToArray();
                 }
             }
             var ProductTimeOut = Storages.AsEnumerable()
                 .Select(s => new
                 {
                     ProductCode = s.storage.ProductCode,
                     ProductName = s.storage.Product.ProductName,
                     cellCode = s.storage.CellCode,
                     cellName = s.storage.Cell.CellName,
                     quantity = s.storage.Quantity / s.storage.Product.Unit.Count,
                     storageTime = s.storage.StorageTime.ToString("yyyy-MM-dd hh:mm:ss"),
                     days = (DateTime.Now - s.storage.StorageTime).Days
                 });
             dt.Columns.Add("商品代码", typeof(string));
             dt.Columns.Add("商品名称", typeof(string));
             dt.Columns.Add("货位编码", typeof(string));
             dt.Columns.Add("货位名称", typeof(string));
             dt.Columns.Add("数量(件)", typeof(string));
             dt.Columns.Add("入库时间", typeof(string));
             dt.Columns.Add("积压时间(天)", typeof(string));
             foreach (var p in ProductTimeOut)
             {
                 dt.Rows.Add
                     (
                        p.ProductCode,
                        p.ProductName,
                        p.cellCode,
                        p.cellName,
                        p.quantity,
                        p.storageTime,
                        p.days
                     );
             }
             return dt;
         }
    }
}
