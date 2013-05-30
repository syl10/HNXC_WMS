using System;
using System.Linq;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using System.Transactions;

namespace THOK.Wms.Bll.Service
{
    public class DailyBalanceService : ServiceBase<DailyBalance>, IDailyBalanceService
    {
        [Dependency]
        public IDailyBalanceRepository DailyBalanceRepository { get; set; }
        [Dependency]
        public IBusinessSystemsDailyBalanceRepository BusinessSystemsDailyBalanceRepository { get; set; }
        [Dependency]
        public IUnitRepository UnitRepository { get; set; }
        [Dependency]
        public IInBillDetailRepository InBillDetailRepository { get; set; }
        [Dependency]
        public IOutBillDetailRepository OutBillDetailRepository { get; set; }
        [Dependency]
        public IProfitLossBillDetailRepository ProfitLossBillDetailRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IDailyBalanceService 成员

        public object GetDetails(int page, int rows, string beginDate, string endDate, string warehouseCode, string unitType)
        {
            IQueryable<DailyBalance> dailyBalanceQuery = DailyBalanceRepository.GetQueryable();

            var dailyBalance = dailyBalanceQuery.Where(i => 1 == 1);
            if (!beginDate.Equals(string.Empty))
            {
                DateTime begin = Convert.ToDateTime(beginDate);
                dailyBalance = dailyBalance.Where(i => i.SettleDate >= begin);
            }

            if (!endDate.Equals(string.Empty))
            {
                DateTime end = Convert.ToDateTime(endDate);
                dailyBalance = dailyBalance.Where(i => i.SettleDate <= end);
            }

            var dailyBalances = dailyBalance.Where(c => c.WarehouseCode.Contains(warehouseCode))
                                       .OrderBy(c => c.SettleDate)
                                       .GroupBy(c => c.SettleDate)
                                       .Select(c => new
                                       {
                                           SettleDate = c.Key,
                                           WarehouseCode = warehouseCode == "" ? "" : c.Max(p => p.WarehouseCode),
                                           WarehouseName = warehouseCode == "" ? "" : c.Max(p => p.Warehouse.WarehouseName),
                                           Beginning = c.Sum(p => p.Beginning),
                                           EntryAmount = c.Sum(p => p.EntryAmount),
                                           DeliveryAmount = c.Sum(p => p.DeliveryAmount),
                                           ProfitAmount = c.Sum(p => p.ProfitAmount),
                                           LossAmount = c.Sum(p => p.LossAmount),
                                           Ending = c.Sum(p => p.Ending)
                                       });

            int total = dailyBalances.Count();
            dailyBalances = dailyBalances.OrderByDescending(s => s.SettleDate).Skip((page - 1) * rows).Take(rows);

            string unitName = "标准件";
            decimal count = 10000;
            if (unitType == "2")
            {
                unitName = "标准条";
                count = 200;
            }

            var temp = dailyBalances.ToArray().Select(d => new
            {
                SettleDate = d.SettleDate.ToString("yyyy-MM-dd"),
                WarehouseCode = d.WarehouseCode,
                WarehouseName = d.WarehouseName == "" ? "全部仓库" : d.WarehouseName,
                UnitName = unitName,
                Beginning = d.Beginning / count,
                EntryAmount = d.EntryAmount / count,
                DeliveryAmount = d.DeliveryAmount / count,
                ProfitAmount = d.ProfitAmount / count,
                LossAmount = d.LossAmount / count,
                Ending = d.Ending / count
            });
            return new { total, rows = temp.ToArray() };
        }

        public object GetInfoDetails(int page, int rows, string warehouseCode, string settleDate, string unitType)
        {
            DateTime date = Convert.ToDateTime(settleDate);
            if (unitType == null || unitType == "")
            {
                unitType = "1";
            }
            IQueryable<DailyBalance> dailyBalanceQuery = DailyBalanceRepository.GetQueryable();
            var query = dailyBalanceQuery.Where(i => i.WarehouseCode.Contains(warehouseCode) && i.SettleDate == date)
                                         .OrderByDescending(i => i.SettleDate)
                                         .OrderBy(i => i.Warehouse.WarehouseName)
                                         .OrderBy(i => i.ProductCode)
                                         .Select(i => new
                                         {
                                             i.SettleDate,
                                             i.ProductCode,
                                             i.Product.ProductName,
                                             UnitCode01 = i.Product.UnitList.Unit01.UnitCode,
                                             UnitName01 = i.Product.UnitList.Unit01.UnitName,
                                             UnitCode02 = i.Product.UnitList.Unit02.UnitCode,
                                             UnitName02 = i.Product.UnitList.Unit02.UnitName,
                                             Count01 = i.Product.UnitList.Unit01.Count,
                                             Count02 = i.Product.UnitList.Unit02.Count,
                                             i.WarehouseCode,
                                             i.Warehouse.WarehouseName,
                                             Beginning = i.Beginning,
                                             EntryAmount = i.EntryAmount,
                                             DeliveryAmount = i.DeliveryAmount,
                                             ProfitAmount = i.ProfitAmount,
                                             LossAmount = i.LossAmount,
                                             Ending = i.Ending
                                         });
            int total = query.Count();
            query = query.Skip((page - 1) * rows).Take(rows);

            string unitName = "";
            decimal count = 1;

            //标准单位（标准件||标准条）
            if (unitType == "1" || unitType == "2")
            {
                if (unitType == "1")
                {
                    unitName = "标准件";
                    count = 10000;
                }

                if (unitType == "2")
                {
                    unitName = "标准条";
                    count = 200;
                }
                var dailyBalance = query.ToArray().Select(i => new
                                         {
                                             SettleDate = i.SettleDate.ToString("yyyy-MM-dd"),
                                             i.ProductCode,
                                             i.ProductName,
                                             UnitCode = "",
                                             UnitName = unitName,
                                             i.WarehouseCode,
                                             i.WarehouseName,
                                             Beginning = i.Beginning / count,
                                             EntryAmount = i.EntryAmount / count,
                                             DeliveryAmount = i.DeliveryAmount / count,
                                             ProfitAmount = i.ProfitAmount / count,
                                             LossAmount = i.LossAmount / count,
                                             Ending = i.Ending / count
                                         });
                return new { total, rows = dailyBalance.ToArray() };
            }

