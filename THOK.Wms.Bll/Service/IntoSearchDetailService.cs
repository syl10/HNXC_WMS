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
    public class IntoSearchDetailService : ServiceBase<InBillAllot>,IIntoSearchDetailService
    {
        [Dependency]
        public IIntoSearchDetailRepository IntoSearchDetailRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IIntoSearchDetailRepository 成员

        public object GetDetails(int page, int rows, string BillNo)
        {
            IQueryable<InBillAllot> inBillDetailQuery = IntoSearchDetailRepository.GetQueryable();
            var inBillAllots = inBillDetailQuery.Where(i => i.BillNo.Contains(BillNo)).OrderBy(i => i.BillNo);
            int total = inBillAllots.Count();
            var inBillAllot = inBillAllots.Skip((page - 1) * rows).Take(rows);
            var inBillAllotDetail = inBillAllot.ToArray().Select(i => new
            {
                i.ID,
                i.BillNo,
                i.ProductCode,
                i.Product.ProductName,
                AllotQuantity = i.AllotQuantity / i.Unit.Count,
                i.CellCode,
                i.Cell.CellName,
                i.StorageCode,
                i.UnitCode,
                i.Unit.UnitName,
                RealQuantity = i.RealQuantity / i.Unit.Count,
                i.Status
            });
            return new { total, rows = inBillAllotDetail.ToArray() };
        }
        #endregion
    }
}
