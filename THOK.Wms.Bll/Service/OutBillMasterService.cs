using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.SignalR.Common;
using System.Transactions;
using THOK.Wms.Download.Interfaces;
using System.Data;
using THOK.WMS.Upload.Bll;

namespace THOK.Wms.Bll.Service
{
    public class OutBillMasterService : ServiceBase<OutBillMaster>, IOutBillMasterService
    {
        [Dependency]
        public IOutBillMasterRepository OutBillMasterRepository { get; set; }

        [Dependency]
        public IOutBillDetailRepository OutBillDetailRepository { get; set; }

        [Dependency]
        public IOutBillAllotRepository OutBillAllotRepository { get; set; }

        [Dependency]
        public IMoveBillDetailRepository MoveBillDetailRepository { get; set; }

        [Dependency]
        public IEmployeeRepository EmployeeRepository { get; set; }

        [Dependency]
        public IStorageLocker Locker { get; set; }

        [Dependency]
        public IOutBillMasterDownService OutBillMasterDownService { get; set; }

        [Dependency]
        public IStorageRepository StorageRepository { get; set; }

        [Dependency]
        public IUnitListRepository UnitListRespository { get; set; }

        UploadBll upload = new UploadBll();

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public string infoStr = "";//错误信息字符串

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

        public object GetDetails(int page, int rows, string BillNo, string beginDate, string endDate, string OperatePersonCode, string CheckPersonCode, string Status, string IsActive)
        {
            IQueryable<OutBillMaster> OutBillMasterQuery = OutBillMasterRepository.GetQueryable().Where(o => o.Status != "6");

            var outBillMaster = OutBillMasterQuery
                                    .OrderByDescending(t => t.BillDate)
                                    .OrderByDescending(t => t.BillNo)
                                    .Select(i => i);
            if (!BillNo.Equals(string.Empty) && BillNo != null)
            {
                outBillMaster = outBillMaster.Where(i => i.BillNo.Contains(BillNo));
            }
            if (!beginDate.Equals(string.Empty))
            {
                DateTime begin = Convert.ToDateTime(beginDate);
                outBillMaster = outBillMaster.Where(i => i.BillDate >= begin);
            }
            if (!endDate.Equals(string.Empty))
            {
                DateTime end = Convert.ToDateTime(endDate).AddDays(1);
                outBillMaster = outBillMaster.Where(i => i.BillDate <= end);
            }
            if (!OperatePersonCode.Equals(string.Empty) && OperatePersonCode != null)
            {
                outBillMaster = outBillMaster.Where(i => i.OperatePerson.EmployeeCode.Contains(OperatePersonCode));
            }
            if (!Status.Equals(string.Empty))
            {
                outBillMaster = outBillMaster.Where(i => i.Status.Contains(Status) && i.Status != "6");
            }
            if (!IsActive.Equals(string.Empty))
            {
                outBillMaster = outBillMaster.Where(i => i.IsActive.Contains(IsActive));
            }
            if (!CheckPersonCode.Equals(string.Empty))
            {
                outBillMaster = outBillMaster.Where(i => i.VerifyPerson.EmployeeCode == CheckPersonCode);
            }
            int total = outBillMaster.Count();
            outBillMaster = outBillMaster.Skip((page - 1) * rows).Take(rows);

            var temp = outBillMaster.ToArray().AsEnumerable().Select(i => new
                    {
                        i.BillNo,
                        BillDate = i.BillDate.ToString("yyyy-MM-dd HH:mm:ss"),
                        i.Warehouse.WarehouseCode,
                        i.Warehouse.WarehouseName,
                        i.OperatePersonID,
                        i.VerifyPersonID,
                        OperatePersonCode = i.OperatePerson.EmployeeCode,
                        OperatePersonName = i.OperatePerson.EmployeeName,
                        VerifyPersonCode = i.VerifyPersonID == null ? string.Empty : i.VerifyPerson.EmployeeCode,
                        VerifyPersonName = i.VerifyPersonID == null ? string.Empty : i.VerifyPerson.EmployeeName,
                        SumQuantity = i.OutBillDetails.Sum(s => s.BillQuantity / s.Product.Unit.Count),
                        BillTypeCode = i.BillType.BillTypeCode,
                        BillTypeName = i.BillType.BillTypeName,
                        VerifyDate = i.VerifyDate == null ? string.Empty : ((DateTime)i.VerifyDate).ToString("yyyy-MM-dd HH:mm:ss"),
                        Status = WhatStatus(i.Status),
                        IsActive = i.IsActive == "1" ? "可用" : "不可用",
                        Description = i.Description,
                        UpdateTime = i.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        i.TargetCellCode
                    });
            return new { total, rows = temp.ToArray() };
        }

