using System;
using THOK.Wms.AutomotiveSystems.Models;
using THOK.Wms.AutomotiveSystems.Interfaces;
using THOK.Wms.Dal.Interfaces;
using Microsoft.Practices.Unity;
using System.Linq;
using System.Transactions;
using THOK.Wms.SignalR.Common;
using THOK.Wms.DbModel;
using Entities.Extensions;
using THOK.Wms.Dal.EntityRepository;
using THOK.Wms.SignalR.Connection;

namespace THOK.Wms.AutomotiveSystems.Service
{
    public class TaskService : ITaskService
    {
        [Dependency]
        public IInBillMasterRepository InBillMasterRepository { get; set; }
        [Dependency]
        public IInBillAllotRepository InBillAllotRepository { get; set; }
        [Dependency]
        public IOutBillMasterRepository OutBillMasterRepository { get; set; }
        [Dependency]
        public IOutBillAllotRepository OutBillAllotRepository { get; set; }
        [Dependency]
        public IMoveBillMasterRepository MoveBillMasterRepository { get; set; }
        [Dependency]
        public IMoveBillDetailRepository MoveBillDetailRepository { get; set; }
        [Dependency]
        public ICheckBillMasterRepository CheckBillMasterRepository { get; set; }
        [Dependency]
        public ICheckBillDetailRepository CheckBillDetailRepository { get; set; }

        [Dependency]
        public ISortWorkDispatchRepository SortWorkDispatchRepository { get; set; }

        [Dependency]
        public IOutBillDetailRepository OutBillDetailRepository { get; set; }

        [Dependency]
        public ISortOrderDispatchRepository SortOrderDispatchRepository { get; set; }

        [Dependency]
        public IStorageRepository StorageRepository { get; set; }

        [Dependency]
        public ISortOrderRepository SortOrderRepository { get; set; }

        [Dependency]
        public ISortOrderDetailRepository SortOrderDetailRepository { get; set; }

        [Dependency]
        public ISortingLineRepository SortingLineRepository { get; set; }

        [Dependency]
        public ITrayInfoRepository TrayInfoRepository { get; set; }

        [Dependency]
        public IStorageLocker Locker { get; set; }
 
        public void GetBillMaster(string[] BillTypes, Result result)
        {
            BillMaster[] billMasters = new BillMaster[] { };
            try
            {
                foreach (var billType in BillTypes)
                {
                    switch (billType)
                    {
                        case "1"://入库单
                            var inBillMasters = InBillMasterRepository.GetQueryable()
                                .Where(i => i.Status == "4" || i.Status == "5")
                                .Select(i => new BillMaster() { BillNo = i.BillNo, BillType = "1" })
                                .ToArray();
                            billMasters = billMasters.Concat(inBillMasters).ToArray();
                            break;
                        case "2"://出库单
                            var outBillMasters = OutBillMasterRepository.GetQueryable()
                                .Where(i => i.Status == "4" || i.Status == "5")
                                .Select(i => new BillMaster() { BillNo = i.BillNo, BillType = "2" })
                                .ToArray();
                            billMasters = billMasters.Concat(outBillMasters).ToArray();
                            break;
                        case "3"://移库单
                            var moveBillMasters = MoveBillMasterRepository.GetQueryable()
                                .Where(i => i.Status == "2" || i.Status == "3")
                                .Select(i => new BillMaster() { BillNo = i.BillNo, BillType = "3" })
                                .ToArray();
                            billMasters = billMasters.Concat(moveBillMasters).ToArray();
                            break;
                        case "4"://盘点单
                            var checkBillMasters = CheckBillMasterRepository.GetQueryable()
                                .Where(i => i.Status == "2" || i.Status == "3")
                                .Select(i => new BillMaster() { BillNo = i.BillNo, BillType = "4" })
                                .ToArray();
                            billMasters = billMasters.Concat(checkBillMasters).ToArray();
                            break;
                        default:
                            break;
                    }
                }
                result.IsSuccess = true;
                result.BillMasters = billMasters;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Message = "调用服务器服务查询订单主表失败，详情：" +e.InnerException.Message+"  其他错误：" +e.Message;
            }
        }

