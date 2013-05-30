using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.Bll.Models;

namespace THOK.Wms.Bll.Service
{
    public class ATBillMasterService : ServiceBase<ATBillMaster>, IATBillMasterService
    {
        [Dependency]
        public IATBillDetailRepository iatBillDetailRep { get; set; }
        [Dependency]
        public IATBillMasterRepository iatBillMasterRep { get; set; }
        [Dependency]
        public IEmployeeRepository EmployeeRepository { get; set; }
        [Dependency]

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, string BeginDate, string EndDate, string WMS_BILL_MASTER_ID, string BILL_NO, string BILL_DATE, string BILL_TYPE, string BIZ_TYPE_CODE, string WAREHOUSE_CODE, string STATE, string OPERATER, string OPERATE_DATE, string CHECKER, string CHECK_DATE, string TASKER, string TASK_DATE)
        {
            IQueryable<ATBillMaster> inBillMasterQuery = iatBillMasterRep.GetQueryable();
            var inBillMaster = inBillMasterQuery
                    .OrderByDescending(t => t.BILL_NO)
                    .Select(i => i);

            int total = inBillMaster.Count();
            inBillMaster = inBillMaster.Skip((page - 1) * rows).Take(rows);

            if (!BeginDate.Equals(string.Empty))
            {
                DateTime begin = Convert.ToDateTime(BeginDate);
                inBillMaster = inBillMaster.Where(i => i.BILL_DATE >= begin);
            }

            if (!EndDate.Equals(string.Empty))
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                inBillMaster = inBillMaster.Where(i => i.BILL_DATE <= end);
            }
            var tmp = inBillMaster.ToArray().AsEnumerable().Select(p => new
            {

              
                   
               p.WMS_BILL_MASTER_ID,
               p.WAREHOUSE_CODE,
               p.OPERATER,
               p.STATE,
               OPERATE_DATE = p.OPERATE_DATE.ToString("yyyy-MM-dd HH:mm:ss"),
               p.CHECKER,
               CHECK_DATE = p.CHECK_DATE.ToString("yyyy-MM-dd HH:mm:ss"),
               p.BIZ_TYPE_CODE,
               p.BillDetailes,
               p.BILL_TYPE,
               p.BILL_NO,
               BILL_DATE= p.BILL_DATE.ToString("yyyy-MM-dd HH:mm:ss"),
               p.TASKER,
               TASK_DATE = p.TASK_DATE.ToString("yyyy-MM-dd HH:mm:ss")
               
                //i.BillNo,
                //BillDate = i.BillDate.ToString("yyyy-MM-dd HH:mm:ss"),
                //i.OperatePersonID,
                //i.WarehouseCode,
                //i.BillTypeCode,
                //i.BillType.BillTypeName,
                //i.Warehouse.WarehouseName,
                //OperatePersonCode = i.OperatePerson.EmployeeCode,
                //OperatePersonName = i.OperatePerson.EmployeeName,
                //VerifyPersonID = i.VerifyPersonID == null ? string.Empty : i.VerifyPerson.EmployeeCode,
                //VerifyPersonName = i.VerifyPersonID == null ? string.Empty : i.VerifyPerson.EmployeeName,
                //VerifyDate = (i.VerifyDate == null ? "" : ((DateTime)i.VerifyDate).ToString("yyyy-MM-dd HH:mm:ss")),
                //Status = WhatStatus(i.Status),
                //IsActive = i.IsActive == "1" ? "可用" : "不可用",
                //Description = i.Description,
                //UpdateTime = i.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                //i.TargetCellCode
            });

            if (!WMS_BILL_MASTER_ID.Equals(""))
                tmp = tmp.Where(b => b.WMS_BILL_MASTER_ID == WMS_BILL_MASTER_ID.Trim());
            if (!STATE.Equals(""))
                tmp = tmp.Where(b => b.STATE == STATE.Trim());
            if (!BILL_NO.Equals(""))
                tmp = tmp.Where(b => b.BILL_NO == BILL_NO.Trim());

            if (!OPERATER.Equals(""))
                tmp = tmp.Where(b => b.OPERATER == OPERATER.Trim());
            if (!BILL_NO.Equals(""))
                tmp = tmp.Where(b => b.BILL_NO == BILL_NO.Trim());

            //if (!BeginDate.Equals(string.Empty))
            //{
            //    DateTime begin = Convert.ToDateTime(BeginDate);
            //    inBillMaster = inBillMaster.Where(i => i.BILL_DATE >= begin);
            //}

            //if (!EndDate.Equals(string.Empty))
            //{
            //    DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
            //    inBillMaster = inBillMaster.Where(i => i.BILL_DATE <= end);
            //}

