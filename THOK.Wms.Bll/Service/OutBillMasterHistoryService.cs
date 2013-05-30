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
    public class OutBillMasterHistoryService : ServiceBase<OutBillMaster>, IOutBillMasterHistoryService
    {
        [Dependency]
        public IOutBillMasterRepository OutBillMasterRepository { get; set; }
        [Dependency]
        public IOutBillMasterHistoryRepository OutBillMasterHistoryRepository { get; set; }
        [Dependency]
        public IOutBillDetailRepository OutBillDetailRepository { get; set; }
        [Dependency]
        public IOutBillDetailHistoryRepository OutBillDetailHistoryRepository { get; set; }
        [Dependency]
        public IOutBillAllotRepository OutBillAllotRepository { get; set; }
        [Dependency]
        public IOutBillAllotHistoryRepository OutBillAllotHistoryRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public bool Add(DateTime datetime, out string strResult)
        {
            bool result = false;
            strResult = string.Empty;

            var outBillMaster = OutBillMasterRepository.GetQueryable().Where(i => i.BillDate <= datetime);
            var outBillDetail = OutBillDetailRepository.GetQueryable().Where(i => i.OutBillMaster.BillDate <= datetime);
            var outBillAllot = OutBillAllotRepository.GetQueryable().Where(i => i.OutBillMaster.BillDate <= datetime);

            if (outBillMaster.Any())
            {
                #region 主表移入历史表
                try
                {
                    foreach (var item in outBillMaster.ToArray())
                    {
                        OutBillMasterHistory history = new OutBillMasterHistory();
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
                        history.Origin = item.Origin;
                        history.TargetCellCode = item.TargetCellCode;
                        OutBillMasterHistoryRepository.Add(history);
                    }
                    result = true;
                }
                catch (Exception e)
                {
                    strResult = "主库单：" + e.InnerException.ToString();
                    result = false;
                }
                #endregion

                if (outBillDetail.Any())
                {
                    #region 细表移入历史表
                    try
                    {
                        foreach (var item in outBillDetail.ToArray())
                        {
                            OutBillDetailHistory history = new OutBillDetailHistory();
                            history.ID = item.ID;
                            history.BillNo = item.BillNo;
                            history.ProductCode = item.ProductCode;
                            history.UnitCode = item.UnitCode;
                            history.Price = item.Price;
                            history.BillQuantity = item.BillQuantity;
                            history.AllotQuantity = item.AllotQuantity;
                            history.RealQuantity = item.RealQuantity;
                            history.Description = item.Description;
                            OutBillDetailHistoryRepository.Add(history);
                        }
                        result = true;
                    }
                    catch (Exception e)
                    {
                        strResult = "细库单" + e.InnerException.ToString();
                        return false;
                    }
                    #endregion

                    if (outBillAllot.Any())
                    {
                        #region 分配表移入历史表
                        try
                        {
                            foreach (var item3 in outBillAllot.ToArray())
                            {
                                OutBillAllotHistory history3 = new OutBillAllotHistory();
                                history3.BillNo = item3.BillNo;
                                history3.ProductCode = item3.ProductCode;
                                history3.OutBillDetailId = item3.OutBillDetailId;
                                history3.CellCode = item3.CellCode;
                                history3.StorageCode = item3.StorageCode;
                                history3.UnitCode = item3.UnitCode;
                                history3.AllotQuantity = item3.AllotQuantity;
                                history3.RealQuantity = item3.RealQuantity;
                                history3.OperatePersonID = item3.OperatePersonID;
                                history3.Operator = item3.Operator;
                                history3.StartTime = item3.StartTime;
                                history3.FinishTime = item3.FinishTime;
                                history3.Status = item3.Status;
                                OutBillAllotHistoryRepository.Add(history3);
                            }
                            result = true;
                        }
                        catch (Exception e)
                        {
                            strResult = "分配单：" + e.InnerException.ToString();
                            result = false;
                        }
                        #endregion
                    }
                }
                if (result == true)
                {
                    #region 删除主细分配表
                    try
                    {
                        foreach (var item in outBillMaster.ToList())
                        {

                            Del(OutBillAllotRepository, item.OutBillAllots);
                            Del(OutBillDetailRepository, item.OutBillDetails);
                            OutBillMasterRepository.Delete(item);

                            result = true;
                        }
                    }
                    catch (Exception e)
                    {
                        strResult = "删除情况：" + e.InnerException.ToString();
                    }
                    OutBillMasterHistoryRepository.SaveChanges();
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
