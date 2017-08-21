using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
   public  interface IWMSBalanceMasterService : IService<WMS_BALANCE_MASTER>
    {
       //获取月结单
       object GetDetails(int page, int rows, string BALANCENO, string BALANCEDATE, string STATE, string OPERATER, string CHECKER, string CHECKDATE);
       //单据审核
       bool Audit(string checker, string BalancNo);
       //反审
       bool Antitrial(string BalancNo);
       //月结
       bool Balance(string Balanceno, DateTime dt, string operater, out string error);
       //获取已月结的年月
       object GetBalanceNo();
       //月结单打印
       bool BalancePrint(string BEGINMONTH, string ENDMONTH, string STATE, string BALANCENO);
    }
}