        public void GetBillDetail(BillMaster[] billMasters, string productCode, string OperateType, string OperateAreas, string Operator, Result result)
        {
            BillDetail[] billDetails = new BillDetail[] { };
            var ops = OperateAreas.Split(',').Select(a => Convert.ToInt32(a)).ToArray();
            
            try
            {
                foreach (var billMaster in billMasters)
                {
                    string billNo = billMaster.BillNo;
                    switch (billMaster.BillType)
                    {
                        #region 读入库单细单
                        case "1"://入库单
                            var inBillDetails = InBillAllotRepository.GetQueryable()
                                .WhereIn(m => m.Cell.Layer, ops)
                                .Where(i => i.BillNo == billNo
                                    && (i.ProductCode == productCode || productCode == string.Empty)                                    
                                    && (i.Status == "0" || (i.Status == "1" && i.Operator == Operator)))
                                .Select(i => new BillDetail() { 
                                    BillNo = i.BillNo, 
                                    BillType = "1" ,

                                    DetailID = i.ID,
                                    StorageName = i.Cell.CellName,
                                    StorageRfid = i.Storage.Rfid,
                                    CellRfid = i.Cell.Rfid,
                                    TargetStorageName = "",
                                    TargetStorageRfid = "",

                                    ProductCode = i.ProductCode,
                                    ProductName = i.Product.ProductName,

                                    PieceQuantity =Math.Floor(i.AllotQuantity/i.Product.UnitList.Unit01.Count),
                                    BarQuantity = Math.Floor((i.AllotQuantity % i.Product.UnitList.Unit01.Count) / i.Product.UnitList.Unit02.Count),
                                    OperatePieceQuantity = Math.Floor(i.AllotQuantity / i.Product.UnitList.Unit01.Count),
                                    OperateBarQuantity = Math.Floor((i.AllotQuantity % i.Product.UnitList.Unit01.Count) / i.Product.UnitList.Unit02.Count),

                                    OperatorCode = string.Empty,
                                    Operator = i.Operator,
                                    Status = i.Status,
                                })
                                .ToArray();
                            billDetails = billDetails.Concat(inBillDetails).ToArray();
                            break;
                            #endregion
                        #region 读出库单细单
                        case "2"://出库单
                            var outBillDetails = OutBillAllotRepository.GetQueryable()
                                .WhereIn(m => m.Cell.Layer, ops)
                                .Where(i => i.BillNo == billNo
                                    && (i.CanRealOperate == "1" || OperateType != "Real")
                                    && (i.Status == "0" || (i.Status == "1" && i.Operator == Operator)))
                                .Select(i => new BillDetail()
                                {
                                    BillNo = i.BillNo,
                                    BillType = "2",

                                    DetailID = i.ID,
                                    StorageName = i.Cell.CellName,
                                    StorageRfid = i.Storage.Rfid,
                                    CellRfid =i.Cell.Rfid,
                                    TargetStorageName = "",
                                    TargetStorageRfid = "",

                                    ProductCode = i.ProductCode,
                                    ProductName = i.Product.ProductName,

                                    PieceQuantity = Math.Floor(i.AllotQuantity / i.Product.UnitList.Unit01.Count),
                                    BarQuantity = Math.Floor((i.AllotQuantity % i.Product.UnitList.Unit01.Count) / i.Product.UnitList.Unit02.Count),
                                    OperatePieceQuantity = Math.Floor(i.AllotQuantity / i.Product.UnitList.Unit01.Count),
                                    OperateBarQuantity = Math.Floor((i.AllotQuantity % i.Product.UnitList.Unit01.Count) / i.Product.UnitList.Unit02.Count),

                                    OperatorCode = string.Empty,
                                    Operator = i.Operator,
                                    Status = i.Status,
                                })
                                .ToArray();
                            billDetails = billDetails.Concat(outBillDetails).ToArray();

                            var outBillMaster = OutBillMasterRepository.GetQueryable()
                                .Where(i => i.BillNo == billNo)
                                .FirstOrDefault();
                            if (outBillMaster != null && outBillMaster.MoveBillMasterBillNo != null)
                            {
                                billNo = outBillMaster.MoveBillMasterBillNo;
                                //todo;
                                var moveBillDetailss = MoveBillDetailRepository.GetQueryable()
                                        .WhereIn(m => m.InCell.Layer, ops)
                                        .Where(i => i.BillNo == billNo
                                            && (i.CanRealOperate == "1" || OperateType != "Real")
                                            && (i.Status == "0" || (i.Status == "1" && i.Operator == Operator)))                                        
                                        .ToArray()
                                        .Select(i => new BillDetail()
                                        {
                                            BillNo = i.BillNo,
                                            BillType = "3",

                                            DetailID = i.ID,
                                            StorageName = i.OutCell.CellName,
                                            StorageRfid = i.OutCell.Rfid,
                                            TargetStorageName = i.InCell.CellName,
                                            TargetStorageRfid = i.InCell.Rfid,

                                            ProductCode = i.ProductCode,
                                            ProductName = i.Product.ProductName,

                                            PieceQuantity = Math.Floor(i.RealQuantity / i.Product.UnitList.Unit01.Count),
                                            BarQuantity = Math.Floor((i.RealQuantity % i.Product.UnitList.Unit01.Count) / i.Product.UnitList.Unit02.Count),
                                            OperatePieceQuantity = Math.Floor(i.RealQuantity / i.Product.UnitList.Unit01.Count),
                                            OperateBarQuantity = Math.Floor((i.RealQuantity % i.Product.UnitList.Unit01.Count) / i.Product.UnitList.Unit02.Count),

                                            OperatorCode = string.Empty,
                                            Operator = i.Operator,
                                            Status = i.Status,
                                        })
                                        .ToArray();
                                billDetails = billDetails.Concat(moveBillDetailss).ToArray();
                            }
                            break;
                        #endregion
                        #region 读移库单细单
                        case "3"://移库单
                            var moveBillDetails = MoveBillDetailRepository.GetQueryable()
                                .WhereIn(m => m.InCell.Layer, ops)
                                .Where(i => i.BillNo == billNo
                                    && (i.CanRealOperate == "1" || OperateType != "Real")
                                    && (i.Status == "0" || (i.Status == "1" && i.Operator == Operator)))                               
                                .Select(i => new BillDetail()
                                {
                                    BillNo = i.BillNo,
                                    BillType = "3",

                                    DetailID = i.ID,
                                    StorageName = i.OutCell.CellName,
                                    StorageRfid = i.OutStorage.Rfid,
                                    CellRfid = i.OutCell.Rfid,
                                    TargetStorageName = i.InCell.CellName,
                                    TargetStorageRfid = i.InCell.Rfid,

                                    ProductCode = i.ProductCode,
                                    ProductName = i.Product.ProductName,

                                    PieceQuantity = Math.Floor(i.RealQuantity / i.Product.UnitList.Unit01.Count),
                                    BarQuantity = Math.Floor((i.RealQuantity % i.Product.UnitList.Unit01.Count) / i.Product.UnitList.Unit02.Count),
                                    OperatePieceQuantity = Math.Floor(i.RealQuantity / i.Product.UnitList.Unit01.Count),
                                    OperateBarQuantity = Math.Floor((i.RealQuantity % i.Product.UnitList.Unit01.Count) / i.Product.UnitList.Unit02.Count),

                                    OperatorCode = string.Empty,
                                    Operator = i.Operator,
                                    Status = i.Status,
                                    PalletTag = i.PalletTag??0
                                })
                                .ToArray();
                            billDetails = billDetails.Concat(moveBillDetails).ToArray();
                            break;
                        #endregion
                        #region 读盘点单细单
                        case "4"://盘点单
                            var checkBillDetails = CheckBillDetailRepository.GetQueryable()
                                .WhereIn(m => m.Cell.Layer, ops)
                                .Where(i => i.BillNo == billNo
                                    && (i.Status == "0" || (i.Status == "1" && i.Operator == Operator)))
                                .Select(i => new BillDetail()
                                {
                                    BillNo = i.BillNo,
                                    BillType = "4",

                                    DetailID = i.ID,
                                    StorageName = i.Cell.CellName,
                                    StorageRfid = i.Storage.Rfid,
                                    CellRfid = i.Cell.Rfid,
                                    TargetStorageName = "",
                                    TargetStorageRfid = "",

                                    ProductCode = i.ProductCode,
                                    ProductName = i.Product.ProductName,

                                    PieceQuantity = Math.Floor(i.RealQuantity / i.Product.UnitList.Unit01.Count),
                                    BarQuantity = Math.Floor((i.RealQuantity % i.Product.UnitList.Unit01.Count) / i.Product.UnitList.Unit02.Count),
                                    OperatePieceQuantity = Math.Floor(i.RealQuantity / i.Product.UnitList.Unit01.Count),
                                    OperateBarQuantity = Math.Floor((i.RealQuantity % i.Product.UnitList.Unit01.Count) / i.Product.UnitList.Unit02.Count),

                                    OperatorCode = string.Empty,
                                    Operator = i.Operator,
                                    Status = i.Status,
                                })
                                .ToArray();
                            billDetails = billDetails.Concat(checkBillDetails).ToArray();
                            break;
                        default:
                            break;
                        #endregion
                    }
                }
                result.IsSuccess = true;
                result.BillDetails = billDetails.OrderByDescending(i => i.Status)
                    .ThenByDescending(b => b.TargetStorageName).ToArray();
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Message = "调用服务器服务查询订单细表失败，详情：" + e.InnerException.Message + "  其他错误" + e.Message;
            }
        }

