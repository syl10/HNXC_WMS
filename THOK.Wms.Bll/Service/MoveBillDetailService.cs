using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.SignalR;
using THOK.Wms.SignalR.Common;

namespace THOK.Wms.Bll.Service
{
    public class MoveBillDetailService : ServiceBase<MoveBillDetail>, IMoveBillDetailService
    {
        [Dependency]
        public IMoveBillMasterRepository MoveBillMasterRepository { get; set; }
        [Dependency]
        public IMoveBillDetailRepository MoveBillDetailRepository { get; set; }
        [Dependency]
        public IEmployeeRepository EmployeeRepository { get; set; }
        [Dependency]
        public ICellRepository CellRepository { get; set; }
        [Dependency]
        public IStorageRepository StorageRepository { get; set; }
        [Dependency]
        public IUnitRepository UnitRepository { get; set; }
        [Dependency]
        public IStorageLocker Locker { get; set; }
        [Dependency]
        public IProductRepository ProductRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public string resultStr = "";//错误信息字符串

        #region IMoveBillDetail 成员

        /// <summary>
        /// 判断处理状态
        /// </summary>
        /// <param name="status">数据库查询出来的状态值</param>
        /// <returns></returns>
        public string WhatStatus(string status)
        {
            string statusStr = "";
            switch (status)
            {
                case "0":
                    statusStr = "已录入";
                    break;
                case "1":
                    statusStr = "已申请";
                    break;
                case "2":
                    statusStr = "已完成";
                    break;
            }
            return statusStr;
        }

        /// <summary>
        /// 查询移库细单
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">行数</param>
        /// <param name="BillNo">移库单号</param>
        /// <returns></returns>
        public object GetDetails(int page, int rows, string BillNo)
        {
            if (BillNo != "" && BillNo != null)
            {
                IQueryable<MoveBillDetail> MoveBillDetailQuery = MoveBillDetailRepository.GetQueryable();
                var moveBillDetail = MoveBillDetailQuery.Where(i => i.BillNo.Contains(BillNo)).OrderBy(i => i.BillNo).Select(i => i);
                int total = moveBillDetail.Count();
                moveBillDetail = moveBillDetail.Skip((page - 1) * rows).Take(rows);

                var temp = moveBillDetail.ToArray().AsEnumerable().Select(i => new
                {
                    i.ID,
                    i.BillNo,
                    i.ProductCode,
                    i.Product.ProductName,
                    i.OutCellCode,
                    OutCellName = i.OutCell.CellName,
                    i.OutStorageCode,
                    i.InCellCode,
                    InCellName = i.InCell.CellName,
                    i.InStorageCode,
                    i.UnitCode,
                    i.Unit.UnitName,
                    RealQuantity = i.RealQuantity / i.Unit.Count,
                    OperatePersonID = i.OperatePersonID == null ? string.Empty : i.OperatePersonID.ToString(),
                    EmployeeName = i.OperatePerson == null ? string.Empty : i.OperatePerson.EmployeeName,
                    StartTime = i.StartTime == null ? null : ((DateTime)i.StartTime).ToString("yyyy-MM-dd HH:mm:ss"),
                    FinishTime = i.FinishTime == null ? null : ((DateTime)i.FinishTime).ToString("yyyy-MM-dd HH:mm:ss"),
                    Status = WhatStatus(i.Status)
                });
                return new { total, rows = temp.ToArray() };
            }
            return "";
        }

