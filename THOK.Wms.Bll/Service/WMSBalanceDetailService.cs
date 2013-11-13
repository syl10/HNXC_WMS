using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.Dal.Interfaces;
using Microsoft.Practices.Unity;

namespace THOK.Wms.Bll.Service
{
    class WMSBalanceDetailService : ServiceBase<WMS_BALANCE_DETAIL>, IWMSBalanceDetailService 
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public IWMSBalanceDetailRepository  BalanceDetailRepository { get; set; }
        [Dependency]
        public ICMDProuductRepository ProductRepository { get; set; }
        [Dependency]
        public ICMDWarehouseRepository CMDWarehouseRepository { get; set; }

        public object GetSubDetails(int page, int rows, string Balanceno)
        {
            IQueryable<WMS_BALANCE_DETAIL> detailquery = BalanceDetailRepository.GetQueryable();
            IQueryable<CMD_PRODUCT> productquery = ProductRepository.GetQueryable();
            IQueryable<CMD_WAREHOUSE> warehousequery = CMDWarehouseRepository.GetQueryable();
            var details = from a in detailquery
                          join b in warehousequery on a.WAREHOUSE_CODE equals b.WAREHOUSE_CODE
                          join c in productquery on a.PRODUCT_CODE equals c.PRODUCT_CODE
                          select new { 
                              a.BALANCE_NO , //月结编号
                              a.WAREHOUSE_CODE , //仓库编号
                              b.WAREHOUSE_NAME ,
                              a.PRODUCT_CODE , //产品代码
                              c.PRODUCT_NAME ,
                              a.IN_QUANTITY , //入库数量
                              a.INCOME_QUANTITY ,//损益数量
                              a.INSPECTIN_QUANTITY ,//抽检补料入数量
                              a.INSPECTOUT_QUANTITY ,//抽检出数量
                              a.OUT_QUANTITY ,//出库数量
                              a.DIFF_QUANTITY ,//差异数量
                              a.ENDQUANTITY ,//期末数量
                              a.BEGIN_QUANTITY ,//期初数量
                              a.FEEDING_QUANTITY //补料出数量
                          };
            var temp = details.ToArray().Where(i => i.BALANCE_NO== Balanceno ).OrderBy(i => i.WAREHOUSE_CODE ).Select(i => i);
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };

        }

        //产品总账
        public object Ledger(int page, int rows, string begin, string end)
        {
            IQueryable<WMS_BALANCE_DETAIL> detailquery = BalanceDetailRepository.GetQueryable();
            IQueryable<CMD_PRODUCT> productquery = ProductRepository.GetQueryable();
            IQueryable<CMD_WAREHOUSE> warehousequery = CMDWarehouseRepository.GetQueryable();
            List<WMS_BALANCE_DETAIL> list = new List<WMS_BALANCE_DETAIL>();
            var details = from a in detailquery
                          join b in warehousequery on a.WAREHOUSE_CODE equals b.WAREHOUSE_CODE
                          join c in productquery on a.PRODUCT_CODE equals c.PRODUCT_CODE
                          select new
                          {
                              BEGINMONTH = begin,
                              ENDMONTH = end,
                              //gp .Key ,
                             
                              a.BALANCE_NO, //月结编号
                              a.WAREHOUSE_CODE, //仓库编号
                              b.WAREHOUSE_NAME,
                              a.PRODUCT_CODE, //产品代码
                              c.PRODUCT_NAME,
                              a.IN_QUANTITY, //入库数量
                              a.INCOME_QUANTITY,//损益数量
                              a.INSPECTIN_QUANTITY,//抽检补料入数量
                              a.INSPECTOUT_QUANTITY,//抽检出数量
                              a.OUT_QUANTITY,//出库数量
                              a.DIFF_QUANTITY,//差异数量
                              a.ENDQUANTITY,//期末数量
                              a.BEGIN_QUANTITY,//期初数量
                              a.FEEDING_QUANTITY //补料出数量
                          };
            //details = details.Where(i => int.Parse(i.BALANCE_NO) >=int .Parse ( begin) && int.Parse(i.BALANCE_NO) <= int.Parse (end));
            //foreach () { 

            //}
            var temp = details.ToArray().OrderBy(i => i.WAREHOUSE_CODE).Select(i => i);
            temp = temp.Where(i => int.Parse(i.BALANCE_NO) >= int.Parse(begin) && int.Parse(i.BALANCE_NO) <= int.Parse(end));
            //for (int i = 0; i < temp.Count(); i++) {
            //    string product = temp.ToArray()[i].PRODUCT_CODE;
            //    string warehouse = temp.ToArray()[i].WAREHOUSE_CODE;
            //    var obj = list.FirstOrDefault (n=> n.PRODUCT_CODE == product && n.WAREHOUSE_CODE == warehouse);
            //    if (obj!=null ) { 
            //        obj .
            //    }
            //}
           
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }
    }
}
