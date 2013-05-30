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
    public class StockCheckSearchService : ServiceBase<CheckBillMaster>, IStockCheckSearchService
    {
        [Dependency]
        public IStockCheckSearchRepository StockCheckSearchRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IStockCheckSearch 成员

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
                    statusStr = "已入库";
                    break;
                case "5":
                    statusStr = "已生成损益";
                    break;
            }
            return statusStr;
        }

        public object GetDetails(int page, int rows, string BillNo, string WarehouseCode, string BeginDate, string EndDate, string OperatePersonCode, string CheckPersonCode, string Operate_Status)
        {
            IQueryable<CheckBillMaster> StockCheckQuery = StockCheckSearchRepository.GetQueryable();
            var StockCheckSearch = StockCheckQuery.Where(i => i.BillNo.Contains(BillNo)
                                                    && i.WarehouseCode.Contains(WarehouseCode)
                                                    && i.OperatePerson.EmployeeCode.Contains(OperatePersonCode)
                                                    && i.Status.Contains(Operate_Status));
            if (!BeginDate.Equals(string.Empty))
            {
                DateTime begin = Convert.ToDateTime(BeginDate);
                StockCheckSearch = StockCheckSearch.Where(i => i.BillDate >= begin);
            }

            if (!EndDate.Equals(string.Empty))
            {
                DateTime end = Convert.ToDateTime(EndDate);
                StockCheckSearch = StockCheckSearch.Where(i => i.BillDate <= end);
            }

            if (!CheckPersonCode.Equals(string.Empty))
            {
                StockCheckSearch = StockCheckSearch.Where(i => i.VerifyPerson.EmployeeCode == CheckPersonCode);
            }

            StockCheckSearch = StockCheckSearch.OrderByDescending(i => new { i.BillDate, i.BillNo });
            int total = StockCheckSearch.Count();
            StockCheckSearch = StockCheckSearch.Skip((page - 1) * rows).Take(rows);
            var CheckSearch = StockCheckSearch.ToArray().Select(s => new
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
            return new { total, rows = CheckSearch.ToArray() };
        }

        #endregion

    }
}
