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
    public class MoveSearchDetailService : ServiceBase<MoveBillDetail>, IMoveSearchDetailService
    {
        [Dependency]
        public IMoveSearchDetailRepository MoveSearchDetailRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IMoveSearchDetailRepository 成员

        public object GetDetails(int page, int rows, string BillNo)
        {
            IQueryable<MoveBillDetail> moveBillDetailQuery = MoveSearchDetailRepository.GetQueryable();
            var moveBillDetails = moveBillDetailQuery.Where(i => i.BillNo.Contains(BillNo)).OrderBy(i => i.BillNo);
            int total = moveBillDetails.Count();
            var moveBillDetail = moveBillDetails.Skip((page - 1) * rows).Take(rows);
            var moveDetail = moveBillDetail.ToArray().Select(i => new
            {
                i.ID,
                i.BillNo,
                i.ProductCode,
                i.Product.ProductName,
                i.UnitCode,
                i.Unit.UnitName,
                i.InCellCode,
                PlaceName_In = i.InCell.CellName,
                i.OutCellCode,
                PlaceName_Out = i.OutCell.CellName,
                RealQuantity = i.RealQuantity / i.Unit.Count,
                i.Status
            });
            return new { total, rows = moveDetail.ToArray() };
        }
        #endregion
    }
}