        public void Apply(BillDetail[] billDetails, string useTag, Result result)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    foreach (var billDetail in billDetails)
                    {
                        switch (billDetail.BillType)
                        {
                            case "1":
                                var inAllot = InBillAllotRepository.GetQueryable()
                                    .Where(i => i.BillNo == billDetail.BillNo
                                        && i.ID == billDetail.DetailID
                                        && i.Status == "0")
                                    .FirstOrDefault();                                
                                if (inAllot != null)
                                {
                                    inAllot.Status = "1";
                                    inAllot.Operator = billDetail.Operator;
                                    inAllot.StartTime = DateTime.Now;
                                    if (useTag == "1")
                                    {
                                        OperateToLabelServer(inAllot.BillNo, inAllot.ID.ToString(), inAllot.Cell.CellName,
                                            billDetail.BillType, inAllot.Product.ProductName, (int)billDetail.PieceQuantity,
                                            (int)billDetail.BarQuantity, "");
                                    }

                                    result.IsSuccess = true;
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = "申请入库失败，原因：没有查询到这条数据！";
                                }
                                break;
                            case "2":
                                var outAllot = OutBillAllotRepository.GetQueryable()
                                    .Where(i => i.BillNo == billDetail.BillNo
                                        && i.ID == billDetail.DetailID
                                        && i.Status == "0")
                                    .FirstOrDefault();
                                if (outAllot != null)
                                {
                                    outAllot.Status = "1";
                                    outAllot.Operator = billDetail.Operator;
                                    outAllot.StartTime = DateTime.Now;
                                    if (useTag == "1")
                                    {
                                        OperateToLabelServer(outAllot.BillNo, outAllot.ID.ToString(), outAllot.Cell.CellName,
                                            billDetail.BillType, outAllot.Product.ProductName, (int)billDetail.PieceQuantity,
                                            (int)billDetail.BarQuantity, "");
                                    }

                                    result.IsSuccess = true;
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = "申请出库失败，原因：没有查询到这条数据！";
                                }
                                break;
                            case "3":
                                var moveDetail = MoveBillDetailRepository.GetQueryable()
                                    .Where(i => i.BillNo == billDetail.BillNo
                                        && i.ID == billDetail.DetailID
                                        && i.Status == "0")
                                    .FirstOrDefault();
                                if (moveDetail != null)
                                {
                                    moveDetail.Status = "1";
                                    moveDetail.Operator = billDetail.Operator;
                                    moveDetail.StartTime = DateTime.Now;
                                    if (useTag == "1")
                                    {
                                        OperateToLabelServer(moveDetail.BillNo, moveDetail.ID.ToString(), moveDetail.OutCell.CellName,
                                             billDetail.BillType, moveDetail.Product.ProductName, (int)billDetail.PieceQuantity,
                                             (int)billDetail.BarQuantity, moveDetail.InCell.CellName);
                                    }

                                    result.IsSuccess = true;
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = "申请移库失败，原因：没有查询到这条数据！";
                                }
                                break;
                            case "4":
                                var checkDetail = CheckBillDetailRepository.GetQueryable()
                                    .Where(i => i.BillNo == billDetail.BillNo
                                        && i.ID == billDetail.DetailID
                                        && i.Status == "0")
                                    .FirstOrDefault();
                                if (checkDetail != null)
                                {
                                    checkDetail.Status = "1";
                                    checkDetail.Operator = billDetail.Operator;
                                    checkDetail.StartTime = DateTime.Now;
                                    if (useTag == "1")
                                    {
                                        OperateToLabelServer(checkDetail.BillNo, checkDetail.ID.ToString(), checkDetail.Cell.CellName,
                                            billDetail.BillType, checkDetail.Product.ProductName, (int)billDetail.PieceQuantity,
                                            (int)billDetail.BarQuantity, "");
                                    }

                                    result.IsSuccess = true;
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = "申请盘点失败，原因：没有查询到这条数据！";
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    InBillAllotRepository.SaveChanges();
                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Message = "调用服务器服务申请作业任务失败，详情：" +e.InnerException.Message+" 其他错误 "+ e.Message;
            }
        }

        public void Cancel(BillDetail[] billDetails, string useTag, Result result)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    foreach (var billDetail in billDetails)
                    {
                        switch (billDetail.BillType)
                        {
                            case "1":
                                var inAllot = InBillAllotRepository.GetQueryable()
                                    .Where(i => i.BillNo == billDetail.BillNo
                                        && i.ID == billDetail.DetailID
                                        && i.Status == "1"
                                        && i.Operator == billDetail.Operator)
                                    .FirstOrDefault();
                                if (inAllot != null)
                                {
                                    inAllot.Status = "0";
                                    inAllot.Operator = string.Empty;
                                    inAllot.StartTime = null;
                                    if (useTag == "1")                                    
                                        CancelOperateToLabelServer(inAllot.BillNo, inAllot.ID.ToString(), inAllot.Cell.CellName);

                                    result.IsSuccess = true;
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = "取消入库申请失败，原因：没有查询到这条数据！";
                                }
                                break;
                            case "2":
                                var outAllot = OutBillAllotRepository.GetQueryable()
                                    .Where(i => i.BillNo == billDetail.BillNo
                                        && i.ID == billDetail.DetailID
                                        && i.Status == "1"
                                        && i.Operator == billDetail.Operator)
                                    .FirstOrDefault();
                                if (outAllot != null)
                                {
                                    outAllot.Status = "0";
                                    outAllot.Operator = string.Empty;
                                    outAllot.StartTime = null;
                                    if (useTag == "1")    
                                        CancelOperateToLabelServer(outAllot.BillNo, outAllot.ID.ToString(), outAllot.Cell.CellName);

                                    result.IsSuccess = true;
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = "取消出库申请失败，原因：没有查询到这条数据！";
                                }
                                break;
                            case "3":
                                var moveDetail = MoveBillDetailRepository.GetQueryable()
                                    .Where(i => i.BillNo == billDetail.BillNo
                                        && i.ID == billDetail.DetailID
                                        && i.Status == "1"
                                        && i.Operator == billDetail.Operator)
                                    .FirstOrDefault();
                                if (moveDetail != null)
                                {
                                    moveDetail.Status = "0";
                                    moveDetail.Operator = string.Empty;
                                    moveDetail.StartTime = null;
                                    if (useTag == "1")    
                                        CancelOperateToLabelServer(moveDetail.BillNo, moveDetail.ID.ToString(), moveDetail.OutCell.CellName);

                                    result.IsSuccess = true;
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = "取消移库申请失败，原因：没有查询到这条数据！";
                                }
                                break;
                            case "4":
                                var checkDetail = CheckBillDetailRepository.GetQueryable()
                                    .Where(i => i.BillNo == billDetail.BillNo
                                        && i.ID == billDetail.DetailID
                                        && i.Status == "1"
                                        && i.Operator == billDetail.Operator)
                                    .FirstOrDefault();
                                if (checkDetail != null)
                                {
                                    checkDetail.Status = "0";
                                    checkDetail.Operator = string.Empty;
                                    checkDetail.StartTime = null;
                                    if (useTag == "1")
                                        CancelOperateToLabelServer(checkDetail.BillNo, checkDetail.ID.ToString(), checkDetail.Cell.CellCode);

                                    result.IsSuccess = true;
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = "取消盘点申请失败，原因：没有查询到这条数据！";
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    InBillAllotRepository.SaveChanges();
                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Message = "调用服务器服务取消作业任务失败，详情：" +e.InnerException.Message+"  其他错误："+ e.Message;
            }
        }

        public void Execute(BillDetail[] billDetails, string useTag, Result result)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    foreach (var billDetail in billDetails)
                    {
                        switch (billDetail.BillType)
                        {
                            #region 完成入库单
                            case "1":
                                var inAllot = InBillAllotRepository.GetQueryable()
                                    .Where(i => i.BillNo == billDetail.BillNo
                                        && i.ID == billDetail.DetailID
                                        && i.Status == "1"
                                        && i.Operator == billDetail.Operator)
                                    .FirstOrDefault();
                                if (inAllot != null
                                    && (inAllot.InBillMaster.Status == "4"
                                    || inAllot.InBillMaster.Status == "5"
                                    ))
                                {
                                    decimal quantity = billDetail.OperatePieceQuantity * inAllot.Product.UnitList.Unit01.Count
                                        + billDetail.OperateBarQuantity * inAllot.Product.UnitList.Unit02.Count;
                                    if (string.IsNullOrEmpty(inAllot.Storage.LockTag)
                                        && inAllot.AllotQuantity >= quantity
                                        && inAllot.Storage.InFrozenQuantity >= quantity)
                                    {
                                        inAllot.Status = "2";
                                        inAllot.Storage.Rfid = billDetail.StorageRfid;
                                        inAllot.RealQuantity += quantity;
                                        inAllot.Storage.Quantity += quantity;
                                        if(inAllot.Storage.Cell.IsSingle=="1")//货位管理更改入库时间
                                            inAllot.Storage.StorageTime = DateTime.Now;
                                        inAllot.Storage.InFrozenQuantity -= quantity;
                                        inAllot.InBillDetail.RealQuantity += quantity;
                                        inAllot.InBillMaster.Status = "5";
                                        inAllot.FinishTime = DateTime.Now;
                                        if (inAllot.InBillMaster.InBillAllots.All(c => c.Status == "2"))
                                        {
                                            inAllot.InBillMaster.Status = "6";
                                        }
                                        if (useTag == "1")    
                                            CancelOperateToLabelServer(inAllot.BillNo, inAllot.ID.ToString(), inAllot.Cell.CellName);

                                        result.IsSuccess = true;
                                    }
                                    else
                                    {
                                        result.IsSuccess = false;
                                        result.Message = "需确认入库的数据别人在操作或完成的数量不对，完成出错！";
                                    }
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = "需确认入库的数据查询为空或者主单状态不对，完成出错！";
                                }
                                break;
                            #endregion
                            #region 完成出库单
                            case "2":
                                var outAllot = OutBillAllotRepository.GetQueryable()
                                    .Where(i => i.BillNo == billDetail.BillNo
                                        && i.ID == billDetail.DetailID
                                        && i.Status == "1"
                                        && i.Operator == billDetail.Operator)
                                    .FirstOrDefault();
                                if (outAllot != null
                                    && (outAllot.OutBillMaster.Status == "4"
                                    || outAllot.OutBillMaster.Status == "5"
                                    ))
                                {
                                    decimal quantity = billDetail.OperatePieceQuantity * outAllot.Product.UnitList.Unit01.Count
                                        + billDetail.OperateBarQuantity * outAllot.Product.UnitList.Unit02.Count;
                                    if (string.IsNullOrEmpty(outAllot.Storage.LockTag)
                                        && outAllot.AllotQuantity >= quantity
                                        && outAllot.Storage.OutFrozenQuantity >= quantity)
                                    {
                                        outAllot.Status = "2";
                                        outAllot.RealQuantity += quantity;
                                        outAllot.Storage.Quantity -= quantity;
                                        if (outAllot.Storage.Quantity == 0)
                                            outAllot.Storage.Rfid = "";
                                        outAllot.Storage.OutFrozenQuantity -= quantity;
                                        outAllot.OutBillDetail.RealQuantity += quantity;
                                        outAllot.OutBillMaster.Status = "5";
                                        outAllot.FinishTime = DateTime.Now;
                                        if (outAllot.OutBillMaster.OutBillAllots.All(c => c.Status == "2"))
                                        {
                                            outAllot.OutBillMaster.Status = "6";
                                        }
                                        if (useTag == "1")    
                                            CancelOperateToLabelServer(outAllot.BillNo, outAllot.ID.ToString(), outAllot.Cell.CellName);

                                        result.IsSuccess = true;
                                    }
                                    else
                                    {
                                        result.IsSuccess = false;
                                        result.Message = "需确认出库的数据别人在操作或完成的数量不对，完成出错！";
                                    }
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = "需确认出库的数据查询为空或者主单状态不对，完成出错！";
                                }
                                break;
                            #endregion
                            #region 完成移库单
                            case "3":
                                var moveDetail = MoveBillDetailRepository.GetQueryable()
                                    .Where(i => i.BillNo == billDetail.BillNo
                                        && i.ID == billDetail.DetailID
                                        && i.Status == "1"
                                        && i.Operator == billDetail.Operator)
                                    .FirstOrDefault();
                                if (moveDetail != null
                                    && (moveDetail.MoveBillMaster.Status =="2"
                                    || moveDetail.MoveBillMaster.Status =="3"
                                    ))
                                {
                                    if (string.IsNullOrEmpty(moveDetail.InStorage.LockTag)
                                        && string.IsNullOrEmpty(moveDetail.OutStorage.LockTag)
                                        && moveDetail.InStorage.InFrozenQuantity >= moveDetail.RealQuantity
                                        && moveDetail.OutStorage.OutFrozenQuantity >= moveDetail.RealQuantity)
                                    {
                                        moveDetail.Status = "2";
                                        moveDetail.InStorage.Quantity += moveDetail.RealQuantity;
                                        moveDetail.InStorage.InFrozenQuantity -= moveDetail.RealQuantity;
                                        moveDetail.InStorage.Rfid = billDetail.StorageRfid;
                                        moveDetail.OutStorage.Quantity -= moveDetail.RealQuantity;
                                        moveDetail.OutStorage.OutFrozenQuantity -= moveDetail.RealQuantity;
                                        moveDetail.OutStorage.Rfid = "";
                                        //判断移入的时间是否小于移出的时间
                                        if (DateTime.Compare(moveDetail.InStorage.StorageTime, moveDetail.OutStorage.StorageTime) == 1)
                                            moveDetail.InStorage.StorageTime = moveDetail.OutStorage.StorageTime;
                                        moveDetail.MoveBillMaster.Status = "3";
                                        moveDetail.FinishTime = DateTime.Now;
                                        var sortwork = SortWorkDispatchRepository.GetQueryable().FirstOrDefault(s => s.MoveBillMaster.BillNo == moveDetail.MoveBillMaster.BillNo && s.DispatchStatus == "2");
                                        //修改分拣调度作业状态
                                        if (sortwork != null)
                                        {
                                            sortwork.DispatchStatus = "3";
                                        }
                                        if (moveDetail.MoveBillMaster.MoveBillDetails.All(c => c.Status == "2"))
                                        {
                                            moveDetail.MoveBillMaster.Status = "4";
                                            string errorInfo = "";
                                            MoveBillDetailRepository.SaveChanges();
                                            SettleSortWokDispatch(moveDetail.BillNo, ref errorInfo);
                                        }
                                        if (useTag == "1")
                                            CancelOperateToLabelServer(moveDetail.BillNo, moveDetail.ID.ToString(), moveDetail.OutCell.CellName);

                                        result.IsSuccess = true;
                                    }
                                    else
                                    {
                                        result.IsSuccess = false;
                                        result.Message = "需确认移库的数据别人在操作或者完成的数量不对，完成出错！";
                                    }
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = "需确认移库的数据查询为空或者主单状态不对，完成出错！";
                                }
                                break;
                            #endregion
                            #region 完成盘点单
                            case "4":
                                var checkDetail = CheckBillDetailRepository.GetQueryable()
                                    .Where(i => i.BillNo == billDetail.BillNo
                                        && i.ID == billDetail.DetailID
                                        && i.Status == "1"
                                        && i.Operator == billDetail.Operator)
                                    .FirstOrDefault();
                                if (checkDetail != null
                                    && (checkDetail.CheckBillMaster.Status == "2"
                                    || checkDetail.CheckBillMaster.Status == "3"))
                                {
                                    decimal quantity = billDetail.OperatePieceQuantity * checkDetail.Product.UnitList.Unit01.Count
                                                       + billDetail.OperateBarQuantity * checkDetail.Product.UnitList.Unit02.Count;

                                    checkDetail.Status = "2";
                                    checkDetail.RealQuantity = quantity;
                                    checkDetail.Storage.IsLock = "0";
                                    checkDetail.CheckBillMaster.Status = "3";
                                    checkDetail.FinishTime = DateTime.Now;
                                    if (checkDetail.CheckBillMaster.CheckBillDetails.All(c => c.Status == "2"))
                                    {
                                        checkDetail.CheckBillMaster.Status = "4";
                                    }
                                    if (useTag == "1")
                                        CancelOperateToLabelServer(checkDetail.BillNo, checkDetail.ID.ToString(), checkDetail.Cell.CellCode);

                                    result.IsSuccess = true;
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = "需确认盘点的数据查询为空或者主单状态不对，完成出错！";
                                }
                                break;
                            default:
                                break;
                            #endregion
                        }
                    }
                    InBillAllotRepository.SaveChanges();
                    //把库存为0，入库，出库冻结量为0，无锁的库存数据的卷烟编码清空
                    UpdateStorageInfo();
                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Message = "调用服务器服务执行作业任务失败，详情：" + e.InnerException.Message+"  其他错误："+e.Message;
            }
        }

