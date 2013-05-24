using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.SignalR.Connection;
using THOK.Wms.SignalR.Dispatch.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.DbModel;
using THOK.Wms.SignalR.Common;
using System.Transactions;
using THOK.Wms.SignalR.Model;
using System.Threading;
using Entities.Extensions;
using THOK.Authority.Dal.Interfaces;

namespace THOK.Wms.SignalR.Dispatch.Service
{
    public class SortWorkDispatchService : Notifier<DispatchSortWorkConnection>, ISortOrderWorkDispatchService
    {
        [Dependency]
        public IStorageLocker Locker { get; set; }

        [Dependency]
        public ISortOrderDispatchRepository SortOrderDispatchRepository { get; set; }
        [Dependency]
        public IEmployeeRepository EmployeeRepository { get; set; }
        [Dependency]
        public IMoveBillMasterRepository MoveBillMasterRepository { get; set; }
        [Dependency]
        public IMoveBillDetailRepository MoveBillDetailRepository { get; set; }
        [Dependency]
        public IOutBillMasterRepository OutBillMasterRepository { get; set; }
        [Dependency]
        public IOutBillDetailRepository OutBillDetailRepository { get; set; }
        [Dependency]
        public ISortOrderRepository SortOrderRepository { get; set; }
        [Dependency]
        public ISortOrderDetailRepository SortOrderDetailRepository { get; set; }
        [Dependency]
        public ISortingLowerlimitRepository SortingLowerlimitRepository { get; set; }
        [Dependency]
        public ISortingLineRepository SortingLineRepository { get; set; }
        [Dependency]
        public ISortWorkDispatchRepository SortWorkDispatchRepository { get; set; }

        [Dependency]
        public ISystemParameterRepository SystemParameterRepository { get; set; }

        [Dependency]
        public IStorageRepository StorageRepository { get; set; }
        [Dependency]
        public IProductRepository ProductRepository { get; set; }

        [Dependency]
        public IUnitRepository UnitRepository { get; set; }

        [Dependency]
        public IMoveBillCreater MoveBillCreater { get; set; }
        [Dependency]
        public IOutBillCreater OutBillCreater { get; set; }

        public void Dispatch(string connectionId, Model.ProgressState ps, CancellationToken cancellationToken, string workDispatchId, string userName)
        {
            Locker.LockKey = workDispatchId;
            ConnectionId = connectionId;
            ps.State = StateType.Start;
            NotifyConnection(ps.Clone());

            IQueryable<SortOrderDispatch> sortOrderDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();
            IQueryable<SortOrderDetail> sortOrderDetailQuery = SortOrderDetailRepository.GetQueryable();

            IQueryable<OutBillMaster> outBillMasterQuery = OutBillMasterRepository.GetQueryable();
            IQueryable<OutBillDetail> outBillDetailQuery = OutBillDetailRepository.GetQueryable();
            IQueryable<MoveBillMaster> moveBillMasterQuery = MoveBillMasterRepository.GetQueryable();
            IQueryable<MoveBillDetail> moveBillDetailQuery = MoveBillDetailRepository.GetQueryable();

            IQueryable<SortingLowerlimit> sortingLowerlimitQuery = SortingLowerlimitRepository.GetQueryable();
            IQueryable<SortingLine> sortingLineQuery = SortingLineRepository.GetQueryable();
            IQueryable<Storage> storageQuery = StorageRepository.GetQueryable();

            IQueryable<SortWorkDispatch> sortWorkDispatchQuery = SortWorkDispatchRepository.GetQueryable();

            IQueryable<THOK.Authority.DbModel.SystemParameter> systemParQuery = SystemParameterRepository.GetQueryable();


            var IsUselowerlimit = systemParQuery.FirstOrDefault(s => s.ParameterName == "IsUselowerlimit");//查询调度是否使用下限 0 否 1是
 
            workDispatchId = workDispatchId.Substring(0, workDispatchId.Length - 1);
            int[] work = workDispatchId.Split(',').Select(s => Convert.ToInt32(s)).ToArray();
            
                //调度表未作业的数据
            var temp = sortOrderDispatchQuery.Join(sortOrderQuery,
                                                 dp => new { dp.OrderDate, dp.DeliverLineCode },
                                                 om => new { om.OrderDate, om.DeliverLineCode },
                                                 (dp, om) => new { dp.ID, dp.WorkStatus, dp.OrderDate, dp.SortingLine, dp.DeliverLineCode, om.OrderID }
                                            ).Join(sortOrderDetailQuery,
                                                 dm => new { dm.OrderID },
                                                 od => new { od.OrderID },
                                                 (dm, od) => new { dm.ID, dm.WorkStatus, dm.OrderDate, dm.SortingLine, od.Product, od.UnitCode, od.Price, od.RealQuantity }
                                            ).WhereIn(s => s.ID, work)
                                             .Where(s => s.WorkStatus == "1")
                                             .GroupBy(r => new { r.OrderDate, r.SortingLine, r.Product, r.UnitCode, r.Price })
                                             .Select(r => new { r.Key.OrderDate, r.Key.SortingLine, r.Key.Product, r.Key.UnitCode, r.Key.Price, SumQuantity = r.Sum(p => p.RealQuantity * r.Key.Product.UnitList.Unit02.Count) })
                                             .AsParallel()
                                             .GroupBy(r => new { r.OrderDate, r.SortingLine })
                                             .Select(r => new { r.Key.OrderDate, r.Key.SortingLine, Products = r })
                                             .ToArray();
           
            
            var employee = EmployeeRepository.GetQueryable().FirstOrDefault(i => i.UserName == userName);
            string operatePersonID = employee != null ? employee.ID.ToString() : "";
            if (employee == null)
            {
                ps.State = StateType.Error;
                ps.Errors.Add("未找到当前用户，或当前用户不可用！");
                NotifyConnection(ps.Clone());
                return;
            }

            decimal sumAllotQuantity = 0;
            decimal sumAllotLineQuantity = 0;

            MoveBillMaster lastMoveBillMaster = null;
            foreach (var item in temp)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested) return;

