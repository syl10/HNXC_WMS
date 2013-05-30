using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using THOK.Wms.SignalR.Common;

namespace THOK.Wms.Bll.Service
{
    public class InBillMasterHistoryService : ServiceBase<InBillMaster>, IInBillMasterHistoryService
    {
        [Dependency]
        public IInBillMasterRepository InBillMasterRepository { get; set; }
        [Dependency]
        public IInBillMasterHistoryRepository InBillMasterHistoryRepository { get; set; }
        [Dependency]
        public IInBillDetailRepository InBillDetailRepository { get; set; }
        [Dependency]
        public IInBillDetailHistoryRepository InBillDetailHistoryRepository { get; set; }
        [Dependency]
        public IInBillAllotRepository InBillAllotRepository { get; set; }
        [Dependency]
        public IInBillAllotHistoryRepository InBillAllotHistoryRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public bool Add(DateTime datetime, out string strResult)
        {
            bool result = false;
            strResult = string.Empty;

            var inBillMaster = InBillMasterRepository.GetQueryable().Where(i => i.BillDate <= datetime);
            var inBillDetail = InBillDetailRepository.GetQueryable().Where(i => i.InBillMaster.BillDate <= datetime);
            var inBillAllot = InBillAllotRepository.GetQueryable().Where(i => i.InBillMaster.BillDate <= datetime);

            if (inBillMaster.Any())
            {
                #region 主表移入历史表
                try
                {
                    foreach (var item in inBillMaster.ToArray())
                    {
                        InBillMasterHistory history = new InBillMasterHistory();
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
                        history.TargetCellCode = item.TargetCellCode;
                        InBillMasterHistoryRepository.Add(history);
                    }
                    result = true;
                }
                catch (Exception e)
                {
                    strResult = "迁移主表时：" + e.InnerException.ToString();
                    result = false;
                }
                #endregion

                if (inBillDetail.Any())
                {
                    #region 细表移入历史表
                    try
                    {
                        foreach (var item2 in inBillDetail.ToArray())
                        {
                            InBillDetailHistory history2 = new InBillDetailHistory();
                            history2.ID = item2.ID;
                            history2.BillNo = item2.BillNo;
                            history2.ProductCode = item2.ProductCode;
                            history2.UnitCode = item2.UnitCode;
                            history2.Price = item2.Price;
                            history2.BillQuantity = item2.BillQuantity;
                            history2.AllotQuantity = item2.AllotQuantity;
                            history2.RealQuantity = item2.RealQuantity;
                            history2.Description = item2.Description;
                            InBillDetailHistoryRepository.Add(history2);
                        }
                        result = true;
                    }
                    catch (Exception e)
                    {
                        strResult = "迁移细表时：" + e.InnerException.ToString();
                        result = false;
                    }
                    #endregion

                    if (inBillAllot.Any())
                    {
                        #region 分配表移入历史表
                        try
                        {
                            foreach (var item3 in inBillAllot.ToArray())
                            {
                                InBillAllotHistory history3 = new InBillAllotHistory();
                                history3.BillNo = item3.BillNo;
                                history3.ProductCode = item3.ProductCode;
                                history3.InBillDetailId = item3.InBillDetailId;
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
                                InBillAllotHistoryRepository.Add(history3);
                            }
                            result = true;
                        }
                        catch (Exception e)
                        {
                            strResult = "迁移分配表时：" + e.InnerException.ToString();
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
                        foreach (var item in inBillMaster.ToList())
                        {
                            Del(InBillAllotRepository, item.InBillAllots);
                            Del(InBillDetailRepository, item.InBillDetails);
                            InBillMasterRepository.Delete(item);
                            result = true;
                        }
                    }
                    catch (Exception e)
                    {
                        strResult = "删除操作时：" + e.InnerException.ToString();
                        result = false;
                    }
                    InBillMasterRepository.SaveChanges();
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
