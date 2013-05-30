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
    public class CellHistoricalService : ServiceBase<InBillAllot>, ICellHistoricalService
    {
        [Dependency]
        public IInBillAllotRepository InBillAllotRepository { get; set; }
        [Dependency]
        public IOutBillAllotRepository OutBillAllotRepository { get; set; }
        [Dependency]
        public IMoveBillDetailRepository MoveBillDetailRepository { get; set; }
        [Dependency]
        public IProfitLossBillDetailRepository ProfitLossBillDetailRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetCellDetails(int page, int rows, string beginDate, string endDate, string type, string id)
        {
            IQueryable<InBillAllot> InBillAllotQuery = InBillAllotRepository.GetQueryable();
            IQueryable<OutBillAllot> OutBillAllotQuery = OutBillAllotRepository.GetQueryable();
            IQueryable<MoveBillDetail> MoveBillDetailQuery = MoveBillDetailRepository.GetQueryable();
            IQueryable<ProfitLossBillDetail> ProfitLossBillDetailQuery = ProfitLossBillDetailRepository.GetQueryable();
            
            if (type == "cell")
            {
                var CellHistoryAllQuery = InBillAllotQuery.Where(i => i.Status == "2" && i.CellCode == id)
                    .Select(i => new
                    {
                    i.CellCode,
                    i.Cell.CellName,
                    i.InBillMaster.BillDate,
                    i.InBillMaster.BillType.BillTypeName,
                    i.InBillMaster.BillNo,
                    i.ProductCode,
                    i.Product.ProductName,
                    i.Unit.UnitName,
                    i.RealQuantity,
                    i.Unit.Count,
                    IfMove = "0"
                    }).Union(OutBillAllotQuery.Where(o => o.Status == "2" && o.CellCode == id)
                            .Select(o => new
                            {
                                o.CellCode,
                                o.Cell.CellName,
                                o.OutBillMaster.BillDate,
                                o.OutBillMaster.BillType.BillTypeName,
                                o.OutBillMaster.BillNo,
                                o.ProductCode,
                                o.Product.ProductName,
                                o.Unit.UnitName,
                                o.RealQuantity,
                                o.Unit.Count,
                                IfMove = "0"
                            })).Union(MoveBillDetailQuery.Where(m => m.Status == "2" && (m.InCellCode == id || m.OutCellCode == id))
                                    .Select(m => new 
                                    {
                                        CellCode = m.InCellCode == id ? m.InCellCode : m.OutCellCode,
                                        CellName = m.InCellCode == id ? m.InCell.CellName : m.OutCell.CellName,
                                        m.MoveBillMaster.BillDate,
                                        m.MoveBillMaster.BillType.BillTypeName,
                                        m.MoveBillMaster.BillNo,
                                        m.ProductCode,
                                        m.Product.ProductName,
                                        m.Unit.UnitName,
                                        m.RealQuantity,
                                        m.Unit.Count,
                                        IfMove = m.InCellCode == id ? "1":"2"
                                    }
                                    )).Union(ProfitLossBillDetailQuery.Where(p => p.ProfitLossBillMaster.Status =="3" && p.CellCode == id)
                                            .Select(p => new
                                            {
                                                p.CellCode,
                                                p.Storage.Cell.CellName,
                                                p.ProfitLossBillMaster.BillDate,
                                                p.ProfitLossBillMaster.BillType.BillTypeName,
                                                p.ProfitLossBillMaster.BillNo,
                                                p.ProductCode,
                                                p.Product.ProductName,
                                                p.Unit.UnitName,
                                                RealQuantity = p.Quantity,
                                                p.Unit.Count,
                                                IfMove = "0"
                                            }));
                if (!beginDate.Equals(string.Empty) && beginDate != "null01")
                {
                    DateTime begin = Convert.ToDateTime(beginDate);
                    CellHistoryAllQuery = CellHistoryAllQuery.Where(c => c.BillDate >= begin);
                }
                //else
                //{
                //    DateTime begin = DateTime.Now.AddDays(-30);
                //    CellHistoryAllQuery = CellHistoryAllQuery.Where(i => i.BillDate >= begin);
                //}
                if (!endDate.Equals(string.Empty) && endDate != "null02")//null02 同null01 代表传回来的日期是空值
                {
                    DateTime end = Convert.ToDateTime(endDate);
                    CellHistoryAllQuery = CellHistoryAllQuery.Where(c => c.BillDate <= end);
                }

                CellHistoryAllQuery = CellHistoryAllQuery.OrderByDescending(c => c.BillDate).Where(c => c.RealQuantity > 0);
                int total = CellHistoryAllQuery.Count();
                CellHistoryAllQuery = CellHistoryAllQuery.Skip((page - 1) * rows).Take(rows);
                var CellHistoryAll = CellHistoryAllQuery.ToArray().ToArray().Select(c => new
                {
                    c.CellCode,
                    c.CellName,
                    BillDate = c.BillDate.ToString("yyyy-MM-dd"),
                    BillTypeName = c.IfMove == "0" ? c.BillTypeName.ToString() : c.IfMove == "1" ? c.BillTypeName.ToString() + "-移入" : c.BillTypeName.ToString() + "-移出",
                    c.BillNo,
                    c.ProductCode,
                    c.ProductName,
                    c.UnitName,
                    Quantity = c.RealQuantity / c.Count
                });
                return new { total, rows = CellHistoryAll.ToArray()};
            }
            return null;
        }

        public System.Data.DataTable GetCellHistory(int page, int rows, string beginDate, string endDate, string type, string id)
        {
            IQueryable<InBillAllot> InBillAllotQuery = InBillAllotRepository.GetQueryable();
            IQueryable<OutBillAllot> OutBillAllotQuery = OutBillAllotRepository.GetQueryable();
            IQueryable<MoveBillDetail> MoveBillDetailQuery = MoveBillDetailRepository.GetQueryable();
            IQueryable<ProfitLossBillDetail> ProfitLossBillDetailQuery = ProfitLossBillDetailRepository.GetQueryable();

            if (type == "cell")
            {
                var CellHistoryAllQuery = InBillAllotQuery.Where(i => i.Status == "2" && i.CellCode == id)
                    .Select(i => new
                    {
                        i.CellCode,
                        i.Cell.CellName,
                        i.InBillMaster.BillDate,
                        i.InBillMaster.BillType.BillTypeName,
                        i.InBillMaster.BillNo,
                        i.ProductCode,
                        i.Product.ProductName,
                        i.Unit.UnitName,
                        i.RealQuantity,
                        i.Unit.Count,
                        IfMove = "0"
                    }).Union(OutBillAllotQuery.Where(o => o.Status == "2" && o.CellCode == id)
                            .Select(o => new
                            {
                                o.CellCode,
                                o.Cell.CellName,
                                o.OutBillMaster.BillDate,
                                o.OutBillMaster.BillType.BillTypeName,
                                o.OutBillMaster.BillNo,
                                o.ProductCode,
                                o.Product.ProductName,
                                o.Unit.UnitName,
                                o.RealQuantity,
                                o.Unit.Count,
                                IfMove = "0"
                            })).Union(MoveBillDetailQuery.Where(m => m.Status == "2" && (m.InCellCode == id || m.OutCellCode == id))
                                    .Select(m => new
                                    {
                                        CellCode = m.InCellCode == id ? m.InCellCode : m.OutCellCode,
                                        CellName = m.InCellCode == id ? m.InCell.CellName : m.OutCell.CellName,
                                        m.MoveBillMaster.BillDate,
                                        m.MoveBillMaster.BillType.BillTypeName,
                                        m.MoveBillMaster.BillNo,
                                        m.ProductCode,
                                        m.Product.ProductName,
                                        m.Unit.UnitName,
                                        m.RealQuantity,
                                        m.Unit.Count,
                                        IfMove = m.InCellCode == id ? "1" : "2"
                                    }
                                    )).Union(ProfitLossBillDetailQuery.Where(p => p.ProfitLossBillMaster.Status == "3" && p.CellCode == id)
                                            .Select(p => new
                                            {
                                                p.CellCode,
                                                p.Storage.Cell.CellName,
                                                p.ProfitLossBillMaster.BillDate,
                                                p.ProfitLossBillMaster.BillType.BillTypeName,
                                                p.ProfitLossBillMaster.BillNo,
                                                p.ProductCode,
                                                p.Product.ProductName,
                                                p.Unit.UnitName,
                                                RealQuantity = p.Quantity,
                                                p.Unit.Count,
                                                IfMove = "0"
                                            }));
                if (!beginDate.Equals(string.Empty) && beginDate != "null01")
                {
                    DateTime begin = Convert.ToDateTime(beginDate);
                    CellHistoryAllQuery = CellHistoryAllQuery.Where(c => c.BillDate >= begin);
                }
                if (!endDate.Equals(string.Empty) && endDate != "null02") //null02 同null01 代表传回来的日期是空值
                {
                    DateTime end = Convert.ToDateTime(endDate);
                    CellHistoryAllQuery = CellHistoryAllQuery.Where(c => c.BillDate <= end);
                }
                CellHistoryAllQuery = CellHistoryAllQuery.OrderByDescending(c => c.BillDate).Where(c => c.RealQuantity > 0);
                var CellHistoryAll = CellHistoryAllQuery.ToArray().ToArray().Select(c => new
                {
                    c.CellCode,
                    c.CellName,
                    BillDate = c.BillDate.ToString("yyyy-MM-dd"),
                    BillTypeName = c.IfMove == "0" ? c.BillTypeName.ToString() : c.IfMove == "1" ? c.BillTypeName.ToString() + "-移入" : c.BillTypeName.ToString() + "-移出",
                    c.BillNo,
                    c.ProductCode,
                    c.ProductName,
                    c.UnitName,
                    Quantity = c.RealQuantity / c.Count
                });
                System.Data.DataTable dt = new System.Data.DataTable();
                dt.Columns.Add("货位名称", typeof(string));
                dt.Columns.Add("变动日期", typeof(string));
                dt.Columns.Add("操作类型", typeof(string));
                dt.Columns.Add("所属订单", typeof(string));
                dt.Columns.Add("商品编码", typeof(string));
                dt.Columns.Add("商品名称", typeof(string));
                dt.Columns.Add("数量", typeof(string));
                dt.Columns.Add("单位名称", typeof(string));
                foreach (var item in CellHistoryAll)
                {
                    dt.Rows.Add
                        (
                            item.CellName,
                            item.BillDate,
                            item.BillTypeName,
                            item.BillNo,
                            item.ProductCode,
                            item.ProductName,
                            item.Quantity,
                            item.UnitName
                        );
                }
                return dt;
            }
            return null;
        }
    }
}