                    bool hasError = false;
                    ps.State = StateType.Info;
                    ps.Messages.Add("开始调度" + item.SortingLine.SortingLineName);
                    NotifyConnection(ps.Clone());

                    using (var scope = new TransactionScope())
                    {
                        if (item.Products.Count() > 0)
                        {
                            if (cancellationToken.IsCancellationRequested) return;

                            if (lastMoveBillMaster != null && lastMoveBillMaster.WarehouseCode != item.SortingLine.Cell.WarehouseCode)
                            {
                                if (MoveBillCreater.CheckIsNeedSyncMoveBill(lastMoveBillMaster.WarehouseCode))
                                {
                                    MoveBillCreater.CreateSyncMoveBillDetail(lastMoveBillMaster);
                                }
                            }

                            sumAllotLineQuantity = 0;

                            if (cancellationToken.IsCancellationRequested) return;
                            MoveBillMaster moveBillMaster = MoveBillCreater.CreateMoveBillMaster(item.SortingLine.Cell.WarehouseCode,
                                                                                                    item.SortingLine.MoveBillTypeCode,
                                                                                                    operatePersonID);
                            moveBillMaster.Origin = "2";
                            moveBillMaster.Description = item.SortingLine.SortingLineCode + " 分拣调度生成！";
                            lastMoveBillMaster = moveBillMaster;
                            foreach (var product in item.Products.ToArray())
                            {
                                if (product.SumQuantity > 0)
                                {
                                    if (cancellationToken.IsCancellationRequested) return;

                                    decimal sumBillQuantity = temp.Sum(t => t.Products.Sum(p => p.SumQuantity));
                                    sumAllotQuantity += product.SumQuantity;

                                    decimal sumBillProductQuantity = item.Products.Sum(p => p.SumQuantity);
                                    sumAllotLineQuantity += product.SumQuantity;

                                    ps.State = StateType.Processing;
                                    ps.TotalProgressName = "分拣作业调度";
                                    ps.TotalProgressValue = (int)(sumAllotQuantity / sumBillQuantity * 100);
                                    ps.CurrentProgressName = "正在调度：" + item.SortingLine.SortingLineName;
                                    ps.CurrentProgressValue = (int)(sumAllotLineQuantity / sumBillProductQuantity * 100);
                                    NotifyConnection(ps.Clone());

                                    if (cancellationToken.IsCancellationRequested) return;
                                    //获取分拣线下限数据
                                    var sortingLowerlimitQuantity = sortingLowerlimitQuery.Where(s => s.ProductCode == product.Product.ProductCode
                                                                                                        && s.SortingLineCode == product.SortingLine.SortingLineCode);
                                    decimal lowerlimitQuantity = 0;
                                    if (sortingLowerlimitQuantity.Count() > 0)
                                    {
                                        lowerlimitQuantity = sortingLowerlimitQuantity.Sum(s => s.Quantity);
                                    }

                                    if (cancellationToken.IsCancellationRequested) return;
                                    //获取分拣备货区库存                    
                                    var storageQuantity = storageQuery.Where(s => s.ProductCode == product.Product.ProductCode)
                                                                      .Join(sortingLineQuery,
                                                                            s => s.Cell,
                                                                            l => l.Cell,
                                                                            (s, l) => new { l.SortingLineCode, s.Quantity }
                                                                      )
                                                                      .Where(r => r.SortingLineCode == product.SortingLine.SortingLineCode);
                                    decimal storQuantity = 0;
                                    if (storageQuantity.Count() > 0)
                                    {
                                        storQuantity = storageQuantity.Sum(s => s.Quantity);
                                    }
                                    //获取当前这个卷烟库存数量
                                    string[] areaTypes = new string[] { "1", "2", "4" };
                                    var areaSumQuantitys = storageQuery.Where(s => areaTypes.Any(t => t == s.Cell.Area.AreaType) &&
                                                                              s.ProductCode == product.Product.ProductCode).ToArray();
                                    decimal areaQuantiy = 0;
                                    if (areaSumQuantitys.Count() > 0)
                                    {
                                        areaQuantiy = areaSumQuantitys.Sum(s => s.Quantity - s.OutFrozenQuantity);
                                    }

                                    //是否使用下限
                                    if (IsUselowerlimit != null && IsUselowerlimit.ParameterValue == "0")
                                        lowerlimitQuantity = 0;

                                    if (cancellationToken.IsCancellationRequested) return;

                                    //获取移库量（按整件计）出库量加上下限量减去备货区库存量取整
                                    decimal quantity = 0;

                                    quantity = Math.Ceiling((product.SumQuantity + lowerlimitQuantity - storQuantity) / product.Product.Unit.Count)
                                                   * product.Product.Unit.Count;

                                    //取整托盘
                                    decimal wholeTray = 0;
                                    if (product.Product.IsRounding == "2")
                                    {
                                        wholeTray = Math.Ceiling(quantity / (product.Product.CellMaxProductQuantity * product.Product.Unit.Count));
                                        quantity = Convert.ToDecimal(wholeTray * (product.Product.Unit.Count * product.Product.CellMaxProductQuantity));
                                    }

                                    if (areaQuantiy < quantity)//判断当前这个卷烟库存是否小于移库量
                                    {
                                        //出库量减去备货区库存量取整
                                        quantity = Math.Ceiling((product.SumQuantity - storQuantity) / product.Product.Unit.Count)
                                                   * product.Product.Unit.Count;

                                        if (areaQuantiy < quantity)
                                        {
                                            //出库量减去备货区库存量
                                            quantity = product.SumQuantity - storQuantity;
                                        }
                                    }

                                    //不取整的烟直接出库。
                                    if (product.Product.IsRounding == "1")
                                    {
                                        quantity = product.SumQuantity - storQuantity;
                                    }

                                    if (quantity > 0)
                                    {
                                        if (cancellationToken.IsCancellationRequested) return;
                                        AlltoMoveBill(moveBillMaster, product.Product, item.SortingLine.Cell, ref quantity, cancellationToken, ps, item.SortingLine.Cell.CellCode);
                                    }

                                    if (quantity > 0)
                                    {
                                        //生成移库不完整,可能是库存不足；
                                        hasError = true;
                                        ps.State = StateType.Error;
                                        ps.Errors.Add(item.SortingLine.SortingLineCode + "线," + product.Product.ProductCode + " " + product.Product.ProductName + ",库存不足！当前总量：" + Convert.ToDecimal(product.SumQuantity / product.Product.UnitList.Unit02.Count) + "(条),缺少：" + Convert.ToDecimal(quantity / product.Product.UnitList.Unit02.Count) + "(条)");
                                        NotifyConnection(ps.Clone());
                                    }
                                }
                            }

                            if (!hasError)
                            {
                                if (cancellationToken.IsCancellationRequested) return;

                                OutBillMaster outBillMaster = OutBillCreater.CreateOutBillMaster(item.SortingLine.Cell.WarehouseCode,
                                                                                                    item.SortingLine.OutBillTypeCode,
                                                                                                    operatePersonID);
                                outBillMaster.Origin = "2";
                                outBillMaster.Description = item.SortingLine.SortingLineCode + " 分拣调度生成!";
                                //添加出库单细单
                                foreach (var product in item.Products.ToArray())
                                {
                                    if (cancellationToken.IsCancellationRequested) return;
                                    OutBillCreater.AddToOutBillDetail(outBillMaster, product.Product, product.Price, product.SumQuantity);
                                }

                                if (cancellationToken.IsCancellationRequested) return;
                                //添加出库、移库主单和作业调度表
                                SortWorkDispatch sortWorkDisp = AddSortWorkDispMaster(moveBillMaster, outBillMaster, item.SortingLine.SortingLineCode, item.OrderDate);

                                //修改线路调度作业状态和作业ID
                                var sortDispTemp = sortOrderDispatchQuery.WhereIn(s => s.ID, work)
                                                                         .Where(s => s.OrderDate == item.OrderDate
                                                                         && s.SortingLineCode == item.SortingLine.SortingLineCode);

                                foreach (var sortDisp in sortDispTemp.ToArray())
                                {
                                    if (cancellationToken.IsCancellationRequested) return;
                                    sortDisp.SortWorkDispatchID = sortWorkDisp.ID;
                                    sortDisp.WorkStatus = "2";
                                }
                                if (cancellationToken.IsCancellationRequested) return;
                                SortWorkDispatchRepository.SaveChanges();
                                scope.Complete();
                                ps.Messages.Add(item.SortingLine.SortingLineName + " 调度成功！");
                            }
                            else
                            {
                                ps.State = StateType.Info;
                                ps.Messages.Add(item.SortingLine.SortingLineName + " 调度失败！");
                                NotifyConnection(ps.Clone());
                                return;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ps.State = StateType.Info;
                    ps.Errors.Add(item.SortingLine.SortingLineName + "作业调度失败！ 详情：" + e.Message);
                    NotifyConnection(ps.Clone());
                    return;
                }
            }

            if (cancellationToken.IsCancellationRequested) return;

            if (MoveBillCreater.CheckIsNeedSyncMoveBill(lastMoveBillMaster.WarehouseCode))
            {
                MoveBillCreater.CreateSyncMoveBillDetail(lastMoveBillMaster);
            }
            MoveBillMasterRepository.SaveChanges();

            ps.State = StateType.Info;
            ps.Messages.Add("调度完成!");
            NotifyConnection(ps.Clone());
        }