        /// <summary>
        /// 新增移库细单
        /// </summary>
        /// <param name="moveBillDetail"></param>
        /// <returns></returns>
        public bool Add(MoveBillDetail moveBillDetail, out string strResult)
        {
            bool result = false;
            try
            {
                IQueryable<MoveBillDetail> moveBillDetailQuery = MoveBillDetailRepository.GetQueryable();
                var product = ProductRepository.GetQueryable().FirstOrDefault(p => p.ProductCode == moveBillDetail.ProductCode);
                var unit = UnitRepository.GetQueryable().FirstOrDefault(u => u.UnitCode == moveBillDetail.UnitCode);
                var storage = StorageRepository.GetQueryable().FirstOrDefault(s => s.StorageCode == moveBillDetail.InStorageCode);
                var outStorage = StorageRepository.GetQueryable().FirstOrDefault(s => s.StorageCode == moveBillDetail.OutStorageCode);
                var outCell = CellRepository.GetQueryable().FirstOrDefault(c => c.CellCode == moveBillDetail.OutCellCode);
                var inCell = CellRepository.GetQueryable().FirstOrDefault(c => c.CellCode == moveBillDetail.InCellCode);
                Storage inStorage = null;
                if (storage != null)
                {
                    inStorage = Locker.LockStorage(storage, product);
                }
                else
                {
                    inStorage = Locker.LockStorage(inCell);
                }
                //判断移出数量是否合理
                bool isOutQuantityRight = IsQuntityRight(moveBillDetail.RealQuantity * unit.Count, outStorage.InFrozenQuantity, outStorage.OutFrozenQuantity, outCell.MaxQuantity * product.Unit.Count, outStorage.Quantity, "out");
                if (Locker.LockStorage(outStorage, product) != null)
                {
                    if (inStorage != null)
                    {
                        //判断移入数量是否合理
                        bool isInQuantityRight = IsQuntityRight(moveBillDetail.RealQuantity * unit.Count, inStorage.InFrozenQuantity, inStorage.OutFrozenQuantity, inCell.MaxQuantity * product.Unit.Count, inStorage.Quantity, "in");
                        if (isInQuantityRight && isOutQuantityRight)
                        {
                            var mbd = new MoveBillDetail();
                            mbd.BillNo = moveBillDetail.BillNo;
                            mbd.ProductCode = moveBillDetail.ProductCode;
                            mbd.OutCellCode = moveBillDetail.OutCellCode;
                            mbd.OutStorageCode = moveBillDetail.OutStorageCode;
                            mbd.InCellCode = moveBillDetail.InCellCode;
                            mbd.InStorageCode = inStorage.StorageCode;
                            mbd.UnitCode = moveBillDetail.UnitCode;
                            mbd.RealQuantity = moveBillDetail.RealQuantity * unit.Count;
                            outStorage.OutFrozenQuantity += moveBillDetail.RealQuantity * unit.Count;
                            inStorage.ProductCode = moveBillDetail.ProductCode;
                            inStorage.InFrozenQuantity += moveBillDetail.RealQuantity * unit.Count;
                            mbd.Status = "0";
                            inStorage.LockTag = string.Empty;
                            outStorage.LockTag = string.Empty;
                            MoveBillDetailRepository.Add(mbd);
                            MoveBillDetailRepository.SaveChanges();
                            result = true;
                        }
                        else
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        resultStr = "移入库存加锁失败";
                    }
                }
                else
                {
                    resultStr = "移出库存加锁失败";
                }
            }
            catch (Exception ex)
            {
                resultStr = ex.ToString();
            }
            strResult = resultStr;
            return result;
        }

        /// <summary>
        /// 删除移库细单
        /// </summary>
        /// <param name="ID">移库细单ID</param>
        /// <returns></returns>
        public bool Delete(string ID, out string strResult)
        {
            bool result = false;
            IQueryable<MoveBillDetail> moveBillDetailQuery = MoveBillDetailRepository.GetQueryable();
            int intID = Convert.ToInt32(ID);
            var mbd = moveBillDetailQuery.FirstOrDefault(i => i.ID == intID);
            var outStorage = StorageRepository.GetQueryable().FirstOrDefault(s => s.StorageCode == mbd.OutStorageCode);
            var inStorage = StorageRepository.GetQueryable().FirstOrDefault(s => s.StorageCode == mbd.InStorageCode);
            var product = ProductRepository.GetQueryable().FirstOrDefault(p => p.ProductCode == mbd.ProductCode);
            var unit = UnitRepository.GetQueryable().FirstOrDefault(u => u.UnitCode == mbd.UnitCode);
            try
            {
                if (Locker.LockStorage(outStorage, product) != null)
                {
                    if (Locker.LockStorage(inStorage, product) != null)
                    {
                        outStorage.OutFrozenQuantity -= mbd.RealQuantity;
                        inStorage.InFrozenQuantity -= mbd.RealQuantity;
                        MoveBillDetailRepository.Delete(mbd);
                        MoveBillDetailRepository.SaveChanges();
                        result = true;
                    }
                    else
                    {
                        resultStr = "加锁移入库存失败，当前库存已有人在操作！";
                    }
                }
                else
                {
                    resultStr = "加锁移出库存失败，当前库存已有人在操作！";
                }
            }
            catch (Exception ex)
            {
                resultStr = ex.ToString();
            }
            strResult = resultStr;
            return result;
        }

