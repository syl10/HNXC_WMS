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
    public class StockMoveSearchService : ServiceBase<MoveBillMaster>, IStockMoveSearchService
    {
        [Dependency]
        public IStockMoveSearchRepository StockMoveSearchRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IStockMoveSearch 成员

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
                    statusStr = "执行中";
                    break;
                case "4":
                    statusStr = "已结单";
                    break;
            }
            return statusStr;
        }

        public object GetDetails(int page, int rows, string BillNo, string WarehouseCode, string BeginDate, string EndDate, string OperatePersonCode, string CheckPersonCode, string Operate_Status)
        {
            IQueryable<MoveBillMaster> StockMoveQuery = StockMoveSearchRepository.GetQueryable();
            var StockMoveSearch = StockMoveQuery.Where(i => i.BillNo.Contains(BillNo)
                                                    && i.WarehouseCode.Contains(WarehouseCode)
                                                    && i.OperatePerson.EmployeeCode.Contains(OperatePersonCode)
                                                    && i.Status.Contains(Operate_Status));
            if (!BeginDate.Equals(string.Empty))
            {
                DateTime begin = Convert.ToDateTime(BeginDate);
                StockMoveSearch = StockMoveSearch.Where(i => i.BillDate >= begin);
            }

            if (!EndDate.Equals(string.Empty))
            {
                DateTime end = Convert.ToDateTime(EndDate);
                StockMoveSearch = StockMoveSearch.Where(i => i.BillDate <= end);
            }

            if (!CheckPersonCode.Equals(string.Empty))
            {
                StockMoveSearch = StockMoveSearch.Where(i => i.VerifyPerson.EmployeeCode == CheckPersonCode);
            }

            StockMoveSearch = StockMoveSearch.OrderByDescending(i => new { i.BillDate, i.BillNo });
            int total = StockMoveSearch.Count();
            StockMoveSearch = StockMoveSearch.Skip((page - 1) * rows).Take(rows);
            var MoveSearch = StockMoveSearch.ToArray().Select(s => new
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
            return new { total, rows = MoveSearch.ToArray() };
        }

        #endregion

    }
}