        private SortWorkDispatch AddSortWorkDispMaster(MoveBillMaster moveBillMaster, OutBillMaster outBillMaster, string sortingLineCode, string orderDate)
        {
            //添加分拣作业调度表
            SortWorkDispatch sortWorkDispatch = new SortWorkDispatch();
            var workDispatch = SortWorkDispatchRepository.GetQueryable()
                                                         .FirstOrDefault(w => w.OrderDate == orderDate
                                                             && w.SortingLineCode == sortingLineCode);
            sortWorkDispatch.ID = Guid.NewGuid();
            sortWorkDispatch.OrderDate = orderDate;
            sortWorkDispatch.SortingLineCode = sortingLineCode;
            sortWorkDispatch.DispatchBatch = workDispatch == null ? "1" : (Convert.ToInt32(workDispatch.DispatchBatch) + 1).ToString();
            sortWorkDispatch.OutBillNo = outBillMaster.BillNo;
            sortWorkDispatch.MoveBillNo = moveBillMaster.BillNo;
            sortWorkDispatch.DispatchStatus = "1";
            sortWorkDispatch.IsActive = "1";
            sortWorkDispatch.UpdateTime = DateTime.Now;
            SortWorkDispatchRepository.Add(sortWorkDispatch);
            return sortWorkDispatch;
        }

