using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.SignalR;
using THOK.Wms.SignalR.Common;
using System.Transactions;
using THOK.Wms.Download.Interfaces;
using System.Data;
using THOK.WMS.Upload.Bll;

namespace THOK.Wms.Bll.Service
{
    public class InBillMasterService : ServiceBase<InBillMaster>, IInBillMasterService
    {
        [Dependency]
        public IInBillMasterRepository InBillMasterRepository { get; set; }
        [Dependency]
        public IBillTypeRepository BillTypeRepository { get; set; }
        [Dependency]
        public IWarehouseRepository WarehouseRepository { get; set; }
        [Dependency]
        public IEmployeeRepository EmployeeRepository { get; set; }
        [Dependency]
        public IInBillDetailRepository InBillDetailRepository { get; set; }
        [Dependency]
        public IInBillAllotRepository InBillAllotRepository { get; set; }
        [Dependency]
        public IStorageLocker Locker { get; set; }
        [Dependency]
        public IInBillMasterDownService InBillMasterDownService { get; set; }
        [Dependency]
        public IStorageRepository StorageRepository { get; set; }
        [Dependency]
        public ICellRepository CellRepository { get; set; }
        [Dependency]
        public IUnitListRepository UnitListRespository { get; set; }

        UploadBll upload = new UploadBll();

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public string resultStr = "";//错误提示信息

        #region//入库主单增、删、改、查、生成单号方法
        /// <summary>
        /// 判断状态
        /// </summary>
        /// <param name="status">数据库查询出来的状态</param>
        /// <returns>返回的状态</returns>
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

        /// <summary>
        /// 入库单查询
        /// </summary>
        /// <param name="page">页面传回的分页参数，页数</param>
        /// <param name="rows">页面传回的分页参数，行数</param>
        /// <param name="BillNo">入库单号</param>
        /// <param name="WareHouseCode">仓库编码</param>
        /// <param name="BeginDate">开始日期</param>
        /// <param name="EndDate">结束日期</param>
        /// <param name="OperatePersonCode">操作员编码</param>
        /// <param name="CheckPersonCode">审核人编码</param>
        /// <param name="Status">操作状态</param>
        /// <param name="IsActive">是否可用</param>
        /// <returns></returns>
        public object GetDetails(int page, int rows, string BillNo, string WareHouseCode, string BeginDate, string EndDate, string OperatePersonCode, string CheckPersonCode, string Status, string IsActive)
        {
            IQueryable<InBillMaster> inBillMasterQuery = InBillMasterRepository.GetQueryable();
            var inBillMaster = inBillMasterQuery.Where(i => i.BillNo.Contains(BillNo)
                    && i.Status != "6"
                    && i.WarehouseCode.Contains(WareHouseCode)
                    && i.OperatePerson.EmployeeCode.Contains(OperatePersonCode)
                    && i.Status.Contains(Status))
                    .OrderByDescending(t => t.BillDate)
                    .OrderByDescending(t => t.BillNo)
                    .Select(i => i);

            if (!BeginDate.Equals(string.Empty))
            {
                DateTime begin = Convert.ToDateTime(BeginDate);
                inBillMaster = inBillMaster.Where(i => i.BillDate >= begin);
            }

            if (!EndDate.Equals(string.Empty))
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                inBillMaster = inBillMaster.Where(i => i.BillDate <= end);
            }

            if (!CheckPersonCode.Equals(string.Empty))
            {
                inBillMaster = inBillMaster.Where(i => i.VerifyPerson.EmployeeCode == CheckPersonCode);
            }
            int total = inBillMaster.Count();
            inBillMaster = inBillMaster.Skip((page - 1) * rows).Take(rows);

            var tmp = inBillMaster.ToArray().AsEnumerable().Select(i => new
            {
                i.BillNo,
                BillDate = i.BillDate.ToString("yyyy-MM-dd HH:mm:ss"),
                i.OperatePersonID,
                i.WarehouseCode,
                i.BillTypeCode,
                i.BillType.BillTypeName,
                i.Warehouse.WarehouseName,
                OperatePersonCode = i.OperatePerson.EmployeeCode,
                OperatePersonName = i.OperatePerson.EmployeeName,
                VerifyPersonID = i.VerifyPersonID == null ? string.Empty : i.VerifyPerson.EmployeeCode,
                VerifyPersonName = i.VerifyPersonID == null ? string.Empty : i.VerifyPerson.EmployeeName,
                VerifyDate = (i.VerifyDate == null ? "" : ((DateTime)i.VerifyDate).ToString("yyyy-MM-dd HH:mm:ss")),
                Status = WhatStatus(i.Status),
                IsActive = i.IsActive == "1" ? "可用" : "不可用",
                Description = i.Description,
                UpdateTime = i.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                i.TargetCellCode
            });
            return new { total, rows = tmp.ToArray() };
        }

