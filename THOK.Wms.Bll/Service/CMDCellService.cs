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
    public class CMDCellService : ServiceBase<CMD_CELL>, ICMDCellService
    {

        [Dependency]
        public ICMDWarehouseRepository CMDWarehouseRepository { get; set; }

        [Dependency]
        public ICMDAreaRepository CMDAreaRepository { get; set; }

        [Dependency]
        public ICMDShelfRepository CMDShelfRepository { get; set; }

        [Dependency]
        public ICMDCellRepository CMDCellRepository { get; set; }

        [Dependency]
        public ICMDProuductRepository CMDProductRepository { get; set; }
        [Dependency]
        public IWMSBillMasterRepository BillMasterRepository { get; set; }

        

        //[Dependency]
        //public IStorageRepository StorageRepository { get; set; }

        UploadBll upload = new UploadBll();

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, string cellCode)
        {
            IQueryable<CMD_CELL> cellQuery = CMDCellRepository.GetQueryable();
            var cell = cellQuery.OrderBy(b => b.CELL_CODE).AsEnumerable().Select(b => new { b.CELL_CODE, b.CELL_NAME, b.AREA_CODE, b.SHELF_CODE, b.CELL_COLUMN, b.CELL_ROW, b.IS_ACTIVE, b.PRODUCT_CODE,b.REAL_WEIGHT, b.SCHEDULE_NO, b.IS_LOCK, b.PALLET_CODE, b.BILL_NO, b.IN_DATE, b.PRIORITY_LEVEL, b.MEMO });
            int total = cell.Count();
            cell = cell.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = cell.ToArray() };
        }
        public object GetDetail(int page, int rows, string type, string id)
        {
            var warehouses = CMDWarehouseRepository.GetQueryable();
            var areas = CMDAreaRepository.GetQueryable();
            var shelfs = CMDShelfRepository.GetQueryable();
            var cells = CMDCellRepository.GetQueryable();
            HashSet<NewWareTree> wareSet = new HashSet<NewWareTree>();
            HashSet<NewWareTree> areaSet = new HashSet<NewWareTree>();
            HashSet<NewWareTree> shelfSet = new HashSet<NewWareTree>();
            HashSet<NewWareTree> cellSet = new HashSet<NewWareTree>();
            var set = wareSet;
            if (type == "area")
            {
                areas = areas.Where(a => a.AREA_CODE == id).OrderBy(a => a.AREA_CODE).Select(a => a);
                foreach (var area in areas)//库区
                {
                    NewWareTree areaTree = new NewWareTree();
                    areaTree.CODE = area.AREA_CODE;
                    areaTree.NAME = "库区：" + area.AREA_NAME;
                    areaTree.AREA_CODE = area.AREA_CODE;
                    areaTree.AREA_NAME = area.AREA_NAME;
                    areaTree.WAREHOUSE_CODE = area.WAREHOUSE_CODE;
                    areaTree.MEMO = area.MEMO;

                    areaTree.ATTRIBUTES = "area";
                    areaSet.Add(areaTree);
                }
                set = areaSet;
            }
            else if (type == "shelf")
            {
                shelfs = shelfs.Where(a => a.SHELF_CODE== id).OrderBy(a => a.SHELF_CODE).Select(a => a);
                cells = CMDCellRepository.GetQueryable().Where(c => c.CMD_SHELF.SHELF_CODE == id)
                                                    .OrderBy(c => c.CELL_CODE).Select(c => c);
                foreach (var shelf in shelfs)//货架
                {
                    NewWareTree shelfTree = new NewWareTree();
                    shelfTree.CODE = shelf.AREA_CODE;
                    shelfTree.NAME = "库区：" + shelf.SHELF_NAME;
                    shelfTree.AREA_CODE = shelf.AREA_CODE;
                    shelfTree.AREA_NAME = shelf.SHELF_NAME;
                    shelfTree.WAREHOUSE_CODE = shelf.WAREHOUSE_CODE;
                    shelfTree.MEMO = shelf.MEMO;

                    shelfTree.ATTRIBUTES = "shelf";
                    foreach (var cell in cells)//货位
                    {
                        NewWareTree cellTree = new NewWareTree();
                        cellTree.CODE = cell.CELL_CODE;
                        cellTree.NAME = "货位：" + cell.CELL_NAME;
                        cellTree.CELL_CODE = cell.CELL_CODE;
                        cellTree.CELL_NAME = cell.CELL_NAME;
                        //cellTree.WAREHOUSE_CODE = cell.Warehouse.WarehouseCode;
                        //cellTree.WAREHOUSE_NAME = cell.Warehouse.WarehouseName;
                        cellTree.AREA_CODE = cell.CMD_AREA.AREA_CODE;
                        cellTree.AREA_NAME = cell.CMD_AREA.AREA_NAME;
                        cellTree.SHELF_CODE = cell.CMD_SHELF.SHELF_CODE;
                        cellTree.SHELF_NAME = cell.CMD_SHELF.SHELF_NAME;
                        
                        cellTree.MEMO = cell.MEMO;
                        cellTree.IS_ACTIVE = cell.IS_ACTIVE == "1" ? "可用" : "不可用";
                        cellTree.ATTRIBUTES = "cell";
                        cellSet.Add(cellTree);
                    }
                    shelfSet.Add(shelfTree);
                }
                set = cellSet;
            }
            else if (type == "cell")
            {
                cells = cells.Where(a => a.CELL_CODE == id).OrderBy(a => a.CELL_CODE).Select(a => a);
                foreach (var cell in cells)//货位
                {
                    NewWareTree cellTree = new NewWareTree();
                    cellTree.CODE = cell.CELL_CODE;
                    cellTree.NAME = "货位：" + cell.CELL_NAME;
                    cellTree.CELL_CODE = cell.CELL_CODE;
                    cellTree.CELL_NAME = cell.CELL_NAME;
                    cellTree.WAREHOUSE_CODE = cell.CMD_WAREHOUSE.WAREHOUSE_CODE;
                    cellTree.WAREHOUSE_NAME = cell.CMD_WAREHOUSE.WAREHOUSE_NAME;
                    cellTree.AREA_CODE = cell.CMD_AREA.AREA_CODE;
                    cellTree.AREA_NAME = cell.CMD_AREA.AREA_NAME;
                    cellTree.SHELF_CODE = cell.CMD_SHELF.SHELF_CODE;
                    cellTree.SHELF_NAME = cell.CMD_SHELF.SHELF_NAME;
                    //cellTree.Type = cell.CellType;
                    cellTree.MEMO = cell.MEMO;
                    cellTree.IS_ACTIVE = cell.IS_ACTIVE == "1" ? "可用" : "不可用";
                    //cellTree.UpdateTime = cell.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    //cellTree.ShortName = cell.ShortName;
                    //cellTree.Layer = cell.Layer;
                    //cellTree.MaxQuantity = cell.MaxQuantity;
                    //cellTree.PRODUCT_NAME = cell.CMD_PRODUCT == null ? string.Empty : cell.CMD_PRODUCT.ProductName;
                    //cellTree.ATTRIBUTES = "cell";
                    cellSet.Add(cellTree);
                }
                set = cellSet;
            }
            else
            {
                if (type == null || type == string.Empty)
                {
                    warehouses = warehouses.Where(w => w.WAREHOUSE_CODE == "001").OrderBy(w => w.WAREHOUSE_CODE).Select(w => w);
                }
                else if (type == "ware")
                {
                    warehouses = warehouses.Where(w => w.WAREHOUSE_CODE == id).OrderBy(w => w.WAREHOUSE_CODE).Select(w => w);
                }
                foreach (var warehouse in warehouses)//仓库
                {
                    NewWareTree NewWareTree = new NewWareTree();
                    NewWareTree.CODE = warehouse.WAREHOUSE_CODE;
                    NewWareTree.NAME = "仓库：" + warehouse.WAREHOUSE_NAME;
                    NewWareTree.WAREHOUSE_CODE = warehouse.WAREHOUSE_NAME;
                    NewWareTree.WAREHOUSE_NAME = warehouse.WAREHOUSE_NAME;
                    //NewWareTree.Type = warehouse.WarehouseType;
                    NewWareTree.MEMO = warehouse.MEMO;
                    //NewWareTree.IS_ACTIVE = warehouse.IsActive == "1" ? "可用" : "不可用";
                    //NewWareTree.UpdateTime = warehouse.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    //NewWareTree.ShortName = warehouse.ShortName;
                    NewWareTree.ATTRIBUTES = "ware";
                    warehouses = warehouses.Where(w => w.WAREHOUSE_CODE == id);
                    wareSet.Add(NewWareTree);
                }
                set = wareSet;
            }
            int total = set.Count();
            var sets = set.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = sets.ToArray() };
        }

        public object GetSingleDetail(string type, string id)
        {
            var warehouses = CMDWarehouseRepository.GetQueryable();
            var areas = CMDAreaRepository.GetQueryable();
            var shelfs = CMDShelfRepository.GetQueryable();
            var cells = CMDCellRepository.GetQueryable();
            
            HashSet<NewWareTree> wareSet = new HashSet<NewWareTree>();
            HashSet<NewWareTree> areaSet = new HashSet<NewWareTree>();
            HashSet<NewWareTree> shelfSet = new HashSet<NewWareTree>();
            HashSet<NewWareTree> cellSet = new HashSet<NewWareTree>();
            var set = wareSet;
            if (type == "area")
            {
                areas = areas.Where(a => a.AREA_CODE == id).OrderBy(a => a.AREA_CODE).Select(a => a);
                foreach (var area in areas)//库区
                {
                    NewWareTree areaTree = new NewWareTree();
                    areaTree.CODE = area.AREA_CODE;
                    areaTree.NAME = "库区：" + area.AREA_NAME;
                    areaTree.AREA_CODE = area.AREA_CODE;
                    areaTree.AREA_NAME = area.AREA_NAME;
                    areaTree.WAREHOUSE_CODE = area.WAREHOUSE_CODE;
                    areaTree.WAREHOUSE_NAME = area.CMD_WAREHOUSE.WAREHOUSE_NAME;
                    areaTree.MEMO = area.MEMO;

                    areaTree.ATTRIBUTES = "area";
                    areaSet.Add(areaTree);
                }
                set = areaSet;
            }
            else if (type == "shelf")
            {
                shelfs = shelfs.Where(a => a.SHELF_CODE == id).OrderBy(a => a.SHELF_CODE).Select(a => a);
                foreach (var shelf in shelfs)//货架
                {
                    NewWareTree shelfTree = new NewWareTree();
                    shelfTree.CODE = shelf.SHELF_CODE;
                    shelfTree.NAME = "库区：" + shelf.SHELF_NAME;
                    shelfTree.AREA_CODE = shelf.AREA_CODE;
                    shelfTree.AREA_NAME = shelf.CMD_AREA.AREA_NAME;
                    shelfTree.WAREHOUSE_CODE = shelf.WAREHOUSE_CODE;
                    shelfTree.WAREHOUSE_NAME = shelf.CMD_WAREHOUSE.WAREHOUSE_NAME;
                    shelfTree.ROW_COUNT = shelf.ROW_COUNT.ToString();
                    shelfTree.COLUMN_COUNT = shelf.COLUMN_COUNT.ToString();
                    shelfTree.SHELF_NAME = shelf.SHELF_NAME;
                    shelfTree.CRANE_NO = shelf.CMD_CRANE.CRANE_NAME;
                    shelfTree.MEMO = shelf.MEMO;
                    shelfTree.ATTRIBUTES = "shelf";
                   
                    shelfSet.Add(shelfTree);
                }
                set = shelfSet;
            }
            else if (type == "cell")
            {
                cells = cells.Where(a => a.CELL_CODE == id).OrderBy(a => a.CELL_CODE).Select(a => a);
                foreach (var cell in cells)//货位
                {
                    NewWareTree cellTree = new NewWareTree();
                    cellTree.CODE = cell.CELL_CODE;
                    cellTree.NAME = "货位：" + cell.CELL_NAME;
                    cellTree.CELL_CODE = cell.CELL_CODE;
                    cellTree.CELL_NAME = cell.CELL_NAME;
                    cellTree.WAREHOUSE_CODE = cell.CMD_WAREHOUSE.WAREHOUSE_CODE;
                    cellTree.WAREHOUSE_NAME = cell.CMD_WAREHOUSE.WAREHOUSE_NAME;
                    cellTree.AREA_CODE = cell.CMD_AREA.AREA_CODE;
                    cellTree.AREA_NAME = cell.CMD_AREA.AREA_NAME;
                    cellTree.SHELF_CODE = cell.CMD_SHELF.SHELF_CODE;
                    cellTree.SHELF_NAME = cell.CMD_SHELF.SHELF_NAME;
                    cellTree.ROW_COUNT = cell.CELL_ROW.ToString();
                    cellTree.COLUMN_COUNT = cell.CELL_COLUMN.ToString();
                    cellTree.MEMO = cell.MEMO;
                    cellTree.IS_ACTIVE = cell.IS_ACTIVE == "1" ? "可用" : "不可用";
                    //cellTree.UpdateTime = cell.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    //cellTree.ShortName = cell.ShortName;
                    //cellTree.Layer = cell.Layer;
                    //cellTree.MaxQuantity = cell.MaxQuantity;
                    //cellTree.PRODUCT_NAME = cell.CMD_PRODUCT == null ? string.Empty : cell.CMD_PRODUCT.ProductName;
                    cellTree.ATTRIBUTES = "cell";
                    cellSet.Add(cellTree);
                }
                set = cellSet;
            }
            else if (type == "ware")
            {
                warehouses = warehouses.Where(w => w.WAREHOUSE_CODE == id).OrderBy(w => w.WAREHOUSE_CODE).Select(w => w);
                foreach (var warehouse in warehouses)//仓库
                {
                    NewWareTree NewWareTree = new NewWareTree();
                    NewWareTree.CODE = warehouse.WAREHOUSE_CODE;
                    NewWareTree.NAME = "仓库：" + warehouse.WAREHOUSE_NAME;
                    NewWareTree.WAREHOUSE_CODE = warehouse.WAREHOUSE_NAME;
                    NewWareTree.WAREHOUSE_NAME = warehouse.WAREHOUSE_NAME;
                    //NewWareTree.Type = warehouse.WarehouseType;
                    NewWareTree.MEMO = warehouse.MEMO;
                    //NewWareTree.IS_ACTIVE = warehouse.IsActive == "1" ? "可用" : "不可用";
                    //NewWareTree.UpdateTime = warehouse.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    //NewWareTree.ShortName = warehouse.ShortName;
                    NewWareTree.ATTRIBUTES = "ware";
                    warehouses = warehouses.Where(w => w.WAREHOUSE_CODE == id);
                    wareSet.Add(NewWareTree);
                }
                set = wareSet;
            }
            return set.ToArray();
        }
        public bool Add(CMD_CELL cell, out string errorInfo)
        {
            errorInfo = string.Empty;
            var cellAdd = new CMD_CELL();
            var warehouse = CMDWarehouseRepository.GetQueryable().FirstOrDefault(w => w.WAREHOUSE_CODE == cell.WAREHOUSE_CODE);
            var area = CMDAreaRepository.GetQueryable().FirstOrDefault(a => a.AREA_CODE == cell.AREA_CODE);
            var shelf = CMDShelfRepository.GetQueryable().FirstOrDefault(s => s.SHELF_CODE == cell.SHELF_CODE);
             var product = CMDProductRepository.GetQueryable().FirstOrDefault(p => p.PRODUCT_CODE  == cell.PRODUCT_CODE );
            //string cellCodeStr = cell.CELL_CODE + "-" + cell.Layer;
            //var cellCode = CMDCellRepository.GetQueryable().FirstOrDefault(c => c.CellCode == cellCodeStr);
            var cellCode = CMDCellRepository.GetQueryable().FirstOrDefault(c => c.CELL_CODE == cell.CELL_CODE);
            if (cellCode == null)
            {
                //cellAdd.CellCode = cell.CellCode + "-" + cell.Layer;
                cellAdd.CELL_CODE = cell.CELL_CODE;
                cellAdd.CELL_NAME = cell.CELL_NAME;
                //cellAdd.ShortName = cell.ShortName;
                //cellAdd.CellType = cell.CellType;
                //cellAdd.Layer = cell.Layer;
                //cellAdd.Col = cell.Col;
                //cellAdd.ImgX = cell.ImgX;
                //cellAdd.ImgY = cell.ImgY;
                //cellAdd.Rfid = cell.Rfid;
                cellAdd.CMD_WAREHOUSE = warehouse;
                cellAdd.CMD_AREA = area;
                cellAdd.CMD_SHELF = shelf;
                cellAdd.CMD_PRODUCT = product;
                //cellAdd.MaxQuantity = cell.MaxQuantity;
                //cellAdd.IsSingle = cell.IsSingle;
                cellAdd.MEMO = cell.MEMO;
                cellAdd.IS_ACTIVE = cell.IS_ACTIVE;
                //cellAdd.UpdateTime = DateTime.Now;

                CMDCellRepository.Add(cellAdd);
                CMDCellRepository.SaveChanges();
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
            var cell = CMDCellRepository.GetQueryable()
                .FirstOrDefault(c => c.CELL_CODE == cellCode);
            if (cell != null)
            {
                CMDCellRepository.Delete(cell);
                CMDCellRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(CMD_CELL cell)
        {
            var cellSave = CMDCellRepository.GetQueryable().FirstOrDefault(c => c.CELL_CODE == cell.CELL_CODE);
            var warehouse = CMDWarehouseRepository.GetQueryable().FirstOrDefault(w => w.WAREHOUSE_CODE == cell.WAREHOUSE_CODE);
            var area = CMDAreaRepository.GetQueryable().FirstOrDefault(a => a.AREA_CODE == cell.AREA_CODE);
            var shelf = CMDShelfRepository.GetQueryable().FirstOrDefault(s => s.SHELF_CODE == cell.SHELF_CODE);
            var product = CMDProductRepository.GetQueryable().FirstOrDefault(p => p.PRODUCT_CODE == cell.PRODUCT_CODE);
            cellSave.CELL_CODE = cellSave.CELL_CODE;
            cellSave.CELL_NAME = cell.CELL_NAME;
            //cellSave.ShortName = cell.ShortName;
            //cellSave.CellType = cell.CellType;
            //cellSave.Layer = cell.Layer;
            //cellSave.Col = cell.Col;
            //cellSave.ImgX = cell.ImgX;
            //cellSave.ImgY = cell.ImgY;
            //cellSave.Rfid = cell.Rfid;
            cellSave.CMD_WAREHOUSE = warehouse;
            cellSave.CMD_AREA = area;
            cellSave.CMD_SHELF = shelf;
            cellSave.CMD_PRODUCT = product;
            //cellSave.MaxQuantity = cell.MaxQuantity;
            //cellSave.IsSingle = cell.IsSingle;
            cellSave.MEMO = cell.MEMO;
            cellSave.IS_ACTIVE = cell.IS_ACTIVE;
            //cellSave.UpdateTime = DateTime.Now;

            CMDCellRepository.SaveChanges();
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
                IQueryable<CMD_CELL> cellQuery = CMDCellRepository.GetQueryable();

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
                    CMDCellRepository.GetObjectSet()
                        .UpdateEntity(
                            c => c.PRODUCT_CODE == defaultProductCode,
                            c => new CMD_CELL() { PRODUCT_CODE = null }
                        );
                }
                CMDCellRepository.GetObjectSet()
                    .UpdateEntity(
                        c => wareCodes.Contains(c.CMD_WAREHOUSE.WAREHOUSE_CODE)
                            || areaCodes.Contains(c.CMD_AREA.AREA_CODE)
                            || shelfCodes.Contains(c.SHELF_CODE)
                            || cellCodes.Contains(c.CELL_CODE),
                        c => new CMD_CELL() { PRODUCT_CODE = defaultProductCode }
                    );
                return true;
            }
            catch (Exception e)
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

        public bool UpdateTree(string type, string id, bool isCheck, string proCode2)
        {
            bool result = false;

            if (type == "cell")
            {
                IQueryable<CMD_CELL> queryCell = CMDCellRepository.GetQueryable();
                var cell = queryCell.FirstOrDefault(i => i.CELL_CODE == id);
                if (isCheck == true)
                {
                    cell.PRODUCT_CODE = proCode2;
                }
                else
                {
                    cell.PRODUCT_CODE  = null;
                }
                CMDCellRepository.SaveChanges();
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
            CMDCellRepository.GetObjectSet()
                .UpdateEntity(
                    c => productCodes.Contains(c.PRODUCT_CODE ),
                    c => new CMD_CELL() { PRODUCT_CODE = null }
                );
            return true;
        }

        /// <summary>加载卷烟信息</summary>
        public object GetCellInfo()
        {
            IQueryable<CMD_CELL> cellQuery = CMDCellRepository.GetQueryable();

            var cellInfo = cellQuery.Where(c1 => c1.PRODUCT_CODE != null)
                .GroupBy(c2 => c2.CMD_PRODUCT)
                .Select(c3 => new
                {
                    ProductCode = c3.Key.PRODUCT_CODE ,
                    ProductName = c3.Key.PRODUCT_NAME,
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
            IQueryable<CMD_CELL> cellQuery = CMDCellRepository.GetQueryable();
            var cell = cellQuery.Where(c => c.CMD_PRODUCT != null && c.PRODUCT_CODE .Contains(productCode) && c.CMD_PRODUCT.PRODUCT_NAME.Contains(productName))
                 .GroupBy(c => c.CMD_PRODUCT)
                 .Select(c => new
                 {
                     ProductCode = c.Key.PRODUCT_CODE ,
                     ProductName = c.Key.PRODUCT_NAME,
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
            IQueryable<CMD_CELL> cellQuery = CMDCellRepository.GetQueryable();
            var cell = cellQuery.Where(c => c.CMD_PRODUCT != null && c.PRODUCT_CODE .Contains(productCode) && c.CMD_PRODUCT.PRODUCT_NAME.Contains(productName))
                 .GroupBy(c => c.CMD_PRODUCT)
                 .Select(c => new
                 {
                     ProductCode = c.Key.PRODUCT_CODE ,
                     ProductName = c.Key.PRODUCT_NAME,
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
                        item.ProductCode ,
                        item.ProductName,
                        item.ProductQuantity
                    );
            }
            return dt;
        }

        /// <summary>查找卷烟信息</summary>
        public object GetCellInfo(string productCode)
        {
            IQueryable<CMD_CELL> cellQuery = CMDCellRepository.GetQueryable();
            var cellInfo = cellQuery.Where(c1 => c1.CMD_PRODUCT != null && c1.PRODUCT_CODE  == productCode)
                .GroupBy(c => c.CMD_PRODUCT)
                .Select(c => new
                {
                    ProductCode = c.Key.PRODUCT_CODE ,
                    ProductName = c.Key.PRODUCT_NAME,
                    ProductQuantity = c.Count()
                });
            return cellInfo;
        }

        /// <summary>编辑储位货位树形菜单</summary>
        public object GetCellCheck(string productCode)
        {
            var warehouses = CMDWarehouseRepository.GetQueryable();
            var areas = CMDAreaRepository.GetQueryable();
            var shelfs = CMDShelfRepository.GetQueryable();
            var cells = CMDCellRepository.GetQueryable();

            var tmp = warehouses.Join(areas,
                                     w => w.WAREHOUSE_CODE,
                                     a => a.WAREHOUSE_CODE,
                                     (w, a) => new { w.WAREHOUSE_CODE, w.WAREHOUSE_NAME, a.AREA_CODE, a.AREA_NAME }
                                 )
                                 .Join(shelfs,
                                    a => a.AREA_CODE,
                                    s => s.AREA_CODE,
                                    (a, s) => new { a.WAREHOUSE_CODE, a.WAREHOUSE_NAME, a.AREA_CODE, a.AREA_NAME, s.SHELF_CODE, s.SHELF_NAME }
                                 )
                                 .Join(cells,
                                    s => s.SHELF_CODE,
                                    c => c.SHELF_CODE,
                                    (s, c) => new { s.WAREHOUSE_CODE, s.WAREHOUSE_NAME, s.AREA_CODE, s.AREA_NAME, s.SHELF_CODE, s.SHELF_NAME, c.CELL_CODE, c.CELL_NAME, c }
                                 )
                                 .GroupBy(c => new { c.WAREHOUSE_CODE, c.WAREHOUSE_NAME })
                                 .AsParallel()
                                 .Select(w => new
                                 {
                                     id = w.Key.WAREHOUSE_CODE,
                                     name = w.Key.WAREHOUSE_NAME,
                                     type = "ware",
                                     // @checked = w.Any(c => c.c.PRODUCT_CODE  == productCode),
                                     open = true,
                                     children = w.GroupBy(a => new { a.AREA_CODE, a.AREA_NAME })
                                                 .Select(a => new
                                                 {
                                                     id = a.Key.AREA_CODE,
                                                     name = a.Key.AREA_NAME,
                                                     type = "area",
                                                     //@checked = a.Any(c => c.c.PRODUCT_CODE  == productCode),
                                                     open = false,
                                                     children = a.GroupBy(s => new { s.SHELF_CODE, s.SHELF_NAME })
                                                                 .Select(s => new
                                                                 {
                                                                     id = s.Key.SHELF_CODE,
                                                                     name = s.Key.SHELF_NAME,
                                                                     type = "shelf",
                                                                     //@checked = s.Any(c => c.c.PRODUCT_CODE  == productCode),
                                                                     open = false,
                                                                     children = s.GroupBy(c => new { c.CELL_CODE, c.CELL_NAME })
                                                                                 .Select(c => new
                                                                                 {
                                                                                     id = c.Key.CELL_CODE,
                                                                                     name = c.Key.CELL_NAME,
                                                                                     type = "cell",
                                                                                     //@checked = c.Any(r => r.c.PRODUCT_CODE  == productCode),
                                                                                     open = false,
                                                                                     children = ""
                                                                                 })
                                                                                 .OrderBy(c=>c.id)
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
            var warehouses = CMDWarehouseRepository.GetQueryable().AsEnumerable();
            HashSet<Tree> wareSet = new HashSet<Tree>();
            if (shelfCode == null || shelfCode == string.Empty)//判断是否是加载货位
            {
                foreach (var warehouse in warehouses)//仓库
                {
                    Tree NewWareTree = new Tree();
                    NewWareTree.id = warehouse.WAREHOUSE_CODE;
                    NewWareTree.text = "仓库：" + warehouse.WAREHOUSE_NAME;
                    NewWareTree.state = "open";
                    NewWareTree.attributes = "ware";

                    var areas = CMDAreaRepository.GetQueryable().Where(a => a.CMD_WAREHOUSE.WAREHOUSE_CODE == warehouse.WAREHOUSE_CODE)
                                                             .OrderBy(a => a.AREA_CODE).Select(a => a);
                    HashSet<Tree> areaSet = new HashSet<Tree>();
                    foreach (var area in areas)//库区
                    {
                        Tree areaTree = new Tree();
                        areaTree.id = area.AREA_CODE;
                        areaTree.text = "库区：" + area.AREA_NAME;
                        areaTree.state = "closed";
                        areaTree.attributes = "area";

                        var shelfs = CMDShelfRepository.GetQueryable().Where(s => s.CMD_AREA.AREA_CODE == area.AREA_CODE)
                                                                   .OrderBy(s => s.SHELF_CODE).Select(s => s);
                        HashSet<Tree> shelfSet = new HashSet<Tree>();
                        foreach (var shelf in shelfs)//货架
                        {
                            Tree shelfTree = new Tree();
                            shelfTree.id = shelf.SHELF_CODE;
                            shelfTree.text = "货架：" + shelf.SHELF_NAME;
                            shelfTree.attributes = "shelf";
                            shelfTree.state = "closed";
                            shelfSet.Add(shelfTree);
                        }
                        areaTree.children = shelfSet.ToArray();
                        areaSet.Add(areaTree);
                    }
                    NewWareTree.children = areaSet.ToArray();
                    wareSet.Add(NewWareTree);
                }
            }
            else
            {
                var cells = CMDCellRepository.GetQueryable().Where(c => c.CMD_SHELF.SHELF_CODE == shelfCode)
                                                         .OrderBy(c => c.CELL_CODE).Select(c => c);
                foreach (var cell in cells)//货位
                {
                    // var product = ProductRepository.GetQueryable().FirstOrDefault(p => p.PRODUCT_CODE  == cell.PRODUCT_CODE );
                    Tree cellTree = new Tree();
                    cellTree.id = cell.CELL_CODE;
                    cellTree.text = "货位：" + cell.CELL_NAME;
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
            IQueryable<CMD_CELL> cellQuery = CMDCellRepository.GetQueryable();
            var cell = cellQuery.Where(c => c.CELL_CODE == cellCode).OrderBy(b => b.CELL_CODE).AsEnumerable()
                                .Select(b => new { b.CELL_CODE, b.CELL_NAME, b.MEMO, b.CMD_WAREHOUSE.WAREHOUSE_NAME, b.CMD_WAREHOUSE.WAREHOUSE_CODE, b.CMD_AREA.AREA_CODE, b.CMD_AREA.AREA_NAME, b.CMD_SHELF.SHELF_CODE, b.CMD_SHELF.SHELF_NAME,b.CELL_ROW,b.CELL_COLUMN, DefaultProductCode = b.CMD_PRODUCT == null ? string.Empty : b.CMD_PRODUCT.PRODUCT_CODE , ProductName = b.CMD_PRODUCT == null ? string.Empty : b.CMD_PRODUCT.PRODUCT_NAME, IsActive = b.IS_ACTIVE == "1" ? "可用" : "不可用" });
            return cell.First(c => c.CELL_CODE == cellCode);
        }

        /// <summary>
        /// 仓库设置时用的TreeGrid的树形结构，可根据仓库Code查询
        /// </summary>
        /// <param name="wareCode">仓库Code</param>
        /// <returns></returns>
        public object GetSearch(string wareCode)
        {
            var warehouses = CMDWarehouseRepository.GetQueryable().AsEnumerable();
            if (wareCode != null && wareCode != string.Empty)
            {
                warehouses = warehouses.Where(w => w.WAREHOUSE_CODE == wareCode);
            }

            HashSet<NewWareTree> wareSet = new HashSet<NewWareTree>();
            foreach (var warehouse in warehouses)//仓库
            {
                NewWareTree NewWareTree = new NewWareTree();
                NewWareTree.CODE = warehouse.WAREHOUSE_CODE;
                NewWareTree.NAME = "仓库：" + warehouse.WAREHOUSE_NAME;
                NewWareTree.WAREHOUSE_CODE = warehouse.WAREHOUSE_CODE;
                NewWareTree.WAREHOUSE_NAME = warehouse.WAREHOUSE_NAME;
                //NewWareTree.Type = warehouse.WarehouseType;
                NewWareTree.MEMO = warehouse.MEMO;
                //NewWareTree.IS_ACTIVE = warehouse.IsActive == "1" ? "可用" : "不可用";
                //NewWareTree.UpdateTime = warehouse.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                //NewWareTree.ShortName = warehouse.ShortName;
                NewWareTree.ATTRIBUTES = "ware";
                var areas = CMDAreaRepository.GetQueryable().Where(a => a.CMD_WAREHOUSE.WAREHOUSE_CODE == warehouse.WAREHOUSE_CODE)
                                                        .OrderBy(a => a.AREA_CODE).Select(a => a);
                HashSet<NewWareTree> areaSet = new HashSet<NewWareTree>();
                foreach (var area in areas)//库区
                {
                    NewWareTree areaTree = new NewWareTree();
                    areaTree.CODE = area.AREA_CODE;
                    areaTree.NAME = "库区：" + area.AREA_NAME;
                    areaTree.AREA_CODE = area.AREA_CODE;
                    areaTree.AREA_NAME = area.AREA_NAME;
                    areaTree.WAREHOUSE_CODE = area.CMD_WAREHOUSE.WAREHOUSE_CODE;
                    areaTree.WAREHOUSE_NAME = area.CMD_WAREHOUSE.WAREHOUSE_NAME;
                    //areaTree.Type = area.AreaType;
                    areaTree.MEMO = area.MEMO;
                    //areaTree.IsActive = area.IsActive == "1" ? "可用" : "不可用";
                    //areaTree.UpdateTime = area.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    //areaTree.ShortName = area.ShortName;
                    //areaTree.AllotInOrder = area.AllotInOrder;
                    //areaTree.AllotOutOrder = area.AllotOutOrder;
                    areaTree.ATTRIBUTES = "area";
                    var shelfs = CMDShelfRepository.GetQueryable().Where(s => s.CMD_AREA.AREA_CODE == area.AREA_CODE)
                                                               .OrderBy(s => s.SHELF_CODE).Select(s => s);
                    HashSet<NewWareTree> shelfSet = new HashSet<NewWareTree>();
                    foreach (var shelf in shelfs)//货架
                    {
                        NewWareTree shelfTree = new NewWareTree();
                        shelfTree.CODE = shelf.SHELF_CODE;
                        shelfTree.NAME = "货架：" + shelf.SHELF_NAME;
                        shelfTree.SHELF_CODE = shelf.SHELF_CODE;
                        shelfTree.SHELF_NAME = shelf.SHELF_NAME;

                        shelfTree.WAREHOUSE_CODE = shelf.CMD_WAREHOUSE.WAREHOUSE_CODE;
                        shelfTree.WAREHOUSE_NAME = shelf.CMD_WAREHOUSE.WAREHOUSE_NAME;
                        shelfTree.AREA_CODE = shelf.CMD_AREA.AREA_CODE;
                        shelfTree.AREA_NAME = shelf.CMD_AREA.AREA_NAME;



                       // shelfTree.Type = shelf.ShelfType;
                        shelfTree.MEMO = shelf.MEMO;
                        //shelfTree.IsActive = shelf.IsActive == "1" ? "可用" : "不可用";
                        //shelfTree.UpdateTime = shelf.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                        //shelfTree.ShortName = shelf.ShortName;
                        shelfTree.ATTRIBUTES = "shelf";
                        shelfSet.Add(shelfTree);
                    }
                    areaTree.children = shelfSet.ToArray();
                    areaSet.Add(areaTree);
                }
                NewWareTree.children = areaSet.ToArray();
                wareSet.Add(NewWareTree);
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
            HashSet<NewWareTree> wareSet = new HashSet<NewWareTree>();
            var cells = CMDCellRepository.GetQueryable().Where(c => c.CMD_SHELF.SHELF_CODE == shelfCode)
                                                     .OrderBy(c => c.CELL_CODE).Select(c => c);
            foreach (var cell in cells)//货位
            {
                NewWareTree cellTree = new NewWareTree();
                cellTree.CODE = cell.CELL_CODE;
                cellTree.NAME = "货位：" + cell.CELL_NAME;
                cellTree.CELL_CODE = cell.CELL_CODE;
                cellTree.CELL_NAME = cell.CELL_NAME;

                cellTree.WAREHOUSE_CODE = cell.CMD_WAREHOUSE.WAREHOUSE_CODE;
                cellTree.WAREHOUSE_NAME = cell.CMD_WAREHOUSE.WAREHOUSE_NAME;

                cellTree.AREA_CODE = cell.CMD_AREA.AREA_CODE;
                cellTree.AREA_NAME = cell.CMD_AREA.AREA_NAME;

                cellTree.SHELF_CODE = cell.CMD_SHELF.SHELF_CODE;
                cellTree.SHELF_NAME = cell.CMD_SHELF.SHELF_NAME;

                //cellTree.Type = cell.CellType;
                cellTree.MEMO = cell.MEMO;
                cellTree.IS_ACTIVE = cell.IS_ACTIVE == "1" ? "可用" : "不可用";
                //cellTree.UpdateTime = cell.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                //cellTree.ShortName = cell.ShortName;
                //cellTree.Layer = cell.Layer;
                //cellTree.MaxQuantity = cell.MaxQuantity;
                //cellTree.PRODUCT_NAME = cell.CMD_PRODUCT == null ? string.Empty : cell.CMD_PRODUCT.ProductName;
                cellTree.ATTRIBUTES = "cell";

                wareSet.Add(cellTree);
            }
            return wareSet;
        }

        ///// <summary>
        ///// 移库时用的树形结构数据，可根据货架Code查询，根据移出的货位和移入的货位查询-移库单使用
        ///// </summary>
        ///// <param name="shelfCode">货架Code</param>
        ///// <param name="inOrOut">移入还是移出</param>
        ///// <param name="productCode">产品代码</param>
        ///// <returns></returns>
        //public object GetMoveCellDetails(string shelfCode, string inOrOut, string productCode)
        //{
        //    var warehouses = CMDWarehouseRepository.GetQueryable().AsEnumerable();
        //    HashSet<Tree> wareSet = new HashSet<Tree>();
        //    if (shelfCode == null || shelfCode == string.Empty)//判断是否是加载货位
        //    {
        //        foreach (var warehouse in warehouses)//仓库
        //        {
        //            Tree NewWareTree = new Tree();
        //            NewWareTree.id = warehouse.WAREHOUSE_CODE;
        //            NewWareTree.text = "仓库：" + warehouse.WAREHOUSE_NAME;
        //            NewWareTree.state = "open";
        //            NewWareTree.attributes = "ware";

        //            var areas = CMDAreaRepository.GetQueryable().Where(a => a.CMD_WAREHOUSE.WAREHOUSE_CODE == warehouse.WAREHOUSE_CODE)
        //                                                     .OrderBy(a => a.AREA_CODE).Select(a => a);
        //            HashSet<Tree> areaSet = new HashSet<Tree>();
        //            foreach (var area in areas)//库区
        //            {
        //                Tree areaTree = new Tree();
        //                areaTree.id = area.AREA_CODE;
        //                areaTree.text = "库区：" + area.AREA_NAME;
        //                areaTree.state = "open";
        //                areaTree.attributes = "area";

        //                var shelfs = CMDShelfRepository.GetQueryable().Where(s => s.CMD_AREA.AREA_CODE == area.AREA_CODE)
        //                                                           .OrderBy(s => s.SHELF_CODE).Select(s => s);
        //                HashSet<Tree> shelfSet = new HashSet<Tree>();
        //                foreach (var shelf in shelfs)//货架
        //                {
        //                    Tree shelfTree = new Tree();
        //                    shelfTree.id = shelf.SHELF_CODE;
        //                    shelfTree.text = "货架：" + shelf.SHELF_NAME;
        //                    shelfTree.attributes = "shelf";
        //                    shelfTree.state = "closed";
        //                    shelfSet.Add(shelfTree);
        //                }
        //                areaTree.children = shelfSet.ToArray();
        //                areaSet.Add(areaTree);
        //            }
        //            NewWareTree.children = areaSet.ToArray();
        //            wareSet.Add(NewWareTree);
        //        }
        //    }
        //    else
        //    {
        //        var cells = CMDCellRepository.GetQueryable().Where(c => c.CELL_CODE == c.CELL_CODE);
        //        if (inOrOut == "out")// 查询出可以移出卷烟的货位
        //        {
        //           // var storages = StorageRepository.GetQueryable().Where(s => (s.Quantity - s.OutFrozenQuantity) > 0 && string.IsNullOrEmpty(s.Cell.LockTag)).Select(s => s.CellCode);
        //            cells = cells.Where(c => c.CMD_SHELF.SHELF_CODE == shelfCode && storages.Any(s => s == c.CELL_CODE) && c.IS_ACTIVE == "1")
        //                                                 .OrderBy(s => s.CELL_CODE);
        //        }
        //        else if (inOrOut == "in")//查询出可以移入卷烟的货位 ,
        //        {
        //            var cellf = cells.Where(c => c.CMD_SHELF.SHELF_CODE == shelfCode && c.IsSingle == "0" && c.IS_ACTIVE == "1").OrderBy(c => c.CellCode).Select(c => c);
        //            cells = cells.Where(c => c.CMD_SHELF.SHELF_CODE == shelfCode && c.IS_ACTIVE == "1"
        //                                    && (c.Storages.Count == 0 || c.Storages.Any(s => (s.PRODUCT_CODE  == productCode && (c.MaxQuantity * s.CMD_PRODUCT.Unit.Count) - (s.Quantity + s.InFrozenQuantity) > 0))
        //                                    || c.Storages.Sum(s => s.Quantity + s.InFrozenQuantity) == 0) && c.IsSingle == "1")
        //                                     .OrderBy(c => c.CellCode).Select(c => c);
        //            foreach (var cell in cellf)//货位
        //            {
        //                string quantityStr = "";
        //                if (inOrOut == "in")
        //                {
        //                    if (cell.Storages.Count != 0 && cell.Storages.Sum(s => s.Quantity + s.InFrozenQuantity) != 0)
        //                    {
        //                        var cellQuantity = cell.Storages.GroupBy(g => new { g.Cell }).Select(s => new { quan = s.Key.Cell.CMD_PRODUCT == null ? s.Key.Cell.MaxQuantity : ((s.Key.Cell.MaxQuantity * s.Key.Cell.CMD_PRODUCT.Unit.Count) - s.Sum(p => p.Quantity + p.InFrozenQuantity)) / s.Key.Cell.CMD_PRODUCT.Unit.Count });
        //                        decimal quantity = Convert.ToDecimal(cellQuantity.Sum(s => s.quan));
        //                        // quantity = Math.Round(quantity, 2);
        //                        quantityStr = "<可入：" + quantity + ">件,当前品牌：" + productCode + "";
        //                    }
        //                    else
        //                        quantityStr = "<可入：" + cell.MaxQuantity + ">件";
        //                }

        //                Tree cellTree = new Tree();
        //                cellTree.id = cell.CELL_CODE;
        //                cellTree.text = "货位：" + cell.CELL_NAME + quantityStr;
        //                cellTree.state = "open";
        //                cellTree.attributes = "cell";
        //                wareSet.Add(cellTree);
        //            }
        //        }
        //        else if (inOrOut == "stockOut")//查询可以出库的数量 --出库使用
        //        {
        //            var storages = StorageRepository.GetQueryable().Where(s => (s.Quantity - s.OutFrozenQuantity) > 0
        //                                                            && string.IsNullOrEmpty(s.Cell.LockTag)
        //                                                            && s.PRODUCT_CODE  == productCode)
        //                                                            .Select(s => s.CellCode);
        //            cells = cells.Where(c => c.CMD_SHELF.SHELF_CODE == shelfCode && storages.Any(s => s == c.CELL_CODE) && c.IS_ACTIVE == "1").OrderBy(c => c.CellCode);
        //        }
        //        else if (inOrOut == "moveIn")//查询出非货位管理的货位用于入库单非货位管理的移入
        //        {
        //            cells = cells.Where(c => c.CMD_SHELF.SHELF_CODE == shelfCode && c.IsSingle == "0" && c.IS_ACTIVE == "1").OrderBy(c => c.CellCode).Select(c => c);
        //        }
        //        foreach (var cell in cells)//货位
        //        {
        //            string quantityStr = "";
        //            if (inOrOut == "in")
        //            {
        //                if (cell.Storages.Count != 0 && cell.Storages.Sum(s => s.Quantity + s.InFrozenQuantity) != 0)
        //                {
        //                    var cellQuantity = cell.Storages.GroupBy(g => new { g.Cell }).Select(s => new { quan = s.Key.Cell.Storages.Max(t => t.PRODUCT_CODE ) == null ? s.Key.Cell.MaxQuantity : ((s.Key.Cell.MaxQuantity * s.Key.Cell.Storages.Max(t => t.CMD_PRODUCT.Unit.Count)) - s.Sum(p => p.Quantity + p.InFrozenQuantity)) / s.Key.Cell.Storages.Max(t => t.CMD_PRODUCT.Unit.Count) });
        //                    decimal quantity = Convert.ToDecimal(cellQuantity.Sum(s => s.quan));
        //                    // quantity = Math.Round(quantity, 2);
        //                    quantityStr = "<可入：" + quantity + ">件,当前品牌：" + productCode + "";
        //                }
        //                else
        //                    quantityStr = "<可入：" + cell.MaxQuantity + ">件";
        //            }

        //            Tree cellTree = new Tree();
        //            cellTree.id = cell.CELL_CODE;
        //            cellTree.text = "货位：" + cell.CELL_NAME + quantityStr;
        //            cellTree.state = "open";
        //            cellTree.attributes = "cell";
        //            wareSet.Add(cellTree);
        //        }
        //    }
        //    return wareSet.ToArray();
        //}

        /// <summary>
        /// 查询库区，用于分拣设置货位
        /// </summary>
        /// <param name="areaType">库区类型</param>
        /// <returns></returns>
        public object GetSortCell(string areaCode)
        {
            var areas = CMDAreaRepository.GetQueryable().Where(a => a.AREA_CODE == areaCode)
                                                     .OrderBy(a => a.AREA_CODE).Select(a => a);
            HashSet<Tree> areaSet = new HashSet<Tree>();
            foreach (var area in areas)//库区
            {
                Tree areaTree = new Tree();
                areaTree.id = area.AREA_CODE;
                areaTree.text = "库区：" + area.AREA_NAME;
                areaTree.state = "open";
                areaTree.attributes = "area";

                var shelfs = CMDShelfRepository.GetQueryable().Where(s => s.CMD_AREA.AREA_CODE == area.AREA_CODE)
                                                           .OrderBy(s => s.SHELF_CODE).Select(s => s);
                HashSet<Tree> shelfSet = new HashSet<Tree>();
                foreach (var shelf in shelfs)//货架
                {
                    Tree shelfTree = new Tree();
                    shelfTree.id = shelf.SHELF_CODE;
                    shelfTree.text = "货架：" + shelf.SHELF_NAME;
                    shelfTree.attributes = "shelf";


                    var cells = CMDCellRepository.GetQueryable().Where(c => c.CMD_SHELF.SHELF_CODE == shelf.SHELF_CODE)
                                                             .OrderBy(c => c.CELL_CODE).Select(c => c);
                    HashSet<Tree> cellSet = new HashSet<Tree>();
                    foreach (var cell in cells)//货位
                    {
                        Tree cellTree = new Tree();
                        cellTree.id = cell.CELL_CODE;
                        cellTree.text = cell.CELL_NAME;
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

        public System.Data.DataTable GetProductCell(string queryString, string value)
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

            IQueryable<CMD_CELL> cellQuery = CMDCellRepository.GetQueryable();

            var cellInfo = cellQuery.Where(c => c.CMD_PRODUCT != null && c.PRODUCT_CODE .Contains(productCode) && c.CMD_PRODUCT.PRODUCT_NAME.Contains(productName))
                .GroupBy(c => c.CMD_PRODUCT)
                .Select(c => new
                {
                    ProductCode = c.Key.PRODUCT_CODE ,
                    ProductName = c.Key.PRODUCT_NAME,
                    ProductQuantity = c.Count()
                });

            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("卷烟编码", typeof(string));
            dt.Columns.Add("卷烟名称", typeof(string));
            dt.Columns.Add("货位数量", typeof(string));

            foreach (var item in cellInfo)
            {
                dt.Rows.Add(item.ProductCode , item.ProductName, item.ProductQuantity);
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
            IQueryable<CMD_CELL> cellQuery = CMDCellRepository.GetQueryable();
            var cellCode = cellQuery.Where(c => c.SHELF_CODE == shelfCode).Max(c => c.CELL_CODE);
            if (cellCode == string.Empty || cellCode == null)
            {
                cellCodeStr = CMDCellRepository.GetNewID("CMD_CELL", "CELL_CODE");
            }
            else
            {
              
            }
            return cellCodeStr;
        }

        public System.Data.DataTable GetCell(int page, int rows, string type, string id)
        {
            var warehouses = CMDWarehouseRepository.GetQueryable();
            var areas = CMDAreaRepository.GetQueryable();
            var shelfs = CMDShelfRepository.GetQueryable();
            var cells = CMDCellRepository.GetQueryable();
            HashSet<NewWareTree> wareSet = new HashSet<NewWareTree>();
            HashSet<NewWareTree> areaSet = new HashSet<NewWareTree>();
            HashSet<NewWareTree> shelfSet = new HashSet<NewWareTree>();
            HashSet<NewWareTree> cellSet = new HashSet<NewWareTree>();
            System.Data.DataTable dt = new System.Data.DataTable();
            var set = wareSet;
            if (type == "area")
            {
                areas = areas.Where(a => a.AREA_CODE == id).OrderBy(a => a.AREA_CODE).Select(a => a);

                dt.Columns.Add("区域名称", typeof(string));
                dt.Columns.Add("仓库名称", typeof(string));
                dt.Columns.Add("备注", typeof(string));
                foreach (var area in areas)//库区
                {
                    //NewWareTree areaTree = new NewWareTree();
                    //areaTree.CODE = area.AREA_CODE;
                    ////areaTree.NAME = "库区：" + area.AREA_NAME;
                    //areaTree.NAME = area.AREA_NAME;
                    //areaTree.AREA_CODE = area.AREA_CODE;
                    //areaTree.AREA_NAME = area.AREA_NAME;
                    //areaTree.WAREHOUSE_CODE = area.CMD_WAREHOUSE.WAREHOUSE_CODE;
                    //areaTree.WAREHOUSE_NAME = area.CMD_WAREHOUSE.WAREHOUSE_NAME;
                    ////areaTree.Type = area.AreaType;
                    //areaTree.MEMO = area.MEMO;
                    ////areaTree.IsActive = area.IsActive == "1" ? "可用" : "不可用";
                    ////areaTree.UpdateTime = area.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    ////areaTree.ShortName = area.ShortName;
                    ////areaTree.AllotInOrder = area.AllotInOrder;
                    ////areaTree.AllotOutOrder = area.AllotOutOrder;
                    //areaTree.ATTRIBUTES = "area";
                    //areaSet.Add(areaTree);
                    dt.Rows.Add(area .AREA_NAME ,area .CMD_WAREHOUSE .WAREHOUSE_NAME ,area .MEMO);
                }
                //set = areaSet;
            }
            else if (type == "shelf")
            {
                shelfs = shelfs.Where(a => a.SHELF_CODE == id).OrderBy(a => a.SHELF_CODE).Select(a => a);
                cells = CMDCellRepository.GetQueryable().Where(c => c.CMD_SHELF.SHELF_CODE == id)
                                                    .OrderBy(c => c.CELL_CODE).Select(c => c);
                dt.Columns.Add("货位名称", typeof(string));
                dt.Columns.Add("所在列", typeof(string));
                dt.Columns.Add("所在层", typeof(string));
                dt.Columns.Add("库区名称", typeof(string));
                dt.Columns.Add("货架名称", typeof(string));
                dt.Columns.Add("状        态", typeof(string));
                dt.Columns.Add("备注", typeof(string));
                foreach (var shelf in shelfs)//货架
                {
                    NewWareTree shelfTree = new NewWareTree();
                    shelfTree.CODE = shelf.SHELF_CODE;
                    shelfTree.NAME = "货架：" + shelf.SHELF_NAME;
                    shelfTree.SHELF_CODE = shelf.SHELF_CODE;
                    shelfTree.SHELF_NAME = shelf.SHELF_NAME;
                    shelfTree.WAREHOUSE_CODE = shelf.CMD_WAREHOUSE.WAREHOUSE_CODE;
                    shelfTree.WAREHOUSE_NAME = shelf.CMD_WAREHOUSE.WAREHOUSE_NAME;
                    shelfTree.AREA_CODE = shelf.CMD_AREA.AREA_CODE;
                    shelfTree.AREA_CODE = shelf.CMD_AREA.AREA_NAME;
                    //shelfTree.Type = shelf.ShelfType;
                    shelfTree.MEMO = shelf.MEMO;
                    //shelfTree.IsActive = shelf.IsActive == "1" ? "可用" : "不可用";
                    //shelfTree.UpdateTime = shelf.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    //shelfTree.ShortName = shelf.ShortName;
                    shelfTree.ATTRIBUTES = "shelf";
                    foreach (var cell in cells)//货位
                    {
                        NewWareTree cellTree = new NewWareTree();
                        //cellTree.CODE = cell.CELL_CODE;
                        //cellTree.NAME = "货位：" + cell.CELL_NAME;
                        //cellTree.CELL_CODE = cell.CELL_CODE;
                        //cellTree.CELL_NAME = cell.CELL_NAME;
                        //cellTree.WAREHOUSE_CODE = cell.CMD_WAREHOUSE.WAREHOUSE_CODE;
                        //cellTree.WAREHOUSE_NAME = cell.CMD_WAREHOUSE.WAREHOUSE_NAME;
                        //cellTree.AREA_CODE = cell.CMD_AREA.AREA_CODE;
                        //cellTree.AREA_NAME = cell.CMD_AREA.AREA_NAME;
                        //cellTree.SHELF_CODE = cell.CMD_SHELF.SHELF_CODE;
                        //cellTree.SHELF_NAME = cell.CMD_SHELF.SHELF_NAME;
                        ////cellTree.Type = cell.CellType;
                        //cellTree.MEMO = cell.MEMO;
                        cellTree.IS_ACTIVE = cell.IS_ACTIVE == "1" ? "可用" : "不可用";
                        ////cellTree.UpdateTime = cell.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                        ////cellTree.ShortName = cell.ShortName;
                        ////cellTree.Layer = cell.Layer;
                        ////cellTree.MaxQuantity = cell.MaxQuantity;
                        ////cellTree.PRODUCT_NAME = cell.CMD_PRODUCT == null ? string.Empty : cell.CMD_PRODUCT.ProductName;
                        //cellTree.ATTRIBUTES = "cell";
                        //cellSet.Add(cellTree);
                        dt.Rows.Add(cell.CELL_NAME,cell .CELL_COLUMN,cell.CELL_ROW ,cell .CMD_AREA .AREA_NAME ,cell .CMD_SHELF .SHELF_NAME ,cellTree .IS_ACTIVE,cell .MEMO);
                    }
                    //shelfSet.Add(shelfTree);
                }
                //set = cellSet;
            }
            else if (type == "cell")
            {
                cells = cells.Where(a => a.CELL_CODE == id).OrderBy(a => a.CELL_CODE).Select(a => a);
                dt.Columns.Add("货位名称", typeof(string));
                dt.Columns.Add("所在列", typeof(string));
                dt.Columns.Add("所在层", typeof(string));
                dt.Columns.Add("库区名称", typeof(string));
                dt.Columns.Add("货架名称", typeof(string));
                dt.Columns.Add("状        态", typeof(string));
                dt.Columns.Add("备注", typeof(string));
                foreach (var cell in cells)//货位
                {
                    NewWareTree cellTree = new NewWareTree();
                    //cellTree.CODE = cell.CELL_CODE;
                    //cellTree.NAME = "货位：" + cell.CELL_NAME;
                    //cellTree.CELL_CODE = cell.CELL_CODE;
                    //cellTree.CELL_NAME = cell.CELL_NAME;
                    //cellTree.WAREHOUSE_CODE = cell.CMD_WAREHOUSE.WAREHOUSE_CODE;
                    //cellTree.WAREHOUSE_NAME = cell.CMD_WAREHOUSE.WAREHOUSE_NAME;
                    //cellTree.AREA_CODE = cell.CMD_AREA.AREA_CODE;
                    //cellTree.AREA_NAME = cell.CMD_AREA.AREA_NAME;
                    //cellTree.SHELF_CODE = cell.CMD_SHELF.SHELF_CODE;
                    //cellTree.SHELF_NAME = cell.CMD_SHELF.SHELF_NAME;
                    ////cellTree.Type = cell.CellType;
                    //cellTree.MEMO = cell.MEMO;
                    cellTree.IS_ACTIVE = cell.IS_ACTIVE == "1" ? "可用" : "不可用";
                    ////cellTree.UpdateTime = cell.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    ////cellTree.ShortName = cell.ShortName;
                    ////cellTree.Layer = cell.Layer;
                    ////cellTree.MaxQuantity = cell.MaxQuantity;
                    ////cellTree.PRODUCT_NAME = cell.CMD_PRODUCT == null ? string.Empty : cell.CMD_PRODUCT.ProductName;
                    //cellTree.ATTRIBUTES = "cell";
                    //cellSet.Add(cellTree);
                    dt.Rows.Add(cell.CELL_NAME, cell.CELL_COLUMN, cell.CELL_ROW, cell.CMD_AREA.AREA_NAME, cell.CMD_SHELF.SHELF_NAME, cellTree.IS_ACTIVE, cell.MEMO);

                }
                //set = cellSet;
            }
            else
            {
                if (type == null || type == string.Empty)
                {
                    warehouses = warehouses.Where(w => w.WAREHOUSE_CODE == "001").OrderBy(w => w.WAREHOUSE_CODE).Select(w => w);
                }
                else if (type == "ware")
                {
                    warehouses = warehouses.Where(w => w.WAREHOUSE_CODE == id).OrderBy(w => w.WAREHOUSE_CODE).Select(w => w);
                }
                dt.Columns.Add("仓库名称", typeof(string));
                dt.Columns.Add("备注", typeof(string));
                foreach (var warehouse in warehouses)//仓库
                {
                    //NewWareTree NewWareTree = new NewWareTree();
                    //NewWareTree.CODE = warehouse.WAREHOUSE_CODE;
                    //NewWareTree.NAME = "仓库：" + warehouse.WAREHOUSE_NAME;
                    //NewWareTree.WAREHOUSE_CODE = warehouse.WAREHOUSE_CODE;
                    //NewWareTree.WAREHOUSE_NAME = warehouse.WAREHOUSE_NAME;
                    ////NewWareTree.Type = warehouse.WarehouseType;
                    //NewWareTree.MEMO = warehouse.MEMO;
                    ////NewWareTree.IsActive = warehouse.IsActive == "1" ? "可用" : "不可用";
                    ////NewWareTree.UpdateTime = warehouse.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss");
                    ////NewWareTree.ShortName = warehouse.ShortName;
                    //NewWareTree.ATTRIBUTES = "ware";
                    //warehouses = warehouses.Where(w => w.WAREHOUSE_CODE == id);
                    //wareSet.Add(NewWareTree);
                    dt.Rows.Add(warehouse .WAREHOUSE_NAME ,warehouse .MEMO);
                }
                //set = wareSet;
            }
            //dt.Columns.Add("名称", typeof(string));
            ////dt.Columns.Add("简称", typeof(string));
            ////dt.Columns.Add("类型", typeof(string));
            //dt.Columns.Add("描述", typeof(string));
            //dt.Columns.Add("是否可用", typeof(string));
            //dt.Columns.Add("预设卷烟名称", typeof(string));
            //dt.Columns.Add("货位层号", typeof(string));
            //dt.Columns.Add("货位最大量", typeof(string));
            //dt.Columns.Add("时间", typeof(string));
            //dt.Columns.Add("入库顺序", typeof(string));
            //dt.Columns.Add("出库顺序", typeof(string));
            //foreach (var item in set)
            //{
            //    dt.Rows.Add
            //        (
            //            item.NAME,
            //            //item.ShortName,
            //            //item.Type,
            //            item.MEMO,
            //            item.IS_ACTIVE
            //            //item.ProductName,
            //            //item.Layer,
            //            //item.MaxQuantity,
            //            //item.UpdateTime,
            //            //item.AllotInOrder,
            //            //item.AllotOutOrder
            //        );
            //}
            return dt;
        }

        //#region 上报仓储属性表
        //public bool uploadCell()
        //{
        //    try
        //    {
        //        IQueryable<CMD_CELL> cellQuery = CMDCellRepository.GetQueryable();
        //        var cells = cellQuery.OrderBy(b => b.CellCode).Select(b => b);
        //        DataSet ds = Insert(cells);
        //        upload.UploadCell(ds);
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        //#endregion

        //#region 插入数据到虚拟表
        //public DataSet Insert(IQueryable<CMD_CELL> cell)
        //{
        //    DataSet ds = this.GenerateEmptyTables();
        //    DataRow inbrddr = ds.Tables["wms_cell"].NewRow();
        //    foreach (var p in cell)
        //    {
        //        inbrddr["STORAGE_CODE"] = p.CellCode;
        //        inbrddr["STORAGE_TYPE"] = "4";
        //        inbrddr["ORDER_NUM"] = "";
        //        inbrddr["CONTAINER"] = "5003";
        //        inbrddr["STORAGE_NAME"] = p.CellName;
        //        inbrddr["UP_CODE"] = p.Area.Warehouse.WarehouseCode;
        //        inbrddr["DIST_CTR_CODE"] = p.Area.Warehouse.WarehouseCode;
        //        inbrddr["N_ORG_CODE"] = "";
        //        inbrddr["N_STORE_ROOM_CODE"] = "";
        //        inbrddr["CAPACITY"] = p.MaxQuantity * 50;
        //        inbrddr["HORIZONTAL_NUM"] = 0;
        //        inbrddr["VERTICAL_NUM"] = 0;
        //        inbrddr["AREA_TYPE"] = p.Area.AreaType;
        //        inbrddr["UPDATE_DATE"] = DateTime.Now;
        //        inbrddr["ISACTIVE"] = p.IsActive;
        //        ds.Tables["wms_cell"].Rows.Add(inbrddr);
        //    }
        //    return ds;
        //}

        //public DataSet Insert(Cell cell)
        //{
        //    DataSet ds = this.GenerateEmptyTables();
        //    DataRow inbrddr = ds.Tables["wms_cell"].NewRow();
        //    inbrddr["STORAGE_CODE"] = cell.CellCode;
        //    inbrddr["STORAGE_TYPE"] = "4";
        //    inbrddr["ORDER_NUM"] = "";
        //    inbrddr["CONTAINER"] = "5003";
        //    inbrddr["STORAGE_NAME"] = cell.CellName;
        //    inbrddr["UP_CODE"] = cell.Area.Warehouse.WarehouseCode;
        //    inbrddr["DIST_CTR_CODE"] = cell.Area.Warehouse.WarehouseCode;
        //    inbrddr["N_ORG_CODE"] = "";
        //    inbrddr["N_STORE_ROOM_CODE"] = "";
        //    inbrddr["CAPACITY"] = cell.MaxQuantity * 50;
        //    inbrddr["HORIZONTAL_NUM"] = 0;
        //    inbrddr["VERTICAL_NUM"] = 0;
        //    inbrddr["AREA_TYPE"] = cell.Area.AreaType;
        //    inbrddr["UPDATE_DATE"] = DateTime.Now;
        //    inbrddr["ISACTIVE"] = cell.IsActive;
        //    ds.Tables["wms_cell"].Rows.Add(inbrddr);

        //    return ds;
        //}
        //#endregion

        //#region 创建一个空的仓储信息表
        //private DataSet GenerateEmptyTables()
        //{
        //    DataSet ds = new DataSet();
        //    DataTable inbrtable = ds.Tables.Add("wms_cell");
        //    inbrtable.Columns.Add("STORAGE_CODE");
        //    inbrtable.Columns.Add("STORAGE_TYPE");
        //    inbrtable.Columns.Add("ORDER_NUM");
        //    inbrtable.Columns.Add("CONTAINER");
        //    inbrtable.Columns.Add("STORAGE_NAME");
        //    inbrtable.Columns.Add("UP_CODE");
        //    inbrtable.Columns.Add("DIST_CTR_CODE");
        //    inbrtable.Columns.Add("N_ORG_CODE");
        //    inbrtable.Columns.Add("N_STORE_ROOM_CODE");
        //    inbrtable.Columns.Add("CAPACITY");
        //    inbrtable.Columns.Add("HORIZONTAL_NUM");
        //    inbrtable.Columns.Add("VERTICAL_NUM");
        //    inbrtable.Columns.Add("AREA_TYPE");
        //    inbrtable.Columns.Add("UPDATE_DATE");
        //    inbrtable.Columns.Add("ISACTIVE");
        //    return ds;
        //}
        //#endregion

        //安排 获取货位
        public object GetCellByshell(string shelfcode)
        {
            IQueryable<CMD_CELL> cellQuery = CMDCellRepository.GetQueryable();
            var temp = cellQuery.Where(i => i.SHELF_CODE == shelfcode).Select(i => new { 
                i.CELL_CODE ,
                i.IS_ACTIVE,
                i.IS_LOCK,
                i.ERROR_FLAG,
                i.PRODUCT_CODE 
            });
            temp = temp.OrderBy(i => i.CELL_CODE);
            int total = temp.Count();
            //temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }

        //根据货位代码  获取对应的货物信息
        public object Getproductbycellcode(string cellcode)
        {
            IQueryable<CMD_CELL> cellquery = CMDCellRepository.GetQueryable();
            List<CellInfo> list = new List<CellInfo>();
            var cellitem= cellquery.Where(i => i.CELL_CODE == cellcode);
            if (cellitem.Count() > 0)//有存在该货位
            {
                foreach (CMD_CELL cell in cellitem)
                {
                    CellInfo info = new CellInfo();
                    if (cell.PRODUCT_CODE  != null)
                    {
                        info.Barcode = cell.PRODUCT_BARCODE;
                        info.BILLNO = cell.BILL_NO;
                        info.GRADE = cell.CMD_PRODUCT.CMD_PRODUCT_GRADE.GRADE_NAME;
                        info.INDATE = cell.IN_DATE.ToString();
                        info.ORIGINAL = cell.CMD_PRODUCT.CMD_PRODUCT_ORIGINAL.ORIGINAL_NAME;
                        info.REALWEIGHT = cell.REAL_WEIGHT.ToString();
                        info.STYLENO = cell.CMD_PRODUCT.CMD_PRODUCT_STYLE.STYLE_NAME;
                        info.YEARS = cell.CMD_PRODUCT.YEARS;
                        var bill = BillMasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO == cell.BILL_NO);
                        if (bill != null)
                        {
                            info.FORMULA = bill.WMS_FORMULA_MASTER.FORMULA_NAME;
                            info.CIGARETTE = bill.CMD_CIGARETTE.CIGARETTE_NAME;
                        }
                        else
                        {
                            info.FORMULA = "";
                            info.CIGARETTE = "";
                        }
                        list.Add(info);
                    }
                }
            }
                var temp = list.AsEnumerable();
                int total = temp.Count();
                return new { total, rows = temp.ToArray() };
        }
    }
}