        public bool Add(OutBillMaster outBillMaster, string userName, out string errorInfo)
        {
            errorInfo = string.Empty;
            var outbm = new OutBillMaster();
            var employee = EmployeeRepository.GetQueryable().FirstOrDefault(i => i.UserName == userName);
            if (employee != null)
            {
                try
                {
                    outbm.BillNo = outBillMaster.BillNo;
                    outbm.BillDate = outBillMaster.BillDate;
                    outbm.BillTypeCode = outBillMaster.BillTypeCode;
                    outbm.WarehouseCode = outBillMaster.WarehouseCode;
                    outbm.OperatePersonID = employee.ID;
                    outbm.Status = "1";
                    outbm.VerifyPersonID = outBillMaster.VerifyPersonID;
                    outbm.VerifyDate = outBillMaster.VerifyDate;
                    outbm.Description = outBillMaster.Description;
                    //outbm.IsActive = outBillMaster.IsActive;
                    outbm.IsActive = "1";
                    outbm.UpdateTime = DateTime.Now;
                    outbm.Origin = "1";
                    outbm.TargetCellCode = outBillMaster.TargetCellCode;

                    OutBillMasterRepository.Add(outbm);
                    OutBillMasterRepository.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    errorInfo = "添加失败！原因：" + e.Message;
                    return false;
                }
            }
            else
            {
                errorInfo = "找不到当前登陆用户！请重新登陆！";
                return false;
            }
        }

        public bool Delete(string BillNo, out string errorInfo)
        {
            errorInfo = string.Empty;
            var ibm = OutBillMasterRepository.GetQueryable().FirstOrDefault(i => i.BillNo == BillNo && i.Status == "1");
            if (ibm != null)
            {
                try
                {
                    //Del(OutBillDetailRepository, ibm.OutBillAllots);
                    Del(OutBillDetailRepository, ibm.OutBillDetails);
                    OutBillMasterRepository.Delete(ibm);
                    OutBillMasterRepository.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    errorInfo = "删除失败！原因：" + e.Message;
                    return false;
                }
            }
            else
            {
                errorInfo = "删除失败！未找到当前需要删除的数据！";
                return false;
            }
        }

        public bool Save(OutBillMaster outBillMaster, out string errorInfo)
        {
            bool result = false;
            errorInfo = string.Empty;
            var outbm = OutBillMasterRepository.GetQueryable().FirstOrDefault(i => i.BillNo == outBillMaster.BillNo && i.Status == "1");
            if (outbm != null)
            {
                try
                {
                    outbm.BillDate = outBillMaster.BillDate;
                    outbm.BillTypeCode = outBillMaster.BillTypeCode;
                    outbm.WarehouseCode = outBillMaster.WarehouseCode;
                    outbm.OperatePersonID = outBillMaster.OperatePersonID;
                    outbm.Status = "1";
                    outbm.VerifyPersonID = outBillMaster.VerifyPersonID;
                    outbm.VerifyDate = outBillMaster.VerifyDate;
                    outbm.Description = outBillMaster.Description;
                    outbm.IsActive = "1";
                    outbm.UpdateTime = DateTime.Now;
                    outbm.Origin = "1";
                    outbm.TargetCellCode = outBillMaster.TargetCellCode;

                    OutBillMasterRepository.SaveChanges();
                    result = true;
                }
                catch (Exception e)
                {
                    errorInfo = "删除失败！原因：" + e.Message;
                }
            }
            else
                errorInfo = "保存失败！没有找到这条数据！";
            return result;
        }