        public void SearchRfidInfo(string rfid, Result result)
        {
            BillDetail[] billDetails = new BillDetail[] { };
            try
            {
                var taryInfo = TrayInfoRepository.GetQueryable()
                               .Where(t => t.TaryRfid == rfid)
                               .Select(t => new BillDetail()
                                {
                                    ProductCode = t.ProductCode,
                                    PieceQuantity = t.Quantity
                                }).ToArray();
                billDetails = billDetails.Concat(taryInfo).ToArray();

                result.IsSuccess = true;
                result.BillDetails = billDetails;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Message = "调用服务器服务查询托盘信息失败！，详情：" + e.InnerException.Message + "  其他错误" + e.Message;
            }
        }

        private bool SettleSortWokDispatch(string moveBillNo,ref string errorInfo)
        {
            var sortWork = SortWorkDispatchRepository.GetQueryable()
                .FirstOrDefault(s => s.MoveBillNo == moveBillNo);

            if (sortWork!= null)
            {
                //出库单作自动出库
                var storages = StorageRepository.GetQueryable()
                    .Where(s => s.CellCode == sortWork.SortingLine.CellCode
                       && s.Quantity - s.OutFrozenQuantity > 0).ToArray();
                var outAllots = sortWork.OutBillMaster.OutBillAllots;
                var outDetails = OutBillDetailRepository.GetQueryableIncludeProduct()
                    .Where(o => o.BillNo == sortWork.OutBillMaster.BillNo);
                outDetails.ToArray().AsParallel().ForAll(
                    (Action<OutBillDetail>)delegate(OutBillDetail o)
                    {
                        var ss = storages.Where(s => s.ProductCode == o.ProductCode).ToArray();
                        foreach (var s in ss)
                        {
                            lock (s)
                            {
                                if (o.BillQuantity - o.AllotQuantity > 0)
                                {
                                    decimal allotQuantity = s.Quantity;
                                    decimal billQuantity = o.BillQuantity - o.AllotQuantity;
                                    allotQuantity = allotQuantity < billQuantity ? allotQuantity : billQuantity;
                                    o.AllotQuantity += allotQuantity;
                                    o.RealQuantity += allotQuantity;
                                    s.Quantity -= allotQuantity;

                                    var billAllot = new OutBillAllot()
                                    {
                                        BillNo = sortWork.OutBillMaster.BillNo,
                                        OutBillDetailId = o.ID,
                                        ProductCode = o.ProductCode,
                                        CellCode = s.CellCode,
                                        StorageCode = s.StorageCode,
                                        UnitCode = o.UnitCode,
                                        AllotQuantity = allotQuantity,
                                        RealQuantity = allotQuantity,
                                        Status = "2"
                                    };
                                    lock (sortWork.OutBillMaster.OutBillAllots)
                                    {
                                        sortWork.OutBillMaster.OutBillAllots.Add(billAllot);
                                    }
                                }
                                else
                                    break;
                            }
                        }

                        if (o.BillQuantity - o.AllotQuantity > 0)
                        {
                            throw new Exception(sortWork.SortingLine.SortingLineName + " " + o.ProductCode + " " + o.Product.ProductName + "库存不足，缺少：" + Convert.ToDecimal((o.BillQuantity - o.AllotQuantity) / o.Product.Unit.Count) + "(件)，未能结单！");
                        }
                    }
                );

                //出库结单
                var outMaster = OutBillMasterRepository.GetQueryable()
                    .FirstOrDefault(o => o.BillNo == sortWork.OutBillNo);
                outMaster.Status = "6";
                outMaster.UpdateTime = DateTime.Now;
                //分拣作业结单                
                sortWork.DispatchStatus = "4";
                sortWork.UpdateTime = DateTime.Now;

                //分拣订单线路调度完成
                //var sortOrderDetail = SortOrderDispatchRepository.GetQueryable().Where(s => s.SortWorkDispatchID == sortWork.ID).ToArray();
                //foreach (var sortOrder in sortOrderDetail)
                //{
                //    sortOrder.WorkStatus = "2";
                //}
                return true;
            }
            return true;
        }

