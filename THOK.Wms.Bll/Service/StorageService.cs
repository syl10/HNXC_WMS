using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using System.Data;
using THOK.WMS.Upload.Bll;

namespace THOK.Wms.Bll.Service
{
    public class StorageService : ServiceBase<Storage>, IStorageService
    {
        [Dependency]
        public IStorageRepository StorageRepository { get; set; }

        [Dependency]
        public ICMDCellRepository CellRepository { get; set; }

        [Dependency]
        public IInBillAllotRepository InBillAllotRepository { get; set; }

        [Dependency]
        public IOutBillAllotRepository OutBillAllotRepository { get; set; }

        [Dependency]
        public IMoveBillDetailRepository MoveBillDetailRepository { get; set; }

        UploadBll upload = new UploadBll();

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IStorageService 成员

        /// <summary>
        /// 根据类型获和id获取存储表的数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="type">类型</param>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public object GetDetails(int page, int rows, string type, string id)
        {
            IQueryable<Storage> storageQuery = StorageRepository.GetQueryable();
            var storages = storageQuery.OrderBy(s => s.StorageCode).Where(s => s.StorageCode != null);
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

            var temp = storages.Where(s=>s.Quantity>0).OrderBy(s=>s.CellCode).Select(s => s);

            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);

            var tmp = temp.ToArray().AsEnumerable().Select(s => new
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
            return new { total, rows = tmp.ToArray() };
        }

        /// <summary>
        /// 根据类型、id、inOrOut获取存储表的数据,-移库单使用
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="type">类型</param>
        /// <param name="id">ID</param>
        /// <param name="inOrOut">移入还是移出</param>
        /// <param name="productCode">产品代码</param>
        /// <returns></returns>
        public object GetMoveStorgeDetails(int page, int rows, string type, string id,string inOrOut,string productCode)
        {
            IQueryable<Storage> storageQuery = StorageRepository.GetQueryable();
            var storages = storageQuery.OrderBy(s => s.StorageCode).Where(s => s.StorageCode != null);
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
            //传入的参数为out时查询的是移出货位的存储信息
            if (inOrOut == "out")
            {
                storages = storages.Where(s => (s.Quantity - s.OutFrozenQuantity) > 0 && string.IsNullOrEmpty(s.Cell.LockTag));
            }
            else if(inOrOut=="in")//传入的参数为in时查询的是移入货位的存储信息
            {
                storages = storages.Where(s => s.Quantity == 0
                    || (s.Cell.IsSingle == "1" && s.ProductCode == productCode 
                       && ((s.Cell.MaxQuantity * s.Product.Unit.Count) - s.InFrozenQuantity - s.Quantity) > 0)
                    || (s.Cell.IsSingle == "0" && string.IsNullOrEmpty(s.Cell.LockTag)));
            }
            else if (inOrOut == "stockOut")
            {
                storages = storages.Where(s => (s.Quantity - s.OutFrozenQuantity) > 0
                                          && string.IsNullOrEmpty(s.Cell.LockTag)
                                          && s.ProductCode == productCode);
            }

            if (!storages.Any())
            {
                return null;
            }
            var temp = storages.OrderBy(i=>i.CellCode).Select(s =>s);

            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);

            var tmp=temp.ToArray().AsEnumerable().Select(s => new
                {
                    s.StorageCode,
                    s.Cell.CellCode,
                    s.Cell.CellName,
                    ProductCode = s.Product == null ? "" : s.Product.ProductCode,
                    ProductName = s.Product == null ? "" : s.Product.ProductName,
                    UnitCode = s.Product == null ? "" : s.Product.Unit.UnitCode,
                    UnitName = s.Product == null ? "" : s.Product.Unit.UnitName,
                    Price = s.Product == null ? 0 : s.Product.CostPrice,
                    Quantity = s.Product == null ? 0 : s.Quantity / s.Product.Unit.Count,
                    IsActive = s.IsActive == "1" ? "可用" : "不可用",
                    StorageTime = s.StorageTime.ToString("yyyy-MM-dd"),
                    UpdateTime = s.UpdateTime.ToString("yyyy-MM-dd")
                });
            return new { total, rows = tmp.ToArray() };
        }
        #endregion

        #region IStorageService 成员


        public object GetMoveInStorgeDetails(int page, int rows, string type, string id, string cellCode, string productCode)
        {
            IQueryable<Storage> storageQuery = StorageRepository.GetQueryable();
            var storages = storageQuery.OrderBy(s => s.StorageCode).Where(s => s.StorageCode != null);
            if (type == "ware")
            {
                storages = storages.Where(s => s.Cell.Shelf.Area.Warehouse.WarehouseCode == id&&s.CellCode!=cellCode);
            }
            else if (type == "area")
            {
                storages = storageQuery.Where(s => s.Cell.Shelf.Area.AreaCode == id && s.CellCode != cellCode);
            }
            else if (type == "shelf")
            {
                storages = storageQuery.Where(s => s.Cell.Shelf.ShelfCode == id && s.CellCode != cellCode);
            }
            else if (type == "cell")
            {
                storages = storageQuery.Where(s => s.Cell.CellCode == id && s.CellCode != cellCode);
            }

