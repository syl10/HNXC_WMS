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
    public class ProfitLossBillMasterHistoryService : ServiceBase<ProfitLossBillMaster>, IProfitLossBillMasterHistoryService
    {
        [Dependency]
        public IProfitLossBillMasterRepository ProfitLossBillMasterRepository { get; set; }
        [Dependency]
        public IProfitLossBillMasterHistoryRepository ProfitLossBillMasterHistoryRepository { get; set; }
        [Dependency]
        public IProfitLossBillDetailRepository ProfitLossBillDetailRepository { get; set; }
        [Dependency]
        public IProfitLossBillDetailHistoryRepository ProfitLossBillDetailHistoryRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public bool Add(DateTime datetime, out string strResult)
        {
            bool result = false;
            strResult = string.Empty;

            var profitLossBillMaster = ProfitLossBillMasterRepository.GetQueryable().Where(i => i.BillDate <= datetime);
            var profitLossBillDetail = ProfitLossBillDetailRepository.GetQueryable().Where(i => i.ProfitLossBillMaster.BillDate <= datetime);

            if (profitLossBillMaster.Any())
            {
                #region 主表移入历史表
                try
                {
                    foreach (var item in profitLossBillMaster.ToArray())
                    {
                        ProfitLossBillMasterHistory history = new ProfitLossBillMasterHistory();
                        history.BillNo = item.BillNo;
                        history.BillDate = item.BillDate;
                        history.BillTypeCode = item.BillTypeCode;
                        history.CheckBillNo = "";
                        history.WarehouseCode = item.WarehouseCode;
                        history.Status = item.Status;
                        history.VerifyPersonID = item.VerifyPersonID;
                        history.VerifyDate = item.VerifyDate;
                        history.Description = item.Description;
                        history.IsActive = item.IsActive;
                        history.UpdateTime = item.UpdateTime;
                        history.OperatePersonID = item.OperatePersonID;
                        history.LockTag = item.LockTag;
                        history.RowVersion = item.RowVersion;
                        ProfitLossBillMasterHistoryRepository.Add(history);
                    }
                    result = true;
                }
                catch (Exception e)
                {
                    strResult = "主库单：" + e.InnerException.ToString();
                    result = false;
                }
                #endregion

                if (profitLossBillDetail.Any())
                {
                    #region 细表移入历史表
                    try
                    {
                        foreach (var item in profitLossBillDetail.ToArray())
                        {
                            ProfitLossBillDetailHistory history = new ProfitLossBillDetailHistory();
                            history.BillNo = item.BillNo;
                            history.CellCode = item.CellCode;
                            history.StorageCode = item.StorageCode;
                            history.ProductCode = item.ProductCode;
                            history.UnitCode = item.UnitCode;
                            history.Price = item.Price;
                            history.Quantity = item.Quantity;
                            history.Description = item.Description;
                            ProfitLossBillDetailHistoryRepository.Add(history);
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
                        foreach (var item in profitLossBillMaster.ToList())
                        {

                            Del(ProfitLossBillDetailRepository, item.ProfitLossBillDetails);
                            ProfitLossBillMasterRepository.Delete(item);
                            result = true;
                        }
                    }
                    catch (Exception e)
                    {
                        strResult = "删除情况：" + e.InnerException.ToString();
                    }

                    ProfitLossBillMasterHistoryRepository.SaveChanges();
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
