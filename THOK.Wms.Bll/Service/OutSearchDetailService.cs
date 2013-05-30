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
    public class OutSearchDetailService : ServiceBase<OutBillAllot>, IOutSearchDetailService
    {
        [Dependency]
        public IOutSearchDetailRepository OutSearchDetailRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IOutSearchDetailRepository 成员

        public object GetDetails(int page, int rows, string BillNo)
        {
            IQueryable<OutBillAllot> outBillDetailQuery = OutSearchDetailRepository.GetQueryable();
            var OutBillAllots = outBillDetailQuery.Where(i => i.BillNo.Contains(BillNo)).OrderBy(i => i.BillNo);
            int total = OutBillAllots.Count();
            var OutBillAllot = OutBillAllots.Skip((page - 1) * rows).Take(rows);
            var OutBillAllotDetail = OutBillAllot.ToArray().Select(i => new
            {
                i.ID,
                i.BillNo,
                i.ProductCode,
                i.Product.ProductName,
                i.UnitCode,
                i.Unit.UnitName,
                i.CellCode,
                i.Cell.CellName,
                i.StorageCode,
                AllotQuantity = i.AllotQuantity / i.Unit.Count,
                RealQuantity = i.RealQuantity / i.Unit.Count,
                i.Status
            });
            return new { total, rows = OutBillAllotDetail.ToArray() };
        }

        #endregion
    }
}