        private void AlltoMoveBill(MoveBillMaster moveBillMaster, Product product, Cell cell, ref decimal quantity, CancellationToken cancellationToken, ProgressState ps, string cellCode)
        {
            IQueryable<Storage> storageQuery = StorageRepository.GetQueryable();
            //选择当前订单操作目标仓库；
            var storages = storageQuery.Where(s => s.Cell.WarehouseCode == moveBillMaster.WarehouseCode);
            storages = storages.Where(s => s.Quantity - s.OutFrozenQuantity > 0 && s.Cell.Area.AllotInOrder > 0 && s.Cell.Area.AllotOutOrder > 0 && s.Cell.IsActive == "1");

            //分配整盘；排除 件烟区 条烟区 备货区 残烟区
            if (cancellationToken.IsCancellationRequested) return;
            string[] areaTypes = new string[] { "2", "3", "5" };
            var ss = storages.Where(s => areaTypes.All(a => a != s.Cell.Area.AreaType)
                                        && s.Cell.IsSingle == "1"
                                        && s.ProductCode == product.ProductCode)
                             .OrderBy(s => new { s.StorageTime, s.Cell.Area.AllotOutOrder });
            if (quantity > 0) AllotPallet(moveBillMaster, ss, cell, ref quantity, cancellationToken, ps);

            //分配件烟；件烟区 
            if (cancellationToken.IsCancellationRequested) return;
            areaTypes = new string[] { "2" };
            ss = storages.Where(s => areaTypes.Any(a => a == s.Cell.Area.AreaType)
                                        && s.ProductCode == product.ProductCode)
                              .OrderBy(s => new { s.StorageTime, s.Cell.Area.AllotOutOrder });
            if (quantity > 0) AllotPiece(moveBillMaster, ss, cell, ref quantity, cancellationToken, ps);

            //分配件烟 (下层储位)；排除 件烟区 条烟区 残烟区 备货区
            if (cancellationToken.IsCancellationRequested) return;
            areaTypes = new string[] { "2", "3", "5" };
            ss = storages.Where(s => areaTypes.All(a => a != s.Cell.Area.AreaType)
                                        && s.ProductCode == product.ProductCode
                                        && s.Cell.Layer == 1)
                              .OrderBy(s => new { s.StorageTime, s.Cell.Area.AllotOutOrder });
            if (quantity > 0) AllotPiece(moveBillMaster, ss, cell, ref quantity, cancellationToken, ps);

            //分配件烟 (非下层储位)；排除 件烟区 条烟区 残烟区 备货区
            if (cancellationToken.IsCancellationRequested) return;
            areaTypes = new string[] { "2", "3", "5" };
            ss = storages.Where(s => areaTypes.All(a => a != s.Cell.Area.AreaType)
                                        && s.ProductCode == product.ProductCode
                                        && s.Cell.Layer != 1)
                              .OrderBy(s => new { s.StorageTime, s.Cell.Area.AllotOutOrder });
            if (quantity > 0) AllotPiece(moveBillMaster, ss, cell, ref quantity, cancellationToken, ps);

            //分配条烟；条烟区
            if (cancellationToken.IsCancellationRequested) return;
            areaTypes = new string[] { "3" };
            ss = storages.Where(s => areaTypes.Any(a => a == s.Cell.Area.AreaType)
                                        && s.ProductCode == product.ProductCode)
                              .OrderBy(s => new { s.StorageTime, s.Cell.Area.AllotOutOrder });
            if (quantity > 0) AllotBar(moveBillMaster, ss, cell, ref quantity, cancellationToken, ps);

            //分配条烟；件烟区
            areaTypes = new string[] { "2" };
            ss = storages.Where(s => areaTypes.Any(a => a == s.Cell.Area.AreaType)
                                        && s.ProductCode == product.ProductCode)
                              .OrderBy(s => new { s.StorageTime, s.Cell.Area.AllotOutOrder });
            if (quantity > 0) AllotBar(moveBillMaster, ss, cell, ref quantity, cancellationToken, ps);

            //分配条烟 (下层储位)；排除 件烟区 条烟区 残烟区 本货位
            if (cancellationToken.IsCancellationRequested) return;
            areaTypes = new string[] { "2", "3" };
            ss = storages.Where(s => areaTypes.All(a => a != s.Cell.Area.AreaType) && !cellCode.Contains(s.Cell.CellCode)
                                        && s.ProductCode == product.ProductCode
                                        && s.Cell.Layer == 1)
                              .OrderBy(s => new { s.StorageTime, s.Cell.Area.AllotOutOrder });
            if (quantity > 0) AllotBar(moveBillMaster, ss, cell, ref quantity, cancellationToken, ps);

            //分配条烟 (非下层储位)；排除 件烟区 条烟区 残烟区 本货位
            if (cancellationToken.IsCancellationRequested) return;
            areaTypes = new string[] { "2", "3" };
            ss = storages.Where(s => areaTypes.All(a => a != s.Cell.Area.AreaType) && !cellCode.Contains(s.Cell.CellCode)
                                        && s.ProductCode == product.ProductCode
                                        && s.Cell.Layer != 1)
                              .OrderBy(s => new { s.StorageTime, s.Cell.Area.AllotOutOrder });
            if (quantity > 0) AllotBar(moveBillMaster, ss, cell, ref quantity, cancellationToken, ps);
        }

