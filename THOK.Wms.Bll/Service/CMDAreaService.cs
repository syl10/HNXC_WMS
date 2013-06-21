using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.Bll.Models;

namespace THOK.Wms.Bll.Service
{
    public class CMDAreaService : ServiceBase<CMD_AREA>, ICMDAreaService
    {
        [Dependency]
        public ICMDAreaRepository CMDAreaRepository { get; set; }

        [Dependency]
        public ICMDWarehouseRepository CMDWarehouseRepository { get; set; }
        [Dependency]
        public ICMDShelfRepository CMDShelfRepository { get; set; }
        [Dependency]
        public ICMDCellRepository CMDCellRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IAreaService 成员

        public object GetDetails(string warehouseCode, string areaCode)
        {
            IQueryable<CMD_AREA> areaQuery = CMDAreaRepository.GetQueryable();
            var area = areaQuery.OrderBy(b => b.AREA_CODE).AsEnumerable().Select(b => new { b.AREA_CODE, b.AREA_NAME, b.WAREHOUSE_CODE, b.MEMO });
            if (warehouseCode != null && warehouseCode != string.Empty){
                area = area.Where(a => a.WAREHOUSE_CODE == warehouseCode).OrderBy(a => a.AREA_CODE).Select(a => a);
            }
            if (areaCode != null && areaCode!=string.Empty){
                area = area.Where(a => a.AREA_CODE == areaCode).OrderBy(a => a.AREA_CODE).Select(a => a);
            }           
            return area.ToArray();
        }
        public object GetDetail(string type, string id)
        {
            IQueryable<CMD_AREA> areaQuery =CMDAreaRepository.GetQueryable();
            IQueryable<CMD_SHELF> shelfQuery = CMDShelfRepository.GetQueryable();
            IQueryable<CMD_CELL> cellQuery = CMDCellRepository.GetQueryable();
            var area = areaQuery.OrderBy(b => b.AREA_CODE).AsEnumerable().Select(b => new { b.AREA_CODE, b.AREA_NAME, b.WAREHOUSE_CODE, b.MEMO });
            if (type == "shelf")
            {
                var areaCode=shelfQuery.Where(s=>s.SHELF_CODE==id).Select(s=>new{s.AREA_CODE}).ToArray();
                area = area.Where(a => a.AREA_CODE == areaCode[0].AREA_CODE);
            }
            else if (type == "cell")
            {
                var areaCode = cellQuery.Where(c => c.CELL_CODE == id).Select(c => new { c.AREA_CODE}).ToArray();
                area = area.Where(a => a.AREA_CODE == areaCode[0].AREA_CODE);
            }  
            else if (type == "area")
            {
                area = area.Where(a => a.AREA_CODE == id);
            }
            return area.ToArray();
        }

        public new bool Add(CMD_AREA area)
        {
            var areaAdd = new CMD_AREA();
            var warehouse = CMDWarehouseRepository.GetQueryable().FirstOrDefault(w => w.WAREHOUSE_CODE == area.WAREHOUSE_CODE);
            areaAdd.AREA_CODE = area.AREA_CODE;
            areaAdd.AREA_NAME = area.AREA_NAME;
            areaAdd.WAREHOUSE_CODE = area.WAREHOUSE_CODE;
            areaAdd.MEMO = area.MEMO;

            CMDAreaRepository.Add(areaAdd);
            CMDAreaRepository.SaveChanges();
            return true;
        }

        public bool Delete(string areaCode)
        {
            var area =CMDAreaRepository.GetQueryable()
                .FirstOrDefault(a => a.AREA_CODE == areaCode);
            if (area != null)
            {
                CMDAreaRepository.Delete(area);
                CMDAreaRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(CMD_AREA area)
        {
            var areaSave =CMDAreaRepository.GetQueryable().FirstOrDefault(a => a.AREA_CODE == area.AREA_CODE);
            var warehouse = CMDWarehouseRepository.GetQueryable().FirstOrDefault(w => w.WAREHOUSE_CODE == area.WAREHOUSE_CODE);
            areaSave.AREA_CODE = area.AREA_CODE;
            areaSave.AREA_NAME = area.AREA_NAME;
            areaSave.WAREHOUSE_CODE = area.WAREHOUSE_CODE;
            areaSave.MEMO = area.MEMO;

            CMDAreaRepository.SaveChanges();
            return true;
        }

        /// <summary>
        /// 查询参数Code查询库区信息
        /// </summary>
        /// <param name="areaCode">库区Code</param>
        /// <returns></returns>
        public object FindArea(string areaCode)
        {
            IQueryable<CMD_AREA> areaQuery =CMDAreaRepository.GetQueryable();
            var area = areaQuery.Where(a => a.AREA_CODE == areaCode).OrderBy(b => b.AREA_CODE).AsEnumerable().Select(b => new { b.AREA_CODE, b.AREA_NAME, b.WAREHOUSE_CODE,b.CMD_WAREHOUSE.WAREHOUSE_NAME, b.MEMO });
            return area.First(a => a.AREA_CODE == areaCode);
        }

        public object GetWareArea()
        {
            var warehouses = CMDWarehouseRepository.GetQueryable().AsEnumerable();
            HashSet<Tree> wareSet = new HashSet<Tree>();
            foreach (var warehouse in warehouses)//仓库
            {
                Tree wareTree = new Tree();
                wareTree.id = warehouse.WAREHOUSE_CODE;
                wareTree.text = "仓库：" + warehouse.WAREHOUSE_CODE;
                wareTree.state = "open";
                wareTree.attributes = "ware";

                var areas =CMDAreaRepository.GetQueryable().Where(a => a.CMD_WAREHOUSE.WAREHOUSE_CODE == warehouse.WAREHOUSE_CODE)
                                                         .OrderBy(a => a.AREA_CODE).Select(a => a);
                HashSet<Tree> areaSet = new HashSet<Tree>();
                foreach (var area in areas)//库区
                {
                    Tree areaTree = new Tree();
                    areaTree.id = area.AREA_CODE;
                    areaTree.text = "库区：" + area.AREA_NAME;
                    areaTree.state = "open";
                    areaTree.attributes = "area";
                    areaSet.Add(areaTree);
                }
                wareTree.children = areaSet.ToArray();
                wareSet.Add(wareTree);
            }
            return wareSet.ToArray();
        }

        /// <summary>
        /// 根据仓库编码获取生成的库区编码
        /// </summary>
        /// <param name="wareCode">仓库编码</param>
        /// <returns></returns>
        public object GetAreaCode(string wareCode)
        {
            string areaCodeStr = "";
            IQueryable<CMD_AREA> areaQuery =CMDAreaRepository.GetQueryable();
            var areaCode = areaQuery.Where(a=>a.WAREHOUSE_CODE==wareCode).Max(a=>a.AREA_CODE);
            if (areaCode == string.Empty || areaCode == null)
            {
                areaCodeStr = wareCode + "-01";
            }
            else
            {
                int i = Convert.ToInt32(areaCode.ToString().Substring(wareCode.Length,2));
                i++;
                string newcode = i.ToString();
                if (newcode.Length <= 2)
                {
                    for (int j = 0; j < 2 - i.ToString().Length; j++)
                    {
                        newcode = "0" + newcode;
                    }
                    areaCodeStr = wareCode + "-" + newcode;
                }
            }
            return areaCodeStr;
        }

        #endregion


        
    }
}
