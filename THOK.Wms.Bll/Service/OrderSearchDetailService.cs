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
    public class OrderSearchDetailService : ServiceBase<SortOrderDetail>, IOrderSearchDetailService
    {
        [Dependency]
        public IOrderSearchDetailRepository OrderSearchDetailRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IOrderSearchDetailRepository 成员

        public object GetDetails(int page, int rows, string OrderID)
        {
            IQueryable<SortOrderDetail> OrderDetailQuery = OrderSearchDetailRepository.GetQueryable();
            var OrderDetails = OrderDetailQuery.Where(i => i.OrderID.Contains(OrderID)).OrderBy(i => i.OrderID);
            int total = OrderDetails.Count();
            var OrderDetail = OrderDetails.Skip((page - 1) * rows).Take(rows);
            var orderDetail = OrderDetail.ToArray().Select(o => new
            {
                o.OrderID,
                o.Price,
                o.ProductCode,
                o.Product.ProductName,
                o.OrderDetailID,
                o.RealQuantity,
                o.UnitCode,
                o.UnitName,
                o.Amount
            });
            return new { total, rows = orderDetail.ToArray() };
        }
        #endregion
    }
}