        /// <summary>
        /// 修改移库细单
        /// </summary>
        /// <param name="moveBillDetail"></param>
        /// <returns></returns>
        public bool Save(MoveBillDetail moveBillDetail, out string strResult)
        {
            bool result = false;
            decimal inFrozenQuantity = 0;
            decimal outFrozenQuantity = 0;
            if (moveBillDetail.OutCellCode == moveBillDetail.InCellCode)
            {
                strResult = "移入和移出货位不能一样！";
                return false;
            }
            IQueryable<MoveBillDetail> moveBillDetailQuery = MoveBillDetailRepository.GetQueryable();
            var mbd = moveBillDetailQuery.FirstOrDefault(i => i.ID == moveBillDetail.ID && i.BillNo == moveBillDetail.BillNo);
            var unit = UnitRepository.GetQueryable().FirstOrDefault(u => u.UnitCode == moveBillDetail.UnitCode);
            Product product = null;
            if (mbd.ProductCode == moveBillDetail.ProductCode)//判断用户选择的移库卷烟编码和之前保存的移库卷烟编码是否相等
            {
                product = ProductRepository.GetQueryable().FirstOrDefault(p => p.ProductCode == mbd.ProductCode);
            }
            else
            {
                product = ProductRepository.GetQueryable().FirstOrDefault(p => p.ProductCode == moveBillDetail.ProductCode);
            }
            var outCell = CellRepository.GetQueryable().FirstOrDefault(c => c.CellCode == moveBillDetail.OutCellCode);
            var inCell = CellRepository.GetQueryable().FirstOrDefault(c => c.CellCode == moveBillDetail.InCellCode);
            Storage outStorage = null;
            Storage oldOutStorage = null;
            if (mbd.OutStorageCode == moveBillDetail.OutStorageCode)//判断用户选择的移出库存和之前保存的移出库存是否相等
            {
                outStorage = StorageRepository.GetQueryable().FirstOrDefault(s => s.StorageCode == mbd.OutStorageCode);
                outFrozenQuantity = outStorage.OutFrozenQuantity - mbd.RealQuantity;
            }
            else
            {
                oldOutStorage = StorageRepository.GetQueryable().FirstOrDefault(s => s.StorageCode == mbd.OutStorageCode);
                outStorage = StorageRepository.GetQueryable().FirstOrDefault(s => s.StorageCode == moveBillDetail.OutStorageCode);
                outFrozenQuantity = outStorage.OutFrozenQuantity;
            }
            Storage inStorage = null;
            Storage oldInStorage = null;
            if (mbd.InCellCode == moveBillDetail.InCellCode)//判断用户选择的移入货位和之前保存的移入货位是否相等
            {
                inStorage = StorageRepository.GetQueryable().FirstOrDefault(s => s.StorageCode == mbd.InStorageCode);
                inFrozenQuantity = inStorage.InFrozenQuantity - mbd.RealQuantity;
            }
            else
            {
                oldInStorage = StorageRepository.GetQueryable().FirstOrDefault(s => s.StorageCode == mbd.InStorageCode);
                inStorage = Locker.LockStorage(inCell);
                if (inStorage == null)
                {
                    strResult = "移入库存加锁失败！";
                    return false;
                }
                inFrozenQuantity = inStorage.InFrozenQuantity;
            }
            if (inCell.IsSingle == "1")
            {
                if (inStorage.Product != null && inStorage.Product.ProductCode != moveBillDetail.ProductCode)
                {
                    strResult = "货位：<" + inCell.CellName + ">是非货位管理货位不能移入不同品牌的卷烟！";
                    return false;
                }
            }
            //判断移出数量是否合理
            bool isOutQuantityRight = IsQuntityRight(moveBillDetail.RealQuantity * unit.Count, outStorage.InFrozenQuantity, outFrozenQuantity, outCell.MaxQuantity * product.Unit.Count, outStorage.Quantity, "out");
            if (Locker.LockStorage(outStorage, product) != null)
            {
                //if (Locker.LockStorage(inStorage, product) != null)
                //{
                //判断移入数量是否合理
                bool isInQuantityRight = IsQuntityRight(moveBillDetail.RealQuantity * unit.Count, inFrozenQuantity, inStorage.OutFrozenQuantity, inCell.MaxQuantity * product.Unit.Count, inStorage.Quantity, "in");
                if (isOutQuantityRight && isInQuantityRight)
                {
                    if (mbd.OutStorageCode == moveBillDetail.OutStorageCode)
                    {
                        outStorage.OutFrozenQuantity -= mbd.RealQuantity;
                        outStorage.OutFrozenQuantity += moveBillDetail.RealQuantity * unit.Count;
                    }
                    else
                    {
                        oldOutStorage.OutFrozenQuantity -= mbd.RealQuantity;
                        outStorage.OutFrozenQuantity += moveBillDetail.RealQuantity * unit.Count;
                    }
                    if (mbd.InCellCode == moveBillDetail.InCellCode)
                    {
                        inStorage.InFrozenQuantity -= mbd.RealQuantity;
                        inStorage.InFrozenQuantity += moveBillDetail.RealQuantity * unit.Count;
                    }
                    else
                    {
                        oldInStorage.InFrozenQuantity -= mbd.RealQuantity;
                        inStorage.InFrozenQuantity += moveBillDetail.RealQuantity * unit.Count;
                    }
                    mbd.ProductCode = moveBillDetail.ProductCode;
                    mbd.OutCellCode = moveBillDetail.OutCellCode;
                    mbd.OutStorageCode = moveBillDetail.OutStorageCode;
                    mbd.InCellCode = moveBillDetail.InCellCode;
                    mbd.InStorageCode = inStorage.StorageCode;
                    mbd.UnitCode = moveBillDetail.UnitCode;
                    mbd.RealQuantity = moveBillDetail.RealQuantity * unit.Count;
                    mbd.Status = "0";
                    outStorage.LockTag = string.Empty;
                    inStorage.LockTag = string.Empty;
                    inStorage.ProductCode = product.ProductCode;
                    MoveBillDetailRepository.SaveChanges();
                    result = true;
                }
                //}
                //else
                //{
                //    resultStr = "加锁移入库存失败，当前库存已有人在操作！";
                //}

            }
            else
            {
                resultStr = "加锁移出库存失败，当前库存已有人在操作！";
            }
            strResult = resultStr;
            return result;
        }