        private void OperateToLabelServer(string billId, string detailId, string storageId, string operateType, string tobaccoName, int piece, int item, string targetStorageName)
        {
            int operateTypeInt = Convert.ToInt32(operateType);
            string sql = @"INSERT INTO SY_SHOWINFO 
                            VALUES('{0}','{1}','{2}',{3},'{4}',{5},{6},0,0,0,'{7}');";
            string tmp = "";
            tmp = tmp + (piece > 0 ? string.Format("{0}件", piece) : "");
            tmp = tmp + (item > 0 ? string.Format("{0}条", item) : "");
            tmp = tmp + (targetStorageName.Length > 0 ? string.Format(@"->{0}", targetStorageName) : "");
            sql = string.Format(sql, billId, detailId, storageId, operateTypeInt, tobaccoName, piece, item, tmp);
            StorageRepository.GetObjectSet().ExecuteStoreCommand(sql);               
        }

        private void CancelOperateToLabelServer(string billId, string detailId, string storageId)
        {
            string sql = @" DELETE SY_SHOWINFO 
                            WHERE STORAGEID = '{0}' AND ORDERMASTERID = '{1}' AND ORDERDETAILID = '{2}'

                            UPDATE SY_SHOWINFO SET
                            READSTATE = 0,HARDWAREREADSTATE=0
                            WHERE STORAGEID = '{0}' AND CONFIRMSTATE = 0 AND READSTATE = 1

                            UPDATE STORAGES SET
                            ACT = '',PRODUCTNAME='',CONTENTS='',NUMBERSHOW='',[SIGN]=0
                            WHERE STORAGEID = '{0}'";
            sql = string.Format(sql, storageId, billId, detailId);
            StorageRepository.GetObjectSet().ExecuteStoreCommand(sql);
        }