        /// <summary>
        /// 入库单新增
        /// </summary>
        /// <param name="inBillMaster">入库主单</param>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public bool Add(InBillMaster inBillMaster, string userName, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var ibm = new InBillMaster();
            var employee = EmployeeRepository.GetQueryable().FirstOrDefault(i => i.UserName == userName);
            if (employee != null)
            {
                try
                {
                    ibm.BillNo = inBillMaster.BillNo;
                    ibm.BillDate = inBillMaster.BillDate;
                    ibm.BillTypeCode = inBillMaster.BillTypeCode;
                    ibm.WarehouseCode = inBillMaster.WarehouseCode;
                    ibm.OperatePersonID = employee.ID;
                    ibm.Status = "1";
                    ibm.VerifyPersonID = inBillMaster.VerifyPersonID;
                    ibm.VerifyDate = inBillMaster.VerifyDate;
                    ibm.Description = inBillMaster.Description;
                    //ibm.IsActive = inBillMaster.IsActive;
                    ibm.IsActive = "1";
                    ibm.UpdateTime = DateTime.Now;
                    ibm.TargetCellCode = inBillMaster.TargetCellCode;

                    InBillMasterRepository.Add(ibm);
                    InBillMasterRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    strResult = "新增失败，原因：" + ex.Message;
                }
            }
            else
            {
                strResult = "找不到当前登陆用户！请重新登陆！";
            }
            return result;
        }

        /// <summary>
        /// 入库单删除
        /// </summary>
        /// <param name="BillNo">入库单号</param>
        /// <returns></returns>
        public bool Delete(string BillNo, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var ibm = InBillMasterRepository.GetQueryable().FirstOrDefault(i => i.BillNo == BillNo && i.Status == "1");
            if (ibm != null)
            {
                try
                {
                    Del(InBillDetailRepository, ibm.InBillDetails);
                    InBillMasterRepository.Delete(ibm);
                    InBillMasterRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    strResult = "删除失败，原因：" + ex.Message;
                }
            }
            else
            {
                strResult = "删除失败！未找到当前需要删除的数据！";
            }
            return result;
        }

        /// <summary>
        /// 入库单修改
        /// </summary>
        /// <param name="inBillMaster">入库主单</param>
        /// <returns></returns>
        public bool Save(InBillMaster inBillMaster, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var ibm = InBillMasterRepository.GetQueryable().FirstOrDefault(i => i.BillNo == inBillMaster.BillNo && i.Status == "1");
            if (ibm != null)
            {
                try
                {
                    ibm.BillDate = inBillMaster.BillDate;
                    ibm.BillTypeCode = inBillMaster.BillTypeCode;
                    ibm.WarehouseCode = inBillMaster.WarehouseCode;
                    ibm.OperatePersonID = inBillMaster.OperatePersonID;
                    ibm.Status = "1";
                    ibm.VerifyPersonID = inBillMaster.VerifyPersonID;
                    ibm.VerifyDate = inBillMaster.VerifyDate;
                    ibm.Description = inBillMaster.Description;
                    //ibm.IsActive = inBillMaster.IsActive;
                    ibm.IsActive = "1";
                    ibm.UpdateTime = DateTime.Now;
                    ibm.TargetCellCode = inBillMaster.TargetCellCode;

                    InBillMasterRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    strResult = "保存失败，原因：" + ex.Message;
                }
            }
            else
            {
                strResult = "保存失败，未找到该条数据！";
            }
            return result;
        }

        /// <summary>
        /// 生成入库单号
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public object GenInBillNo(string userName)
        {
            IQueryable<InBillMaster> inBillMasterQuery = InBillMasterRepository.GetQueryable();
            string sysTime = System.DateTime.Now.ToString("yyMMdd");
            string billNo = "";
            var employee = EmployeeRepository.GetQueryable().FirstOrDefault(i => i.UserName == userName);
            var inBillMaster = inBillMasterQuery.Where(i => i.BillNo.Contains(sysTime)).ToArray().OrderBy(i => i.BillNo).Select(i => new { i.BillNo }.BillNo);
            if (inBillMaster.Count() == 0)
            {
                billNo = System.DateTime.Now.ToString("yyMMdd") + "0001" + "IN";
            }
            else
            {
                string billNoStr = inBillMaster.Last(b => b.Contains(sysTime));
                int i = Convert.ToInt32(billNoStr.ToString().Substring(6, 4));
                i++;
                string newcode = i.ToString();
                for (int j = 0; j < 4 - i.ToString().Length; j++)
                {
                    newcode = "0" + newcode;
                }
                billNo = System.DateTime.Now.ToString("yyMMdd") + newcode + "IN";
            }
            var findBillInfo = new
            {
                BillNo = billNo,
                billNoDate = DateTime.Now.ToString("yyyy-MM-dd"),
                employeeID = employee == null ? "" : employee.ID.ToString(),
                employeeCode = employee == null ? "" : employee.EmployeeCode.ToString(),
                employeeName = employee == null ? "" : employee.EmployeeName.ToString()
            };
            return findBillInfo;
        }
        #endregion

        #region//入库主单审核、反审、结单、非货位管理入库方法
        /// <summary>
        /// 入库单审核
        /// </summary>
        /// <param name="BillNo">入库单号</param>
        /// <param name="userName">用户名</param>
        /// <param name="strResult">操作提示信息</param>
        /// <returns></returns>
        public bool Audit(string BillNo, string userName, out string strResult)
        {
            bool result = false;
            strResult = string.Empty;
            var ibm = InBillMasterRepository.GetQueryable().FirstOrDefault(i => i.BillNo == BillNo && i.Status == "1");
            var employee = EmployeeRepository.GetQueryable().FirstOrDefault(i => i.UserName == userName);
            if (ibm != null)
            {
                if (string.IsNullOrEmpty(ibm.TargetCellCode))//判断入库主单是否指定货位
                {
                    ibm.Status = "2";
                    ibm.VerifyDate = DateTime.Now;
                    ibm.UpdateTime = DateTime.Now;
                    ibm.VerifyPersonID = employee.ID;
                    InBillMasterRepository.SaveChanges();
                    result = true;
                }
                else//如果入库主单指定了货位那么就进行入库分配
                {
                    result = InAllot(ibm, employee.ID);
                    strResult = resultStr;
                }
            }
            return result;
        }

        /// <summary>
        /// 入库分配
        /// </summary>
        /// <param name="inBillMaster">入库主单</param>
        /// <returns></returns>
        public bool InAllot(InBillMaster inBillMaster, Guid employeeId)
        {
            try
            {
                var inBillDetails = inBillMaster.InBillDetails.ToArray();
                var cell = CellRepository.GetQueryable().FirstOrDefault(c => c.CellCode == inBillMaster.TargetCellCode);
                //入库单入库
                inBillMaster.InBillDetails.AsParallel().ForAll(
               (Action<InBillDetail>)delegate(InBillDetail i)
               {
                   if (i.BillQuantity - i.AllotQuantity > 0)
                   {
                       Storage inStorage = null;
                       lock (cell)
                       {
                           inStorage = Locker.LockStorage(cell);
                           if (inStorage == null)
                           {
                               throw new Exception("锁定储位失败，储位其他人正在操作，无法分配请稍候重试！");
                           }
                           inStorage.LockTag = inBillMaster.BillNo;
                       }
                       if (inStorage.Quantity == 0
                           && inStorage.InFrozenQuantity == 0)
                       {
                           decimal allotQuantity = i.BillQuantity;
                           i.AllotQuantity += allotQuantity;
                           i.RealQuantity += allotQuantity;
                           inStorage.ProductCode = i.ProductCode;
                           inStorage.Quantity += allotQuantity;
                           inStorage.LockTag = string.Empty;

                           var billAllot = new InBillAllot()
                           {
                               BillNo = inBillMaster.BillNo,
                               InBillDetailId = i.ID,
                               ProductCode = i.ProductCode,
                               CellCode = inStorage.CellCode,
                               StorageCode = inStorage.StorageCode,
                               UnitCode = i.UnitCode,
                               AllotQuantity = allotQuantity,
                               RealQuantity = allotQuantity,
                               Status = "2"
                           };

                           lock (inBillMaster.InBillAllots)
                           {
                               inBillMaster.InBillAllots.Add(billAllot);
                           }
                       }
                       else
                       {
                           throw new Exception("储位数量不等于0，无法分配请稍候重试！");
                       }
                   }
               });
                //入库结单
                inBillMaster.Status = "6";
                inBillMaster.VerifyDate = DateTime.Now;
                inBillMaster.VerifyPersonID = employeeId;
                inBillMaster.UpdateTime = DateTime.Now;
                InBillMasterRepository.SaveChanges();
                return true;
            }
            catch (AggregateException ex)
            {
                resultStr = "审核失败，详情：" + ex.InnerExceptions.Select(i => i.Message).Aggregate((m, n) => m + n);
                return false;
            }
        }

        /// <summary>
        /// 入库单反审
        /// </summary>
        /// <param name="BillNo">入库单号</param>
        /// <returns></returns>
        public bool AntiTrial(string BillNo, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var ibm = InBillMasterRepository.GetQueryable().FirstOrDefault(i => i.BillNo == BillNo && i.Status == "2");
            if (ibm != null)
            {
                try
                {
                    ibm.Status = "1";
                    ibm.VerifyDate = null;
                    ibm.UpdateTime = DateTime.Now;
                    ibm.VerifyPersonID = null;
                    InBillMasterRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    strResult = "反审失败，原因：" + ex.Message;
                }
            }
            else
            {
                strResult = "反审失败，未找到该条数据！";
            }
            return result;
        }

        /// <summary>
        /// 入库单结单
        /// </summary>
        /// <param name="BillNo">入库单号</param>
        /// <param name="strResult">提示信息</param>
        /// <returns></returns>
        public bool Settle(string BillNo, out string strResult)
        {
            bool result = false;
            strResult = string.Empty;
            var ibm = InBillMasterRepository.GetQueryable().FirstOrDefault(i => i.BillNo == BillNo);
            if (ibm != null && ibm.Status == "5")
            {
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        //修改分配入库冻结量
                        var inAllot = InBillAllotRepository.GetQueryable()
                                                           .Where(o => o.BillNo == ibm.BillNo
                                                               && o.Status != "2")
                                                           .ToArray();
                        var storages = inAllot.Select(i => i.Storage).ToArray();

                        if (!Locker.Lock(storages))
                        {
                            strResult = "锁定储位失败，储位其他人正在操作，无法结单请稍候重试！";
                            return false;
                        }

                        inAllot.AsParallel().ForAll(
                            (Action<InBillAllot>)delegate(InBillAllot i)
                            {
                                if (i.Storage.ProductCode == i.ProductCode
                                    && i.Storage.InFrozenQuantity >= i.AllotQuantity)
                                {
                                    i.Storage.InFrozenQuantity -= i.AllotQuantity;
                                    i.Storage.LockTag = string.Empty;
                                }
                                else
                                {
                                    throw new Exception("储位的卷烟或入库冻结量与当前分配不符，信息可能被异常修改，不能结单！");
                                }
                            }
                        );

                        ibm.Status = "6";
                        ibm.UpdateTime = DateTime.Now;
                        InBillMasterRepository.SaveChanges();
                        scope.Complete();
                        result = true;
                    }
                    catch (Exception e)
                    {
                        strResult = "入库单结单出错！原因：" + e.Message;
                    }
                }
            }
            return result;
        }
        #endregion

        #region//入库主单查询订单类型、查询仓库信息方法
        /// <summary>
        /// 根据条件查询订单类型
        /// </summary>
        /// <param name="BillClass">订单类别</param>
        /// <param name="IsActive">是否可用</param>
        /// <returns></returns>
        public object GetBillTypeDetail(string BillClass, string IsActive)
        {
            IQueryable<BillType> billtypeQuery = BillTypeRepository.GetQueryable();
            var billtype = billtypeQuery.Where(b => b.BillClass == BillClass
                && b.IsActive.Contains(IsActive)).ToArray().OrderBy(b => b.BillTypeCode).Select(b => new
            {
                b.BillTypeCode,
                b.BillTypeName,
                b.BillClass,
                b.Description,
                IsActive = b.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = b.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss")
            });
            return billtype.ToArray();
        }

        /// <summary>
        /// 根据条件查询仓库信息
        /// </summary>
        /// <param name="IsActive">是否可用</param>
        /// <returns></returns>
        public object GetWareHouseDetail(string IsActive)
        {
            IQueryable<Warehouse> wareQuery = WarehouseRepository.GetQueryable();
            var warehouse = wareQuery.Where(w => w.IsActive == IsActive).OrderBy(w => w.WarehouseCode).ToArray().Select(w => new
                {
                    w.WarehouseCode,
                    w.WarehouseName,
                    w.WarehouseType,
                    w.Description,
                    w.ShortName,
                    IsActive = w.IsActive == "1" ? "可用" : "不可用",
                    UpdateTime = w.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss")
                });
            return warehouse.ToArray();
        }
        #endregion

        public bool DownInBillMaster(string BeginDate, string EndDate, out string errorInfo)
        {
            errorInfo = string.Empty;
            bool result = false;
            string inBillStr = "";
            string inBillMasterStr = "";
            try
            {
                var inBillNos = InBillMasterRepository.GetQueryable().Where(i => i.BillNo == i.BillNo).Select(i => new { i.BillNo }).ToArray();

                for (int i = 0; i < inBillNos.Length; i++)
                {
                    inBillStr += inBillNos[i].BillNo + ",";
                }
                InBillMaster[] inBillMasterList = InBillMasterDownService.GetInBillMaster(inBillStr);
                foreach (var master in inBillMasterList)
                {
                    var inBillMaster = new InBillMaster();
                    inBillMaster.BillNo = master.BillNo;
                    inBillMaster.BillDate = master.BillDate;
                    inBillMaster.BillTypeCode = master.BillTypeCode;
                    inBillMaster.WarehouseCode = master.WarehouseCode;
                    inBillMaster.Status = "1";
                    inBillMaster.IsActive = master.IsActive;
                    inBillMaster.UpdateTime = DateTime.Now;
                    InBillMasterRepository.Add(inBillMaster);
                    inBillMasterStr += master.BillNo + ",";
                }
                if (inBillMasterStr != string.Empty)
                {
                    InBillDetail[] inBillDetailList = InBillMasterDownService.GetInBillDetail(inBillMasterStr);
                    foreach (var detail in inBillDetailList)
                    {
                        var inBillDetail = new InBillDetail();
                        inBillDetail.BillNo = detail.BillNo;
                        inBillDetail.ProductCode = detail.ProductCode;
                        inBillDetail.UnitCode = detail.UnitCode;
                        inBillDetail.Price = detail.Price;
                        inBillDetail.BillQuantity = detail.BillQuantity;
                        inBillDetail.AllotQuantity = detail.AllotQuantity;
                        inBillDetail.RealQuantity = detail.RealQuantity;
                        inBillDetail.Description = detail.Description;
                        InBillDetailRepository.Add(inBillDetail);
                    }
                }
                InBillMasterRepository.SaveChanges();
                result = true;
            }
            catch (Exception e)
            {
                errorInfo = "出错，原因：" + e.Message;
            }
            return result;
        }

        #region  入库单上报
        public bool uploadInBill()
        {
            try
            {
                DataSet ds = Insert();
                if (ds.Tables["WMS_IN_BILLMASTER"].Rows.Count > 0)
                {
                    upload.InsertInMasterBill(ds);
                }
                if (ds.Tables["WMS_IN_BILLDETAIL"].Rows.Count > 0)
                {
                    upload.InsertInDetailBill(ds);
                }
                if (ds.Tables["WMS_IN_BILLALLOT"].Rows.Count > 0)
                {
                    upload.InsertInBusiBill(ds);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 插入数据到虚拟表
        public DataSet Insert()
        {
            IQueryable<InBillMaster> inBillMaster = InBillMasterRepository.GetQueryable();
            IQueryable<InBillAllot> inBillAllot = InBillAllotRepository.GetQueryable();
            IQueryable<InBillDetail> inBillDetail = InBillDetailRepository.GetQueryable();
            IQueryable<UnitList> unitCode = UnitListRespository.GetQueryable();
            var inBillAllots = inBillAllot.Join(unitCode, s => s.UnitCode, p => p.UnitCode01, (s, p) => new { inBillAllot = s, unitCode = p }).ToArray();
            var inBillDetails = inBillDetail.Join(unitCode, s => s.UnitCode, p => p.UnitCode01, (s, p) => new { inBillDetail = s, unitCode = p }).ToArray();
            var inBillMasterQuery = inBillMaster.ToArray().Where(i => i.Status == "6").Select(i => new
            {
                STORE_BILL_ID = i.BillNo,
                RELATE_BUSI_BILL_NUM =inBillAllot.Count(a => a.BillNo == i.BillNo),
                DIST_CTR_CODE = i.WarehouseCode,
                QUANTITY_SUM = inBillAllots.Where(a => a.inBillAllot.BillNo == i.BillNo).Sum(a => a.inBillAllot.AllotQuantity / (a.unitCode.Quantity02 * a.unitCode.Quantity03)),
                AMOUNT_SUM = inBillDetails.Where(d => d.inBillDetail.BillNo == i.BillNo).Sum(d => d.inBillDetail.Price * d.inBillDetail.AllotQuantity / (d.unitCode.Quantity02 * d.unitCode.Quantity03)),
                DETAIL_NUM = inBillDetail.Count(d => d.BillNo == i.BillNo),
                personCode = i.VerifyPerson,
                personDate = i.VerifyDate,
                operater = i.OperatePerson,
                operateDate = Convert.ToDateTime(i.BillDate).ToString("yyyyMMdd"),
                BILL_TYPE = i.BillTypeCode
            });
            DataSet ds = this.GenerateEmptyTables();
            foreach (var p in inBillMasterQuery)
            {
                DataRow inbrddr = ds.Tables["WMS_IN_BILLMASTER"].NewRow();
                inbrddr["STORE_BILL_ID"] = p.STORE_BILL_ID;
                inbrddr["RELATE_BUSI_BILL_NUM"] = p.RELATE_BUSI_BILL_NUM;
                inbrddr["DIST_CTR_CODE"] = p.DIST_CTR_CODE;
                inbrddr["AREA_TYPE"] = "0901";
                inbrddr["QUANTITY_SUM"] = p.QUANTITY_SUM;
                inbrddr["AMOUNT_SUM"] = p.AMOUNT_SUM;
                inbrddr["DETAIL_NUM"] = p.DETAIL_NUM;
                inbrddr["CREATOR_CODE"] = "";
                inbrddr["CREATE_DATE"] = Convert.ToDateTime(p.personDate).ToString("yyyyMMdd");
                inbrddr["AUDITOR_CODE"] ="";
                inbrddr["AUDIT_DATE"] = Convert.ToDateTime(p.personDate).ToString("yyyyMMdd");
                inbrddr["ASSIGNER_CODE"] ="";
                inbrddr["ASSIGN_DATE"] = Convert.ToDateTime(p.personDate).ToString("yyyyMMdd");
                inbrddr["AFFIRM_CODE"] = "";
                inbrddr["AFFIRM_DATE"] = Convert.ToDateTime(p.personDate).ToString("yyyyMMdd");
                inbrddr["IN_OUT_TYPE"] = "1202";
                inbrddr["BILL_TYPE"] = p.BILL_TYPE;
                inbrddr["BILL_STATUS"] = "99";
                inbrddr["DISUSE_STATUS"] = "0";
                inbrddr["IS_IMPORT"] = "0";
                ds.Tables["WMS_IN_BILLMASTER"].Rows.Add(inbrddr);
            }
            var inBillDetailQuery = inBillDetails.ToArray().Where(i => i.inBillDetail.InBillMaster.Status == "6").Select(i => new
            {
                STORE_BILL_DETAIL_ID = i.inBillDetail.ID,
                STORE_BILL_ID = i.inBillDetail.BillNo,
                BRAND_CODE = i.inBillDetail.ProductCode,
                BRAND_NAME = i.inBillDetail.Product.ProductName,
                QUANTITY =i.inBillDetail.BillQuantity /(i.unitCode.Quantity02*i.unitCode.Quantity03)
            });
            foreach (var p in inBillDetailQuery)
            {
                DataRow inbrddrDetail = ds.Tables["WMS_IN_BILLDETAIL"].NewRow();
                inbrddrDetail["STORE_BILL_DETAIL_ID"] = p.STORE_BILL_DETAIL_ID;
                inbrddrDetail["STORE_BILL_ID"] = p.STORE_BILL_ID;
                inbrddrDetail["BRAND_CODE"] = p.BRAND_CODE;
                inbrddrDetail["BRAND_NAME"] = p.BRAND_NAME;
                inbrddrDetail["QUANTITY"] = p.QUANTITY;
                inbrddrDetail["IS_IMPORT"] = "0";
                ds.Tables["WMS_IN_BILLDETAIL"].Rows.Add(inbrddrDetail);
            }
            var inBillAllotQuery = inBillAllots.ToArray().Where(i => i.inBillAllot.InBillMaster.Status == "6").Select(i => new
            {
                BUSI_ACT_ID = i.inBillAllot.ID,
                BUSI_BILL_DETAIL_ID = i.inBillAllot.InBillDetailId,
                BUSI_BILL_ID = i.inBillAllot.BillNo,
                BRAND_CODE = i.inBillAllot.ProductCode,
                BRAND_NAME = i.inBillAllot.Product.ProductName,
                QUANTITY = i.inBillAllot.AllotQuantity / (i.unitCode.Quantity02*i.unitCode.Quantity03),
                DIST_CTR_CODE = i.inBillAllot.InBillMaster.WarehouseCode,
                STORE_PLACE_CODE = i.inBillAllot.Storage.CellCode,
                STORE_PLACE_NAME=i.inBillAllot.Storage.Cell.CellName,
                UPDATE_CODE = i.inBillAllot.Operator,
                BILL_TYPE = i.inBillAllot.InBillMaster.BillTypeCode
                //BEGIN_STOCK_QUANTITY = StorageRepository.GetQueryable().Where(s => s.ProductCode == i.ProductCode).Sum(s => s.Quantity / 200) + i.AllotQuantity,
                //END_STOCK_QUANTITY = i.AllotQuantity,
            });
            foreach (var p in inBillAllotQuery)
            {
                DataRow inbrddrAllot = ds.Tables["WMS_IN_BILLALLOT"].NewRow();
                inbrddrAllot["BUSI_ACT_ID"] = p.BUSI_ACT_ID;
                inbrddrAllot["BUSI_BILL_DETAIL_ID"] = p.BUSI_BILL_DETAIL_ID;
                inbrddrAllot["BUSI_BILL_ID"] = p.BUSI_BILL_ID;
                inbrddrAllot["BRAND_CODE"] = p.BRAND_CODE;
                inbrddrAllot["BRAND_NAME"] = p.BRAND_NAME;
                inbrddrAllot["QUANTITY"] = p.QUANTITY;
                inbrddrAllot["DIST_CTR_CODE"] = p.DIST_CTR_CODE;
                inbrddrAllot["ORG_CODE"] = "01";
                inbrddrAllot["STORE_ROOM_CODE"] = "001";
                inbrddrAllot["STORE_PLACE_CODE"] = "10002";
                inbrddrAllot["TARGET_NAME"] =p.STORE_PLACE_NAME;
                inbrddrAllot["IN_OUT_TYPE"] = "1202";
                inbrddrAllot["BILL_TYPE"] = p.BILL_TYPE;
                inbrddrAllot["BEGIN_STOCK_QUANTITY"] = 0;
                inbrddrAllot["END_STOCK_QUANTITY"] = 0;
                inbrddrAllot["DISUSE_STATUS"] = "0";
                inbrddrAllot["RECKON_STATUS"] = "1";
                inbrddrAllot["RECKON_DATE"] = DateTime.Now.ToString("yyyyMMdd");
                inbrddrAllot["UPDATE_CODE"] = "050000";
                inbrddrAllot["UPDATE_DATE"] = DateTime.Now.ToString("yyyyMMdd");
                inbrddrAllot["IS_IMPORT"] = "0";
                ds.Tables["WMS_IN_BILLALLOT"].Rows.Add(inbrddrAllot);
            }
            return ds;
        }
        #endregion

        #region 构建入库单据表虚拟表
        private DataSet GenerateEmptyTables()
        {
            DataSet ds = new DataSet();
            DataTable mastertable = ds.Tables.Add("WMS_IN_BILLMASTER");
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

            DataTable detailtable = ds.Tables.Add("WMS_IN_BILLDETAIL");
            detailtable.Columns.Add("STORE_BILL_DETAIL_ID");
            detailtable.Columns.Add("STORE_BILL_ID");
            detailtable.Columns.Add("BRAND_CODE");
            detailtable.Columns.Add("BRAND_NAME");
            detailtable.Columns.Add("QUANTITY");
            detailtable.Columns.Add("IS_IMPORT");


            DataTable allottable = ds.Tables.Add("WMS_IN_BILLALLOT");
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