            return new { total, rows = tmp.ToArray() };

        }

        public bool Add(ATBillMaster atBillMaster, string userName, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var atBM = new ATBillMaster();
            try
            {
                atBM.BILL_DATE = atBillMaster.BILL_DATE;
                atBM.BILL_NO = atBillMaster.BILL_NO;
                atBM.BILL_TYPE = atBillMaster.BILL_TYPE;
                atBM.BIZ_TYPE_CODE = atBillMaster.BIZ_TYPE_CODE;
                atBM.CHECK_DATE = atBillMaster.CHECK_DATE;
                atBM.CHECKER = atBillMaster.CHECKER;
                atBM.OPERATE_DATE = atBillMaster.OPERATE_DATE;
                atBM.OPERATER = atBillMaster.OPERATER;
                atBM.STATE = atBillMaster.STATE;
                atBM.TASKER = atBillMaster.TASKER;
                atBM.TASK_DATE = atBillMaster.TASK_DATE;
                atBM.WAREHOUSE_CODE = atBillMaster.WAREHOUSE_CODE;
                atBM.WMS_BILL_MASTER_ID = atBillMaster.WMS_BILL_MASTER_ID;
                iatBillMasterRep.Add(atBM);
                iatBillMasterRep.SaveChanges();

                result = true;
            }
            catch (Exception ex)
            {
                strResult = "新增失败，原因：" + ex.Message;
            }
            return result;
        }


        public bool Delete(string WMS_BILL_MASTER_ID, out string strResult)
        {
            strResult = string.Empty;
            bool Result = false;
            var atBM = iatBillMasterRep.GetQueryable().FirstOrDefault(i => i.WMS_BILL_MASTER_ID == WMS_BILL_MASTER_ID);
            if (atBM != null)
            {
                try
                {
                    Del(iatBillDetailRep, atBM.BillDetailes);
                    iatBillMasterRep.Delete(atBM);
                    iatBillMasterRep.SaveChanges();
                    Result = true;
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

            return Result;
        }
        public bool Save(ATBillMaster atBillMaster, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var atBM = iatBillMasterRep.GetQueryable().FirstOrDefault(i => i.WMS_BILL_MASTER_ID == atBillMaster.WMS_BILL_MASTER_ID);
            if (atBM != null)
            {
                try
                {

                    atBM.BILL_DATE = atBillMaster.BILL_DATE;
                    atBM.BILL_NO = atBillMaster.BILL_NO;
                    atBM.BILL_TYPE = atBillMaster.BILL_TYPE;
                    atBM.BIZ_TYPE_CODE = atBillMaster.BIZ_TYPE_CODE;
                    atBM.CHECK_DATE = atBillMaster.CHECK_DATE;
                    atBM.CHECKER = atBillMaster.CHECKER;
                    atBM.OPERATE_DATE = atBillMaster.OPERATE_DATE;
                    atBM.OPERATER = atBillMaster.OPERATER;
                    atBM.STATE = atBillMaster.STATE;
                    atBM.TASKER = atBillMaster.TASKER;
                    atBM.TASK_DATE = atBillMaster.TASK_DATE;
                    atBM.WAREHOUSE_CODE = atBillMaster.WAREHOUSE_CODE;
                    atBM.WMS_BILL_MASTER_ID = atBillMaster.WMS_BILL_MASTER_ID;
                    iatBillMasterRep.SaveChanges();
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
        public object GenInBillNo(string userName)
        {
            IQueryable<ATBillMaster> iatBillMasterQuery = iatBillMasterRep.GetQueryable();
            string sysTime = System.DateTime.Now.ToString("yyMMdd");
            string WMS_BILL_MASTER_ID = "";
            var employee = EmployeeRepository.GetQueryable().FirstOrDefault(i => i.UserName == userName);
            var atBillMaster = iatBillMasterQuery.Where(i => i.WMS_BILL_MASTER_ID.Contains(sysTime)).ToArray().OrderBy(i => i.WMS_BILL_MASTER_ID).Select(i => new { i.WMS_BILL_MASTER_ID }.WMS_BILL_MASTER_ID);
            if (atBillMaster.Count() == 0)
            {
                WMS_BILL_MASTER_ID = System.DateTime.Now.ToString("yyMMdd") + "0001" + "IN";
            }
            else
            {
                string billNoStr = atBillMaster.Last(b => b.Contains(sysTime));
                int i = Convert.ToInt32(billNoStr.ToString().Substring(6, 4));
                i++;
                string newcode = i.ToString();
                for (int j = 0; j < 4 - i.ToString().Length; j++)
                {
                    newcode = "0" + newcode;
                }
                WMS_BILL_MASTER_ID = System.DateTime.Now.ToString("yyMMdd") + newcode + "IN";
            }
            var findBillInfo = new
            {
                WMS_BILL_MASTER_ID = WMS_BILL_MASTER_ID,
                billNoDate = DateTime.Now.ToString("yyyy-MM-dd"),
                employeeID = employee == null ? "" : employee.ID.ToString(),
                employeeCode = employee == null ? "" : employee.EmployeeCode.ToString(),
                employeeName = employee == null ? "" : employee.EmployeeName.ToString()
            };
            return findBillInfo;
        }
        // public  bool Audit(string BillNo, string userName, out string strResult)
        //  {
        //      bool Result = true;
        //      strResult = "";
        //      return Result;
        //  }
        // public bool AntiTrial(string BillNo, out string strResult)
        //  {
        //      bool Result = true;
        //      strResult = "";
        //      return Result;
        //  }
        //public  object GetBillTypeDetail(string BillClass, string IsActive)
        //  {
        //      bool Result = true;
        //      return Result;
        //  }
        // public object GetWareHouseDetail(string IsActive)
        //  {
        //      bool Result = true;
        //      return Result;
        //  }
        // public bool Settle(string BillNo, out string strResult)
        //  {
        //      bool Result = true;
        //      strResult = "";
        //      return Result;
        //  }
        // public bool DownInBillMaster(string BeginDate, string EndDate, out string errorInfo)
        //  {
        //      bool Result = true;
        //      errorInfo = "";
        //      return Result;
        //  }
        // public bool uploadInBill()
        //  {
        //      bool Result = true;
        //      return Result;
        //  }
    }
}
