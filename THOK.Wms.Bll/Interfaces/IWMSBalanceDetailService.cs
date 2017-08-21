using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
   public  interface IWMSBalanceDetailService : IService<WMS_BALANCE_DETAIL>
    {
       object GetSubDetails(int page, int rows, string Balanceno);
       //产品总账,begin为开始年月,end为结束年月
       object Ledger(int page, int rows, string begin, string end);
       //产品明细,begin为开始年月,end为结束年月
       object Detailed(int page, int rows, string begin, string end);
    }
}
