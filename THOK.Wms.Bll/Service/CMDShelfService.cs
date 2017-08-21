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
    public class CMDShelfService : ServiceBase<CMD_SHELF>, ICMDShelfService
    {
        [Dependency]
        public ICMDAreaRepository AreaRepository { get; set; }

        [Dependency]
        public ICMDWarehouseRepository WarehouseRepository { get; set; }

        [Dependency]
        public ICMDShelfRepository ShelfRepository { get; set; }

        [Dependency]
        public ICMDCellRepository CellRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IShelfService 成员

        public object GetDetails(string warehouseCode,string areaCode, string shelfCode)
        {
            IQueryable<CMD_SHELF> shelfQuery = ShelfRepository.GetQueryable();
            var shelf = shelfQuery.OrderBy(b => b.SHELF_CODE).AsEnumerable().Select(b => new { b.SHELF_CODE, b.SHELF_NAME, b.ROW_COUNT, b.COLUMN_COUNT, b.WAREHOUSE_CODE, b.AREA_CODE, b.CRANE_NO, b.MEMO, b.STATION_NO});
            if (warehouseCode != null && warehouseCode != string.Empty)
            {
                shelf = shelf.Where(s => s.WAREHOUSE_CODE == warehouseCode).OrderBy(s => s.SHELF_CODE).Select(s => s);
            }
            if (areaCode != null && areaCode != string.Empty)
            {
                shelf = shelf.Where(s => s.AREA_CODE == areaCode).OrderBy(s => s.SHELF_CODE).Select(s => s);
            }
            if (shelfCode != null && shelfCode!=string.Empty)
            {
                shelf = shelf.Where(s => s.SHELF_CODE == shelfCode).OrderBy(s => s.SHELF_CODE).Select(s => s);
            }
            return shelf.ToArray();
        }

        public object GetDetail(string type, string id)
        {
            IQueryable<CMD_SHELF> shelfQuery = ShelfRepository.GetQueryable();
            IQueryable<CMD_CELL> cellQuery = CellRepository.GetQueryable();
            var shelf = shelfQuery.OrderBy(b => b.SHELF_CODE).AsEnumerable().Select(b => new { b.SHELF_CODE, b.SHELF_NAME, b.ROW_COUNT, b.COLUMN_COUNT, b.WAREHOUSE_CODE, b.AREA_CODE, b.CRANE_NO, b.MEMO, b.STATION_NO });
            if (type=="shelf")
            {
                shelf = shelf.Where(s => s.SHELF_CODE == id);
            }
            else if (type == "cell")
            {
                var shelfCode = cellQuery.Where(c => c.CELL_CODE == id).Select(c => new { c.SHELF_CODE }).ToArray();
                shelf = shelf.Where(s => s.SHELF_CODE == shelfCode[0].SHELF_CODE);
            }  
            return shelf.ToArray();
        }
        public new bool Add(CMD_SHELF shelf)
        {
            var shelfAdd = new CMD_SHELF();
            var warehouse = WarehouseRepository.GetQueryable().FirstOrDefault(w => w.WAREHOUSE_CODE == shelf.WAREHOUSE_CODE);
            var area = AreaRepository.GetQueryable().FirstOrDefault(a => a.AREA_CODE == shelf.AREA_CODE);
            shelfAdd.SHELF_CODE = shelf.SHELF_CODE;
            shelfAdd.SHELF_NAME = shelf.SHELF_NAME;
            shelfAdd.STATION_NO = shelf.STATION_NO;
            shelfAdd.CRANE_NO = shelf.CRANE_NO;
            shelfAdd.ROW_COUNT = shelf.ROW_COUNT;
            shelfAdd.COLUMN_COUNT = shelf.COLUMN_COUNT;
            shelfAdd.WAREHOUSE_CODE = warehouse.WAREHOUSE_CODE;
            shelfAdd.AREA_CODE = area.AREA_CODE;
            shelfAdd.MEMO = shelf.MEMO;

            ShelfRepository.Add(shelfAdd);
            ShelfRepository.SaveChanges();
            return true;
        }

        public bool Delete(string shelfCode)
        {
            var shelf = ShelfRepository.GetQueryable()
                .FirstOrDefault(s => s.SHELF_CODE == shelfCode);
            if (shelf != null)
            {
                //Del(CellRepository, shelf.Cells);
                ShelfRepository.Delete(shelf);
                ShelfRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(CMD_SHELF shelf)
        {
            var shelfSave = ShelfRepository.GetQueryable().FirstOrDefault(s => s.SHELF_CODE == shelf.SHELF_CODE);            
            var warehouse = WarehouseRepository.GetQueryable().FirstOrDefault(w => w.WAREHOUSE_CODE == shelf.WAREHOUSE_CODE);
            var area = AreaRepository.GetQueryable().FirstOrDefault(a => a.AREA_CODE == shelf.AREA_CODE);
            shelfSave.SHELF_CODE = shelf.SHELF_CODE;
            shelfSave.SHELF_NAME = shelf.SHELF_NAME;
            shelfSave.STATION_NO = shelf.STATION_NO;
            shelfSave.CRANE_NO = shelf.CRANE_NO;
            shelfSave.ROW_COUNT = shelf.ROW_COUNT;
            shelfSave.COLUMN_COUNT = shelf.COLUMN_COUNT;
            shelfSave.WAREHOUSE_CODE = warehouse.WAREHOUSE_CODE;
            shelfSave.AREA_CODE = area.AREA_CODE;
            shelfSave.MEMO = shelf.MEMO;

            ShelfRepository.SaveChanges();
            return true;
        }
       
        /// <summary>
        /// 根据参数Code查询货架信息
        /// </summary>
        /// <param name="shelfCode">货架Code</param>
        /// <returns></returns>
        public object FindShelf(string shelfCode)
        {
            IQueryable<CMD_SHELF> shelfQuery = ShelfRepository.GetQueryable();
            var shelf = shelfQuery.Where(s=>s.SHELF_CODE==shelfCode).OrderBy(b => b.SHELF_CODE).AsEnumerable()
                                  .Select(b => new { b.SHELF_CODE, b.SHELF_NAME, b.ROW_COUNT, b.COLUMN_COUNT, b.WAREHOUSE_CODE,b.CMD_WAREHOUSE.WAREHOUSE_NAME, b.AREA_CODE,b.CMD_AREA.AREA_NAME, b.CRANE_NO,b.CMD_CRANE.CRANE_NAME, b.MEMO });
            return shelf.First(s => s.SHELF_CODE == shelfCode);
        }

        /// <summary>
        /// 根据库区编码获取生成的货架编码
        /// </summary>
        /// <param name="areaCode">库区编码</param>
        /// <returns></returns>
        public object GetShelfCode(string areaCode)
        {
            string shelfCodeStr = "";
            IQueryable<CMD_SHELF> shelfQuery = ShelfRepository.GetQueryable();
            var shelfCode = shelfQuery.Where(s=>s.AREA_CODE==areaCode).Max(s => s.SHELF_CODE);
            if (shelfCode == string.Empty || shelfCode == null)
            {
                shelfCodeStr = areaCode + "-001";
            }
            else
            {
                int i = Convert.ToInt32(shelfCode.ToString().Substring(areaCode.Length+1, 3));
                i++;
                string newcode = i.ToString();
                if (newcode.Length <= 3)
                {
                    for (int j = 0; j < 3 - i.ToString().Length; j++)
                    {
                        newcode = "0" + newcode;
                    }
                    shelfCodeStr = areaCode + "-" + newcode;
                }
            }
            return shelfCodeStr;
        }

        #endregion


       
    }
}