            //自然件
            if (unitType == "3" || unitType == "4")
            {
                var dailyBalance = query.ToArray().Select(i => new
                                         {
                                             SettleDate = i.SettleDate.ToString("yyyy-MM-dd"),
                                             i.ProductCode,
                                             i.ProductName,
                                             UnitCode = unitType == "3" ? i.UnitCode01 : i.UnitCode02,
                                             UnitName = unitType == "3" ? i.UnitName01 : i.UnitName02,
                                             i.WarehouseCode,
                                             i.WarehouseName,
                                             Beginning = i.Beginning / (unitType == "3" ? i.Count01 : i.Count02),
                                             EntryAmount = i.EntryAmount / (unitType == "3" ? i.Count01 : i.Count02),
                                             DeliveryAmount = i.DeliveryAmount / (unitType == "3" ? i.Count01 : i.Count02),
                                             ProfitAmount = i.ProfitAmount / (unitType == "3" ? i.Count01 : i.Count02),
                                             LossAmount = i.LossAmount / (unitType == "3" ? i.Count01 : i.Count02),
                                             Ending = i.Ending / (unitType == "3" ? i.Count01 : i.Count02),
                                         });
                return new { total, rows = dailyBalance.ToArray() };
            }
            return new { total, rows = query.ToArray() };
        }

        public object GetInfoCheck(int page, int rows, string warehouseCode, string settleDate, string unitType)
        {
            DateTime date = Convert.ToDateTime(settleDate);
            if (unitType == null || unitType == "")
            {
                unitType = "1";
            }
            IQueryable<DailyBalance> dailyBalanceQuery = DailyBalanceRepository.GetQueryable();
            IQueryable<BusinessSystemsDailyBalance> BusinessSystemsDailyBalanceQuery = BusinessSystemsDailyBalanceRepository.GetQueryable();
            var query = dailyBalanceQuery.Where(d => d.WarehouseCode.Contains(warehouseCode) && d.SettleDate == date)
                                         .Select(d => new //本系统产品日结结果
                                         {
                                             d.SettleDate,
                                             d.ProductCode,
                                             d.Product.ProductName,
                                             UnitCode01 = d.Product.UnitList.Unit01.UnitCode,
                                             UnitName01 = d.Product.UnitList.Unit01.UnitName,
                                             UnitCode02 = d.Product.UnitList.Unit02.UnitCode,
                                             UnitName02 = d.Product.UnitList.Unit02.UnitName,
                                             Count01 = d.Product.UnitList.Unit01.Count,
                                             Count02 = d.Product.UnitList.Unit02.Count,
                                             d.WarehouseCode,
                                             d.Warehouse.WarehouseName,
                                             Beginning = d.Beginning,
                                             EntryAmount = d.EntryAmount,
                                             DeliveryAmount = d.DeliveryAmount,
                                             ProfitAmount = d.ProfitAmount,
                                             LossAmount = d.LossAmount,
                                             Ending = d.Ending,
                                             BSys_Beginning = decimal.Zero,
                                             BSys_EntryAmount = decimal.Zero,
                                             BSys_DeliveryAmount = decimal.Zero,
                                             BSys_ProfitAmount = decimal.Zero,
                                             BSys_LossAmount = decimal.Zero,
                                             BSys_Ending = decimal.Zero
                                         }).Concat(BusinessSystemsDailyBalanceQuery.Where(b => b.WarehouseCode.Contains(warehouseCode) && b.SettleDate == date)
                                                                                   .Select(b => new //营销系统产品日结结果
                                                                                    {
                                                                                        b.SettleDate,
                                                                                        b.ProductCode,
                                                                                        b.Product.ProductName,
                                                                                        UnitCode01 = b.Product.UnitList.Unit01.UnitCode,
                                                                                        UnitName01 = b.Product.UnitList.Unit01.UnitName,
                                                                                        UnitCode02 = b.Product.UnitList.Unit02.UnitCode,
                                                                                        UnitName02 = b.Product.UnitList.Unit02.UnitName,
                                                                                        Count01 = b.Product.UnitList.Unit01.Count,
                                                                                        Count02 = b.Product.UnitList.Unit02.Count,
                                                                                        b.WarehouseCode,
                                                                                        b.Warehouse.WarehouseName,
                                                                                        Beginning = decimal.Zero,
                                                                                        EntryAmount = decimal.Zero,
                                                                                        DeliveryAmount = decimal.Zero,
                                                                                        ProfitAmount = decimal.Zero,
                                                                                        LossAmount = decimal.Zero,
                                                                                        Ending = decimal.Zero,
                                                                                        BSys_Beginning = b.Beginning,
                                                                                        BSys_EntryAmount = b.EntryAmount,
                                                                                        BSys_DeliveryAmount = b.DeliveryAmount,
                                                                                        BSys_ProfitAmount = b.ProfitAmount,
                                                                                        BSys_LossAmount = b.LossAmount,
                                                                                        BSys_Ending = b.Ending
                                                                                    }));
            var InfoCheck = query.GroupBy(q => new { q.SettleDate, q.WarehouseCode, q.ProductCode })
                        .Select(q => new   //两个系统产品日结结果汇总
                        {
                            q.Key.SettleDate,
                            q.Key.WarehouseCode,
                            q.Key.ProductCode,
                            ProductName = q.Max(i => i.ProductName),
                            WarehouseName = q.Max(i => i.WarehouseName),
                            UnitCode01 = q.Max(i => i.UnitCode01),
                            UnitName01 = q.Max(i => i.UnitName01),
                            UnitCode02 = q.Max(i => i.UnitCode02),
                            UnitName02 = q.Max(i => i.UnitName02),
                            Count01 = q.Max(i => i.Count01),
                            Count02 = q.Max(i => i.Count02),
                            Beginning = q.Sum(i => i.Beginning),
                            EntryAmount = q.Sum(i => i.EntryAmount),
                            DeliveryAmount = q.Sum(i => i.DeliveryAmount),
                            ProfitAmount = q.Sum(i => i.ProfitAmount),
                            LossAmount = q.Sum(i => i.LossAmount),
                            Ending = q.Sum(i => i.Ending),
                            BSys_Beginning = q.Sum(i => i.BSys_Beginning),
                            BSys_EntryAmount = q.Sum(i => i.BSys_EntryAmount),
                            BSys_DeliveryAmount = q.Sum(i => i.BSys_DeliveryAmount),
                            BSys_ProfitAmount = q.Sum(i => i.BSys_ProfitAmount),
                            BSys_LossAmount = q.Sum(i => i.BSys_LossAmount),
                            BSys_Ending = q.Sum(i => i.BSys_Ending),
                        }).Select(n => new //两个系统产品日结结果比对
                        {
                            n.SettleDate,
                            n.WarehouseCode,
                            n.ProductCode,
                            n.ProductName,
                            n.WarehouseName,
                            n.UnitCode01,
                            n.UnitName01,
                            n.UnitCode02,
                            n.UnitName02,
                            n.Count01,
                            n.Count02,
                            n.Beginning,
                            n.EntryAmount,
                            n.DeliveryAmount,
                            n.ProfitAmount,
                            n.LossAmount,
                            n.Ending,
                            n.BSys_Beginning,
                            n.BSys_EntryAmount,
                            n.BSys_DeliveryAmount,
                            n.BSys_ProfitAmount,
                            n.BSys_LossAmount,
                            n.BSys_Ending,
                            Status = n.Beginning == n.BSys_Beginning &&
                                        n.EntryAmount == n.BSys_EntryAmount &&
                                        n.DeliveryAmount == n.BSys_DeliveryAmount &&
                                        n.ProfitAmount == n.BSys_ProfitAmount &&
                                        n.LossAmount == n.BSys_LossAmount &&
                                        n.Ending == n.BSys_Ending ?
                                        "T" : "F",
                            Status1 = n.Beginning == n.BSys_Beginning ?
                                "T" : "F",
                            Status2 = n.EntryAmount == n.BSys_EntryAmount ?
                                "T" : "F",
                            Status3 = n.DeliveryAmount == n.BSys_DeliveryAmount ?
                                "T" : "F",
                            Status4 = n.ProfitAmount == n.BSys_ProfitAmount ?
                                "T" : "F",
                            Status5 = n.LossAmount == n.BSys_LossAmount ?
                                "T" : "F",
                            Status6 = n.Ending == n.BSys_Ending ?
                                "true" : "F"
                        }).OrderBy(n => n.Status)
                          .OrderBy(n => n.WarehouseName)
                          .OrderBy(n => n.ProductCode);

