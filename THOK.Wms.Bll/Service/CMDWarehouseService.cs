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
    public class CMDWarehouseService : ServiceBase<CMD_WAREHOUSE>, ICMDWarehouseService
    {
        [Dependency]
        public ICMDWarehouseRepository CMDWarehouseRepository { get; set; }
        //[Dependency]
        //public ICMDCompanyRepository CompanyRepository { get; set; }
        [Dependency]
        public ICMDShelfRepository CMDShelfRepository { get; set; }
        [Dependency]
        public ICMDAreaRepository CMDAreaRepository { get; set; }
        [Dependency]
        public ICMDCellRepository CMDCellRepository { get; set; }





        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IWarehouseService 成员

        public object GetDetails(int page, int rows, string warehouseCode)
        {
            IQueryable<CMD_WAREHOUSE> wareQuery = CMDWarehouseRepository.GetQueryable();
            var warehouse = wareQuery.OrderBy(b => b.WAREHOUSE_CODE).AsEnumerable().Select(b => new { b.WAREHOUSE_CODE, b.WAREHOUSE_NAME, b.MEMO });
            if (warehouseCode != null)
            {
                warehouse = warehouse.Where(w => w.WAREHOUSE_CODE == warehouseCode);
            }
            int total = warehouse.Count();
            warehouse = warehouse.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = warehouse.ToArray() };
        }

        public object GetDetail(int page, int rows, string type,string id)
        {
            IQueryable<CMD_WAREHOUSE> wareQuery = CMDWarehouseRepository.GetQueryable();
            IQueryable<CMD_SHELF> shelfQuery = CMDShelfRepository.GetQueryable();
            IQueryable<CMD_AREA> areaQuery = CMDAreaRepository.GetQueryable();
            IQueryable<CMD_CELL> cellQuery = CMDCellRepository.GetQueryable();
            var warehouse = wareQuery.OrderBy(b => b.WAREHOUSE_CODE).AsEnumerable().Select(b => new { b.WAREHOUSE_CODE });
            if (type=="ware")
            {
                warehouse = warehouse.Where(w => w.WAREHOUSE_CODE == id);
            }
            else if (type == "area")
            {
                var WarehouseCode = areaQuery.Where(a => a.AREA_CODE == id).Select(a => new { a.WAREHOUSE_CODE }).ToArray();
                warehouse = warehouse.Where(w => w.WAREHOUSE_CODE == WarehouseCode[0].WAREHOUSE_CODE);
            }
            else if (type == "shelf")
            {
                var WarehouseCode = shelfQuery.Where(s => s.SHELF_CODE == id).Select(s => new { s.WAREHOUSE_CODE }).ToArray();
                warehouse = warehouse.Where(w=>w.WAREHOUSE_CODE==WarehouseCode[0].WAREHOUSE_CODE);
            }
            else if (type == "cell")
            {
                var WarehouseCode = cellQuery.Where(c => c.CELL_CODE == id).Select(c => new { c.WAREHOUSE_CODE}).ToArray();
                warehouse = warehouse.Where(w=>w.WAREHOUSE_CODE==WarehouseCode[0].WAREHOUSE_CODE);
            }
            else
            {
                warehouse = warehouse.Select(w => w);
            }
            int total = warehouse.Count();
            warehouse = warehouse.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = warehouse.ToArray() };
        }
        public new bool Add(CMD_WAREHOUSE warehouse)
        {
            var ware = new CMD_WAREHOUSE();
            ware.WAREHOUSE_CODE = warehouse.WAREHOUSE_CODE;
            ware.WAREHOUSE_NAME = warehouse.WAREHOUSE_NAME;
            //ware.WarehouseType = warehouse.WarehouseType;
            //ware.ShortName = warehouse.ShortName;
            //ware.CompanyCode = "";// warehouse.CompanyCode;
            ware.MEMO = warehouse.MEMO;
            //ware.IsActive = warehouse.IsActive;
            //ware.UpdateTime = DateTime.Now;

            CMDWarehouseRepository.Add(ware);
            CMDWarehouseRepository.SaveChanges();
            return true;
        }

        public bool Delete(string warehouseCode)
        {
            var warehouse = CMDWarehouseRepository.GetQueryable()
                .FirstOrDefault(w => w.WAREHOUSE_CODE == warehouseCode);
            if (warehouse != null)
            {
                CMDWarehouseRepository.Delete(warehouse);
                CMDWarehouseRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(CMD_WAREHOUSE warehouse)
        {
            var ware = CMDWarehouseRepository.GetQueryable().FirstOrDefault(w => w.WAREHOUSE_CODE == warehouse.WAREHOUSE_CODE);
          //  var company = CompanyRepository.GetQueryable().FirstOrDefault(c => c.CompanyCode == warehouse.CompanyCode);
            ware.WAREHOUSE_CODE = ware.WAREHOUSE_CODE;
            ware.WAREHOUSE_NAME = warehouse.WAREHOUSE_NAME;
            ware.MEMO = warehouse.MEMO;

            CMDWarehouseRepository.SaveChanges();
            return true;
        }

        /// <summary>
        /// 根据参数Code查询仓库信息
        /// </summary>
        /// <param name="wareCode">仓库Code</param>
        /// <returns></returns>
        public object FindWarehouse(string wareCode)
        {
            IQueryable<CMD_WAREHOUSE> wareQuery = CMDWarehouseRepository.GetQueryable();
            var warehouse = wareQuery.Where(w => w.WAREHOUSE_CODE == wareCode).AsEnumerable().Select(w => new { w.WAREHOUSE_CODE, w.WAREHOUSE_NAME, w.MEMO });
            return warehouse.First(w => w.WAREHOUSE_CODE == wareCode);
        }

        /// <summary>
        /// 获取生成的仓库编码
        /// </summary>
        /// <returns></returns>
        public object GetWareCode()
        {
            string warehouseCode = "";
            IQueryable<CMD_WAREHOUSE> wareQuery = CMDWarehouseRepository.GetQueryable();
            var wareCode = wareQuery.Max(w => w.WAREHOUSE_CODE);
            if (wareCode == string.Empty || wareCode == null)
            {
                warehouseCode = "01";
            }
            else
            {
                int i = Convert.ToInt32(wareCode);
                i++;
                string newcode = i.ToString();
                if (newcode.Length <= 2)
                {
                    for (int j = 0; j < 2 - i.ToString().Length; j++)
                    {
                        newcode = "0" + newcode;
                    }
                    warehouseCode = newcode;
                }
            }
            return warehouseCode;
        }

        #endregion
               
    }
}