        /// <summary>
        /// 生成出库单号
        /// </summary>
        /// <param name="userName">登陆用户</param>
        /// <returns></returns>
        public object GenInBillNo(string userName)
        {
            string billno = "";
            IQueryable<OutBillMaster> outBillMasterQuery = OutBillMasterRepository.GetQueryable();
            string sysTime = System.DateTime.Now.ToString("yyMMdd");
            var outBillMaster = outBillMasterQuery.Where(i => i.BillNo.Contains(sysTime)).ToArray().OrderBy(i => i.BillNo).Select(i => new { i.BillNo }.BillNo);
            var employee = EmployeeRepository.GetQueryable().FirstOrDefault(i => i.UserName == userName);
            if (outBillMaster.Count() == 0)
            {
                billno = System.DateTime.Now.ToString("yyMMdd") + "0001" + "OU";
            }
            else
            {
                string billNoStr = outBillMaster.Last(b => b.Contains(sysTime));
                int i = Convert.ToInt32(billNoStr.ToString().Substring(6, 4));
                i++;
                string newcode = i.ToString();
                for (int j = 0; j < 4 - i.ToString().Length; j++)
                {
                    newcode = "0" + newcode;
                }
                billno = System.DateTime.Now.ToString("yyMMdd") + newcode + "OU";
            }
            var findBillInfo = new
            {
                BillNo = billno,
                billNoDate = DateTime.Now.ToString("yyyy-MM-dd"),
                employeeID = employee == null ? string.Empty : employee.ID.ToString(),
                employeeCode = employee == null ? string.Empty : employee.EmployeeCode,
                employeeName = employee == null ? string.Empty : employee.EmployeeName

            };
            return findBillInfo;
        }

        /// <summary>
        /// 出库审核
        /// </summary>
        /// <param name="billNo">单据号</param>
        /// <param name="userName">登陆用户</param>
        /// <returns></returns>
        public bool Audit(string billNo, string userName, out string errorInfo)
        {
            bool result = false;
            errorInfo = string.Empty;
            var outbm = OutBillMasterRepository.GetQueryable().FirstOrDefault(i => i.BillNo == billNo);
            var employee = EmployeeRepository.GetQueryable().FirstOrDefault(i => i.UserName == userName);
            if (outbm != null && outbm.Status == "1")
            {
                if (string.IsNullOrEmpty(outbm.TargetCellCode))//判断出库主单是否有指定货位
                {
                    outbm.Status = "2";
                    outbm.VerifyDate = DateTime.Now;
                    outbm.UpdateTime = DateTime.Now;
                    outbm.VerifyPersonID = employee.ID;
                    OutBillMasterRepository.SaveChanges();
                    result = true;
                }
                else//如果出库主单指定了货位那么就从指定的货位出库
                {
                    result = OutAllot(outbm, employee.ID);
                    errorInfo = infoStr;
                }
            }
            return result;
        }