            int total = InfoCheck.Count();
            var infoCheck = InfoCheck.Skip((page - 1) * rows).Take(rows);

            //两个系统产品日结结果比对结果按照单位换算
            string unitName = "";
            decimal count = 1;

            //标准单位（标准件||标准条）
            if (unitType == "1" || unitType == "2")
            {
                if (unitType == "1")
                {
                    unitName = "标准件";
                    count = 10000;
                }

                if (unitType == "2")
                {
                    unitName = "标准条";
                    count = 200;
                }
                var dailyBalance = infoCheck.ToArray().Select(i => new
                {
                    SettleDate = i.SettleDate.ToString("yyyy-MM-dd"),
                    i.ProductCode,
                    i.ProductName,
                    UnitCode = "",
                    UnitName = unitName,
                    i.WarehouseCode,
                    i.WarehouseName,
                    Beginning = i.Beginning == i.BSys_Beginning ?
                    (i.Beginning / count).ToString() :
                    (i.Beginning / count).ToString() + "≠" + (i.BSys_Beginning / count).ToString(),
                    EntryAmount = i.EntryAmount == i.BSys_EntryAmount ?
                    (i.EntryAmount / count).ToString() :
                    (i.EntryAmount / count).ToString() + "≠" + (i.BSys_EntryAmount / count).ToString(),
                    DeliveryAmount = i.DeliveryAmount == i.BSys_DeliveryAmount ?
                    (i.DeliveryAmount / count).ToString() :
                    (i.DeliveryAmount / count).ToString() + "≠" + (i.BSys_DeliveryAmount / count).ToString(),
                    ProfitAmount = i.ProfitAmount == i.BSys_ProfitAmount ?
                    (i.ProfitAmount / count).ToString() :
                    (i.ProfitAmount / count).ToString() + "≠" + (i.BSys_ProfitAmount / count).ToString(),
                    LossAmount = i.LossAmount == i.BSys_LossAmount ?
                    (i.LossAmount / count).ToString() :
                    (i.LossAmount / count).ToString() + "≠" + (i.BSys_LossAmount / count).ToString(),
                    Ending = i.Ending == i.BSys_Ending ?
                    (i.Ending / count).ToString() :
                    (i.Ending / count).ToString() + "≠" + (i.BSys_Ending / count).ToString(),
                    i.Status1,
                    i.Status2,
                    i.Status3,
                    i.Status4,
                    i.Status5,
                    i.Status6
                });
                return new { total, rows = dailyBalance.ToArray() };
            }