        /// <summary>
        /// 判断移库的数量是否合理
        /// </summary>
        /// <param name="inputQuantity">用户输入的移库数量</param>
        /// <param name="inFrozenQuantity">入库冻结量</param>
        /// <param name="outFrozenQuantity">出库冻结量</param>
        /// <param name="maxQuantity">货位最大存储量</param>
        /// <param name="currentQuantity">当前数量</param>
        /// <param name="inOrOut">移出还是移入</param>
        /// <returns></returns>
        public bool IsQuntityRight(decimal inputQuantity, decimal inFrozenQuantity, decimal outFrozenQuantity, decimal maxQuantity, decimal currentQuantity, string inOrOut)
        {
            bool result = false;
            if (inOrOut == "in")
            {
                if (inputQuantity <= (maxQuantity - inFrozenQuantity - currentQuantity))
                {
                    result = true;
                }
                else
                {
                    resultStr = "入库的数量必须小于或等于[货位最大量-（当前货位库存+入库冻结量）]";
                    return result;
                }
            }
            else if (inOrOut == "out")
            {
                if (Math.Abs(inputQuantity) <= (currentQuantity - outFrozenQuantity))
                {
                    result = true;
                }
                else
                {
                    resultStr = "出库数量必须小于或等于[当前库存量-出库冻结量]";
                    return result;
                }
            }
            return result;
        }

        #endregion

        #region IMoveBillDetail 成员
        /// <summary>获得移库细单信息</summary>
        public System.Data.DataTable GetMoveBillDetail(int page, int rows, string BillNo,bool isAbnormity)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            if (BillNo != "" && BillNo != null)
            {
                IQueryable<MoveBillDetail> MoveBillDetailQuery = MoveBillDetailRepository.GetQueryable();
                var moves = MoveBillDetailQuery.Where(i => i.BillNo.Contains(BillNo));
                if(isAbnormity==false)
                    moves = moves.Where(i => i.Product.IsAbnormity != "1");
                var moveBillDetail = moves.OrderBy(i => new {i.OutCell.CellName,i.ProductCode })
                                                        .Select(i => new
                {
                    OutCellName = i.OutCell.CellName,
                    OutStorageCode = i.OutStorageCode,
                    InCellName = i.InCell.CellName,
                    InStorageCode = i.InStorageCode,
                    ProductCode = i.ProductCode,
                    ProductName = i.Product.ProductName,
                    UnitCode = i.UnitCode,
                    UnitName = i.Unit.UnitName,
                    RealQuantity = i.RealQuantity / i.Unit.Count,
                    OperatePersonName = i.OperatePerson == null ? string.Empty : i.OperatePerson.EmployeeName,
                    Status = i.Status == "0" ? "未开始" : i.Status == "1" ? "已申请" : i.Status == "2" ? "已完成" : "空",
                });
                dt.Columns.Add("移出储位名称", typeof(string));
                //dt.Columns.Add("移出存储编码", typeof(string));
                dt.Columns.Add("移入储位名称", typeof(string));
                //dt.Columns.Add("移入存储编码", typeof(string));
                dt.Columns.Add("产品代码", typeof(string));
                dt.Columns.Add("产品名称", typeof(string));
                dt.Columns.Add("单位编码", typeof(string));
                dt.Columns.Add("单位名称", typeof(string));
                dt.Columns.Add("数量", typeof(decimal));
                //dt.Columns.Add("作业人员", typeof(string));
                //dt.Columns.Add("作业状态", typeof(string));

                //dt.Columns.Add("Description", typeof(string));
                foreach (var m in moveBillDetail)
                {
                    dt.Rows.Add
                        (
                              m.OutCellName
                        //, m.OutStorageCode
                            , m.InCellName
                        //, m.InStorageCode
                            , m.ProductCode
                            , m.ProductName
                            , m.UnitCode
                            , m.UnitName
                            , m.RealQuantity
                            //, m.OperatePersonName
                            //, m.Status
                        );
                }
                if (moveBillDetail.Count() > 0)
                {
                    dt.Rows.Add(
                        null, null, null, null, null, "总数：",
                        moveBillDetail.Sum(m => m.RealQuantity));
                }
            }
            return dt;
        }
        #endregion

