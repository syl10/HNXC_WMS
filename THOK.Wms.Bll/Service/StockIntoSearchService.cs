using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    public class StockIntoSearchService : ServiceBase<InBillMaster>, IStockIntoSearchService
    {
        [Dependency]
        public IStockIntoSearchRepository StockIntoSearchRepository { get; set; }
        [Dependency]
        public IInBillDetailRepository InBillDetailRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IStockIntoSearch 成员

        public string WhatStatus(string status)
        {
            string statusStr = "";
            switch (status)
            {
                case "1":
                    statusStr = "已录入";
                    break;
                case "2":
                    statusStr = "已审核";
                    break;
                case "3":
                    statusStr = "已分配";
                    break;
                case "4":
                    statusStr = "已确认";
                    break;
                case "5":
                    statusStr = "执行中";
                    break;
                case "6":
                    statusStr = "已结单";
                    break;
            }
            return statusStr;
        }

        public object GetDetails(int page, int rows, string BillNo, string WarehouseCode, string BeginDate, string EndDate, string OperatePersonCode, string CheckPersonCode, string Operate_Status)
        {
            IQueryable<InBillMaster> StockIntoQuery = StockIntoSearchRepository.GetQueryable();
            var StockIntoSearch = StockIntoQuery.Where(i => i.BillNo.Contains(BillNo)
                                                    && i.WarehouseCode.Contains(WarehouseCode)
                                                    && i.OperatePerson.EmployeeCode.Contains(OperatePersonCode)
                                                    && i.Status.Contains(Operate_Status));
            if (!BeginDate.Equals(string.Empty))
            {
                DateTime begin = Convert.ToDateTime(BeginDate);
                StockIntoSearch = StockIntoSearch.Where(i => i.BillDate >= begin);
            }

            if (!EndDate.Equals(string.Empty))
            {
                DateTime end = Convert.ToDateTime(EndDate);
                StockIntoSearch = StockIntoSearch.Where(i => i.BillDate <= end);
            }

            if (!CheckPersonCode.Equals(string.Empty))
            {
                StockIntoSearch = StockIntoSearch.Where(i => i.VerifyPerson.EmployeeCode == CheckPersonCode);
            }

            StockIntoSearch = StockIntoSearch.OrderByDescending(i => new{i.BillDate,i.BillNo});
            int total = StockIntoSearch.Count();
            StockIntoSearch = StockIntoSearch.Skip((page - 1) * rows).Take(rows);
            var IntoSearch = StockIntoSearch.ToArray().Select(s => new
            {
                s.BillNo,
                s.Warehouse.WarehouseName,
                BillDate = s.BillDate.ToString("yyyy-MM-dd hh:mm:ss"),
                OperatePersonName = s.OperatePerson.EmployeeName,
                s.OperatePersonID,
                Status = WhatStatus(s.Status),
                VerifyPersonName = s.VerifyPersonID == null ? string.Empty : s.VerifyPerson.EmployeeName,
                VerifyDate = (s.VerifyDate == null ? string.Empty : ((DateTime)s.VerifyDate).ToString("yyyy-MM-dd hh:mm:ss")),
                Description = s.Description,
                UpdateTime = s.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss")
            });
            return new { total, rows = IntoSearch.ToArray() };
        }

        public object GetDetailInfos(int page, int rows, string BillNo)
        {
            IQueryable<InBillDetail> StockIntoQuery = InBillDetailRepository.GetQueryable();
            var StockIntoDetail = StockIntoQuery.Where(i => i.BillNo.Contains(BillNo)).OrderBy(i => i.BillNo);
            int total = StockIntoDetail.Count();
            var StockIntoDetails = StockIntoDetail.Skip((page - 1) * rows).Take(rows);
            var StockInto = StockIntoDetails.Select(i => new
                 {
                     i.ID,
                     i.BillNo,
                     i.ProductCode,
                     i.Product.ProductName,
                     i.UnitCode,
                     i.Unit.UnitName,
                     BillQuantity = i.BillQuantity / i.Unit.Count,
                     RealQuantity = i.RealQuantity / i.Unit.Count,
                     i.Price,
                     i.Description
                 });
            return new { total, rows = StockInto.ToArray() };
        }

        #endregion

    }
}