            //自然件
            if (unitType == "3" || unitType == "4")
            {
                var dailyBalance = infoCheck.ToArray().Select(i => new
                {
                    SettleDate = i.SettleDate.ToString("yyyy-MM-dd"),
                    i.ProductCode,
                    i.ProductName,
                    UnitCode = unitType == "3" ? i.UnitCode01 : i.UnitCode02,
                    UnitName = unitType == "3" ? i.UnitName01 : i.UnitName02,
                    i.WarehouseCode,
                    i.WarehouseName,
                    Beginning = i.Beginning == i.BSys_Beginning ?
                    (i.Beginning / (unitType == "3" ? i.Count01 : i.Count02)).ToString() :
                    (i.Beginning / (unitType == "3" ? i.Count01 : i.Count02)).ToString() + "≠" + (i.BSys_Beginning / (unitType == "3" ? i.Count01 : i.Count02)).ToString(),
                    EntryAmount = i.EntryAmount == i.BSys_EntryAmount ?
                    (i.EntryAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString() :
                    (i.EntryAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString() + "≠" + (i.BSys_EntryAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString(),
                    DeliveryAmount = i.DeliveryAmount == i.BSys_DeliveryAmount ?
                    (i.DeliveryAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString() :
                    (i.DeliveryAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString() + "≠" + (i.BSys_DeliveryAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString(),
                    ProfitAmount = i.ProfitAmount == i.BSys_ProfitAmount ?
                    (i.ProfitAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString() :
                    (i.ProfitAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString() + "≠" + (i.BSys_ProfitAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString(),
                    LossAmount = i.LossAmount == i.BSys_LossAmount ?
                    (i.LossAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString() :
                    (i.LossAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString() + "≠" + (i.BSys_LossAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString(),
                    Ending = i.Ending == i.BSys_Ending ?
                    (i.Ending / (unitType == "3" ? i.Count01 : i.Count02)).ToString() :
                    (i.Ending / (unitType == "3" ? i.Count01 : i.Count02)).ToString() + "≠" + (i.BSys_Ending / (unitType == "3" ? i.Count01 : i.Count02)).ToString(),
                    i.Status1,
                    i.Status2,
                    i.Status3,
                    i.Status4,
                    i.Status5,
                    i.Status6
                });
                return new { total, rows = dailyBalance.ToArray() };
            }
            return new { total, rows = query.ToArray() };
        }

        public Boolean DoDailyBalance(string warehouseCode, string settleDate, ref string errorInfo)
        {
            try
            {
                //using (var scope = new TransactionScope())
                //{
                    var inQuery = InBillDetailRepository.GetQueryable().AsEnumerable();
                    var outQuery = OutBillDetailRepository.GetQueryable().AsEnumerable();
                    var profitLossQuery = ProfitLossBillDetailRepository.GetQueryable().AsEnumerable();
                    var dailyBalanceQuery = DailyBalanceRepository.GetQueryable().AsEnumerable();

                    DateTime dt1 = Convert.ToDateTime(settleDate);

                    if (DateTime.Now < dt1)
                    {
                        errorInfo = "选择日结日期大于当前日期，不可以进行日结！";
                        return false;
                    }
                    var dailyBalance = dailyBalanceQuery.Where(d => d.SettleDate < dt1)
                                               .OrderByDescending(d => d.SettleDate)
                                               .FirstOrDefault();
                    string t = dailyBalance != null ? dailyBalance.SettleDate.ToString("yyyy-MM-dd") : "";

                    var oldDailyBalance = dailyBalanceQuery.Where(d => (d.WarehouseCode == warehouseCode
                                                                         || string.IsNullOrEmpty(warehouseCode))
                                                     && d.SettleDate.ToString("yyyy-MM-dd") == settleDate)
                                           .ToArray();
                    DailyBalanceRepository.Delete(oldDailyBalance);
                    DailyBalanceRepository.SaveChanges();

                    var query = inQuery.Where(a => (a.InBillMaster.WarehouseCode == warehouseCode
                                                     || string.IsNullOrEmpty(warehouseCode))
                                                  && a.InBillMaster.BillDate.ToString("yyyy-MM-dd") == settleDate
                                      ).Select(a => new
                    {
                        BillDate = a.InBillMaster.BillDate.ToString("yyyy-MM-dd"),
                        WarehouseCode = a.InBillMaster.Warehouse.WarehouseCode,
                        ProductCode = a.ProductCode,
                        UnitCode = a.Product.UnitCode,
                        Beginning = decimal.Zero,
                        EntryAmount = a.RealQuantity,
                        DeliveryAmount = decimal.Zero,
                        ProfitAmount = decimal.Zero,
                        LossAmount = decimal.Zero,
                        Ending = decimal.Zero
                    }).Concat(outQuery.Where(a => (a.OutBillMaster.WarehouseCode == warehouseCode
                                                   || string.IsNullOrEmpty(warehouseCode))
                                                && a.OutBillMaster.BillDate.ToString("yyyy-MM-dd") == settleDate
                                    ).Select(a => new
                    {
                        BillDate = a.OutBillMaster.BillDate.ToString("yyyy-MM-dd"),
                        WarehouseCode = a.OutBillMaster.Warehouse.WarehouseCode,
                        ProductCode = a.ProductCode,
                        UnitCode = a.Product.UnitCode,
                        Beginning = decimal.Zero,
                        EntryAmount = decimal.Zero,
                        DeliveryAmount = a.RealQuantity,
                        ProfitAmount = decimal.Zero,
                        LossAmount = decimal.Zero,
                        Ending = decimal.Zero
                    })).Concat(profitLossQuery.Where(a => (a.ProfitLossBillMaster.WarehouseCode == warehouseCode
                                                           || string.IsNullOrEmpty(warehouseCode))
                                                        && a.ProfitLossBillMaster.BillDate.ToString("yyyy-MM-dd") == settleDate
                                            ).Select(a => new
                    {
                        BillDate = a.ProfitLossBillMaster.BillDate.ToString("yyyy-MM-dd"),
                        WarehouseCode = a.ProfitLossBillMaster.Warehouse.WarehouseCode,
                        ProductCode = a.ProductCode,
                        UnitCode = a.Product.UnitCode,
                        Beginning = decimal.Zero,
                        EntryAmount = decimal.Zero,
                        DeliveryAmount = decimal.Zero,
                        ProfitAmount = a.Quantity > 0 ? Math.Abs(a.Quantity) : decimal.Zero,
                        LossAmount = a.Quantity < 0 ? Math.Abs(a.Quantity) : decimal.Zero,
                        Ending = decimal.Zero
                    })).Concat(dailyBalanceQuery.Where(d => (d.WarehouseCode == warehouseCode
                                                             || string.IsNullOrEmpty(warehouseCode))
                                                          && d.SettleDate.ToString("yyyy-MM-dd") == t
                                                          && d.Ending != decimal.Zero
                                              ).Select(a => new
                    {
                        BillDate = settleDate,
                        WarehouseCode = a.WarehouseCode,
                        ProductCode = a.ProductCode,
                        UnitCode = a.Product.UnitCode,
                        Beginning = a.Ending,
                        EntryAmount = decimal.Zero,
                        DeliveryAmount = decimal.Zero,
                        ProfitAmount = decimal.Zero,
                        LossAmount = decimal.Zero,
                        Ending = decimal.Zero
                    }
                    ));

                    var newDailyBalance = query.GroupBy(a => new { a.BillDate, a.WarehouseCode, a.ProductCode, a.UnitCode })
                                        .Select(a => new DailyBalance
                    {
                        SettleDate = Convert.ToDateTime(a.Key.BillDate),
                        WarehouseCode = a.Key.WarehouseCode,
                        ProductCode = a.Key.ProductCode,
                        UnitCode = a.Key.UnitCode,
                        Beginning = a.Sum(d => d.Beginning),
                        EntryAmount = a.Sum(d => d.EntryAmount),
                        DeliveryAmount = a.Sum(d => d.DeliveryAmount),
                        ProfitAmount = a.Sum(d => d.ProfitAmount),
                        LossAmount = a.Sum(d => d.LossAmount),
                        Ending = a.Sum(d => d.Beginning) + a.Sum(d => d.EntryAmount) - a.Sum(d => d.DeliveryAmount) + a.Sum(d => d.ProfitAmount) - a.Sum(d => d.LossAmount),
                    }).ToArray();

                    newDailyBalance.AsParallel().ForAll(b => b.ID = Guid.NewGuid());
                    foreach (var item in newDailyBalance)
                    {
                        item.ID = Guid.NewGuid();
                        DailyBalanceRepository.Add(item);
                    }

                    DailyBalanceRepository.SaveChanges();
                    //scope.Complete();

                    return true;
               // }
            }
            catch (Exception e)
            {
                errorInfo = "日结时出现错误，详情：" + e.Message;
                return false;
            }
        }

        #endregion
        
        #region 日结明细
        public System.Data.DataTable GetInfoDetail(int page, int rows, string warehouseCode, string settleDate, string unitType)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            DateTime date = Convert.ToDateTime(settleDate);
            if (unitType == null || unitType == "")
                unitType = "1";
            IQueryable<DailyBalance> dailyBalanceQuery = DailyBalanceRepository.GetQueryable();
            var query = dailyBalanceQuery.Where(i => i.WarehouseCode.Contains(warehouseCode) && i.SettleDate == date)
                                         .OrderByDescending(i => i.SettleDate)
                                         .OrderBy(i => i.Warehouse.WarehouseName)
                                         .OrderBy(i => i.ProductCode)
                                         .Select(i => new
                                         {
                                             i.SettleDate,
                                             i.ProductCode,
                                             i.Product.ProductName,
                                             UnitCode01 = i.Product.UnitList.Unit01.UnitCode,
                                             UnitName01 = i.Product.UnitList.Unit01.UnitName,
                                             UnitCode02 = i.Product.UnitList.Unit02.UnitCode,
                                             UnitName02 = i.Product.UnitList.Unit02.UnitName,
                                             Count01 = i.Product.UnitList.Unit01.Count,
                                             Count02 = i.Product.UnitList.Unit02.Count,
                                             i.WarehouseCode,
                                             i.Warehouse.WarehouseName,
                                             Beginning = i.Beginning,
                                             EntryAmount = i.EntryAmount,
                                             DeliveryAmount = i.DeliveryAmount,
                                             ProfitAmount = i.ProfitAmount,
                                             LossAmount = i.LossAmount,
                                             Ending = i.Ending
                                         });
            string unitName = "";
            decimal count = 1;
            //标准单位（标准件||标准条）
            if (unitType == "1" || unitType == "2")
            {
                if (unitType == "1")
                {
                    unitName = "标准件";
                    count = 10000;
                }
                if (unitType == "2")
                {
                    unitName = "标准条";
                    count = 200;
                }
                var dailyBalance = query.ToArray().Select(i => new
                {
                    SettleDate = i.SettleDate.ToString("yyyy-MM-dd"),
                    i.ProductCode,
                    i.ProductName,
                    UnitCode = "",
                    UnitName = unitName,
                    i.WarehouseCode,
                    i.WarehouseName,
                    Beginning = i.Beginning / count,
                    EntryAmount = i.EntryAmount / count,
                    DeliveryAmount = i.DeliveryAmount / count,
                    ProfitAmount = i.ProfitAmount / count,
                    LossAmount = i.LossAmount / count,
                    Ending = i.Ending / count
                });
                dt.Columns.Add("结算日期", typeof(string));
                dt.Columns.Add("仓库名称", typeof(string));
                dt.Columns.Add("商品代码", typeof(string));
                dt.Columns.Add("商品名称", typeof(string));
                dt.Columns.Add("单位", typeof(string));
                dt.Columns.Add("期初量", typeof(string));
                dt.Columns.Add("入库量", typeof(string));
                dt.Columns.Add("出库量", typeof(string));
                dt.Columns.Add("报损量", typeof(string));
                dt.Columns.Add("报益量", typeof(string));
                dt.Columns.Add("结余量", typeof(string));
                foreach (var item in dailyBalance)
                {
                    dt.Rows.Add
                        (
                            item.SettleDate,
                            item.WarehouseName,
                            item.ProductCode,
                            item.ProductName,
                            item.UnitName,
                            item.Beginning,
                            item.EntryAmount,
                            item.DeliveryAmount,
                            item.ProfitAmount,
                            item.LossAmount,
                            item.Ending
                        );
                }
                return dt;
            }
            //自然件
            if (unitType == "3" || unitType == "4")
            {
                var dailyBalance = query.ToArray().Select(i => new
                {
                    SettleDate = i.SettleDate.ToString("yyyy-MM-dd"),
                    i.ProductCode,
                    i.ProductName,
                    UnitCode = unitType == "3" ? i.UnitCode01 : i.UnitCode02,
                    UnitName = unitType == "3" ? i.UnitName01 : i.UnitName02,
                    i.WarehouseCode,
                    i.WarehouseName,
                    Beginning = i.Beginning / (unitType == "3" ? i.Count01 : i.Count02),
                    EntryAmount = i.EntryAmount / (unitType == "3" ? i.Count01 : i.Count02),
                    DeliveryAmount = i.DeliveryAmount / (unitType == "3" ? i.Count01 : i.Count02),
                    ProfitAmount = i.ProfitAmount / (unitType == "3" ? i.Count01 : i.Count02),
                    LossAmount = i.LossAmount / (unitType == "3" ? i.Count01 : i.Count02),
                    Ending = i.Ending / (unitType == "3" ? i.Count01 : i.Count02),
                });
                dt.Columns.Add("结算日期", typeof(string));
                dt.Columns.Add("仓库名称", typeof(string));
                dt.Columns.Add("商品代码", typeof(string));
                dt.Columns.Add("商品名称", typeof(string));
                dt.Columns.Add("单位", typeof(string));
                dt.Columns.Add("期初量", typeof(string));
                dt.Columns.Add("入库量", typeof(string));
                dt.Columns.Add("出库量", typeof(string));
                dt.Columns.Add("报损量", typeof(string));
                dt.Columns.Add("报益量", typeof(string));
                dt.Columns.Add("结余量", typeof(string));
                foreach (var item in dailyBalance)
                {
                    dt.Rows.Add
                        (
                            item.SettleDate,
                            item.WarehouseName,
                            item.ProductCode,
                            item.ProductName,
                            item.UnitName,
                            item.Beginning,
                            item.EntryAmount,
                            item.DeliveryAmount,
                            item.ProfitAmount,
                            item.LossAmount,
                            item.Ending
                        );
                }
                return dt;
            }
            return dt;
        } 
        #endregion

        #region 日结核对
        public System.Data.DataTable GetInfoChecking(int page, int rows, string warehouseCode, string settleDate, string unitType)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            DateTime date = Convert.ToDateTime(settleDate);
            if (unitType == null || unitType == "")
            {
                unitType = "1";
            }
            IQueryable<DailyBalance> dailyBalanceQuery = DailyBalanceRepository.GetQueryable();
            IQueryable<BusinessSystemsDailyBalance> BusinessSystemsDailyBalanceQuery = BusinessSystemsDailyBalanceRepository.GetQueryable();
            var query = dailyBalanceQuery.Where(d => d.WarehouseCode.Contains(warehouseCode) && d.SettleDate == date)
                                         .Select(d => new //本系统产品日结结果
                                         {
                                             d.SettleDate,
                                             d.ProductCode,
                                             d.Product.ProductName,
                                             UnitCode01 = d.Product.UnitList.Unit01.UnitCode,
                                             UnitName01 = d.Product.UnitList.Unit01.UnitName,
                                             UnitCode02 = d.Product.UnitList.Unit02.UnitCode,
                                             UnitName02 = d.Product.UnitList.Unit02.UnitName,
                                             Count01 = d.Product.UnitList.Unit01.Count,
                                             Count02 = d.Product.UnitList.Unit02.Count,
                                             d.WarehouseCode,
                                             d.Warehouse.WarehouseName,
                                             Beginning = d.Beginning,
                                             EntryAmount = d.EntryAmount,
                                             DeliveryAmount = d.DeliveryAmount,
                                             ProfitAmount = d.ProfitAmount,
                                             LossAmount = d.LossAmount,
                                             Ending = d.Ending,
                                             BSys_Beginning = decimal.Zero,
                                             BSys_EntryAmount = decimal.Zero,
                                             BSys_DeliveryAmount = decimal.Zero,
                                             BSys_ProfitAmount = decimal.Zero,
                                             BSys_LossAmount = decimal.Zero,
                                             BSys_Ending = decimal.Zero
                                         }).Concat(BusinessSystemsDailyBalanceQuery.Where(b => b.WarehouseCode.Contains(warehouseCode) && b.SettleDate == date)
                                                                                   .Select(b => new //营销系统产品日结结果
                                                                                   {
                                                                                       b.SettleDate,
                                                                                       b.ProductCode,
                                                                                       b.Product.ProductName,
                                                                                       UnitCode01 = b.Product.UnitList.Unit01.UnitCode,
                                                                                       UnitName01 = b.Product.UnitList.Unit01.UnitName,
                                                                                       UnitCode02 = b.Product.UnitList.Unit02.UnitCode,
                                                                                       UnitName02 = b.Product.UnitList.Unit02.UnitName,
                                                                                       Count01 = b.Product.UnitList.Unit01.Count,
                                                                                       Count02 = b.Product.UnitList.Unit02.Count,
                                                                                       b.WarehouseCode,
                                                                                       b.Warehouse.WarehouseName,
                                                                                       Beginning = decimal.Zero,
                                                                                       EntryAmount = decimal.Zero,
                                                                                       DeliveryAmount = decimal.Zero,
                                                                                       ProfitAmount = decimal.Zero,
                                                                                       LossAmount = decimal.Zero,
                                                                                       Ending = decimal.Zero,
                                                                                       BSys_Beginning = b.Beginning,
                                                                                       BSys_EntryAmount = b.EntryAmount,
                                                                                       BSys_DeliveryAmount = b.DeliveryAmount,
                                                                                       BSys_ProfitAmount = b.ProfitAmount,
                                                                                       BSys_LossAmount = b.LossAmount,
                                                                                       BSys_Ending = b.Ending
                                                                                   }));
            var InfoCheck = query.GroupBy(q => new { q.SettleDate, q.WarehouseCode, q.ProductCode })
                        .Select(q => new   //两个系统产品日结结果汇总
                        {
                            q.Key.SettleDate,
                            q.Key.WarehouseCode,
                            q.Key.ProductCode,
                            ProductName = q.Max(i => i.ProductName),
                            WarehouseName = q.Max(i => i.WarehouseName),
                            UnitCode01 = q.Max(i => i.UnitCode01),
                            UnitName01 = q.Max(i => i.UnitName01),
                            UnitCode02 = q.Max(i => i.UnitCode02),
                            UnitName02 = q.Max(i => i.UnitName02),
                            Count01 = q.Max(i => i.Count01),
                            Count02 = q.Max(i => i.Count02),
                            Beginning = q.Sum(i => i.Beginning),
                            EntryAmount = q.Sum(i => i.EntryAmount),
                            DeliveryAmount = q.Sum(i => i.DeliveryAmount),
                            ProfitAmount = q.Sum(i => i.ProfitAmount),
                            LossAmount = q.Sum(i => i.LossAmount),
                            Ending = q.Sum(i => i.Ending),
                            BSys_Beginning = q.Sum(i => i.BSys_Beginning),
                            BSys_EntryAmount = q.Sum(i => i.BSys_EntryAmount),
                            BSys_DeliveryAmount = q.Sum(i => i.BSys_DeliveryAmount),
                            BSys_ProfitAmount = q.Sum(i => i.BSys_ProfitAmount),
                            BSys_LossAmount = q.Sum(i => i.BSys_LossAmount),
                            BSys_Ending = q.Sum(i => i.BSys_Ending),
                        }).Select(n => new //两个系统产品日结结果比对
                        {
                            n.SettleDate,
                            n.WarehouseCode,
                            n.ProductCode,
                            n.ProductName,
                            n.WarehouseName,
                            n.UnitCode01,
                            n.UnitName01,
                            n.UnitCode02,
                            n.UnitName02,
                            n.Count01,
                            n.Count02,
                            n.Beginning,
                            n.EntryAmount,
                            n.DeliveryAmount,
                            n.ProfitAmount,
                            n.LossAmount,
                            n.Ending,
                            n.BSys_Beginning,
                            n.BSys_EntryAmount,
                            n.BSys_DeliveryAmount,
                            n.BSys_ProfitAmount,
                            n.BSys_LossAmount,
                            n.BSys_Ending,
                            Status = n.Beginning == n.BSys_Beginning &&
                                        n.EntryAmount == n.BSys_EntryAmount &&
                                        n.DeliveryAmount == n.BSys_DeliveryAmount &&
                                        n.ProfitAmount == n.BSys_ProfitAmount &&
                                        n.LossAmount == n.BSys_LossAmount &&
                                        n.Ending == n.BSys_Ending ?
                                        "T" : "F",
                            Status1 = n.Beginning == n.BSys_Beginning ?
                                "T" : "F",
                            Status2 = n.EntryAmount == n.BSys_EntryAmount ?
                                "T" : "F",
                            Status3 = n.DeliveryAmount == n.BSys_DeliveryAmount ?
                                "T" : "F",
                            Status4 = n.ProfitAmount == n.BSys_ProfitAmount ?
                                "T" : "F",
                            Status5 = n.LossAmount == n.BSys_LossAmount ?
                                "T" : "F",
                            Status6 = n.Ending == n.BSys_Ending ?
                                "true" : "F"
                        }).OrderBy(n => n.Status)
                          .OrderBy(n => n.WarehouseName)
                          .OrderBy(n => n.ProductCode);
            //两个系统产品日结结果比对结果按照单位换算
            string unitName = "";
            decimal count = 1;

            //标准单位（标准件||标准条）
            if (unitType == "1" || unitType == "2")
            {
                if (unitType == "1")
                {
                    unitName = "标准件";
                    count = 10000;
                }

                if (unitType == "2")
                {
                    unitName = "标准条";
                    count = 200;
                }
                var dailyBalance = InfoCheck.ToArray().Select(i => new
                {
                    SettleDate = i.SettleDate.ToString("yyyy-MM-dd"),
                    i.ProductCode,
                    i.ProductName,
                    UnitCode = "",
                    UnitName = unitName,
                    i.WarehouseCode,
                    i.WarehouseName,
                    Beginning = i.Beginning == i.BSys_Beginning ?
                    (i.Beginning / count).ToString() :
                    (i.Beginning / count).ToString() + "≠" + (i.BSys_Beginning / count).ToString(),
                    EntryAmount = i.EntryAmount == i.BSys_EntryAmount ?
                    (i.EntryAmount / count).ToString() :
                    (i.EntryAmount / count).ToString() + "≠" + (i.BSys_EntryAmount / count).ToString(),
                    DeliveryAmount = i.DeliveryAmount == i.BSys_DeliveryAmount ?
                    (i.DeliveryAmount / count).ToString() :
                    (i.DeliveryAmount / count).ToString() + "≠" + (i.BSys_DeliveryAmount / count).ToString(),
                    ProfitAmount = i.ProfitAmount == i.BSys_ProfitAmount ?
                    (i.ProfitAmount / count).ToString() :
                    (i.ProfitAmount / count).ToString() + "≠" + (i.BSys_ProfitAmount / count).ToString(),
                    LossAmount = i.LossAmount == i.BSys_LossAmount ?
                    (i.LossAmount / count).ToString() :
                    (i.LossAmount / count).ToString() + "≠" + (i.BSys_LossAmount / count).ToString(),
                    Ending = i.Ending == i.BSys_Ending ?
                    (i.Ending / count).ToString() :
                    (i.Ending / count).ToString() + "≠" + (i.BSys_Ending / count).ToString(),
                    i.Status1,
                    i.Status2,
                    i.Status3,
                    i.Status4,
                    i.Status5,
                    i.Status6
                });
                dt.Columns.Add("结算日期", typeof(string));
                dt.Columns.Add("仓库名称", typeof(string));
                dt.Columns.Add("商品代码", typeof(string));
                dt.Columns.Add("商品名称", typeof(string));
                dt.Columns.Add("单位", typeof(string));
                dt.Columns.Add("期初量(本/销)", typeof(string));
                dt.Columns.Add("入库量(本/销)", typeof(string));
                dt.Columns.Add("出库量(本/销)", typeof(string));
                dt.Columns.Add("报损量(本/销)", typeof(string));
                dt.Columns.Add("报益量(本/销)", typeof(string));
                dt.Columns.Add("结余量(本/销)", typeof(string));
                foreach (var item in dailyBalance)
                {
                    dt.Rows.Add
                        (
                            item.SettleDate,
                            item.WarehouseName,
                            item.ProductCode,
                            item.ProductName,
                            item.UnitName,
                            item.Beginning,
                            item.EntryAmount,
                            item.DeliveryAmount,
                            item.ProfitAmount,
                            item.LossAmount,
                            item.Ending
                        );
                }
                return dt;
            }

            //自然件
            if (unitType == "3" || unitType == "4")
            {
                var dailyBalance = InfoCheck.ToArray().Select(i => new
                {
                    SettleDate = i.SettleDate.ToString("yyyy-MM-dd"),
                    i.ProductCode,
                    i.ProductName,
                    UnitCode = unitType == "3" ? i.UnitCode01 : i.UnitCode02,
                    UnitName = unitType == "3" ? i.UnitName01 : i.UnitName02,
                    i.WarehouseCode,
                    i.WarehouseName,
                    Beginning = i.Beginning == i.BSys_Beginning ?
                    (i.Beginning / (unitType == "3" ? i.Count01 : i.Count02)).ToString() :
                    (i.Beginning / (unitType == "3" ? i.Count01 : i.Count02)).ToString() + "≠" + (i.BSys_Beginning / (unitType == "3" ? i.Count01 : i.Count02)).ToString(),
                    EntryAmount = i.EntryAmount == i.BSys_EntryAmount ?
                    (i.EntryAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString() :
                    (i.EntryAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString() + "≠" + (i.BSys_EntryAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString(),
                    DeliveryAmount = i.DeliveryAmount == i.BSys_DeliveryAmount ?
                    (i.DeliveryAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString() :
                    (i.DeliveryAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString() + "≠" + (i.BSys_DeliveryAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString(),
                    ProfitAmount = i.ProfitAmount == i.BSys_ProfitAmount ?
                    (i.ProfitAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString() :
                    (i.ProfitAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString() + "≠" + (i.BSys_ProfitAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString(),
                    LossAmount = i.LossAmount == i.BSys_LossAmount ?
                    (i.LossAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString() :
                    (i.LossAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString() + "≠" + (i.BSys_LossAmount / (unitType == "3" ? i.Count01 : i.Count02)).ToString(),
                    Ending = i.Ending == i.BSys_Ending ?
                    (i.Ending / (unitType == "3" ? i.Count01 : i.Count02)).ToString() :
                    (i.Ending / (unitType == "3" ? i.Count01 : i.Count02)).ToString() + "≠" + (i.BSys_Ending / (unitType == "3" ? i.Count01 : i.Count02)).ToString(),
                    i.Status1,
                    i.Status2,
                    i.Status3,
                    i.Status4,
                    i.Status5,
                    i.Status6
                });
                dt.Columns.Add("结算日期", typeof(string));
                dt.Columns.Add("仓库名称", typeof(string));
                dt.Columns.Add("商品代码", typeof(string));
                dt.Columns.Add("商品名称", typeof(string));
                dt.Columns.Add("单位", typeof(string));
                dt.Columns.Add("期初量(本/销)", typeof(string));
                dt.Columns.Add("入库量(本/销)", typeof(string));
                dt.Columns.Add("出库量(本/销)", typeof(string));
                dt.Columns.Add("报损量(本/销)", typeof(string));
                dt.Columns.Add("报益量(本/销)", typeof(string));
                dt.Columns.Add("结余量(本/销)", typeof(string));
                foreach (var item in dailyBalance)
                {
                    dt.Rows.Add
                        (
                            item.SettleDate,
                            item.WarehouseName,
                            item.ProductCode,
                            item.ProductName,
                            item.UnitName,
                            item.Beginning,
                            item.EntryAmount,
                            item.DeliveryAmount,
                            item.ProfitAmount,
                            item.LossAmount,
                            item.Ending
                        );
                }
                return dt;
            }
            return dt;
        }
        #endregion
    }
}