            var storage = storages.Where(s => s.Quantity == 0
                || (s.Cell.IsSingle == "1" && s.ProductCode == productCode
                   && ((s.Cell.MaxQuantity * s.Product.Unit.Count) - s.InFrozenQuantity - s.Quantity) > 0)
                || (s.Cell.IsSingle == "0" && string.IsNullOrEmpty(s.Cell.LockTag)))
                .GroupBy(s => new { s.Cell })
                .Select(s => new
                {
                    s.Key.Cell.CellCode,
                    s.Key.Cell.CellName,
                    //s.Key.Product.ProductCode,
                    //s.Key.Product.ProductName,
                    //s.Key.Unit.UnitCode,
                    //s.Key.Unit.UnitName,
                    //Quantity = s.Sum(q => (q.Cell.MaxQuantity * q.Product.Unit.Count - q.Quantity - q.InFrozenQuantity) / q.Product.Unit.Count),
                    //storagecode = s.Key.storage.StorageCode
                });

            if (!storages.Any())
            {
                return null;
            }

            int total = storage.Count();
            storage = storage.OrderBy(s=>s.CellCode).Skip((page - 1) * rows).Take(rows);
            return new { total, rows = storage.ToArray() };
        }

        #endregion

        #region 上报仓库库存表
        public bool uploadStorage()
        {
            try
            {
                IQueryable<Storage> storageQuery = StorageRepository.GetQueryable();
                var storage = storageQuery.Where(s=>s.Quantity>0).GroupBy(s=>s.ProductCode).Select(s => new
                {
                    productCode = s.Max(t=>t.ProductCode),
                    areaType = s.Max(t=>t.Cell.Area.AreaType),
                    warehouseCode = s.Max(t=>t.Cell.WarehouseCode),
                    storageTime = s.Max(t=>t.StorageTime),
                    quantity = s.Sum(t=>t.Quantity / (s.Max(r=>r.Product.UnitList.Quantity02) * s.Max(r=>r.Product.UnitList.Quantity03)))
                });
                DataSet ds = this.GenerateEmptyTables();
                for (int i=0;i<storage.ToArray().Length;i++)
                {
                    DataRow inbrddr = ds.Tables["wms_storage"].NewRow();
                    var p = storage.ToArray();
                    if (p[i].areaType == "6")
                    {
                        inbrddr["area_type"] = "0902";
                    }
                    else if (p[i].areaType == "7")
                    {
                        inbrddr["area_type"] = "0903";
                    }
                    else
                    {
                        inbrddr["area_type"] = "0901";
                    }
                    inbrddr["cell_code"] = "10002";
                    inbrddr["product_code"] = p[i].productCode;
                    inbrddr["brand_batch"] = p[i].storageTime;
                    inbrddr["dist_ctr_code"] = p[i].warehouseCode;
                    inbrddr["quantity"] = p[i].quantity;
                    ds.Tables["wms_storage"].Rows.Add(inbrddr);
                }
                upload.QueryStoreStock(ds);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 上报业务库存表
        public bool uploadBusiStorage()
        {
            try
            {
                IQueryable<Storage> storageQuery = StorageRepository.GetQueryable();
                var storage = storageQuery.GroupBy(s=>s.ProductCode).Select(s => new
                {
                    productCode = s.Max(t=>t.ProductCode),
                    warehouseCode =s.Max(t=>t.Cell.WarehouseCode),
                    quantity = s.Sum(t=>t.Quantity /(s.Max(r=>r.Product.UnitList.Quantity02) * s.Max(r=>r.Product.UnitList.Quantity03)))
                });
                DataSet ds = this.GenerateEmptyTable();
                foreach (var p in storage.Where(s=>s.quantity>0))
                {
                    DataRow inbrddr = ds.Tables["wms_busistorage"].NewRow();
                    inbrddr["ORG_CODE"] = p.warehouseCode;
                    inbrddr["BRAND_CODE"] = p.productCode;
                    inbrddr["DIST_CTR_CODE"] = p.warehouseCode;
                    inbrddr["QUANTITY"] = p.quantity;
                    ds.Tables["wms_busistorage"].Rows.Add(inbrddr);
                }
                upload.QueryBusiStock(ds);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 创建一个空的仓库库存表
        private DataSet GenerateEmptyTables()
        {
            DataSet ds = new DataSet();
            DataTable inbrtable = ds.Tables.Add("wms_storage");
            inbrtable.Columns.Add("cell_code");
            inbrtable.Columns.Add("product_code");
            inbrtable.Columns.Add("area_type");
            inbrtable.Columns.Add("brand_batch");
            inbrtable.Columns.Add("dist_ctr_code");
            inbrtable.Columns.Add("quantity");
            return ds;
        }
        #endregion

        #region 创建一个空的业务库存表
        private DataSet GenerateEmptyTable()
        {
            DataSet ds = new DataSet();
            DataTable inbrtable = ds.Tables.Add("wms_busistorage");
            inbrtable.Columns.Add("ORG_CODE");
            inbrtable.Columns.Add("BRAND_CODE");
            inbrtable.Columns.Add("DIST_CTR_CODE");
            inbrtable.Columns.Add("QUANTITY");
            return ds;
        }
        #endregion
    }
}
