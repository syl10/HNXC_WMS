using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Service
{
    public class CheckBillMasterHistoryService : ServiceBase<CheckBillMaster>, ICheckBillMasterHistoryService
    {
        [Dependency]
        public ICheckBillMasterRepository CheckBillMasterRepository { get; set; }
        [Dependency]
        public ICheckBillMasterHistoryRepository CheckBillMasterHistoryRepository { get; set; }
        [Dependency]
        public ICheckBillDetailRepository CheckBillDetailRepository { get; set; }
        [Dependency]
        public ICheckBillDetailHistoryRepository CheckBillDetailHistoryRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public bool Add(DateTime datetime, out string strResult)
        {
            bool result = false;
            strResult = string.Empty;

            var checkBillMaster = CheckBillMasterRepository.GetQueryable().Where(i => i.BillDate <= datetime);
            var checkBillDetail = CheckBillDetailRepository.GetQueryable().Where(i => i.CheckBillMaster.BillDate <= datetime);

            if (checkBillMaster.Any())
            {
                #region 主表移入历史表
                try
                {
                    foreach (var item in checkBillMaster.ToArray())
                    {
                        CheckBillMasterHistory history = new CheckBillMasterHistory();

                        history.BillNo = item.BillNo;
                        history.BillDate = item.BillDate;
                        history.BillTypeCode = item.BillTypeCode;
                        history.WarehouseCode = item.WarehouseCode;
                        history.OperatePersonID = item.OperatePersonID;
                        history.Status = item.Status;
                        history.VerifyPersonID = item.VerifyPersonID;
                        history.VerifyDate = item.VerifyDate;
                        history.Description = item.Description;
                        history.IsActive = item.IsActive;
                        history.UpdateTime = item.UpdateTime;

                        CheckBillMasterHistoryRepository.Add(history);
                    }
                    result = true;
                }
                catch (Exception e)
                {
                    strResult = "主库单：" + e.InnerException.ToString();
                    result = false;
                }
                #endregion

                if (checkBillDetail.Any())
                {
                    #region 细表移入历史表
                    try
                    {
                        foreach (var item in checkBillDetail.ToArray())
                        {
                            CheckBillDetailHistory history = new CheckBillDetailHistory();
                            history.BillNo = item.BillNo;
                            history.CellCode = item.CellCode;
                            history.StorageCode = item.StorageCode;
                            history.ProductCode = item.ProductCode;
                            history.UnitCode = item.UnitCode;
                            history.Quantity = item.Quantity;
                            history.RealProductCode = item.ProductCode;
                            history.RealUnitCode = item.RealUnitCode;
                            history.RealQuantity = item.Quantity;
                            history.Status = item.Status;

                            CheckBillDetailHistoryRepository.Add(history);
                        }
                        result = true;
                    }
                    catch (Exception e)
                    {
                        strResult = "细库单：" + e.InnerException.ToString(); ;
                    }
                    #endregion
                }
                if (result == true)
                {
                    #region 删除主细分配表
                    try
                    {
                        foreach (var item in checkBillMaster.ToList())
                        {
                            Del(CheckBillDetailRepository, item.CheckBillDetails);
                            CheckBillMasterRepository.Delete(item);
                            result = true;
                        }
                    }
                    catch (Exception e)
                    {
                        strResult = "删除情况：" + e.InnerException.ToString();
                    }
                    CheckBillMasterHistoryRepository.SaveChanges();
                    #endregion
                }
            }
            else
            {
                strResult = "数据不存在！";
            }
            return result;
        }
    }
}