        private void AllotBar(MoveBillMaster moveBillMaster, IOrderedQueryable<Storage> ss, Cell cell, ref decimal quantity, CancellationToken cancellationToken, ProgressState ps)
        {
            foreach (var s in ss.ToArray())
            {
                if (cancellationToken.IsCancellationRequested) return;
                if (quantity > 0)
                {
                    decimal allotQuantity = s.Quantity - s.OutFrozenQuantity;
                    decimal billQuantity = quantity;
                    allotQuantity = allotQuantity < billQuantity ? allotQuantity : billQuantity;
                    if (allotQuantity > 0)
                    {
                        var sourceStorage = Locker.LockNoEmptyStorage(s, s.Product);
                        var targetStorage = Locker.LockStorage(cell);
                        if (sourceStorage != null && targetStorage != null
                            && targetStorage.Quantity == 0
                            && targetStorage.InFrozenQuantity == 0)
                        {
                            MoveBillCreater.AddToMoveBillDetail(moveBillMaster, sourceStorage, targetStorage, allotQuantity);
                            quantity -= allotQuantity;
                        }
                        else ps.Errors.Add("可用的移入目标库存记录不足！");                       
                    }
                    //else break;
                }
                else break;
            }
        }

        private void AllotPiece(MoveBillMaster moveBillMaster, IOrderedQueryable<Storage> ss, Cell cell, ref decimal quantity, CancellationToken cancellationToken, ProgressState ps)
        {
            foreach (var s in ss.ToArray())
            {
                if (cancellationToken.IsCancellationRequested) return;
                if (quantity > 0)
                {
                    decimal allotQuantity = s.Quantity - s.OutFrozenQuantity;
                    decimal billQuantity = Math.Floor(quantity / s.Product.Unit.Count)
                                            * s.Product.Unit.Count;
                    allotQuantity = allotQuantity < billQuantity ? allotQuantity : billQuantity;
                    if (allotQuantity > 0)
                    {
                        var sourceStorage = Locker.LockNoEmptyStorage(s, s.Product);
                        var targetStorage = Locker.LockStorage(cell);
                        if (sourceStorage != null && targetStorage != null
                            && targetStorage.Quantity == 0
                            && targetStorage.InFrozenQuantity == 0)
                        {
                            MoveBillCreater.AddToMoveBillDetail(moveBillMaster, sourceStorage, targetStorage, allotQuantity);
                            quantity -= allotQuantity;
                        }
                        else ps.Errors.Add("可用的移入目标库存记录不足！");  
                    }
                    //else break;
                }
                else break;
            }
        }

