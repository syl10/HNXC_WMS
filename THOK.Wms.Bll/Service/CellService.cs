using System;
using System.Collections.Generic;
using System.Linq;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.Bll.Models;
using Entities.Extensions;
using THOK.WMS.Upload.Bll;
using System.Data;
namespace THOK.Wms.Bll.Service
{
    public class CellService : ServiceBase<Cell>, ICellService
    {

        [Dependency]
        public IWarehouseRepository WarehouseRepository { get; set; }

        [Dependency]
        public IAreaRepository AreaRepository { get; set; }

        [Dependency]
        public IShelfRepository ShelfRepository { get; set; }

        [Dependency]
        public ICellRepository CellRepository { get; set; }

        [Dependency]
        public IProductRepository ProductRepository { get; set; }

        [Dependency]
        public IStorageRepository StorageRepository { get; set; }

        UploadBll upload = new UploadBll();

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, string cellCode)
        {
            IQueryable<Cell> cellQuery = CellRepository.GetQueryable();
            var cell = cellQuery.OrderBy(b => b.CellCode).AsEnumerable().Select(b => new { b.CellCode, b.CellName, b.CellType, b.ShortName, b.Rfid, b.Layer, b.Col, b.ImgX, b.ImgY, b.IsSingle, b.MaxQuantity, b.Description, b.Warehouse.WarehouseName, b.Warehouse.WarehouseCode, b.Area.AreaCode, b.Area.AreaName, b.Shelf.ShelfCode, b.Shelf.ShelfName, IsActive = b.IsActive == "1" ? "可用" : "不可用", UpdateTime = b.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss") });
            int total = cell.Count();
            cell = cell.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = cell.ToArray() };
        }
        public object GetDetail(int page, int rows, string type, string id)
        {
            var warehouses = WarehouseRepository.GetQueryable();
            var areas = AreaRepository.GetQueryable();
            var shelfs = ShelfRepository.GetQueryable();
            var cells = CellRepository.GetQueryable();
            HashSet<WareTree> wareSet = new HashSet<WareTree>();
            HashSet<WareTree> areaSet = new HashSet<WareTree>();
            HashSet<WareTree> shelfSet = new HashSet<WareTree>();
            HashSet<WareTree> cellSet = new HashSet<WareTree>();
            var set = wareSet;
            if (type == "area")
            {
                areas = areas.Where(a => a.AreaCode == id).OrderBy(a => a.AreaCode).Select(a => a);
                foreach (var area in areas)//库区
                {
                    WareTree areaTree = new WareTree();
                    areaTree.Code = area.AreaCode;
                    areaTree.Name = "库区：" + area.AreaName;
                    areaTree.AreaCode = area.AreaCode;
                    areaTree.AreaName = area.AreaName;
                    areaTree.WarehouseCode = area.Warehouse.WarehouseCode;
                    areaTree.WarehouseName = area.Warehouse.WarehouseName;
                    areaTree.Type = area.AreaType;
                    areaTree.Description = area.Description;
                    areaTree.IsActive = area.IsActive == "1" ? "可用" : "不可用";
                    areaTree.UpdateTime = area.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    areaTree.ShortName = area.ShortName;
                    areaTree.AllotInOrder = area.AllotInOrder;
                    areaTree.AllotOutOrder = area.AllotOutOrder;
                    areaTree.attributes = "area";
                    areaSet.Add(areaTree);
                }
                set = areaSet;
            }
            else if (type == "shelf")
            {
                shelfs = shelfs.Where(a => a.ShelfCode== id).OrderBy(a => a.ShelfCode).Select(a => a);
                cells = CellRepository.GetQueryable().Where(c => c.Shelf.ShelfCode == id)
                                                    .OrderBy(c => c.CellCode).Select(c => c);
                foreach (var shelf in shelfs)//货架
                {
                    WareTree shelfTree = new WareTree();
                    shelfTree.Code = shelf.ShelfCode;
                    shelfTree.Name = "货架：" + shelf.ShelfName;
                    shelfTree.ShelfCode = shelf.ShelfCode;
                    shelfTree.ShelfName = shelf.ShelfName;
                    shelfTree.WarehouseCode = shelf.Warehouse.WarehouseCode;
                    shelfTree.WarehouseName = shelf.Warehouse.WarehouseName;
                    shelfTree.AreaCode = shelf.Area.AreaCode;
                    shelfTree.AreaName = shelf.Area.AreaName;
                    shelfTree.Type = shelf.ShelfType;
                    shelfTree.Description = shelf.Description;
                    shelfTree.IsActive = shelf.IsActive == "1" ? "可用" : "不可用";
                    shelfTree.UpdateTime = shelf.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    shelfTree.ShortName = shelf.ShortName;
                    shelfTree.attributes = "shelf";
                    foreach (var cell in cells)//货位
                    {
                        WareTree cellTree = new WareTree();
                        cellTree.Code = cell.CellCode;
                        cellTree.Name = "货位：" + cell.CellName;
                        cellTree.CellCode = cell.CellCode;
                        cellTree.CellName = cell.CellName;
                        cellTree.WarehouseCode = cell.Warehouse.WarehouseCode;
                        cellTree.WarehouseName = cell.Warehouse.WarehouseName;
                        cellTree.AreaCode = cell.Area.AreaCode;
                        cellTree.AreaName = cell.Area.AreaName;
                        cellTree.ShelfCode = cell.Shelf.ShelfCode;
                        cellTree.ShelfName = cell.Shelf.ShelfName;
                        cellTree.Type = cell.CellType;
                        cellTree.Description = cell.Description;
                        cellTree.IsActive = cell.IsActive == "1" ? "可用" : "不可用";
                        cellTree.UpdateTime = cell.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                        cellTree.ShortName = cell.ShortName;
                        cellTree.Layer = cell.Layer;
                        cellTree.MaxQuantity = cell.MaxQuantity;
                        cellTree.ProductName = cell.Product == null ? string.Empty : cell.Product.ProductName;
                        cellTree.attributes = "cell";
                        cellSet.Add(cellTree);
                    }
                    shelfSet.Add(shelfTree);
                }
                set = cellSet;
            }
            else if (type == "cell")
            {
                cells = cells.Where(a => a.CellCode == id).OrderBy(a => a.CellCode).Select(a => a);
                foreach (var cell in cells)//货位
                {
                    WareTree cellTree = new WareTree();
                    cellTree.Code = cell.CellCode;
                    cellTree.Name = "货位：" + cell.CellName;
                    cellTree.CellCode = cell.CellCode;
                    cellTree.CellName = cell.CellName;
                    cellTree.WarehouseCode = cell.Warehouse.WarehouseCode;
                    cellTree.WarehouseName = cell.Warehouse.WarehouseName;
                    cellTree.AreaCode = cell.Area.AreaCode;
                    cellTree.AreaName = cell.Area.AreaName;
                    cellTree.ShelfCode = cell.Shelf.ShelfCode;
                    cellTree.ShelfName = cell.Shelf.ShelfName;
                    cellTree.Type = cell.CellType;
                    cellTree.Description = cell.Description;
                    cellTree.IsActive = cell.IsActive == "1" ? "可用" : "不可用";
                    cellTree.UpdateTime = cell.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    cellTree.ShortName = cell.ShortName;
                    cellTree.Layer = cell.Layer;
                    cellTree.MaxQuantity = cell.MaxQuantity;
                    cellTree.ProductName = cell.Product == null ? string.Empty : cell.Product.ProductName;
                    cellTree.attributes = "cell";
                    cellSet.Add(cellTree);
                }
                set = cellSet;
            }
            else
            {
                if (type == null || type == string.Empty)
                {
                    warehouses = warehouses.Where(w => w.WarehouseCode == "0101").OrderBy(w => w.WarehouseCode).Select(w => w);
                }
                else if (type == "ware")
                {
                    warehouses = warehouses.Where(w => w.WarehouseCode == id).OrderBy(w => w.WarehouseCode).Select(w => w);
                }
                foreach (var warehouse in warehouses)//仓库
                {
                    WareTree wareTree = new WareTree();
                    wareTree.Code = warehouse.WarehouseCode;
                    wareTree.Name = "仓库：" + warehouse.WarehouseName;
                    wareTree.WarehouseCode = warehouse.WarehouseCode;
                    wareTree.WarehouseName = warehouse.WarehouseName;
                    wareTree.Type = warehouse.WarehouseType;
                    wareTree.Description = warehouse.Description;
                    wareTree.IsActive = warehouse.IsActive == "1" ? "可用" : "不可用";
                    wareTree.UpdateTime = warehouse.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    wareTree.ShortName = warehouse.ShortName;
                    wareTree.attributes = "ware";
                    warehouses = warehouses.Where(w => w.WarehouseCode == id);
                    wareSet.Add(wareTree);
                }
                set = wareSet;
            }
            int total = set.Count();
            var sets = set.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = sets.ToArray() };
        }
        public bool Add(Cell cell, out string errorInfo)
        {
            errorInfo = string.Empty;
            var cellAdd = new Cell();
            var warehouse = WarehouseRepository.GetQueryable().FirstOrDefault(w => w.WarehouseCode == cell.WarehouseCode);
            var area = AreaRepository.GetQueryable().FirstOrDefault(a => a.AreaCode == cell.AreaCode);
            var shelf = ShelfRepository.GetQueryable().FirstOrDefault(s => s.ShelfCode == cell.ShelfCode);
            var product = ProductRepository.GetQueryable().FirstOrDefault(p => p.ProductCode == cell.DefaultProductCode);
            string cellCodeStr = cell.CellCode + "-" + cell.Layer;
            var cellCode = CellRepository.GetQueryable().FirstOrDefault(c => c.CellCode == cellCodeStr);
            if (cellCode == null)
            {
                cellAdd.CellCode = cell.CellCode + "-" + cell.Layer;
                cellAdd.CellName = cell.CellName;
                cellAdd.ShortName = cell.ShortName;
                cellAdd.CellType = cell.CellType;
                cellAdd.Layer = cell.Layer;
                cellAdd.Col = cell.Col;
                cellAdd.ImgX = cell.ImgX;
                cellAdd.ImgY = cell.ImgY;
                cellAdd.Rfid = cell.Rfid;
                cellAdd.Warehouse = warehouse;
                cellAdd.Area = area;
                cellAdd.Shelf = shelf;
                cellAdd.Product = product;
                cellAdd.MaxQuantity = cell.MaxQuantity;
                cellAdd.IsSingle = cell.IsSingle;
                cellAdd.Description = cell.Description;
                cellAdd.IsActive = cell.IsActive;
                cellAdd.UpdateTime = DateTime.Now;

                CellRepository.Add(cellAdd);
                CellRepository.SaveChanges();
                //仓储属性上报
                //DataSet ds = Insert(cell);
                //upload.UploadCell(ds);
                return true;
            }
            else
            {
                errorInfo = "添加失败!原因：所添加数据已存在或者选择的层号已存在，请重选！";
                return false;
            }
        }

