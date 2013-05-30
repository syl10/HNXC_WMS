using System;
using System.Linq;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    public class StockledgerService : IStockledgerService
    {
        [Dependency]
        public IDailyBalanceRepository StockledgerRepository { get; set; }

        [Dependency]
        public IInBillDetailRepository InBillDetailRepository { get; set; }
        [Dependency]
        public IOutBillDetailRepository OutBillDetailRepository { get; set; }
        [Dependency]
        public IProfitLossBillDetailRepository ProfitLossBillDetailRepository { get; set; }

        //protected override Type LogPrefix
        //{
        //    get { return this.GetType(); }
        //}

        #region IStockledgerService 成员

        public object GetDetails(int page, int rows, string warehouseCode, string productCode, string beginDate, string endDate, string unitType)
        {
            if (unitType == null || unitType == "")
            {
                unitType = "1";
            }
            var query = StockledgerRepository.GetQueryable().Where(i => i.ProductCode.Contains(productCode)
                                                                     && i.WarehouseCode.Contains(warehouseCode))
                                         .Select(i => new
                                         {
                                             i.SettleDate,
                                             i.ProductCode,
                                             i.Product.ProductName,
                                             UnitCode01 = i.Product.UnitList.Unit01.UnitCode,
                                             UnitName01 = i.Product.UnitList.Unit01.UnitName,
                                             UnitCode02 = i.Product.UnitList.Unit02.UnitCode,
                                             UnitName02 = i.Product.UnitList.Unit02.UnitName,
                                             Count01 = i.Product.UnitList.Unit01.Count,
                                             Count02 = i.Product.UnitList.Unit02.Count,
                                             i.WarehouseCode,
                                             i.Warehouse.WarehouseName,
                                             i.Beginning,
                                             i.EntryAmount,
                                             i.DeliveryAmount,
                                             i.ProfitAmount,
                                             i.LossAmount,
                                             i.Ending
                                         }).OrderBy(i => i.WarehouseName)
                                           .ThenBy(i => i.ProductName)
                                           .ThenByDescending(i => i.SettleDate);
            if (!beginDate.Equals(string.Empty))
            {
                DateTime begin = Convert.ToDateTime(beginDate);
                query = query.Where(i => i.SettleDate >= begin).OrderBy(i => i.ProductName).ThenByDescending(i => i.SettleDate);
            }

            if (!endDate.Equals(string.Empty))
            {
                DateTime end = Convert.ToDateTime(endDate);
                query = query.Where(i => i.SettleDate <= end).OrderBy(i => i.ProductName).ThenByDescending(i => i.SettleDate);
            }
            int total = query.Count();
            var querys = query.Skip((page - 1) * rows).Take(rows);

            string unitName = "";
            decimal count = 1;
            
            //标准单位（标准件||标准条）
            if ( unitType == "1" || unitType == "2")
            {
                if (unitType == "1")
                {
                    unitName = "标准件";
                    count = 10000;
                }

                if (unitType == "2")
                {
                    unitName = "标准条";
                    count = 200;
                }
                var Stockledger = querys.ToArray().Select(i => new
                                         {
                                             SettleDate = i.SettleDate.ToString("yyyy-MM-dd"),
                                             i.ProductCode,
                                             i.ProductName,
                                             UnitCode = "",
                                             UnitName = unitName,
                                             i.WarehouseCode,
                                             i.WarehouseName,
                                             Beginning = i.Beginning / count,
                                             EntryAmount = i.EntryAmount / count,
                                             DeliveryAmount = i.DeliveryAmount / count,
                                             ProfitAmount = i.ProfitAmount / count,
                                             LossAmount = i.LossAmount / count,
                                             Ending = i.Ending / count
                                         });
                return new { total, rows = Stockledger.ToArray() };
            }

            //自然件
            if (unitType == "3" || unitType == "4")
            {
                var Stockledger = querys.ToArray().Select(i => new
                                         {
                                             SettleDate = i.SettleDate.ToString("yyyy-MM-dd"),
                                             i.ProductCode,
                                             i.ProductName,
                                             UnitCode = unitType == "3" ? i.UnitCode01 : i.UnitCode02,
                                             UnitName = unitType == "3" ? i.UnitName01 : i.UnitName02,
                                             i.WarehouseCode,
                                             i.WarehouseName,
                                             Beginning = i.Beginning / (unitType == "3" ? i.Count01 : i.Count02),
                                             EntryAmount = i.EntryAmount / (unitType == "3" ? i.Count01 : i.Count02),
                                             DeliveryAmount = i.DeliveryAmount / (unitType == "3" ? i.Count01 : i.Count02),
                                             ProfitAmount = i.ProfitAmount / (unitType == "3" ? i.Count01 : i.Count02),
                                             LossAmount = i.LossAmount / (unitType == "3" ? i.Count01 : i.Count02),
                                             Ending = i.Ending / (unitType == "3" ? i.Count01 : i.Count02),
                                         });
                return new { total, rows = Stockledger.ToArray() };
            }
            return new { total, rows = querys.ToArray() };
        }
        
        public object GetInfoDetails(int page, int rows, string warehouseCode, string productCode, string settleDate)
        {
            var inQuery = InBillDetailRepository.GetQueryable();
            var outQuery = OutBillDetailRepository.GetQueryable();
            var differQuery = ProfitLossBillDetailRepository.GetQueryable();
            var Allquery = inQuery.Where(a => a.BillQuantity > 0 && a.RealQuantity > 0).Select(a => new
            {
                BillDate = a.InBillMaster.BillDate,
                a.InBillMaster.Warehouse.WarehouseCode,
                a.InBillMaster.Warehouse.WarehouseName,
                a.BillNo,
                a.InBillMaster.BillType.BillTypeCode,
                a.InBillMaster.BillType.BillTypeName,
                a.ProductCode,
                a.Product.ProductName,
                a.RealQuantity,
                a.Unit.Count,
                Count1 = a.Product.UnitList.Unit01.Count,//自然件单位
                Count2 = a.Product.UnitList.Unit02.Count,//条单位
                a.Unit.UnitName,
                Status = a.BillQuantity == a.RealQuantity ? "1" : "0"
            }).Union(outQuery.Where(a => a.BillQuantity > 0 && a.RealQuantity > 0).Select(a => new
            {
                BillDate = a.OutBillMaster.BillDate,
                a.OutBillMaster.Warehouse.WarehouseCode,
                a.OutBillMaster.Warehouse.WarehouseName,
                a.BillNo,
                a.OutBillMaster.BillType.BillTypeCode,
                a.OutBillMaster.BillType.BillTypeName,
                a.ProductCode,
                a.Product.ProductName,
                a.RealQuantity,
                a.Unit.Count,
                Count1 = a.Product.UnitList.Unit01.Count,//自然件单位
                Count2 = a.Product.UnitList.Unit02.Count,//条单位
                a.Unit.UnitName,
                Status = a.BillQuantity == a.RealQuantity ? "1" : "0"
            })).Union(differQuery.Where(a => a.Quantity > 0).Select(a => new
            {
                BillDate = a.ProfitLossBillMaster.BillDate,
                a.ProfitLossBillMaster.Warehouse.WarehouseCode,
                a.ProfitLossBillMaster.Warehouse.WarehouseName,
                a.BillNo,
                a.ProfitLossBillMaster.BillType.BillTypeCode,
                a.ProfitLossBillMaster.BillType.BillTypeName,
                a.ProductCode,
                a.Product.ProductName,
                RealQuantity = a.Quantity,
                a.Unit.Count,
                Count1 = a.Product.UnitList.Unit01.Count,//自然件单位
                Count2 = a.Product.UnitList.Unit02.Count,//条单位
                a.Unit.UnitName,
                Status = a.ProfitLossBillMaster.Status == "2" ? "1" : "0"
            }));
            if (!settleDate.Equals(string.Empty))
            {
                DateTime date = Convert.ToDateTime(settleDate);
                Allquery = Allquery.Where(i => i.BillDate.Year == date.Year && i.BillDate.Month == date.Month && i.BillDate.Day == date.Day);
            }
            Allquery = Allquery.Where(i => i.ProductCode.Contains(productCode) && i.WarehouseCode.Contains(warehouseCode)).OrderBy(a => a.BillDate).ThenBy(a => a.WarehouseName);
            int total = Allquery.Count();
            Allquery = Allquery.Skip((page - 1) * rows).Take(rows);
            var query = Allquery.ToArray().Select(i => new
            {
                BillDate = i.BillDate.ToString("yyyy-MM-dd"),
                i.WarehouseCode,
                i.WarehouseName,
                i.BillNo,
                i.BillTypeCode,
                i.BillTypeName,
                i.ProductCode,
                i.ProductName,
                RealQuantity = Convert.ToDouble(i.RealQuantity / i.Count),
                JQuantity = Convert.ToDouble(i.RealQuantity / i.Count1),
                TQuantity = Convert.ToInt32(i.RealQuantity / i.Count2),
                i.UnitName,
                i.Status

            });
            return new { total, rows = query.ToArray() };
        }

        #endregion


        public System.Data.DataTable GetInfoDetail(int page, int rows, string warehouseCode, string productCode, string settleDate)
        {
            var inQuery = InBillDetailRepository.GetQueryable();
            var outQuery = OutBillDetailRepository.GetQueryable();
            var differQuery = ProfitLossBillDetailRepository.GetQueryable();
            var Allquery = inQuery.Select(a => new
            {
                BillDate = a.InBillMaster.BillDate,
                a.InBillMaster.Warehouse.WarehouseCode,
                a.InBillMaster.Warehouse.WarehouseName,
                a.BillNo,
                a.InBillMaster.BillType.BillTypeCode,
                a.InBillMaster.BillType.BillTypeName,
                a.ProductCode,
                a.Product.ProductName,
                a.RealQuantity,
                a.Unit.UnitName
            }).Union(outQuery.Select(a => new
            {
                BillDate = a.OutBillMaster.BillDate,
                a.OutBillMaster.Warehouse.WarehouseCode,
                a.OutBillMaster.Warehouse.WarehouseName,
                a.BillNo,
                a.OutBillMaster.BillType.BillTypeCode,
                a.OutBillMaster.BillType.BillTypeName,
                a.ProductCode,
                a.Product.ProductName,
                a.RealQuantity,
                a.Unit.UnitName
            })).Union(differQuery.Select(a => new
            {
                BillDate = a.ProfitLossBillMaster.BillDate,
                a.ProfitLossBillMaster.Warehouse.WarehouseCode,
                a.ProfitLossBillMaster.Warehouse.WarehouseName,
                a.BillNo,
                a.ProfitLossBillMaster.BillType.BillTypeCode,
                a.ProfitLossBillMaster.BillType.BillTypeName,
                a.ProductCode,
                a.Product.ProductName,
                RealQuantity = a.Quantity,
                a.Unit.UnitName
            }));
            if (!settleDate.Equals(string.Empty))
            {
                DateTime date = Convert.ToDateTime(settleDate);
                Allquery = Allquery.Where(i => i.BillDate.Year == date.Year && i.BillDate.Month == date.Month && i.BillDate.Day == date.Day);
            }
            Allquery = Allquery.Where(i => i.ProductCode.Contains(productCode) && i.WarehouseCode.Contains(warehouseCode)).OrderBy(a => a.BillDate).ThenBy(a => a.WarehouseName);
            var query = Allquery.ToArray().Select(i => new
            {
                BillDate = i.BillDate.ToString("yyyy-MM-dd"),
                i.WarehouseCode,
                i.WarehouseName,
                i.BillNo,
                i.BillTypeCode,
                i.BillTypeName,
                i.ProductCode,
                i.ProductName,
                i.RealQuantity,
                JQuantity = Convert.ToDouble(i.RealQuantity / 50),
                TQuantity = i.RealQuantity,
                i.UnitName

            });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("日期", typeof(string));
            dt.Columns.Add("单据编号", typeof(string));
            dt.Columns.Add("单据业务", typeof(string));
            dt.Columns.Add("商品代码", typeof(string));
            dt.Columns.Add("商品名称", typeof(string));
            dt.Columns.Add("账面数量", typeof(string));
            dt.Columns.Add("数量(自然件)", typeof(string));
            dt.Columns.Add("数量(条)", typeof(string));
            dt.Columns.Add("单据单位", typeof(string));
            foreach (var item in query)
            {
                dt.Rows.Add
                    (
                        item.BillDate,
                        item.BillNo,
                        item.BillTypeName,
                        item.ProductCode,
                        item.ProductName,
                        item.RealQuantity,
                        item.JQuantity,
                        item.TQuantity,
                        item.UnitName
                    );
            }
            return dt;
        }
    }
}
