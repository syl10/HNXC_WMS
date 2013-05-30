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
    public class DailyBalanceHistoryService : IDailyBalanceHistoryService
    {
        [Dependency]
        public IDailyBalanceRepository DailyBalanceRepository { get; set; }
        [Dependency]
        public IDailyBalanceHistoryRepository DailyBalanceHistoryRepository { get; set; }

        public bool Add(DateTime datetime, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var dailyBalancel = DailyBalanceRepository.GetQueryable().Where(i => i.SettleDate <= datetime);

            if (dailyBalancel.Any())
            {
                #region 日结表移入历史表
                try
                {
                    foreach (var item in dailyBalancel.ToArray())
                    {
                        DailyBalanceHistory history = new DailyBalanceHistory();
                        history.ID = item.ID;
                        history.SettleDate = item.SettleDate;
                        history.WarehouseCode = item.WarehouseCode;
                        history.ProductCode = item.ProductCode;
                        history.UnitCode = item.UnitCode;
                        history.Beginning = item.Beginning;
                        history.EntryAmount = item.EntryAmount;
                        history.DeliveryAmount = item.DeliveryAmount;
                        history.ProfitAmount = item.ProfitAmount;
                        history.LossAmount = item.LossAmount;
                        history.Ending = item.Ending;
                        DailyBalanceHistoryRepository.Add(history);
                    }
                    result = true;
                }
                catch (Exception e)
                {
                    strResult = e.InnerException.ToString();
                    result = false;
                }
                #endregion

                #region 删除数据
                try
                {
                    foreach (var item in dailyBalancel.ToList())
                    {
                        DailyBalanceRepository.Delete(item);
                        DailyBalanceRepository.SaveChanges();
                        result = true;
                    }
                }
                catch (Exception e)
                {
                    strResult = e.InnerException.ToString();
                    result = false;
                } 
                #endregion

                DailyBalanceHistoryRepository.SaveChanges();
            }
            else
            {
                strResult = "数据不存在！";
            }
            return result;
        }
    }
}