        private void UpdateStorageInfo()
        {
            var storages = StorageRepository.GetQueryable().Where(s => string.IsNullOrEmpty(s.LockTag) && s.Quantity == 0 && s.InFrozenQuantity == 0 && s.OutFrozenQuantity == 0).ToArray();
            foreach (var item in storages)
            {
                item.Product = null;
            }
            StorageRepository.SaveChanges();
        }
        
        public bool ProcessSortInfo(string orderdate, string batchId, string sortingLineCode, string orderId, ref string error)
        {
            bool result = false;
            try
            {
                var sortOrderQuery = SortOrderRepository.GetQueryable();
                var sortOrderDetailQuery = SortOrderDetailRepository.GetQueryable();
                var sortOrderDispatchQuery = SortOrderDispatchRepository.GetQueryable();
                var sortWorkDispatchQuery = SortWorkDispatchRepository.GetQueryable();
                var sortingLineQuery = SortingLineRepository.GetQueryable();
                var storageQuery = StorageRepository.GetQueryable();
                var moveBillDetailQuery = MoveBillDetailRepository.GetQueryable();

                //todo @条要换成支；加标志以记录分拣订单状态；并用状态查询已分拣订单；
                var sortOrder = sortOrderQuery.Join(sortOrderDispatchQuery,
                                        o => new { o.OrderDate, o.DeliverLineCode },
                                        d => new { d.OrderDate, d.DeliverLineCode },
                                        (o, d) => new { d.SortingLineCode, o }
                                    ).Where(r => r.o.OrderDate == orderdate 
                                             && r.o.OrderID == orderId
                                             && r.SortingLineCode == sortingLineCode)
                                    .Select(r => r.o).FirstOrDefault();
                if (sortOrder != null)
                {
                    sortOrder.Status = "1";
                    SortOrderRepository.SaveChanges();
                }
                else
                {
                    throw new Exception("当前订单不存在请确认！");
                }

                var tmp1 = sortOrderQuery.Join(sortOrderDetailQuery,
                                m => m.OrderID,
                                d => d.OrderID,
                                (m, d) => new { m.OrderDate, m.DeliverLineCode, m.OrderID, m.Status,d.ProductCode, d.ProductName, Quantity = d.RealQuantity * d.Product.UnitList.Unit02.Count}
                            ).Join(sortOrderDispatchQuery,
                                r => new { r.OrderDate, r.DeliverLineCode },
                                d => new { d.OrderDate, d.DeliverLineCode },
                                (r, d) => new { r.OrderDate, d.SortingLineCode, d.SortWorkDispatchID, r.DeliverLineCode, r.OrderID,r.Status, r.ProductCode, r.ProductName, r.Quantity }
                            ).Join(sortWorkDispatchQuery,
                                r => r.SortWorkDispatchID,
                                w => w.ID,
                                (r, w) => new { r.OrderDate, r.SortingLineCode, r.SortWorkDispatchID, w.DispatchStatus, r.DeliverLineCode, r.OrderID, r.Status, r.ProductCode, r.ProductName, r.Quantity }
                            ).Where(r => r.OrderDate == orderdate
                                && r.SortingLineCode == sortingLineCode
                                && r.SortWorkDispatchID != null
                                && (r.DispatchStatus == "2" || r.DispatchStatus == "3")
                                && r.Status == "1"
                            ).GroupBy(r => new { r.ProductCode, r.ProductName })
                            .Select(r => new { r.Key.ProductCode,r.Key.ProductName,Quantity = - r.Sum(q=>q.Quantity)});
                
                var tmp2 = sortingLineQuery
                             .Join(storageQuery,
                                l => l.CellCode,
                                s => s.CellCode,
                                (l, s) => new { l.SortingLineCode,s.CellCode, s.ProductCode, s.Product.ProductName, s.Quantity }
                             ).Where(r => r.SortingLineCode == sortingLineCode)
                             .GroupBy(r => new { r.ProductCode, r.ProductName })
                             .Select(r => new { r.Key.ProductCode, r.Key.ProductName, Quantity = r.Sum(q => q.Quantity) });

                var tmp3 = sortingLineQuery
                             .Join(moveBillDetailQuery,
                                l => l.CellCode,
                                m => m.InCellCode,
                                (l, m) => new { l.SortingLineCode, m.InCellCode, m.ProductCode, m.Product.ProductName, m.RealQuantity, m.CanRealOperate }
                             ).Where(r => r.SortingLineCode == sortingLineCode
                                && r.CanRealOperate == "1")
                             .GroupBy(r => new { r.ProductCode,r.ProductName,r.RealQuantity})
                             .Select(r => new { r.Key.ProductCode, r.Key.ProductName, Quantity = r.Sum(q => q.RealQuantity) });

                var tmp4 = tmp1.Concat(tmp2).Concat(tmp3)
                               .GroupBy(r => new { r.ProductCode, r.ProductName })
                               .Select(r => new { r.Key.ProductCode, r.Key.ProductName, Quantity = r.Sum(q => q.Quantity) })
                               .Where(r=>r.Quantity <300000).ToArray();

                var tmp5 = sortingLineQuery
                             .Join(moveBillDetailQuery,
                                l => l.CellCode,
                                m => m.InCellCode,
                                (l, m) => new { l.SortingLineCode, m}
                             ).Where(r => r.SortingLineCode == sortingLineCode
                                && r.m.CanRealOperate != "1")
                             .Select(r => r.m);
                var tmp6 = tmp5.ToArray().OrderBy(m => m.RealQuantity);

                bool tmp8 = false;
                tmp4.AsParallel().ForAll(t =>
                    {
                        var tmp7 = tmp6.FirstOrDefault(p=>p.ProductCode == t.ProductCode);
                        if (tmp7 != null)
                        {
                            tmp7.CanRealOperate = "1";
                            tmp8 = true;
                        }
                    });
                if (tmp8)
                {
                    Notify();
                }
                MoveBillDetailRepository.SaveChanges();
                result = true;
            }
            catch (Exception e)
            {
                result = false;
                error = e.Message;
            }
            return result;
        }

        private void Notify()
        {
            AutomotiveSystemsNotify.Notify();
        }
    }
}
