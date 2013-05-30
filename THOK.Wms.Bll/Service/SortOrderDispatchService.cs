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
    public class SortOrderDispatchService : ServiceBase<SortOrderDispatch>, ISortOrderDispatchService
    {
        [Dependency]
        public ISortOrderDispatchRepository SortOrderDispatchRepository { get; set; }

        [Dependency]
        public ISortOrderRepository SortOrderRepository { get; set; }
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region ISortOrderDispatchService 成员

        public object GetDetails(int page, int rows, string OrderDate, string SortingLineCode)
        {
            IQueryable<SortOrderDispatch> sortDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            var sortDispatch = sortDispatchQuery.Where(s => s.WorkStatus != "2");
            if (OrderDate != string.Empty && OrderDate != null)
            {
                OrderDate = Convert.ToDateTime(OrderDate).ToString("yyyyMMdd");
                sortDispatch = sortDispatch.Where(s => s.OrderDate == OrderDate);
            }
            if (SortingLineCode != string.Empty && SortingLineCode != null)
            {
                sortDispatch = sortDispatch.Where(s => s.SortingLineCode == SortingLineCode);
            }
            var temp = sortDispatch.OrderBy(b => b.SortingLineCode).AsEnumerable().Select(b => new
           {
               b.ID,
               b.SortingLineCode,
               b.SortingLine.SortingLineName,
               b.OrderDate,
               b.DeliverLineCode,
               WorkStatus = b.WorkStatus == "1" ? "未作业" : "已作业",
               b.DeliverLine.DeliverLineName,
               IsActive = b.IsActive == "1" ? "可用" : "不可用",
               UpdateTime = b.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
           });

            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }

        public new bool Add(string SortingLineCode, string DeliverLineCodes, string orderDate)
        {
            if (orderDate == string.Empty || orderDate == null)
            {
                orderDate = DateTime.Now.ToString("yyyyMMdd");
            }
            else
            {
                orderDate = Convert.ToDateTime(orderDate).ToString("yyyyMMdd");
            }

            var sortOder = SortOrderRepository.GetQueryable().Where(s => DeliverLineCodes.Contains(s.DeliverLineCode) && s.OrderDate == orderDate)
                                              .GroupBy(s => new { s.DeliverLineCode, s.OrderDate })
                                              .Select(s => new { DeliverLineCode = s.Key.DeliverLineCode, OrderDate = s.Key.OrderDate });
            foreach (var item in sortOder.ToArray())
            {
                var sortOrderDispatch = new SortOrderDispatch();
                sortOrderDispatch.SortingLineCode = SortingLineCode;
                sortOrderDispatch.DeliverLineCode = item.DeliverLineCode;
                sortOrderDispatch.WorkStatus = "1";
                sortOrderDispatch.OrderDate = item.OrderDate;
                sortOrderDispatch.IsActive = "1";
                sortOrderDispatch.UpdateTime = DateTime.Now;

                SortOrderDispatchRepository.Add(sortOrderDispatch);
            }
            SortOrderDispatchRepository.SaveChanges();
            return true;
        }

        public bool Delete(string id)
        {
            int ID = Convert.ToInt32(id);
            var sortOrderDispatch = SortOrderDispatchRepository.GetQueryable()
               .FirstOrDefault(s => s.ID == ID);
            if (sortOrderDispatch != null)
            {
                SortOrderDispatchRepository.Delete(sortOrderDispatch);
                SortOrderDispatchRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(SortOrderDispatch sortDispatch)
        {
            var sortOrderDispatch = SortOrderDispatchRepository.GetQueryable().FirstOrDefault(s => s.ID == sortDispatch.ID);
            sortOrderDispatch.SortingLineCode = sortDispatch.SortingLineCode;
            sortOrderDispatch.DeliverLineCode = sortDispatch.DeliverLineCode;
            sortOrderDispatch.WorkStatus = "1";
            sortOrderDispatch.OrderDate = sortDispatch.OrderDate;
            sortOrderDispatch.IsActive = sortDispatch.IsActive;
            sortOrderDispatch.UpdateTime = DateTime.Now;

            SortOrderDispatchRepository.SaveChanges();
            return true;
        }

        public object GetWorkStatus()
        {
            IQueryable<SortOrderDispatch> sortDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();

            var temp = sortDispatchQuery.Where(s => s.WorkStatus == "1").ToArray().AsEnumerable()
                                           .Join(sortOrderQuery,
                                                dp => new { dp.OrderDate, dp.DeliverLineCode },
                                                om => new { om.OrderDate, om.DeliverLineCode },
                                                (dp, om) => new
                                                {
                                                    dp.ID,
                                                    dp.OrderDate,
                                                    dp.SortingLine,
                                                    dp.DeliverLine,
                                                    dp.IsActive,
                                                    dp.UpdateTime,
                                                    dp.WorkStatus,
                                                    om.QuantitySum
                                                }
                                           ).Select(r => new { r.ID, r.OrderDate, r.SortingLine, r.DeliverLine, r.WorkStatus, r.IsActive, r.UpdateTime, r.QuantitySum })
                                            .GroupBy(g => new { g.ID, g.OrderDate, g.DeliverLine, g.SortingLine, g.IsActive, g.UpdateTime, g.WorkStatus })
                                            .Select(r => new
                                            {
                                                r.Key.ID,
                                                r.Key.OrderDate,
                                                r.Key.SortingLine.SortingLineCode,
                                                r.Key.SortingLine.SortingLineName,
                                                r.Key.DeliverLine.DeliverLineCode,
                                                r.Key.DeliverLine.DeliverLineName,
                                                IsActive = r.Key.IsActive == "1" ? "可用" : "不可用",
                                                UpdateTime = r.Key.UpdateTime.ToString("yyyy-MM-dd"),
                                                WorkStatus = r.Key.WorkStatus == "1" ? "未作业" : "已作业",
                                                QuantitySum = r.Sum(q => q.QuantitySum)
                                            });
            return temp.ToArray();
        }

        #endregion

        public System.Data.DataTable GetSortOrderDispatch(int page, int rows, string OrderDate, string SortingLineCode)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            IQueryable<SortOrderDispatch> sortDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            var sortDispatch = sortDispatchQuery.Where(s => s.WorkStatus != "2");
            if (OrderDate != string.Empty && OrderDate != null)
            {
                OrderDate = Convert.ToDateTime(OrderDate).ToString("yyyyMMdd");
                sortDispatch = sortDispatch.Where(s => s.OrderDate == OrderDate);
            }
            if (SortingLineCode != string.Empty && SortingLineCode != null)
            {
                sortDispatch = sortDispatch.Where(s => s.SortingLineCode == SortingLineCode);
            }
            var temp = sortDispatch.OrderBy(b => b.SortingLineCode).AsEnumerable().Select(b => new
            {
                b.ID,
                b.SortingLineCode,
                b.SortingLine.SortingLineName,
                b.OrderDate,
                b.DeliverLineCode,
                WorkStatus = b.WorkStatus == "1" ? "未作业" : "已作业",
                b.DeliverLine.DeliverLineName,
                IsActive = b.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = b.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
            dt.Columns.Add("订单日期", typeof(string));
            dt.Columns.Add("分拣线编码", typeof(string));
            dt.Columns.Add("分拣线名称", typeof(string));
            dt.Columns.Add("送货线路编码", typeof(string));
            dt.Columns.Add("送货线路名称", typeof(string));
            dt.Columns.Add("作业状态", typeof(string));
            dt.Columns.Add("是否可用", typeof(string));
            dt.Columns.Add("修改时间", typeof(string));
            foreach (var t in temp)
            {
                dt.Rows.Add(t.OrderDate,
                            t.SortingLineCode,
                            t.SortingLineName,
                            t.DeliverLineCode,
                            t.DeliverLineName,
                            t.WorkStatus,
                            t.IsActive,
                            t.UpdateTime);
            }
            return dt;
        }
    }
}