        private void AllotPallet(MoveBillMaster moveBillMaster, IOrderedQueryable<Storage> ss, Cell cell, ref decimal quantity, CancellationToken cancellationToken, ProgressState ps)
        {
            foreach (var s in ss.ToArray())
            {
                if (cancellationToken.IsCancellationRequested) return;
                if (quantity > 0)
                {
                    decimal allotQuantity = s.Quantity - s.OutFrozenQuantity;
                    decimal billQuantity = Math.Floor(quantity / s.Product.Unit.Count)
                                            * s.Product.Unit.Count;
                    if (billQuantity >= allotQuantity)
                    {
                        var sourceStorage = Locker.LockNoEmptyStorage(s, s.Product);
                        var targetStorage = Locker.LockStorage(cell);
                        if (sourceStorage != null && targetStorage != null
                            && targetStorage.Quantity == 0
                            && targetStorage.InFrozenQuantity ==0)
                        {
                            MoveBillCreater.AddToMoveBillDetail(moveBillMaster, sourceStorage, targetStorage, allotQuantity);
                            quantity -= allotQuantity;
                        }
                        else ps.Errors.Add("可用的移入目标库存记录不足！");
                    }
                    //else break;
                }
                else break;
            }
        }

        public bool LowerLimitMoveLibrary(string userName, bool isEnableStocking, out string errorInfo)
        {
            IQueryable<SortingLowerlimit> sortingLowerlimitQuery = SortingLowerlimitRepository.GetQueryable();
            IQueryable<SortingLine> sortingLineQuery = SortingLineRepository.GetQueryable();
            IQueryable<Storage> storageQuery = StorageRepository.GetQueryable();

            bool Result = true;
            errorInfo = string.Empty;

            var sortLowerlimit = sortingLowerlimitQuery.Where(s => s.Quantity > 0)
                                                       .GroupBy(s => new { s.SortingLine, s.Product, s.UnitCode })
                                                       .Select(l => new { l.Key.SortingLine, l.Key.Product, l.Key.UnitCode, SumQuantity = l.Sum(p => p.Quantity) })
                                                       .GroupBy(o => new { o.SortingLine })
                                                       .Select(t => new { t.Key.SortingLine, product = t })
                                                       .ToArray();

            string cellCode = "";
            var sortings = sortingLineQuery.Where(s => s.SortingLineCode == s.SortingLineCode).ToArray();
            foreach (var sort in sortings)
            {
                cellCode += sort.Cell.CellCode;
            }
         
            var employee = EmployeeRepository.GetQueryable().FirstOrDefault(i => i.UserName == userName);
            string operatePersonID = employee != null ? employee.ID.ToString() : "";
            MoveBillMaster lastMoveBillMaster = null;
            if (sortLowerlimit.Count() > 0)
            {
                foreach (var item in sortLowerlimit)
                {
                    if (item.product.Count() > 0)
                    {
                        if (lastMoveBillMaster != null && lastMoveBillMaster.WarehouseCode != item.SortingLine.Cell.WarehouseCode)
                        {
                            if (MoveBillCreater.CheckIsNeedSyncMoveBill(lastMoveBillMaster.WarehouseCode))
                            {
                                MoveBillCreater.CreateSyncMoveBillDetail(lastMoveBillMaster);
                            }
                        }

                        MoveBillMaster moveBillMaster = MoveBillCreater.CreateMoveBillMaster(item.SortingLine.Cell.WarehouseCode,
                                                                                                        item.SortingLine.MoveBillTypeCode,
                                                                                                        operatePersonID);
                        moveBillMaster.Origin = "1";
                        moveBillMaster.Description = "根据 " + item.SortingLine.SortingLineName + "下限生成移库单！";
                        bool hasError = false;
                        lastMoveBillMaster = moveBillMaster;
                        foreach (var product in item.product.ToArray())
                        {

                            //获取分拣备货区库存                    
                            var storageQuantity = storageQuery.Where(s => s.ProductCode == product.Product.ProductCode)
                                                              .Join(sortingLineQuery,
                                                                    s => s.Cell,
                                                                    l => l.Cell,
                                                                    (s, l) => new { l.SortingLineCode, s.Quantity }
                                                              )
                                                              .Where(r => r.SortingLineCode == product.SortingLine.SortingLineCode);

                            decimal storQuantity = 0;
                            if (storageQuantity.Count() > 0)
                            {
                                storQuantity = storageQuantity.Sum(s => s.Quantity);
                            }

                            //获取移库量（按整件计）
                            decimal quantity = 0;
                            if (isEnableStocking)
                                quantity = Math.Ceiling(product.SumQuantity - storQuantity);
                            else
                                quantity = Math.Ceiling(product.SumQuantity);

                            CancellationToken cancellationToken = new CancellationToken();
                            ProgressState ps = new ProgressState();
                            AlltoMoveBill(moveBillMaster, product.Product, item.SortingLine.Cell, ref quantity, cancellationToken, ps, cellCode);

                            if (quantity > 0)
                            {
                                //生成移库不完整,可能是库存不足；
                                hasError = true;
                                errorInfo += item.SortingLine.SortingLineName + "  卷烟：" + product.Product.ProductCode + "(" + product.Product.ProductName + ")  库存不足！";
                            }
                        }
                        if (!hasError && Result)
                        {
                            MoveBillMasterRepository.SaveChanges();
                            errorInfo += "分拣线：" + item.SortingLine.SortingLineName + " 根据下限生成移库单成功！";

                            if (lastMoveBillMaster != null)
                            {
                                if (MoveBillCreater.CheckIsNeedSyncMoveBill(lastMoveBillMaster.WarehouseCode))
                                {
                                    MoveBillCreater.CreateSyncMoveBillDetail(lastMoveBillMaster);
                                }
                            }
                            MoveBillMasterRepository.SaveChanges();
                        }
                        else
                            Result = false;
                    }
                }
            }
            return Result;
        }
    }
}
