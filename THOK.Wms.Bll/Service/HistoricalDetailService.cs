using System;
using System.Linq;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    public class HistoricalDetailService : IHistoricalDetailService
    {
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

        #region IHistoricalDetailService 成员

        public object GetDetails(int page, int rows, string warehouseCode, string productCode, string beginDate, string endDate)
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
                Status = a.BillQuantity == a.RealQuantity ? "1":"0"
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
            })).Union(differQuery.Where(a => a.Quantity > 0 ).Select(a => new
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
            if (!beginDate.Equals(string.Empty))
            {
                DateTime begin = Convert.ToDateTime(beginDate);
                Allquery = Allquery.Where(i => i.BillDate >= begin);
            }
            if (!endDate.Equals(string.Empty))
            {
                DateTime end = Convert.ToDateTime(endDate);
                Allquery = Allquery.Where(i => i.BillDate <= end);
            }
            Allquery = Allquery.Where(i => i.ProductCode.Contains(productCode) && i.WarehouseCode.Contains(warehouseCode)).OrderBy(a => a.WarehouseName).ThenBy(a => a.ProductName).ThenByDescending(a => a.BillDate);
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
        
        public System.Data.DataTable GetHistoryDetail(int page, int rows, string warehouseCode, string productCode, string beginDate, string endDate)
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
            if (!beginDate.Equals(string.Empty))
            {
                DateTime begin = Convert.ToDateTime(beginDate);
                Allquery = Allquery.Where(i => i.BillDate >= begin).OrderByDescending(a => a.BillDate);
            }
            if (!endDate.Equals(string.Empty))
            {
                DateTime end = Convert.ToDateTime(endDate);
                Allquery = Allquery.Where(i => i.BillDate <= end).OrderByDescending(a => a.BillDate);
            }
            Allquery = Allquery.Where(a => 1 == 1).OrderBy(a => a.WarehouseName).OrderByDescending(a => a.BillDate);           
            var query = Allquery.Where(i => i.ProductCode.Contains(productCode) && i.WarehouseCode.Contains(warehouseCode)).ToArray().Select(i => new
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
                i.UnitName

            });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("日期", typeof(string));
            dt.Columns.Add("仓库名称", typeof(string));
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
                        item.WarehouseName,
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
