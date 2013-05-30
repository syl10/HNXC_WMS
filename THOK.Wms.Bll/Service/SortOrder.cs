using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.Download.Interfaces;

namespace THOK.Wms.Bll.Service
{
    public class SortOrderService : ServiceBase<SortOrder>, ISortOrderService
    {
        [Dependency]
        public ISortOrderRepository SortOrderRepository { get; set; }

        [Dependency]
        public ISortOrderDetailRepository SortOrderDetailRepository { get; set; }

        [Dependency]
        public ISortOrderDispatchRepository SortOrderDispatchRepository { get; set; }

        [Dependency]
        public IProductRepository ProductRepository { get; set; }

        [Dependency]
        public ISortingDownService SortingDownService { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region ISortOrderService 成员

        public object GetDetails(int page, int rows, string OrderID, string orderDate, string productCode)
        {
            if (orderDate == string.Empty || orderDate == null)
            {
                orderDate = DateTime.Now.ToString("yyyyMMdd");
            }
            else
            {
                orderDate = Convert.ToDateTime(orderDate).ToString("yyyyMMdd");
            }
            IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();
            IQueryable<SortOrderDetail> sortOrderDetailQuery = SortOrderDetailRepository.GetQueryable();
            var sortOrderDetail = sortOrderDetailQuery.Where(s => s.OrderDetailID == s.OrderDetailID).Select(s => new {s.OrderID,s.Product,s.RealQuantity });
            if (productCode != string.Empty && productCode != null)
            {
                sortOrderDetail = sortOrderDetail.Where(s => s.Product.ProductCode == productCode && s.RealQuantity > 0);
            }

            var sortOrder = sortOrderQuery.Where(s => s.OrderDate == orderDate && sortOrderDetail.Any(d => d.OrderID == s.OrderID));           
            
            if (OrderID != string.Empty && OrderID != null)
            {
                sortOrder = sortOrder.Where(s => s.OrderID == OrderID);
            }

            var temp = sortOrder.AsEnumerable().OrderBy(t => t.OrderID).Select(s => new
            {
                s.OrderID,
                s.CompanyCode,
                s.SaleRegionCode,
                s.OrderDate,
                s.OrderType,
                s.CustomerCode,
                s.CustomerName,
                s.QuantitySum,
                s.DeliverLineCode,
                s.AmountSum,
                s.DetailNum,
                s.DeliverOrder,
                s.DeliverDate,
                IsActive = s.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = s.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                s.Description
            });

            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }


        public object GetDetails(string orderDate)
        {
            if (orderDate == string.Empty || orderDate == null)
            {
                orderDate = DateTime.Now.ToString("yyyyMMdd");
            }
            else
            {
                orderDate = Convert.ToDateTime(orderDate).ToString("yyyyMMdd");
            }
            IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();
            IQueryable<SortOrderDispatch> SortOrderDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            var sortorderDisp = SortOrderDispatchQuery.Where(s => s.OrderDate.Contains(orderDate));
            var sortOrder = sortOrderQuery.Where(s => s.OrderDate.Contains(orderDate)&&!sortorderDisp.Any(d=>d.DeliverLineCode==s.DeliverLineCode))
                                          //.Join(SortOrderDispatchQuery,
                                          //so => new { so.OrderDate, so.DeliverLineCode },
                                          //sd => new { sd.OrderDate, sd.DeliverLineCode },
                                          //(so, sd) => new { so.OrderDate, so.DeliverLineCode, so.DeliverLine.DeliverLineName, so.QuantitySum, so.DetailNum, sd.SortingLine, so.AmountSum })
                                          .GroupBy(s => new { s.OrderDate, s.DeliverLineCode,s.DeliverLine})
                                          .Select(s => new
                                          {
                                              DeliverLineCode = s.Key.DeliverLineCode,
                                              DeliverLineName = s.Key.DeliverLine.DeliverLineName,
                                              OrderDate = s.Key.OrderDate,
                                              //SortingLineCode = s.Key.SortingLine.SortingLineCode,
                                              //SortingLineName = s.Key.SortingLine.SortingLineName,
                                              QuantitySum = s.Sum(p => p.QuantitySum),
                                              AmountSum = s.Sum(p => p.AmountSum),
                                              DetailNum = s.Sum(p => p.DetailNum),
                                              IsActive = "可用"
                                          });

            return sortOrder.OrderBy(s=>s.DeliverLineName).ToArray();
        }

        #endregion

        public bool DownSortOrder(string beginDate, string endDate, out string errorInfo)
        {
            errorInfo = string.Empty;
            bool result = false;
            string sortOrderStrs = "";
            string sortOrderList = "";
            try
            {
                var sortOrderIds = SortOrderRepository.GetQueryable().Where(s=>s.OrderID==s.OrderID).Select(s => new { s.OrderID }).ToArray();
                
                for (int i = 0; i < sortOrderIds.Length; i++)
                {
                    sortOrderStrs += sortOrderIds[i].OrderID + ",";
                }

                SortOrder[] SortOrders = SortingDownService.GetSortOrder(beginDate, endDate, sortOrderStrs);

                foreach (var item in SortOrders)
                {
                    var sortOrder = new SortOrder();
                    sortOrder.OrderID = item.OrderID;
                    sortOrder.CompanyCode = item.CompanyCode;
                    sortOrder.SaleRegionCode = item.SaleRegionCode;
                    sortOrder.OrderDate = item.OrderDate;
                    sortOrder.OrderType = item.OrderType;
                    sortOrder.CustomerCode = item.CustomerCode;
                    sortOrder.CustomerName = item.CustomerName;
                    sortOrder.DeliverLineCode = item.DeliverLineCode;
                    sortOrder.QuantitySum = item.QuantitySum;
                    sortOrder.AmountSum = item.AmountSum;
                    sortOrder.DetailNum = item.DetailNum;
                    sortOrder.DeliverOrder = item.DeliverOrder;
                    sortOrder.DeliverDate = item.DeliverDate;
                    sortOrder.Description = item.Description;
                    sortOrder.IsActive = item.IsActive;
                    sortOrder.UpdateTime = item.UpdateTime;
                    SortOrderRepository.Add(sortOrder);
                    sortOrderList += item.OrderID + ",";
                }
                if (sortOrderList != string.Empty)
                {
                    SortOrderDetail[] SortOrderDetails = null; //SortingDownService.GetSortOrderDetail(sortOrderList);
                    foreach (var detail in SortOrderDetails)
                    {
                        var sortOrderDetail = new SortOrderDetail();
                        var product = ProductRepository.GetQueryable().FirstOrDefault(p => p.ProductCode == detail.ProductCode);                        
                        sortOrderDetail.OrderDetailID = detail.OrderDetailID;
                        sortOrderDetail.OrderID = detail.OrderID;
                        sortOrderDetail.ProductCode = detail.ProductCode;
                        sortOrderDetail.ProductName = detail.ProductName;
                        sortOrderDetail.UnitCode = detail.UnitCode;
                        sortOrderDetail.UnitName = detail.UnitName;
                        sortOrderDetail.DemandQuantity = detail.DemandQuantity * product.UnitList.Unit02.Count;
                        sortOrderDetail.RealQuantity = detail.RealQuantity * product.UnitList.Unit02.Count;
                        sortOrderDetail.Price = detail.Price;
                        sortOrderDetail.Amount = detail.Amount;
                        sortOrderDetail.UnitQuantity = product.UnitList.Quantity02;
                        SortOrderDetailRepository.Add(sortOrderDetail);
                    }
                }
                SortOrderRepository.SaveChanges();
                result = true;
            }
            catch (Exception e)
            {
                errorInfo = "出错，原因：" + e.Message;
            }
            return result;
        }
    }
}
