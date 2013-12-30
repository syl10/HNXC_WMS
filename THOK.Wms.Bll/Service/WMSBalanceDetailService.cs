using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.Dal.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Models;
using System.Data;

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
        [Dependency]
        public IWMSBillMasterHRepository BillMasterRepository { get; set; }
        [Dependency]
        public IWMSBillDetailHRepository BillDetailRepository { get; set; }

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
            details = details.Where(i => i.BALANCE_NO == Balanceno).OrderBy(i => i.WAREHOUSE_CODE);
            int total = details.Count();
            details = details.Skip((page - 1) * rows).Take(rows);
            var temp = details.ToArray().Select(i => i);
            return new { total, rows = temp};

        }

        //产品总账
        public object Ledger(int page, int rows, string begin, string end)
        {
            IQueryable<WMS_BALANCE_DETAIL> detailquery = BalanceDetailRepository.GetQueryable();
            IQueryable<CMD_PRODUCT> productquery = ProductRepository.GetQueryable();
            IQueryable<CMD_WAREHOUSE> warehousequery = CMDWarehouseRepository.GetQueryable();
            List<ProductLedgerInfo> list = new List<ProductLedgerInfo>();
            var details = from a in detailquery
                          join b in warehousequery on a.WAREHOUSE_CODE equals b.WAREHOUSE_CODE
                          join c in productquery on a.PRODUCT_CODE equals c.PRODUCT_CODE
                          //group a by new {a.PRODUCT_CODE ,a.WAREHOUSE_CODE } into gp
                          select new
                          {
                              BEGINMONTH = begin,
                              ENDMONTH = end,
                              //gp.Key,
                              //IN_QUANTITY = a.IN_QUANTITY + a.INSPECTIN_QUANTITY

                              a.BALANCE_NO, //月结编号
                              a.WAREHOUSE_CODE, //仓库编号
                              b.WAREHOUSE_NAME,
                              a.PRODUCT_CODE, //产品代码
                              c.PRODUCT_NAME,
                              IN_QUANTITY = a.IN_QUANTITY + a.INSPECTIN_QUANTITY, //入库数量
                              a.INCOME_QUANTITY,//损益数量
                              a.INSPECTIN_QUANTITY,//抽检补料入数量
                              a.INSPECTOUT_QUANTITY,//抽检出数量
                              OUT_QUANTITY = a.OUT_QUANTITY + a.INSPECTOUT_QUANTITY + a.FEEDING_QUANTITY,//出库数量
                              a.DIFF_QUANTITY,//差异数量
                              a.ENDQUANTITY,//期末数量
                              a.BEGIN_QUANTITY,//期初数量
                              a.FEEDING_QUANTITY //补料出数量
                          };
            var temp = details.ToArray().OrderBy(i => i.WAREHOUSE_CODE).Select(i => i);
            temp = temp.Where(i => int.Parse(i.BALANCE_NO) >= int.Parse(begin) && int.Parse(i.BALANCE_NO) <= int.Parse(end));
            for (int i = 0; i < temp.Count(); i++) {
                string product = temp.ToArray()[i].PRODUCT_CODE;
                string warehouse = temp.ToArray()[i].WAREHOUSE_CODE;
                string balanceno=temp .ToArray ()[i].BALANCE_NO ;
                var obj = list.FirstOrDefault (n=> n.PRODUCT_CODE == product && n.WAREHOUSE_CODE == warehouse);
                if (obj != null)
                {
                    obj.IN_QUANTITY += temp.ToArray()[i].IN_QUANTITY;
                    obj.INCOME_QUANTITY += temp.ToArray()[i].INCOME_QUANTITY;
                    obj.INSPECTIN_QUANTITY += temp.ToArray()[i].INSPECTIN_QUANTITY;
                    obj.INSPECTOUT_QUANTITY += temp.ToArray()[i].INSPECTOUT_QUANTITY;
                    obj.OUT_QUANTITY += temp.ToArray()[i].OUT_QUANTITY;
                    obj.DIFF_QUANTITY += temp.ToArray()[i].DIFF_QUANTITY;
                    obj.FEEDING_QUANTITY += temp.ToArray()[i].FEEDING_QUANTITY;
                    obj.BEGIN_QUANTITY += temp.ToArray()[i].BEGIN_QUANTITY;
                    obj.ENDQUANTITY += temp.ToArray()[i].ENDQUANTITY;

                }
                else {
                    ProductLedgerInfo item = new ProductLedgerInfo();
                    //item.BALANCE_NO = temp.ToArray()[i].BALANCE_NO;
                    item.BEGINMONTH = begin;
                    item.ENDMONTH = end;
                    item.BEGIN_QUANTITY = temp.ToArray()[i].BEGIN_QUANTITY;
                    item.DIFF_QUANTITY = temp.ToArray()[i].DIFF_QUANTITY;
                    item.ENDQUANTITY = temp.ToArray()[i].ENDQUANTITY;
                    item.FEEDING_QUANTITY = temp.ToArray()[i].FEEDING_QUANTITY;
                    item.IN_QUANTITY = temp.ToArray()[i].IN_QUANTITY;
                    item.INCOME_QUANTITY = temp.ToArray()[i].INCOME_QUANTITY;
                    item.INSPECTIN_QUANTITY = temp.ToArray()[i].INSPECTIN_QUANTITY;
                    item.INSPECTOUT_QUANTITY = temp.ToArray()[i].INSPECTOUT_QUANTITY;
                    item.OUT_QUANTITY = temp.ToArray()[i].OUT_QUANTITY;
                    item.PRODUCT_CODE = temp.ToArray()[i].PRODUCT_CODE;
                    item.PRODUCT_NAME = temp.ToArray()[i].PRODUCT_NAME;
                    item.WAREHOUSE_NAME = temp.ToArray()[i].WAREHOUSE_NAME;
                    item.WAREHOUSE_CODE = temp.ToArray()[i].WAREHOUSE_CODE;
                    list.Add(item);
                }
            }
            int total = list.Count();
            var tmp = list.OrderBy(i => i.PRODUCT_CODE).Select(i => i);
            DataTable dt = THOK.Common.ConvertData.LinqQueryToDataTable(tmp);
            THOK.Common.PrintHandle.dt = dt;
            tmp = tmp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = tmp.ToArray() };
        }

        //产品明细
        public object Detailed(int page, int rows, string begin, string end)
        {
            IQueryable<WMS_BALANCE_DETAIL> detailquery = BalanceDetailRepository.GetQueryable();
            IQueryable<WMS_BILL_MASTERH> billquery = BillMasterRepository.GetQueryable();
            IQueryable<WMS_BILL_DETAILH> billdetailquery = BillDetailRepository.GetQueryable();
             IQueryable<CMD_PRODUCT> productquery = ProductRepository.GetQueryable();
            List<ProductLedgerInfo> list = new List<ProductLedgerInfo>();
            //从月结明细表中找出,在该时间段的哪些仓库中的哪些产品代码
            var balance = detailquery.Where(i => int.Parse(i.BALANCE_NO) >= int.Parse(begin) && int.Parse(i.BALANCE_NO) <= int.Parse(end));

            //
            var bill = from a in billquery
                       join b in billdetailquery on a.BILL_NO equals b.BILL_NO
                        join c in productquery on b.PRODUCT_CODE  equals c.PRODUCT_CODE
                       select new { 
                           a.BILL_NO ,
                           a.BILL_DATE ,
                           a.WAREHOUSE_CODE ,
                           a.CMD_WAREHOUSE .WAREHOUSE_NAME ,
                           b.PRODUCT_CODE ,
                           c.PRODUCT_NAME ,
                           a.BTYPE_CODE ,
                           a.BILL_METHOD ,
                           b.PACKAGE_COUNT ,
                           b.REAL_WEIGHT,
                           b.IS_MIX 
                       };
             var   bills = bill.ToArray ().Where(i => int.Parse((i.BILL_DATE.ToString("yyyyMM"))) >= int.Parse(begin) && int.Parse((i.BILL_DATE.ToString("yyyyMM"))) <= int.Parse(end));
             for (int n = 0; n <bills.Count(); n++)
             {
                 string warehouse = bills.ToArray()[n].WAREHOUSE_CODE;
                 string warehousename = bills.ToArray()[n].WAREHOUSE_NAME;
                 string productcode = bills.ToArray()[n].PRODUCT_CODE;
                 string product = bills.ToArray()[n].PRODUCT_NAME;
                 string btypecode = bills.ToArray()[n].BTYPE_CODE;
                 decimal packagecout = bills.ToArray()[n].PACKAGE_COUNT;
                 decimal realweight = bills.ToArray()[n].REAL_WEIGHT;
                var obj = balance.Where  (i => i.WAREHOUSE_CODE == warehouse && i.PRODUCT_CODE == productcode);
                if (obj != null) //该单据符合条件
                {
                    ProductLedgerInfo item = new ProductLedgerInfo();
                    item.WAREHOUSE_CODE = warehouse;
                    item.PRODUCT_CODE = productcode;
                    item.WAREHOUSE_NAME = warehousename;
                    item.PRODUCT_NAME = product;
                    item.BEGINMONTH = begin;
                    item.ENDMONTH = end;
                    item.BILL_NO = bills.ToArray()[n].BILL_NO;
                    item.BILLDATE = bills.ToArray()[n].BILL_DATE.ToString("yyyy-MM-dd");

                    var isexit = list.FirstOrDefault (i => i.WAREHOUSE_CODE == warehouse && i.PRODUCT_CODE == productcode && i.BILL_NO == item.BILL_NO);
                    if (btypecode == "001" || btypecode == "007") { //入库单 包括退库单,抽检补料入库单
                        if (bills.ToArray()[n].BILL_METHOD == "0" || bills.ToArray()[n].BILL_METHOD == "1")
                        { //入库单和退库单
                            if (isexit == null)
                                item.IN_QUANTITY = packagecout * realweight;
                            else
                                isexit.IN_QUANTITY += packagecout * realweight;
                        }
                        if (bills.ToArray()[n].BILL_METHOD == "2" || bills.ToArray()[n].BILL_METHOD == "3")
                        { //抽检补料入库单
                            if (isexit == null)
                                item.INSPECTIN_QUANTITY = packagecout * realweight;
                            else
                                isexit.INSPECTIN_QUANTITY += packagecout * realweight;
                        }
                    }
                    else if (btypecode == "002" || btypecode == "006") { //出库单包括倒库单
                        if (isexit == null)
                            item.OUT_QUANTITY = packagecout * realweight;
                        else
                            isexit.OUT_QUANTITY += packagecout * realweight;
                    }
                    else if (btypecode == "003") { //抽检单
                        if (isexit == null)
                            item.INSPECTOUT_QUANTITY = packagecout * realweight;
                        else
                            isexit.INSPECTOUT_QUANTITY += packagecout * realweight;
                    }
                    else if (btypecode == "009") { //损益单
                        if (isexit == null)
                            item.INCOME_QUANTITY = packagecout * realweight;
                        else
                            isexit.INCOME_QUANTITY  += packagecout * realweight;
                    }
                    else if (btypecode == "005")
                    { //紧急补料单
                        if (isexit == null)
                            item.FEEDING_QUANTITY = packagecout * realweight;
                        else
                            isexit.FEEDING_QUANTITY += packagecout * realweight;
                    }
                    else { 
                    }
                    if (isexit == null)
                        list.Add(item);

                }
                else { // 不符合条件

                }
            }
            int total = list.Count();
            var tmp = list.OrderBy(i => i.PRODUCT_CODE ).Select(i => i);
            DataTable dt = THOK.Common.ConvertData.LinqQueryToDataTable(tmp);
            THOK.Common.PrintHandle.dt = dt;
            tmp = tmp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = tmp.ToArray() };
        }
    }
}