        /// <summary>
        /// 出库分配
        /// </summary>
        /// <param name="outBillMaster">出库主单</param>
        /// <returns></returns>
        public bool OutAllot(OutBillMaster outBillMaster, Guid employeeId)
        {
            try
            {
                bool result = false;
                //出库单出库
                var storages = StorageRepository.GetQueryable().Where(s => s.CellCode == outBillMaster.TargetCellCode
                                                                        && s.Quantity - s.OutFrozenQuantity > 0).ToArray();

                if (!Locker.Lock(storages))
                {
                    throw new Exception("锁定储位失败，储位其他人正在操作，无法取消分配请稍候重试！");
                }
                var outDetails = OutBillDetailRepository.GetQueryableIncludeProduct()
                    .Where(o => o.BillNo == outBillMaster.BillNo);
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
                                   BillNo = outBillMaster.BillNo,
                                   OutBillDetailId = o.ID,
                                   ProductCode = o.ProductCode,
                                   CellCode = s.CellCode,
                                   StorageCode = s.StorageCode,
                                   UnitCode = o.UnitCode,
                                   AllotQuantity = allotQuantity,
                                   RealQuantity = allotQuantity,
                                   Status = "2"
                               };
                               lock (outBillMaster.OutBillAllots)
                               {
                                   outBillMaster.OutBillAllots.Add(billAllot);
                               }
                           }
                           else
                               break;
                       }
                   }

                   if (o.BillQuantity - o.AllotQuantity > 0)
                   {
                       throw new Exception(o.ProductCode + " " + o.Product.ProductName + "库存不足，未能结单！");
                   }
               });

                result = true;
                storages.AsParallel().ForAll(s => s.LockTag = string.Empty);
                //出库结单
                outBillMaster.Status = "6";
                outBillMaster.VerifyDate = DateTime.Now;
                outBillMaster.VerifyPersonID = employeeId;
                outBillMaster.UpdateTime = DateTime.Now;
                OutBillMasterRepository.SaveChanges();

                return result;
            }
            catch (AggregateException ex)
            {
                infoStr = "审核失败，详情：" + ex.InnerExceptions.Select(i => i.Message).Aggregate((m, n) => m + n);
                return false;
            }
        }

        /// <summary>
        /// 出库反审
        /// </summary>
        /// <param name="billNo">单据号</param>
        /// <returns></returns>
        public bool AntiTrial(string billNo, out string errorInfo)
        {
            bool result = false;
            errorInfo = string.Empty;
            var outbm = OutBillMasterRepository.GetQueryable().Where(i => billNo.Contains(i.BillNo));
            if (outbm.Count() > 0)
            {
                foreach (var item in outbm.ToArray())
                {
                    if (item.Status == "2")
                    {
                        try
                        {
                            item.Status = "1";
                            item.VerifyDate = null;
                            item.UpdateTime = DateTime.Now;
                            item.VerifyPersonID = null;
                            OutBillMasterRepository.SaveChanges();
                            result = true;
                        }
                        catch (Exception e)
                        {
                            errorInfo = item.BillNo + "其他人员正在操作！无法保存！" + e.Message;
                        }
                    }
                    else
                        errorInfo = item.BillNo + "这条单据状态不是已审核！";
                }
            }
            else
                errorInfo = "保存失败！没有找到这些数据！";
            return result;
        }

        /// <summary>
        /// 出库结单
        /// </summary>
        /// <param name="billNo">单据号</param>
        /// <param name="errorInfo">错误信息</param>
        /// <returns></returns>
        public bool Settle(string billNo, out string errorInfo)
        {
            bool result = false;
            errorInfo = string.Empty;
            var outbm = OutBillMasterRepository.GetQueryable().FirstOrDefault(i => i.BillNo == billNo);
            if (outbm != null && outbm.Status == "5")
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        //结单移库单,修改冻结量
                        var moveDetail = MoveBillDetailRepository.GetQueryable()
                                                                 .Where(m => m.BillNo == outbm.MoveBillMasterBillNo
                                                                     && m.Status != "2");
                        //结单出库单,修改冻结量
                        var outAllot = OutBillAllotRepository.GetQueryable()
                                                             .Where(o => o.BillNo == outbm.BillNo
                                                                 && o.Status != "2");

                        var sourceStorages = moveDetail.Select(m => m.OutStorage).ToArray();
                        var targetStorages = moveDetail.Select(m => m.InStorage).ToArray();
                        var storages = outAllot.Select(i => i.Storage).ToArray();

                        if (!Locker.Lock(storages)
                            || !Locker.Lock(sourceStorages)
                            || !Locker.Lock(targetStorages))
                        {
                            errorInfo = "锁定储位失败，储位其他人正在操作，无法取消分配请稍候重试！";
                            return false;
                        }

                        moveDetail.AsParallel().ForAll(
                            (Action<MoveBillDetail>)delegate(MoveBillDetail m)
                            {
                                if (m.InStorage.ProductCode == m.ProductCode
                                    && m.OutStorage.ProductCode == m.ProductCode
                                    && m.InStorage.InFrozenQuantity >= m.RealQuantity
                                    && m.OutStorage.OutFrozenQuantity >= m.RealQuantity)
                                {
                                    m.InStorage.InFrozenQuantity -= m.RealQuantity;
                                    m.OutStorage.OutFrozenQuantity -= m.RealQuantity;
                                    m.InStorage.LockTag = string.Empty;
                                    m.OutStorage.LockTag = string.Empty;
                                }
                                else
                                {
                                    throw new Exception("储位的卷烟或入库冻结量与当前分配不符，信息可能被异常修改，不能结单！");
                                }
                            }
                        );
                        MoveBillDetailRepository.SaveChanges();

                        outAllot.AsParallel().ForAll(
                            (Action<OutBillAllot>)delegate(OutBillAllot o)
                            {
                                if (o.Storage.ProductCode == o.ProductCode
                                    && o.Storage.OutFrozenQuantity >= o.AllotQuantity)
                                {
                                    o.Storage.OutFrozenQuantity -= o.AllotQuantity;
                                    o.Storage.LockTag = string.Empty;
                                }
                                else
                                {
                                    throw new Exception("储位的卷烟或入库冻结量与当前分配不符，信息可能被异常修改，不能结单！");
                                }
                            }
                        );

                        if (outbm.MoveBillMaster != null)
                        {
                            outbm.MoveBillMaster.Status = "4";
                            outbm.MoveBillMaster.UpdateTime = DateTime.Now;
                        }

                        outbm.Status = "6";
                        outbm.UpdateTime = DateTime.Now;
                        OutBillMasterRepository.SaveChanges();
                        scope.Complete();
                        result = true;
                    }
                    catch (Exception e)
                    {
                        errorInfo = "出库单结单出错！原因：" + e.Message;
                        return false;
                    }
                }
            }
            return result;
        }


        public bool DownOutBillMaster(string beginDate, string endDate, out string errorInfo)
        {
            errorInfo = string.Empty;
            bool result = false;
            string outBillStr = "";
            string outBillMasterStr = "";
            try
            {
                var outBillNos = OutBillMasterRepository.GetQueryable().Where(i => i.BillNo == i.BillNo).Select(i => new { i.BillNo }).ToArray();

                for (int i = 0; i < outBillNos.Length; i++)
                {
                    outBillStr += outBillNos[i].BillNo + ",";
                }
                OutBillMaster[] outBillMasterList = OutBillMasterDownService.GetOutBillMaster(outBillStr);
                foreach (var master in outBillMasterList)
                {
                    var outBillMaster = new OutBillMaster();
                    outBillMaster.BillNo = master.BillNo;
                    outBillMaster.BillDate = master.BillDate;
                    outBillMaster.BillTypeCode = master.BillTypeCode;
                    outBillMaster.WarehouseCode = master.WarehouseCode;
                    outBillMaster.Status = "1";
                    outBillMaster.IsActive = master.IsActive;
                    outBillMaster.UpdateTime = DateTime.Now;
                    OutBillMasterRepository.Add(outBillMaster);
                    outBillMasterStr += master.BillNo + ",";
                }
                if (outBillMasterStr != string.Empty)
                {
                    OutBillDetail[] outBillDetailList = OutBillMasterDownService.GetOutBillDetail(outBillMasterStr);
                    foreach (var detail in outBillDetailList)
                    {
                        var outBillDetail = new OutBillDetail();
                        outBillDetail.BillNo = detail.BillNo;
                        outBillDetail.ProductCode = detail.ProductCode;
                        outBillDetail.UnitCode = detail.UnitCode;
                        outBillDetail.Price = detail.Price;
                        outBillDetail.BillQuantity = detail.BillQuantity;
                        outBillDetail.AllotQuantity = detail.AllotQuantity;
                        outBillDetail.RealQuantity = detail.RealQuantity;
                        outBillDetail.Description = detail.Description;
                        OutBillDetailRepository.Add(outBillDetail);
                    }
                }
                OutBillMasterRepository.SaveChanges();
                result = true;
            }
            catch (Exception e)
            {
                errorInfo = "出错，原因：" + e.Message;
            }
            return result;
        }

        public System.Data.DataTable GetStockOut(int page, int rows, string BillNo)
        {
            IQueryable<OutBillMaster> OutBillMasterQuery = OutBillMasterRepository.GetQueryable().Where(o => o.Status != "6");

            var outBillMaster = OutBillMasterQuery.Where(i => i.BillNo.Contains(BillNo)).OrderBy(i => i.BillNo).Select(s => new
            {
                s.BillNo,
                s.Warehouse.WarehouseName,
                s.BillType.BillTypeName,
                //BillDate = s.BillDate.ToString("yyyy-MM-dd hh:mm:ss"),
                OperatePersonName = s.OperatePerson.EmployeeName,
                //s.OperatePersonID,
                //Status = WhatStatus(s.Status),
                s.Status,
                VerifyPersonName = s.VerifyPersonID == null ? string.Empty : s.VerifyPerson.EmployeeName,
                //VerifyDate = (s.VerifyDate == null ? string.Empty : ((DateTime)s.VerifyDate).ToString("yyyy-MM-dd hh:mm:ss")),
                Description = s.Description,
                //UpdateTime = s.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss"),
                s.TargetCellCode
            });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("出库单号", typeof(string));
            dt.Columns.Add("仓库名称", typeof(string));
            dt.Columns.Add("订单类型", typeof(string));
            dt.Columns.Add("操作员", typeof(string));
            dt.Columns.Add("审核人", typeof(string));
            dt.Columns.Add("处理状态", typeof(string));
            //dt.Columns.Add("BillDate", typeof(string));
            //dt.Columns.Add("VerifyDate", typeof(string));
            //dt.Columns.Add("UpdateTime", typeof(string));
            dt.Columns.Add("备注", typeof(string));
            dt.Columns.Add("目标货位编码", typeof(string));

            foreach (var o in outBillMaster)
            {
                dt.Rows.Add
                    (
                          o.BillNo
                        , o.WarehouseName
                        , o.BillTypeName
                        , o.OperatePersonName
                        , o.VerifyPersonName
                        , o.Status
                    //, o.BillDate
                    //, o.VerifyDate
                    //, o.UpdateTime
                        , o.Description
                        , o.TargetCellCode
                    );
            }
            return dt;
        }


        #region  出库单上报
        public bool uploadOutBill()
        {
            try
            {
                DataSet ds = Insert();
                if (ds.Tables["WMS_OUT_BILLMASTER"].Rows.Count > 0)
                {
                    upload.InsertOutMasterBill(ds);
                }
                if (ds.Tables["WMS_OUT_BILLDETAIL"].Rows.Count > 0)
                {
                    upload.InsertOutDetailBill(ds);
                }
                if (ds.Tables["WMS_OUT_BILLALLOT"].Rows.Count > 0)
                {
                    upload.InsertOutBusiBill(ds);
                }
                return true;
            }
            catch { return false; }
        }
        #endregion

        #region 插入数据到虚拟表
        public DataSet Insert()
        {
            IQueryable<OutBillMaster> outBillMaster = OutBillMasterRepository.GetQueryable();
            IQueryable<OutBillAllot> outBillAllot = OutBillAllotRepository.GetQueryable();
            IQueryable<OutBillDetail> outBillDetail = OutBillDetailRepository.GetQueryable();
            IQueryable<UnitList> unitCode = UnitListRespository.GetQueryable();
            var outBillAllots = outBillAllot.Join(unitCode, s => s.UnitCode, p => p.UnitCode01, (s, p) => new { outBillAllot = s, unitCode = p }).ToArray();
            var outBillDetails = outBillDetail.Join(unitCode, s => s.UnitCode, p => p.UnitCode01, (s, p) => new { outBillDetail = s, unitCode = p }).ToArray();
            var outBillMasterQuery = outBillMaster.ToArray().Where(i => i.Status == "6").Select(i => new
            {
                STORE_BILL_ID = i.BillNo,
                RELATE_BUSI_BILL_NUM = outBillAllot.Count(a => a.BillNo == i.BillNo),
                DIST_CTR_CODE = i.WarehouseCode,
                QUANTITY_SUM = outBillAllots.Where(a => a.outBillAllot.BillNo == i.BillNo).Sum(a => a.outBillAllot.AllotQuantity / (a.unitCode.Quantity02*a.unitCode.Quantity03)),
                AMOUNT_SUM = outBillDetails.Where(d => d.outBillDetail.BillNo == i.BillNo).Sum(d => d.outBillDetail.Price * d.outBillDetail.AllotQuantity / (d.unitCode.Quantity02 * d.unitCode.Quantity03)),
                DETAIL_NUM = outBillDetail.Count(d => d.BillNo == i.BillNo),
                personCode = i.VerifyPerson,
                personDate = i.VerifyDate,
                operater = i.OperatePerson,
                operateDate = Convert.ToDateTime(i.BillDate).ToString("yyyyMMdd"),
                BILL_TYPE = i.BillTypeCode
            });
            DataSet ds = this.GenerateEmptyTables();
            foreach (var p in outBillMasterQuery)
            {
                DataRow inbrddr = ds.Tables["WMS_OUT_BILLMASTER"].NewRow();
                inbrddr["STORE_BILL_ID"] = p.STORE_BILL_ID;
                inbrddr["RELATE_BUSI_BILL_NUM"] = p.RELATE_BUSI_BILL_NUM;
                inbrddr["DIST_CTR_CODE"] = p.DIST_CTR_CODE;
                inbrddr["AREA_TYPE"] = "0901";
                inbrddr["QUANTITY_SUM"] =-p.QUANTITY_SUM;
                inbrddr["AMOUNT_SUM"] = p.AMOUNT_SUM;
                inbrddr["DETAIL_NUM"] = p.DETAIL_NUM;
                inbrddr["CREATOR_CODE"] ="";
                inbrddr["CREATE_DATE"] = Convert.ToDateTime(p.personDate).ToString("yyyyMMdd");
                inbrddr["AUDITOR_CODE"] ="";
                inbrddr["AUDIT_DATE"] = Convert.ToDateTime(p.personDate).ToString("yyyyMMdd");
                inbrddr["ASSIGNER_CODE"] ="";
                inbrddr["ASSIGN_DATE"] = Convert.ToDateTime(p.personDate).ToString("yyyyMMdd");
                inbrddr["AFFIRM_CODE"] = "";
                inbrddr["AFFIRM_DATE"] = Convert.ToDateTime(p.personDate).ToString("yyyyMMdd");
                inbrddr["IN_OUT_TYPE"] = "1203";
                inbrddr["BILL_TYPE"] = p.BILL_TYPE;
                inbrddr["BILL_STATUS"] = "99";
                inbrddr["DISUSE_STATUS"] = "0";
                inbrddr["IS_IMPORT"] = "0";
                ds.Tables["WMS_OUT_BILLMASTER"].Rows.Add(inbrddr);
            }
            var outBillDetailQuery = outBillDetails.Where(i => i.outBillDetail.OutBillMaster.Status == "6").Select(i => new
            {
                STORE_BILL_DETAIL_ID = i.outBillDetail.ID,
                STORE_BILL_ID = i.outBillDetail.BillNo,
                BRAND_CODE = i.outBillDetail.ProductCode,
                BRAND_NAME = i.outBillDetail.Product.ProductName,
                QUANTITY = i.outBillDetail.BillQuantity /(i.unitCode.Quantity02*i.unitCode.Quantity03)
            });
            foreach (var p in outBillDetailQuery)
            {
                DataRow inbrddrDetail = ds.Tables["WMS_OUT_BILLDETAIL"].NewRow();
                inbrddrDetail["STORE_BILL_DETAIL_ID"] = p.STORE_BILL_DETAIL_ID;
                inbrddrDetail["STORE_BILL_ID"] = p.STORE_BILL_ID;
                inbrddrDetail["BRAND_CODE"] = p.BRAND_CODE;
                inbrddrDetail["BRAND_NAME"] = p.BRAND_NAME;
                inbrddrDetail["QUANTITY"] =-p.QUANTITY;
                inbrddrDetail["IS_IMPORT"] = "0";
                ds.Tables["WMS_OUT_BILLDETAIL"].Rows.Add(inbrddrDetail);
            }
            var outBillAllotQuery = outBillAllots.Where(i => i.outBillAllot.OutBillMaster.Status == "6").Select(i => new
            {
                BUSI_ACT_ID = i.outBillAllot.ID,
                BUSI_BILL_DETAIL_ID = i.outBillAllot.OutBillDetailId,
                BUSI_BILL_ID = i.outBillAllot.BillNo,
                BRAND_CODE = i.outBillAllot.ProductCode,
                BRAND_NAME = i.outBillAllot.Product.ProductName,
                QUANTITY = i.outBillAllot.AllotQuantity / (i.unitCode.Quantity02 * i.unitCode.Quantity03),
                DIST_CTR_CODE = i.outBillAllot.OutBillMaster.WarehouseCode,
                STORE_PLACE_CODE = i.outBillAllot.Storage.CellCode,
                STORE_PLACE_NAME = i.outBillAllot.Storage.Cell.CellName,
                UPDATE_CODE = i.outBillAllot.Operator,
                //BEGIN_STOCK_QUANTITY =StorageRepository.GetQueryable().Where(s=>s.ProductCode==i.ProductCode).Sum(s=>s.Quantity/200)-i.AllotQuantity,
                //END_STOCK_QUANTITY = i.AllotQuantity,
                BILL_TYPE=i.outBillAllot.OutBillMaster.BillTypeCode
            });
            foreach (var p in outBillAllotQuery)
            {
                DataRow inbrddrAllot = ds.Tables["WMS_OUT_BILLALLOT"].NewRow();
                inbrddrAllot["BUSI_ACT_ID"] = p.BUSI_ACT_ID;
                inbrddrAllot["BUSI_BILL_DETAIL_ID"] = p.BUSI_BILL_DETAIL_ID;
                inbrddrAllot["BUSI_BILL_ID"] = p.BUSI_BILL_ID;
                inbrddrAllot["BRAND_CODE"] = p.BRAND_CODE;
                inbrddrAllot["BRAND_NAME"] = p.BRAND_NAME;
                inbrddrAllot["QUANTITY"] = -p.QUANTITY;
                inbrddrAllot["DIST_CTR_CODE"] = p.DIST_CTR_CODE;
                inbrddrAllot["ORG_CODE"] = "01";
                inbrddrAllot["STORE_ROOM_CODE"] = "001";
                inbrddrAllot["STORE_PLACE_CODE"] = "10002";
                inbrddrAllot["TARGET_NAME"] = p.STORE_PLACE_NAME;
                inbrddrAllot["IN_OUT_TYPE"] = "1203";
                inbrddrAllot["BILL_TYPE"] = p.BILL_TYPE;
                inbrddrAllot["BEGIN_STOCK_QUANTITY"] = 0;
                inbrddrAllot["END_STOCK_QUANTITY"] = 0;
                inbrddrAllot["DISUSE_STATUS"] = "0";
                inbrddrAllot["RECKON_STATUS"] = "1";
                inbrddrAllot["RECKON_DATE"] = DateTime.Now.ToString("yyyyMMdd");
                inbrddrAllot["UPDATE_CODE"] ="050000";
                inbrddrAllot["UPDATE_DATE"] = DateTime.Now.ToString("yyyyMMdd");
                inbrddrAllot["IS_IMPORT"] = "0";
                ds.Tables["WMS_OUT_BILLALLOT"].Rows.Add(inbrddrAllot);
            }
            return ds;
        }
        #endregion

        #region 构建出库单据表虚拟表
        private DataSet GenerateEmptyTables()
        {
            DataSet ds = new DataSet();
            DataTable mastertable = ds.Tables.Add("WMS_OUT_BILLMASTER");
            mastertable.Columns.Add("STORE_BILL_ID");
            mastertable.Columns.Add("RELATE_BUSI_BILL_NUM");
            mastertable.Columns.Add("DIST_CTR_CODE");
            mastertable.Columns.Add("AREA_TYPE");
            mastertable.Columns.Add("QUANTITY_SUM");
            mastertable.Columns.Add("AMOUNT_SUM");
            mastertable.Columns.Add("DETAIL_NUM");
            mastertable.Columns.Add("CREATOR_CODE");
            mastertable.Columns.Add("CREATE_DATE");
            mastertable.Columns.Add("AUDITOR_CODE");
            mastertable.Columns.Add("AUDIT_DATE");
            mastertable.Columns.Add("ASSIGNER_CODE");
            mastertable.Columns.Add("ASSIGN_DATE");
            mastertable.Columns.Add("AFFIRM_CODE");
            mastertable.Columns.Add("AFFIRM_DATE");
            mastertable.Columns.Add("IN_OUT_TYPE");
            mastertable.Columns.Add("BILL_TYPE");
            mastertable.Columns.Add("BILL_STATUS");
            mastertable.Columns.Add("DISUSE_STATUS");
            mastertable.Columns.Add("IS_IMPORT");

            DataTable detailtable = ds.Tables.Add("WMS_OUT_BILLDETAIL");
            detailtable.Columns.Add("STORE_BILL_DETAIL_ID");
            detailtable.Columns.Add("STORE_BILL_ID");
            detailtable.Columns.Add("BRAND_CODE");
            detailtable.Columns.Add("BRAND_NAME");
            detailtable.Columns.Add("QUANTITY");
            detailtable.Columns.Add("IS_IMPORT");


            DataTable allottable = ds.Tables.Add("WMS_OUT_BILLALLOT");
            allottable.Columns.Add("BUSI_ACT_ID");
            allottable.Columns.Add("BUSI_BILL_DETAIL_ID");
            allottable.Columns.Add("BUSI_BILL_ID");
            allottable.Columns.Add("BRAND_CODE");
            allottable.Columns.Add("BRAND_NAME");
            allottable.Columns.Add("QUANTITY");
            allottable.Columns.Add("DIST_CTR_CODE");
            allottable.Columns.Add("ORG_CODE");
            allottable.Columns.Add("STORE_ROOM_CODE");
            allottable.Columns.Add("STORE_PLACE_CODE");
            allottable.Columns.Add("TARGET_NAME");
            allottable.Columns.Add("IN_OUT_TYPE");
            allottable.Columns.Add("BILL_TYPE");
            allottable.Columns.Add("BEGIN_STOCK_QUANTITY");
            allottable.Columns.Add("END_STOCK_QUANTITY");
            allottable.Columns.Add("DISUSE_STATUS");
            allottable.Columns.Add("RECKON_STATUS");
            allottable.Columns.Add("RECKON_DATE");
            allottable.Columns.Add("UPDATE_CODE");
            allottable.Columns.Add("UPDATE_DATE");
            allottable.Columns.Add("IS_IMPORT");

            return ds;
        }
        #endregion
    }
}