        #region 车载系统
        public string SwitchStatus(string status)
        {
            string statusStr = "";
            switch (status)
            {
                case "0":
                    statusStr = "未开始";
                    break;
                case "1":
                    statusStr = "已申请";
                    break;
                case "2":
                    statusStr = "已完成";
                    break;
            }
            return statusStr;
        }
        public object GetMoveBillMaster()
        {
            var moveBillMaster = MoveBillMasterRepository.GetQueryable()
                                 .Where(i => i.Status == "2" || i.Status == "3")
                                 .Select(i => new { BillNo = i.BillNo, BillType = "3" })
                                 .ToArray();
            return moveBillMaster;
        }
        public object SearchMoveBillDetail(string billNo, int page, int rows)
        {
            var query = MoveBillDetailRepository.GetQueryable()
                                .Where(a => a.BillNo == billNo && a.Status != "2")
                                .OrderByDescending(a => a.Status == "1")
                                .Select(i => i);
            int total = query.Count();
            query = query.Skip((page - 1) * rows).Take(rows);

            var temp = query.ToArray().Select(a => new
            {
                a.ID,
                a.BillNo,
                a.ProductCode,
                a.Product.ProductName,
                a.InCellCode,
                InCellName = a.InCell.CellName,
                a.OutCellCode,
                OutCellName = a.OutCell.CellName,
                BillType = "移库",
                a.InStorageCode,
                a.UnitCode,
                a.Unit.UnitName,
                a.RealQuantity,
                PieceQuantity = Math.Floor(a.RealQuantity / a.Product.UnitList.Unit01.Count),
                BarQuantity = Math.Floor(a.RealQuantity % a.Product.UnitList.Unit01.Count / a.Product.UnitList.Unit02.Count),
                Status = SwitchStatus(a.Status),
                a.Operator
            });
            return new { total, rows = temp.ToArray() };
        }
        public bool EditAllot(string id, string status, string operater, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            string[] ids = id.Split(',');
            string strId = "";
            MoveBillDetail detail = null;

            var employee = EmployeeRepository.GetQueryable().FirstOrDefault(e => e.UserName == operater);

            for (int i = 0; i < ids.Length; i++)
            {
                strId = ids[i].ToString();
                detail = MoveBillDetailRepository.GetQueryable().AsEnumerable().FirstOrDefault(a => strId == a.ID.ToString());
                if (detail != null)
                {
                    if (detail.Status == "0" && status == "1"
                     || detail.Status == "1" && status == "0"
                     || detail.Status == "1" && status == "2")
                    {
                        try
                        {
                            detail.Status = status;
                            if (operater != "")
                            {
                                detail.Operator = employee.EmployeeName;
                            }
                            else
                            {
                                detail.Operator = "";
                            }
                            MoveBillDetailRepository.SaveChanges();
                            result = true;
                        }
                        catch (Exception ex)
                        {
                            strResult = "原因：" + ex.Message;
                        }
                    }
                    else
                    {
                        strResult = "原因：操作错误！";
                    }
                }
                else
                {
                    strResult = "原因：未找到该记录！";
                }
            }
            return result;
        }
        #endregion
    }
}