        public bool Delete(string cellCode)
        {
            var cell = CellRepository.GetQueryable()
                .FirstOrDefault(c => c.CellCode == cellCode);
            if (cell != null)
            {
                CellRepository.Delete(cell);
                CellRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(Cell cell)
        {
            var cellSave = CellRepository.GetQueryable().FirstOrDefault(c => c.CellCode == cell.CellCode);
            var warehouse = WarehouseRepository.GetQueryable().FirstOrDefault(w => w.WarehouseCode == cell.WarehouseCode);
            var area = AreaRepository.GetQueryable().FirstOrDefault(a => a.AreaCode == cell.AreaCode);
            var shelf = ShelfRepository.GetQueryable().FirstOrDefault(s => s.ShelfCode == cell.ShelfCode);
            var product = ProductRepository.GetQueryable().FirstOrDefault(p => p.ProductCode == cell.DefaultProductCode);
            cellSave.CellCode = cellSave.CellCode;
            cellSave.CellName = cell.CellName;
            cellSave.ShortName = cell.ShortName;
            cellSave.CellType = cell.CellType;
            cellSave.Layer = cell.Layer;
            cellSave.Col = cell.Col;
            cellSave.ImgX = cell.ImgX;
            cellSave.ImgY = cell.ImgY;
            cellSave.Rfid = cell.Rfid;
            cellSave.Warehouse = warehouse;
            cellSave.Area = area;
            cellSave.Shelf = shelf;
            cellSave.Product = product;
            cellSave.MaxQuantity = cell.MaxQuantity;
            cellSave.IsSingle = cell.IsSingle;
            cellSave.Description = cell.Description;
            cellSave.IsActive = cell.IsActive;
            cellSave.UpdateTime = DateTime.Now;

            CellRepository.SaveChanges();
            //仓储属性上报
            //DataSet ds = Insert(cell);
            //upload.UploadCell(ds);
            return true;
        }

        /// <summary>修改货位</summary>
        public bool SaveCell(string wareCodes, string areaCodes, string shelfCodes, string cellCodes, string defaultProductCode, string editType)
        {
            try
            {
                IQueryable<Cell> cellQuery = CellRepository.GetQueryable();

                if (wareCodes != string.Empty && wareCodes != null)
                {
                    wareCodes = wareCodes.Substring(0, wareCodes.Length - 1);
                }
                else if (areaCodes != string.Empty && areaCodes != null)
                {
                    areaCodes = areaCodes.Substring(0, areaCodes.Length - 1);
                }
                else if (shelfCodes != string.Empty && shelfCodes != null)
                {
                    shelfCodes = shelfCodes.Substring(0, shelfCodes.Length - 1);
                }
                else if (cellCodes != string.Empty && cellCodes != null)
                {
                    cellCodes = cellCodes.Substring(0, cellCodes.Length - 1);
                }

                if (editType == "edit")
                {
                    CellRepository.GetObjectSet()
                        .UpdateEntity(
                            c => c.DefaultProductCode == defaultProductCode,
                            c => new Cell() { DefaultProductCode = null }
                        );
                }
                CellRepository.GetObjectSet()
                    .UpdateEntity(
                        c => wareCodes.Contains(c.Warehouse.WarehouseCode)
                            || areaCodes.Contains(c.Area.AreaCode)
                            || shelfCodes.Contains(c.ShelfCode)
                            || cellCodes.Contains(c.CellCode),
                        c => new Cell() { DefaultProductCode = defaultProductCode }
                    );
                return true;
            }catch(Exception e)
            {
                return false;
            }            
        }
        
        //Test 
        public bool SetTree2(string strId, string proCode)
        {
            string[] arrayList = strId.Split(',');
            string id;
            string type;
            bool isCheck;
            bool result = false;
            for (int i = 0; i < arrayList.Length - 1; i++)
            {
                string[] array = arrayList[i].Split('^');
                type = array[0];
                id = array[1];
                isCheck = Convert.ToBoolean(array[2]);
                string proCode2 = proCode;
                UpdateTree(type, id, isCheck, proCode2);
                result = true;
            }
            return result;
        }

        public bool UpdateTree(string type, string id,bool isCheck, string proCode2)
        {
            bool result = false;

            if (type == "cell")
            {
                IQueryable<Cell> queryCell = CellRepository.GetQueryable();
                var cell = queryCell.FirstOrDefault(i => i.CellCode == id);
                if (isCheck == true)
                {
                    cell.DefaultProductCode = proCode2;
                }
                else
                {
                    cell.DefaultProductCode = null;
                }
                CellRepository.SaveChanges();
                result = true;
            }
            else
            {
                return false;
            }
            return result;
        }
        
        /// <summary>删除货位数量的信息</summary>
        public bool DeleteCell(string productCodes)
        {
            CellRepository.GetObjectSet()
                .UpdateEntity(
                    c => productCodes.Contains(c.DefaultProductCode),
                    c => new Cell() { DefaultProductCode = null }
                );
            return true;
        }

        /// <summary>加载卷烟信息</summary>
        public object GetCellInfo()
        {
            IQueryable<Cell> cellQuery = CellRepository.GetQueryable();

            var cellInfo = cellQuery.Where(c1 => c1.Product != null)
                .GroupBy(c2 => c2.Product)
                .Select(c3 => new
                {
                    ProductCode = c3.Key.ProductCode,
                    ProductName = c3.Key.ProductName,
                    ProductQuantity = c3.Count()
                });
            return cellInfo;
        }

        public object GetCellBy(int page, int rows, string QueryString, string Value)
        {
            string productCode = "", productName = "";

            if (QueryString == "ProductCode")
            {
                productCode = Value;
            }
            else
            {
                productName = Value;
            }
            IQueryable<Cell> cellQuery = CellRepository.GetQueryable();
            var cell = cellQuery.Where(c => c.Product != null && c.DefaultProductCode.Contains(productCode) && c.Product.ProductName.Contains(productName))
                 .GroupBy(c => c.Product)
                 .Select(c => new
                 {
                     ProductCode = c.Key.ProductCode,
                     ProductName = c.Key.ProductName,
                     ProductQuantity = c.Count()
                 });
            return cell;
        }
        
        public System.Data.DataTable GetCellByE(int page, int rows, string QueryString, string Value)
         {
             string productCode = "", productName = "";
 
             if (QueryString == "ProductCode")
             {
                 productCode = Value;
             }
             else
             {
                 productName = Value;
             }
             IQueryable<Cell> cellQuery = CellRepository.GetQueryable();
             var cell = cellQuery.Where(c => c.Product != null && c.DefaultProductCode.Contains(productCode) && c.Product.ProductName.Contains(productName))
                  .GroupBy(c => c.Product)
                  .Select(c => new
                  {
                      ProductCode = c.Key.ProductCode,
                      ProductName = c.Key.ProductName,
                      ProductQuantity = c.Count()
                  });
             System.Data.DataTable dt = new System.Data.DataTable();
             dt.Columns.Add("卷烟编码", typeof(string));
             dt.Columns.Add("卷烟名称", typeof(string));
             dt.Columns.Add("卷烟数量", typeof(string));
             foreach (var item in cell)
             {
                 dt.Rows.Add
                     (
                         item.ProductCode,
                         item.ProductName,
                         item.ProductQuantity
                     );
             }
             return dt;
         }

        /// <summary>查找卷烟信息</summary>
        public object GetCellInfo(string productCode)
        {
            IQueryable<Cell> cellQuery = CellRepository.GetQueryable();
            var cellInfo = cellQuery.Where(c1 => c1.Product != null && c1.DefaultProductCode == productCode)
                .GroupBy(c => c.Product)
                .Select(c => new
                {
                    ProductCode = c.Key.ProductCode,
                    ProductName = c.Key.ProductName,
                    ProductQuantity = c.Count()
                });
            return cellInfo;
        }
        
        /// <summary>编辑储位货位树形菜单</summary>
        public object GetCellCheck(string productCode)
        {
            var warehouses = WarehouseRepository.GetQueryable();
            var areas = AreaRepository.GetQueryable();
            var shelfs = ShelfRepository.GetQueryable();
            var cells = CellRepository.GetQueryable();

            var tmp = warehouses.Join(areas,
                                     w => w.WarehouseCode,
                                     a => a.WarehouseCode,
                                     (w, a) => new { w.WarehouseCode, w.WarehouseName, a.AreaCode, a.AreaName }
                                 )
                                 .Join(shelfs,
                                    a => a.AreaCode,
                                    s => s.AreaCode,
                                    (a, s) => new { a.WarehouseCode, a.WarehouseName, a.AreaCode, a.AreaName, s.ShelfCode, s.ShelfName }
                                 )
                                 .Join(cells,
                                    s => s.ShelfCode,
                                    c => c.ShelfCode,
                                    (s, c) => new { s.WarehouseCode, s.WarehouseName, s.AreaCode, s.AreaName, s.ShelfCode, s.ShelfName, c.CellCode, c.CellName, c }
                                 )
                                 .GroupBy(c => new { c.WarehouseCode, c.WarehouseName })
                                 .AsParallel()
                                 .Select(w => new
                                 {
                                     id = w.Key.WarehouseCode,
                                     name = w.Key.WarehouseName,
                                     type = "ware",
                                     @checked = w.Any(c => c.c.DefaultProductCode == productCode),
                                     open = true,
                                     children = w.GroupBy(a => new { a.AreaCode, a.AreaName })
                                                 .Select(a => new
                                                 {
                                                     id = a.Key.AreaCode,
                                                     name = a.Key.AreaName,
                                                     type = "area",
                                                     @checked = a.Any(c => c.c.DefaultProductCode == productCode),
                                                     open = false,
                                                     children = a.GroupBy(s => new { s.ShelfCode, s.ShelfName })
                                                                 .Select(s => new
                                                                 {
                                                                     id = s.Key.ShelfCode,
                                                                     name = s.Key.ShelfName,
                                                                     type = "shelf",
                                                                     @checked = s.Any(c => c.c.DefaultProductCode == productCode),
                                                                     open = false,
                                                                     children = s.GroupBy(c => new { c.CellCode, c.CellName })
                                                                                 .Select(c => new
                                                                                 {
                                                                                     id = c.Key.CellCode,
                                                                                     name = c.Key.CellName,
                                                                                     type = "cell",
                                                                                     @checked = c.Any(r => r.c.DefaultProductCode == productCode),
                                                                                     open = false,
                                                                                     children = ""
                                                                                 })
                                                                 })
                                                             })
                                 }).ToArray();
            return tmp;            
        }
        
        /// <summary>
        /// 盘点时用的树形结构数据，可根据货架Code查询
        /// </summary>
        /// <param name="shelfCode">货架Code</param>
        /// <returns></returns>
        public object GetWareCheck(string shelfCode)
        {
            var warehouses = WarehouseRepository.GetQueryable().AsEnumerable();
            HashSet<Tree> wareSet = new HashSet<Tree>();
            if (shelfCode == null || shelfCode == string.Empty)//判断是否是加载货位
            {
                foreach (var warehouse in warehouses)//仓库
                {
                    Tree wareTree = new Tree();
                    wareTree.id = warehouse.WarehouseCode;
                    wareTree.text = "仓库：" + warehouse.WarehouseName;
                    wareTree.state = "open";
                    wareTree.attributes = "ware";

                    var areas = AreaRepository.GetQueryable().Where(a => a.Warehouse.WarehouseCode == warehouse.WarehouseCode && a.IsActive=="1")
                                                             .OrderBy(a => a.AreaCode).Select(a => a);
                    HashSet<Tree> areaSet = new HashSet<Tree>();
                    foreach (var area in areas)//库区
                    {
                        Tree areaTree = new Tree();
                        areaTree.id = area.AreaCode;
                        areaTree.text = "库区：" + area.AreaName;
                        areaTree.state = "closed";
                        areaTree.attributes = "area";

                        var shelfs = ShelfRepository.GetQueryable().Where(s => s.Area.AreaCode == area.AreaCode)
                                                                   .OrderBy(s => s.ShelfCode).Select(s => s);
                        HashSet<Tree> shelfSet = new HashSet<Tree>();
                        foreach (var shelf in shelfs)//货架
                        {
                            Tree shelfTree = new Tree();
                            shelfTree.id = shelf.ShelfCode;
                            shelfTree.text = "货架：" + shelf.ShelfName;
                            shelfTree.attributes = "shelf";
                            shelfTree.state = "closed";
                            shelfSet.Add(shelfTree);
                        }
                        areaTree.children = shelfSet.ToArray();
                        areaSet.Add(areaTree);
                    }
                    wareTree.children = areaSet.ToArray();
                    wareSet.Add(wareTree);
                }
            }
            else
            {
                var cells = CellRepository.GetQueryable().Where(c => c.Shelf.ShelfCode == shelfCode)
                                                         .OrderBy(c => c.CellCode).Select(c => c);
                foreach (var cell in cells)//货位
                {
                    var product = ProductRepository.GetQueryable().FirstOrDefault(p => p.ProductCode == cell.DefaultProductCode);
                    Tree cellTree = new Tree();
                    cellTree.id = cell.CellCode;
                    cellTree.text = "货位：" + cell.CellName;
                    cellTree.state = "open";
                    cellTree.attributes = "cell";
                    wareSet.Add(cellTree);
                }
            }
            return wareSet.ToArray();
        }

        /// <summary>
        /// 根据参数Code查询货位信息
        /// </summary>
        /// <param name="cellCode">货位Code</param>
        /// <returns></returns>
        public object FindCell(string cellCode)
        {
            IQueryable<Cell> cellQuery = CellRepository.GetQueryable();
            var cell = cellQuery.Where(c => c.CellCode == cellCode).OrderBy(b => b.CellCode).AsEnumerable()
                                .Select(b => new { b.CellCode, b.CellName, b.CellType, b.ShortName, b.Rfid, b.Layer, b.Col, b.ImgX, b.ImgY, b.IsSingle, b.MaxQuantity, b.Description, b.Warehouse.WarehouseName, b.Warehouse.WarehouseCode, b.Area.AreaCode, b.Area.AreaName, b.Shelf.ShelfCode, b.Shelf.ShelfName, DefaultProductCode = b.Product == null ? string.Empty : b.Product.ProductCode, ProductName = b.Product == null ? string.Empty : b.Product.ProductName, IsActive = b.IsActive == "1" ? "可用" : "不可用", UpdateTime = b.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss") });
            return cell.First(c => c.CellCode == cellCode);
        }

        /// <summary>
        /// 仓库设置时用的TreeGrid的树形结构，可根据仓库Code查询
        /// </summary>
        /// <param name="wareCode">仓库Code</param>
        /// <returns></returns>
        public object GetSearch(string wareCode)
        {
            var warehouses = WarehouseRepository.GetQueryable().AsEnumerable();
            if (wareCode != null && wareCode != string.Empty)
            {
                warehouses = warehouses.Where(w => w.WarehouseCode == wareCode);
            }

            HashSet<WareTree> wareSet = new HashSet<WareTree>();
            foreach (var warehouse in warehouses)//仓库
            {
                WareTree wareTree = new WareTree();
                wareTree.Code = warehouse.WarehouseCode;
                wareTree.Name = "仓库：" + warehouse.WarehouseName;
                wareTree.WarehouseCode = warehouse.WarehouseCode;
                wareTree.WarehouseName = warehouse.WarehouseName;
                wareTree.Type = warehouse.WarehouseType;
                wareTree.Description = warehouse.Description;
                wareTree.IsActive = warehouse.IsActive == "1" ? "可用" : "不可用";
                wareTree.UpdateTime = warehouse.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                wareTree.ShortName = warehouse.ShortName;
                wareTree.attributes = "ware";
                var areas = AreaRepository.GetQueryable().Where(a => a.Warehouse.WarehouseCode == warehouse.WarehouseCode)
                                                        .OrderBy(a => a.AreaCode).Select(a => a);
                HashSet<WareTree> areaSet = new HashSet<WareTree>();
                foreach (var area in areas)//库区
                {
                    WareTree areaTree = new WareTree();
                    areaTree.Code = area.AreaCode;
                    areaTree.Name = "库区：" + area.AreaName;
                    areaTree.AreaCode = area.AreaCode;
                    areaTree.AreaName = area.AreaName;
                    areaTree.WarehouseCode = area.Warehouse.WarehouseCode;
                    areaTree.WarehouseName = area.Warehouse.WarehouseName;
                    areaTree.Type = area.AreaType;
                    areaTree.Description = area.Description;
                    areaTree.IsActive = area.IsActive == "1" ? "可用" : "不可用";
                    areaTree.UpdateTime = area.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    areaTree.ShortName = area.ShortName;
                    areaTree.AllotInOrder = area.AllotInOrder;
                    areaTree.AllotOutOrder = area.AllotOutOrder;
                    areaTree.attributes = "area";
                    var shelfs = ShelfRepository.GetQueryable().Where(s => s.Area.AreaCode == area.AreaCode)
                                                               .OrderBy(s => s.ShelfCode).Select(s => s);
                    HashSet<WareTree> shelfSet = new HashSet<WareTree>();
                    foreach (var shelf in shelfs)//货架
                    {
                        WareTree shelfTree = new WareTree();
                        shelfTree.Code = shelf.ShelfCode;
                        shelfTree.Name = "货架：" + shelf.ShelfName;
                        shelfTree.ShelfCode = shelf.ShelfCode;
                        shelfTree.ShelfName = shelf.ShelfName;

                        shelfTree.WarehouseCode = shelf.Warehouse.WarehouseCode;
                        shelfTree.WarehouseName = shelf.Warehouse.WarehouseName;
                        shelfTree.AreaCode = shelf.Area.AreaCode;
                        shelfTree.AreaName = shelf.Area.AreaName;

                        shelfTree.Type = shelf.ShelfType;
                        shelfTree.Description = shelf.Description;
                        shelfTree.IsActive = shelf.IsActive == "1" ? "可用" : "不可用";
                        shelfTree.UpdateTime = shelf.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                        shelfTree.ShortName = shelf.ShortName;
                        shelfTree.attributes = "shelf";
                        shelfSet.Add(shelfTree);
                    }
                    areaTree.children = shelfSet.ToArray();
                    areaSet.Add(areaTree);
                }
                wareTree.children = areaSet.ToArray();
                wareSet.Add(wareTree);
            }
            return wareSet.ToArray();
        }

        /// <summary>
        /// 仓库设置点击货架查询货架的货位信息
        /// </summary>
        /// <param name="shelfCode">货架Code</param>
        /// <returns></returns>
        public object GetCell(string shelfCode)
        {
            HashSet<WareTree> wareSet = new HashSet<WareTree>();
            var cells = CellRepository.GetQueryable().Where(c => c.Shelf.ShelfCode == shelfCode)
                                                     .OrderBy(c => c.CellCode).Select(c => c);
            foreach (var cell in cells)//货位
            {
                WareTree cellTree = new WareTree();
                cellTree.Code = cell.CellCode;
                cellTree.Name = "货位：" + cell.CellName;
                cellTree.CellCode = cell.CellCode;
                cellTree.CellName = cell.CellName;

                cellTree.WarehouseCode = cell.Warehouse.WarehouseCode;
                cellTree.WarehouseName = cell.Warehouse.WarehouseName;

                cellTree.AreaCode = cell.Area.AreaCode;
                cellTree.AreaName = cell.Area.AreaName;

                cellTree.ShelfCode = cell.Shelf.ShelfCode;
                cellTree.ShelfName = cell.Shelf.ShelfName;

                cellTree.Type = cell.CellType;
                cellTree.Description = cell.Description;
                cellTree.IsActive = cell.IsActive == "1" ? "可用" : "不可用";
                cellTree.UpdateTime = cell.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                cellTree.ShortName = cell.ShortName;
                cellTree.Layer = cell.Layer;
                cellTree.MaxQuantity = cell.MaxQuantity;
                cellTree.ProductName = cell.Product == null ? string.Empty : cell.Product.ProductName;
                cellTree.attributes = "cell";

                wareSet.Add(cellTree);
            }
            return wareSet;
        }

        /// <summary>
        /// 移库时用的树形结构数据，可根据货架Code查询，根据移出的货位和移入的货位查询-移库单使用
        /// </summary>
        /// <param name="shelfCode">货架Code</param>
        /// <param name="inOrOut">移入还是移出</param>
        /// <param name="productCode">产品代码</param>
        /// <returns></returns>
        public object GetMoveCellDetails(string shelfCode, string inOrOut, string productCode)
        {
            var warehouses = WarehouseRepository.GetQueryable().AsEnumerable();
            HashSet<Tree> wareSet = new HashSet<Tree>();
            if (shelfCode == null || shelfCode == string.Empty)//判断是否是加载货位
            {
                foreach (var warehouse in warehouses)//仓库
                {
                    Tree wareTree = new Tree();
                    wareTree.id = warehouse.WarehouseCode;
                    wareTree.text = "仓库：" + warehouse.WarehouseName;
                    wareTree.state = "open";
                    wareTree.attributes = "ware";

                    var areas = AreaRepository.GetQueryable().Where(a => a.Warehouse.WarehouseCode == warehouse.WarehouseCode && a.IsActive=="1")
                                                             .OrderBy(a => a.AreaCode).Select(a => a);
                    HashSet<Tree> areaSet = new HashSet<Tree>();
                    foreach (var area in areas)//库区
                    {
                        Tree areaTree = new Tree();
                        areaTree.id = area.AreaCode;
                        areaTree.text = "库区：" + area.AreaName;
                        areaTree.state = "open";
                        areaTree.attributes = "area";

                        var shelfs = ShelfRepository.GetQueryable().Where(s => s.Area.AreaCode == area.AreaCode)
                                                                   .OrderBy(s => s.ShelfCode).Select(s => s);
                        HashSet<Tree> shelfSet = new HashSet<Tree>();
                        foreach (var shelf in shelfs)//货架
                        {
                            Tree shelfTree = new Tree();
                            shelfTree.id = shelf.ShelfCode;
                            shelfTree.text = "货架：" + shelf.ShelfName;
                            shelfTree.attributes = "shelf";
                            shelfTree.state = "closed";
                            shelfSet.Add(shelfTree);
                        }
                        areaTree.children = shelfSet.ToArray();
                        areaSet.Add(areaTree);
                    }
                    wareTree.children = areaSet.ToArray();
                    wareSet.Add(wareTree);
                }
            }
            else
            {
                var cells = CellRepository.GetQueryable().Where(c => c.CellCode == c.CellCode);
                if (inOrOut == "out")// 查询出可以移出卷烟的货位
                {
                    var storages = StorageRepository.GetQueryable().Where(s => (s.Quantity - s.OutFrozenQuantity) > 0 && string.IsNullOrEmpty(s.Cell.LockTag)).Select(s => s.CellCode);
                    cells = cells.Where(c => c.Shelf.ShelfCode == shelfCode && storages.Any(s => s == c.CellCode) && c.IsActive == "1")
                                                         .OrderBy(s => s.CellCode);
                }
                else if (inOrOut == "in")//查询出可以移入卷烟的货位 ,
                {
                    var cellf = cells.Where(c => c.Shelf.ShelfCode == shelfCode && c.IsSingle == "0" && c.IsActive == "1").OrderBy(c => c.CellCode).Select(c => c);
                    cells = cells.Where(c => c.Shelf.ShelfCode == shelfCode && c.IsActive == "1"
                                            && (c.Storages.Count == 0 || c.Storages.Any(s => (s.ProductCode == productCode && (c.MaxQuantity * s.Product.Unit.Count) - (s.Quantity + s.InFrozenQuantity) > 0))
                                            || c.Storages.Sum(s => s.Quantity + s.InFrozenQuantity) == 0) && c.IsSingle == "1")
                                             .OrderBy(c => c.CellCode).Select(c => c);
                    foreach (var cell in cellf)//货位
                    {
                        string quantityStr = "";
                        if (inOrOut == "in")
                        {
                            if (cell.Storages.Count != 0 && cell.Storages.Sum(s => s.Quantity + s.InFrozenQuantity) != 0)
                            {
                                var cellQuantity = cell.Storages.GroupBy(g => new { g.Cell }).Select(s => new { quan = s.Key.Cell.Product == null ? s.Key.Cell.MaxQuantity : ((s.Key.Cell.MaxQuantity * s.Key.Cell.Product.Unit.Count) - s.Sum(p => p.Quantity + p.InFrozenQuantity)) / s.Key.Cell.Product.Unit.Count });
                                decimal quantity = Convert.ToDecimal(cellQuantity.Sum(s => s.quan));
                                // quantity = Math.Round(quantity, 2);
                                quantityStr = "<可入：" + quantity + ">件,当前品牌：" + productCode + "";
                            }
                            else
                                quantityStr = "<可入：" + cell.MaxQuantity + ">件";
                        }

                        Tree cellTree = new Tree();
                        cellTree.id = cell.CellCode;
                        cellTree.text = "货位：" + cell.CellName + quantityStr;
                        cellTree.state = "open";
                        cellTree.attributes = "cell";
                        wareSet.Add(cellTree);
                    }
                }
                else if (inOrOut == "stockOut")//查询可以出库的数量 --出库使用
                {
                    var storages = StorageRepository.GetQueryable().Where(s => (s.Quantity - s.OutFrozenQuantity) > 0
                                                                    && string.IsNullOrEmpty(s.Cell.LockTag)
                                                                    && s.ProductCode == productCode)
                                                                    .Select(s => s.CellCode);
                    cells = cells.Where(c => c.Shelf.ShelfCode == shelfCode && storages.Any(s => s == c.CellCode) && c.IsActive=="1").OrderBy(c => c.CellCode);
                }
                else if (inOrOut=="moveIn")//查询出非货位管理的货位用于入库单非货位管理的移入
                {
                    cells = cells.Where(c => c.Shelf.ShelfCode == shelfCode && c.IsSingle == "0" && c.IsActive == "1").OrderBy(c => c.CellCode).Select(c => c);
                }
                foreach (var cell in cells)//货位
                {
                    string quantityStr = "";
                    if (inOrOut == "in")
                    {
                        if (cell.Storages.Count != 0 && cell.Storages.Sum(s => s.Quantity + s.InFrozenQuantity) != 0)
                        {
                            var cellQuantity = cell.Storages.GroupBy(g => new { g.Cell }).Select(s => new { quan = s.Key.Cell.Storages.Max(t => t.ProductCode) == null ? s.Key.Cell.MaxQuantity : ((s.Key.Cell.MaxQuantity * s.Key.Cell.Storages.Max(t => t.Product.Unit.Count)) - s.Sum(p => p.Quantity + p.InFrozenQuantity)) / s.Key.Cell.Storages.Max(t => t.Product.Unit.Count) });
                            decimal quantity = Convert.ToDecimal(cellQuantity.Sum(s => s.quan));
                           // quantity = Math.Round(quantity, 2);
                            quantityStr = "<可入：" + quantity + ">件,当前品牌："+productCode+"";
                        }
                        else
                            quantityStr = "<可入：" + cell.MaxQuantity + ">件";
                    }

                    Tree cellTree = new Tree();
                    cellTree.id = cell.CellCode;
                    cellTree.text = "货位：" + cell.CellName + quantityStr;
                    cellTree.state = "open";
                    cellTree.attributes = "cell";
                    wareSet.Add(cellTree);
                }
            }
            return wareSet.ToArray();
        }

        /// <summary>
        /// 查询库区，用于分拣设置货位
        /// </summary>
        /// <param name="areaType">库区类型</param>
        /// <returns></returns>
        public object GetSortCell(string areaType)
        {
            var areas = AreaRepository.GetQueryable().Where(a => a.AreaType == areaType)
                                                     .OrderBy(a => a.AreaCode).Select(a => a);
            HashSet<Tree> areaSet = new HashSet<Tree>();
            foreach (var area in areas)//库区
            {
                Tree areaTree = new Tree();
                areaTree.id = area.AreaCode;
                areaTree.text = "库区：" + area.AreaName;
                areaTree.state = "open";
                areaTree.attributes = "area";

                var shelfs = ShelfRepository.GetQueryable().Where(s => s.Area.AreaCode == area.AreaCode)
                                                           .OrderBy(s => s.ShelfCode).Select(s => s);
                HashSet<Tree> shelfSet = new HashSet<Tree>();
                foreach (var shelf in shelfs)//货架
                {
                    Tree shelfTree = new Tree();
                    shelfTree.id = shelf.ShelfCode;
                    shelfTree.text = "货架：" + shelf.ShelfName;
                    shelfTree.attributes = "shelf";


                    var cells = CellRepository.GetQueryable().Where(c => c.Shelf.ShelfCode == shelf.ShelfCode)
                                                             .OrderBy(c => c.CellCode).Select(c => c);
                    HashSet<Tree> cellSet = new HashSet<Tree>();
                    foreach (var cell in cells)//货位
                    {
                        Tree cellTree = new Tree();
                        cellTree.id = cell.CellCode;
                        cellTree.text = cell.CellName;
                        cellTree.state = "open";
                        cellTree.attributes = "cell";
                        cellSet.Add(cellTree);
                    }
                    shelfTree.children = cellSet.ToArray();
                    shelfSet.Add(shelfTree);
                }
                areaTree.children = shelfSet.ToArray();
                areaSet.Add(areaTree);
            }
            return areaSet.ToArray();
        }

        public System.Data.DataTable GetProductCell(string queryString,string value)
        {
            string productCode = "", productName = "";

            if (queryString == "ProductCode")
            {
                productCode = value;
            }
            else
            {
                productName = value;
            }

            IQueryable<Cell> cellQuery = CellRepository.GetQueryable();

            var cellInfo = cellQuery.Where(c => c.Product != null && c.DefaultProductCode.Contains(productCode) && c.Product.ProductName.Contains(productName))
                .GroupBy(c => c.Product)
                .Select(c => new
                {
                    ProductCode = c.Key.ProductCode,
                    ProductName = c.Key.ProductName,
                    ProductQuantity = c.Count()
                });

            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("卷烟编码", typeof(string));
            dt.Columns.Add("卷烟名称", typeof(string));
            dt.Columns.Add("货位数量", typeof(string));

            foreach (var item in cellInfo)
            {
                dt.Rows.Add(item.ProductCode, item.ProductName, item.ProductQuantity);
            }
            return dt;
        }


        /// <summary>
        /// 根据货编码架获取生成的货位编码
        /// </summary>
        /// <param name="shelfCode">货架编码</param>
        /// <returns></returns>
        public object GetCellCode(string shelfCode)
        {
            string cellCodeStr = "";
            IQueryable<Cell> cellQuery = CellRepository.GetQueryable();
            var cellCode = cellQuery.Where(c => c.ShelfCode == shelfCode).Max(c=>c.CellCode);
            if (cellCode == string.Empty || cellCode == null)
            {
                cellCodeStr = shelfCode + "-01";
            }
            else
            {
                int i = Convert.ToInt32(cellCode.ToString().Substring(shelfCode.Length+1, 2));
                if (cellCode.ToString().Substring(cellCode.Length - 1, 1) == "3")
                {
                    i++;
                }
                string newcode = i.ToString();
                if (newcode.Length <= 2)
                {
                    for (int j = 0; j < 2 - i.ToString().Length; j++)
                    {
                        newcode = "0" + newcode;
                    }
                    cellCodeStr = shelfCode + "-" + newcode;
                }
            }
            return cellCodeStr;
        }

        public System.Data.DataTable GetCell(int page, int rows, string type, string id)
        {
            var warehouses = WarehouseRepository.GetQueryable();
            var areas = AreaRepository.GetQueryable();
            var shelfs = ShelfRepository.GetQueryable();
            var cells = CellRepository.GetQueryable();
            HashSet<WareTree> wareSet = new HashSet<WareTree>();
            HashSet<WareTree> areaSet = new HashSet<WareTree>();
            HashSet<WareTree> shelfSet = new HashSet<WareTree>();
            HashSet<WareTree> cellSet = new HashSet<WareTree>();
            var set = wareSet;
            if (type == "area")
            {
                areas = areas.Where(a => a.AreaCode == id).OrderBy(a => a.AreaCode).Select(a => a);
                foreach (var area in areas)//库区
                {
                    WareTree areaTree = new WareTree();
                    areaTree.Code = area.AreaCode;
                    areaTree.Name = "库区：" + area.AreaName;
                    areaTree.AreaCode = area.AreaCode;
                    areaTree.AreaName = area.AreaName;
                    areaTree.WarehouseCode = area.Warehouse.WarehouseCode;
                    areaTree.WarehouseName = area.Warehouse.WarehouseName;
                    areaTree.Type = area.AreaType;
                    areaTree.Description = area.Description;
                    areaTree.IsActive = area.IsActive == "1" ? "可用" : "不可用";
                    areaTree.UpdateTime = area.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    areaTree.ShortName = area.ShortName;
                    areaTree.AllotInOrder = area.AllotInOrder;
                    areaTree.AllotOutOrder = area.AllotOutOrder;
                    areaTree.attributes = "area";
                    areaSet.Add(areaTree);
                }
                set = areaSet;
            }
            else if (type == "shelf")
            {
                shelfs = shelfs.Where(a => a.ShelfCode == id).OrderBy(a => a.ShelfCode).Select(a => a);
                cells = CellRepository.GetQueryable().Where(c => c.Shelf.ShelfCode == id)
                                                    .OrderBy(c => c.CellCode).Select(c => c);
                foreach (var shelf in shelfs)//货架
                {
                    WareTree shelfTree = new WareTree();
                    shelfTree.Code = shelf.ShelfCode;
                    shelfTree.Name = "货架：" + shelf.ShelfName;
                    shelfTree.ShelfCode = shelf.ShelfCode;
                    shelfTree.ShelfName = shelf.ShelfName;
                    shelfTree.WarehouseCode = shelf.Warehouse.WarehouseCode;
                    shelfTree.WarehouseName = shelf.Warehouse.WarehouseName;
                    shelfTree.AreaCode = shelf.Area.AreaCode;
                    shelfTree.AreaName = shelf.Area.AreaName;
                    shelfTree.Type = shelf.ShelfType;
                    shelfTree.Description = shelf.Description;
                    shelfTree.IsActive = shelf.IsActive == "1" ? "可用" : "不可用";
                    shelfTree.UpdateTime = shelf.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    shelfTree.ShortName = shelf.ShortName;
                    shelfTree.attributes = "shelf";
                    foreach (var cell in cells)//货位
                    {
                        WareTree cellTree = new WareTree();
                        cellTree.Code = cell.CellCode;
                        cellTree.Name = "货位：" + cell.CellName;
                        cellTree.CellCode = cell.CellCode;
                        cellTree.CellName = cell.CellName;
                        cellTree.WarehouseCode = cell.Warehouse.WarehouseCode;
                        cellTree.WarehouseName = cell.Warehouse.WarehouseName;
                        cellTree.AreaCode = cell.Area.AreaCode;
                        cellTree.AreaName = cell.Area.AreaName;
                        cellTree.ShelfCode = cell.Shelf.ShelfCode;
                        cellTree.ShelfName = cell.Shelf.ShelfName;
                        cellTree.Type = cell.CellType;
                        cellTree.Description = cell.Description;
                        cellTree.IsActive = cell.IsActive == "1" ? "可用" : "不可用";
                        cellTree.UpdateTime = cell.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                        cellTree.ShortName = cell.ShortName;
                        cellTree.Layer = cell.Layer;
                        cellTree.MaxQuantity = cell.MaxQuantity;
                        cellTree.ProductName = cell.Product == null ? string.Empty : cell.Product.ProductName;
                        cellTree.attributes = "cell";
                        cellSet.Add(cellTree);
                    }
                    shelfSet.Add(shelfTree);
                }
                set = cellSet;
            }
            else if (type == "cell")
            {
                cells = cells.Where(a => a.CellCode == id).OrderBy(a => a.CellCode).Select(a => a);
                foreach (var cell in cells)//货位
                {
                    WareTree cellTree = new WareTree();
                    cellTree.Code = cell.CellCode;
                    cellTree.Name = "货位：" + cell.CellName;
                    cellTree.CellCode = cell.CellCode;
                    cellTree.CellName = cell.CellName;
                    cellTree.WarehouseCode = cell.Warehouse.WarehouseCode;
                    cellTree.WarehouseName = cell.Warehouse.WarehouseName;
                    cellTree.AreaCode = cell.Area.AreaCode;
                    cellTree.AreaName = cell.Area.AreaName;
                    cellTree.ShelfCode = cell.Shelf.ShelfCode;
                    cellTree.ShelfName = cell.Shelf.ShelfName;
                    cellTree.Type = cell.CellType;
                    cellTree.Description = cell.Description;
                    cellTree.IsActive = cell.IsActive == "1" ? "可用" : "不可用";
                    cellTree.UpdateTime = cell.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    cellTree.ShortName = cell.ShortName;
                    cellTree.Layer = cell.Layer;
                    cellTree.MaxQuantity = cell.MaxQuantity;
                    cellTree.ProductName = cell.Product == null ? string.Empty : cell.Product.ProductName;
                    cellTree.attributes = "cell";
                    cellSet.Add(cellTree);
                }
                set = cellSet;
            }
            else
            {
                if (type == null || type == string.Empty)
                {
                    warehouses = warehouses.Where(w => w.WarehouseCode == "0101").OrderBy(w => w.WarehouseCode).Select(w => w);
                }
                else if (type == "ware")
                {
                    warehouses = warehouses.Where(w => w.WarehouseCode == id).OrderBy(w => w.WarehouseCode).Select(w => w);
                }
                foreach (var warehouse in warehouses)//仓库
                {
                    WareTree wareTree = new WareTree();
                    wareTree.Code = warehouse.WarehouseCode;
                    wareTree.Name = "仓库：" + warehouse.WarehouseName;
                    wareTree.WarehouseCode = warehouse.WarehouseCode;
                    wareTree.WarehouseName = warehouse.WarehouseName;
                    wareTree.Type = warehouse.WarehouseType;
                    wareTree.Description = warehouse.Description;
                    wareTree.IsActive = warehouse.IsActive == "1" ? "可用" : "不可用";
                    wareTree.UpdateTime = warehouse.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    wareTree.ShortName = warehouse.ShortName;
                    wareTree.attributes = "ware";
                    warehouses = warehouses.Where(w => w.WarehouseCode == id);
                    wareSet.Add(wareTree);
                }
                set = wareSet;
            }
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("名称", typeof(string));
            dt.Columns.Add("简称", typeof(string));
            dt.Columns.Add("类型", typeof(string));
            dt.Columns.Add("描述", typeof(string));
            dt.Columns.Add("是否可用", typeof(string));
            dt.Columns.Add("预设卷烟名称", typeof(string));
            dt.Columns.Add("货位层号", typeof(string));
            dt.Columns.Add("货位最大量", typeof(string));
            dt.Columns.Add("时间", typeof(string));
            dt.Columns.Add("入库顺序", typeof(string));
            dt.Columns.Add("出库顺序", typeof(string));
            foreach (var item in set)
            {
                dt.Rows.Add
                    (
                        item.Name,
                        item.ShortName,
                        item.Type,
                        item.Description,
                        item.IsActive,
                        item.ProductName,
                        item.Layer,
                        item.MaxQuantity,
                        item.UpdateTime,
                        item.AllotInOrder,
                        item.AllotOutOrder
                    );
            }
            return dt;
        }

        #region 上报仓储属性表
        public bool uploadCell()
        {
            try
            {
                IQueryable<Cell> cellQuery = CellRepository.GetQueryable();
                var cells = cellQuery.OrderBy(b => b.CellCode).Select(b => b);
                DataSet ds = Insert(cells);
                upload.UploadCell(ds);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 插入数据到虚拟表
        public DataSet Insert(IQueryable<Cell> cell)
        {
            DataSet ds = this.GenerateEmptyTables();
            DataRow inbrddr = ds.Tables["wms_cell"].NewRow();
            foreach (var p in cell)
            {
                inbrddr["STORAGE_CODE"] = p.CellCode;
                inbrddr["STORAGE_TYPE"] = "4";
                inbrddr["ORDER_NUM"] = "";
                inbrddr["CONTAINER"] = "5003";
                inbrddr["STORAGE_NAME"] = p.CellName;
                inbrddr["UP_CODE"] = p.Area.Warehouse.WarehouseCode;
                inbrddr["DIST_CTR_CODE"] = p.Area.Warehouse.WarehouseCode;
                inbrddr["N_ORG_CODE"] = "";
                inbrddr["N_STORE_ROOM_CODE"] = "";
                inbrddr["CAPACITY"] = p.MaxQuantity * 50;
                inbrddr["HORIZONTAL_NUM"] = 0;
                inbrddr["VERTICAL_NUM"] = 0;
                inbrddr["AREA_TYPE"] = p.Area.AreaType;
                inbrddr["UPDATE_DATE"] = DateTime.Now;
                inbrddr["ISACTIVE"] = p.IsActive;
                ds.Tables["wms_cell"].Rows.Add(inbrddr);
            }
            return ds;
        }

        public DataSet Insert(Cell cell)
        {
            DataSet ds = this.GenerateEmptyTables();
            DataRow inbrddr = ds.Tables["wms_cell"].NewRow();
            inbrddr["STORAGE_CODE"] = cell.CellCode;
            inbrddr["STORAGE_TYPE"] = "4";
            inbrddr["ORDER_NUM"] = "";
            inbrddr["CONTAINER"] = "5003";
            inbrddr["STORAGE_NAME"] = cell.CellName;
            inbrddr["UP_CODE"] = cell.Area.Warehouse.WarehouseCode;
            inbrddr["DIST_CTR_CODE"] = cell.Area.Warehouse.WarehouseCode;
            inbrddr["N_ORG_CODE"] = "";
            inbrddr["N_STORE_ROOM_CODE"] = "";
            inbrddr["CAPACITY"] = cell.MaxQuantity * 50;
            inbrddr["HORIZONTAL_NUM"] = 0;
            inbrddr["VERTICAL_NUM"] = 0;
            inbrddr["AREA_TYPE"] = cell.Area.AreaType;
            inbrddr["UPDATE_DATE"] = DateTime.Now;
            inbrddr["ISACTIVE"] = cell.IsActive;
            ds.Tables["wms_cell"].Rows.Add(inbrddr);

            return ds;
        }
        #endregion

        #region 创建一个空的仓储信息表
        private DataSet GenerateEmptyTables()
        {
            DataSet ds = new DataSet();
            DataTable inbrtable = ds.Tables.Add("wms_cell");
            inbrtable.Columns.Add("STORAGE_CODE");
            inbrtable.Columns.Add("STORAGE_TYPE");
            inbrtable.Columns.Add("ORDER_NUM");
            inbrtable.Columns.Add("CONTAINER");
            inbrtable.Columns.Add("STORAGE_NAME");
            inbrtable.Columns.Add("UP_CODE");
            inbrtable.Columns.Add("DIST_CTR_CODE");
            inbrtable.Columns.Add("N_ORG_CODE");
            inbrtable.Columns.Add("N_STORE_ROOM_CODE");
            inbrtable.Columns.Add("CAPACITY");
            inbrtable.Columns.Add("HORIZONTAL_NUM");
            inbrtable.Columns.Add("VERTICAL_NUM");
            inbrtable.Columns.Add("AREA_TYPE");
            inbrtable.Columns.Add("UPDATE_DATE");
            inbrtable.Columns.Add("ISACTIVE");
            return ds;
        }
        #endregion
    }
}
