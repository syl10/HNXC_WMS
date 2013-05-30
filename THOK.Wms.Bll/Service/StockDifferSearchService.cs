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
    public class StockDifferSearchService : ServiceBase<ProfitLossBillMaster>, IStockDifferSearchService
    {
        [Dependency]
        public IStockDifferSearchRepository StockDifferSearchRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IStockDifferSearch 成员

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
                    statusStr = "已更新库存";
                    break;
            }
            return statusStr;
        }

        public object GetDetails(int page, int rows, string BillNo, string CheckBillNo, string WarehouseCode, string BeginDate, string EndDate, string OperatePersonCode, string CheckPersonCode, string Operate_Status)
        {
            IQueryable<ProfitLossBillMaster> StockDifferQuery = StockDifferSearchRepository.GetQueryable();
            var StockDifferSearch = StockDifferQuery.Where(i => i.BillNo.Contains(BillNo)
                                                    && i.WarehouseCode.Contains(WarehouseCode)
                                                    && i.OperatePerson.EmployeeCode.Contains(OperatePersonCode)
                                                    && i.Status.Contains(Operate_Status));
            if (!BeginDate.Equals(string.Empty))
            {
                DateTime begin = Convert.ToDateTime(BeginDate);
                StockDifferSearch = StockDifferSearch.Where(i => i.BillDate >= begin);
            }

            if (!EndDate.Equals(string.Empty))
            {
                DateTime end = Convert.ToDateTime(EndDate);
                StockDifferSearch = StockDifferSearch.Where(i => i.BillDate <= end);
            }

            if (!CheckBillNo.Equals(string.Empty))
            {
                StockDifferSearch = StockDifferSearch.Where(i => i.CheckBillNo == CheckBillNo);
            }

            if (!CheckPersonCode.Equals(string.Empty))
            {
                StockDifferSearch = StockDifferSearch.Where(i => i.VerifyPerson.EmployeeCode == CheckPersonCode);
            }

            StockDifferSearch = StockDifferSearch.OrderByDescending(i => new { i.BillDate, i.BillNo });
            int total = StockDifferSearch.Count();
            StockDifferSearch = StockDifferSearch.Skip((page - 1) * rows).Take(rows);
            var DifferSearch = StockDifferSearch.ToArray().Select(s => new
            {
                s.BillNo,
                s.CheckBillNo,
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
            return new { total, rows = DifferSearch.ToArray() };
        }

        #endregion

    }
}