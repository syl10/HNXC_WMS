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
    public class WarehouseService : ServiceBase<Warehouse>, IWarehouseService
    {
        [Dependency]
        public IWarehouseRepository WarehouseRepository { get; set; }

        [Dependency]
        public ICompanyRepository CompanyRepository { get; set; }
        [Dependency]
        public IShelfRepository ShelfRepository { get; set; }
        [Dependency]
        public IAreaRepository AreaRepository { get; set; }
        [Dependency]
        public ICellRepository CellRepository { get; set; }


        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IWarehouseService 成员

        public object GetDetails(int page, int rows, string warehouseCode)
        {
            IQueryable<Warehouse> wareQuery = WarehouseRepository.GetQueryable();
            var warehouse = wareQuery.OrderBy(b => b.WarehouseCode).AsEnumerable().Select(b => new { b.WarehouseCode, b.WarehouseName, b.WarehouseType, b.Description, b.ShortName, IsActive = b.IsActive == "1" ? "可用" : "不可用", UpdateTime = b.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss") });
            if (warehouseCode != null)
            {
                warehouse = warehouse.Where(w => w.WarehouseCode == warehouseCode);
            }
            int total = warehouse.Count();
            warehouse = warehouse.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = warehouse.ToArray() };
        }

        public object GetDetail(int page, int rows, string type,string id)
        {
            IQueryable<Warehouse> wareQuery = WarehouseRepository.GetQueryable();
            IQueryable<Shelf> shelfQuery = ShelfRepository.GetQueryable();
            IQueryable<Area> areaQuery = AreaRepository.GetQueryable();
            IQueryable<Cell> cellQuery = CellRepository.GetQueryable();
            var warehouse = wareQuery.OrderBy(b => b.WarehouseCode).AsEnumerable().Select(b => new { b.WarehouseCode, b.WarehouseName, b.WarehouseType, b.Description, b.ShortName, IsActive = b.IsActive == "1" ? "可用" : "不可用", UpdateTime = b.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss") });
            if (type=="ware")
            {
                warehouse = warehouse.Where(w => w.WarehouseCode == id);
            }
            else if (type == "area")
            {
                var WarehouseCode = areaQuery.Where(a => a.AreaCode == id).Select(a => new { a.WarehouseCode }).ToArray();
                warehouse = warehouse.Where(w => w.WarehouseCode == WarehouseCode[0].WarehouseCode);
            }
            else if (type == "shelf")
            {
                var WarehouseCode = shelfQuery.Where(s => s.ShelfCode == id).Select(s => new { s.WarehouseCode }).ToArray();
                warehouse = warehouse.Where(w=>w.WarehouseCode==WarehouseCode[0].WarehouseCode);
            }
            else if (type == "cell")
            {
                var WarehouseCode = cellQuery.Where(c => c.CellCode == id).Select(c => new { c.WarehouseCode}).ToArray();
                warehouse = warehouse.Where(w=>w.WarehouseCode==WarehouseCode[0].WarehouseCode);
            }
            else
            {
                warehouse = warehouse.Select(w => w);
            }
            int total = warehouse.Count();
            warehouse = warehouse.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = warehouse.ToArray() };
        }
        public new bool Add(Warehouse warehouse)
        {
            var ware = new Warehouse();
            ware.WarehouseCode = warehouse.WarehouseCode;
            ware.WarehouseName = warehouse.WarehouseName;
            ware.WarehouseType = warehouse.WarehouseType;
            ware.ShortName = warehouse.ShortName;
            ware.CompanyCode = "";// warehouse.CompanyCode;
            ware.Description = warehouse.Description;
            ware.IsActive = warehouse.IsActive;
            ware.UpdateTime = DateTime.Now;

            WarehouseRepository.Add(ware);
            WarehouseRepository.SaveChanges();
            return true;
        }

        public bool Delete(string warehouseCode)
        {
            var warehouse = WarehouseRepository.GetQueryable()
                .FirstOrDefault(w => w.WarehouseCode == warehouseCode);
            if (warehouse != null)
            {
                WarehouseRepository.Delete(warehouse);
                WarehouseRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(Warehouse warehouse)
        {
            var ware = WarehouseRepository.GetQueryable().FirstOrDefault(w => w.WarehouseCode == warehouse.WarehouseCode);
            var company = CompanyRepository.GetQueryable().FirstOrDefault(c => c.CompanyCode == warehouse.CompanyCode);
            ware.WarehouseCode = ware.WarehouseCode;
            ware.WarehouseName = warehouse.WarehouseName;
            ware.WarehouseType = warehouse.WarehouseType;
            ware.ShortName = warehouse.ShortName;
            ware.CompanyCode = company != null ? company.CompanyCode : "";
            ware.Description = warehouse.Description;
            ware.IsActive = warehouse.IsActive;
            ware.UpdateTime = DateTime.Now;

            WarehouseRepository.SaveChanges();
            return true;
        }

        /// <summary>
        /// 根据参数Code查询仓库信息
        /// </summary>
        /// <param name="wareCode">仓库Code</param>
        /// <returns></returns>
        public object FindWarehouse(string wareCode)
        {
            IQueryable<Warehouse> wareQuery = WarehouseRepository.GetQueryable();
            var warehouse = wareQuery.Where(w => w.WarehouseCode == wareCode).AsEnumerable().Select(w => new { w.WarehouseCode, w.WarehouseName, w.WarehouseType, w.ShortName, w.Description, IsActive = w.IsActive == "1" ? "可用" : "不可用", UpdateTime = w.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss") });
            return warehouse.First(w => w.WarehouseCode == wareCode);
        }

        /// <summary>
        /// 获取生成的仓库编码
        /// </summary>
        /// <returns></returns>
        public object GetWareCode()
        {
            string warehouseCode = "";
            IQueryable<Warehouse> wareQuery = WarehouseRepository.GetQueryable();
            var wareCode = wareQuery.Max(w => w.WarehouseCode);
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
