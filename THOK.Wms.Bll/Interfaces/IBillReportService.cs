using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
   public   interface IBillReportService : IService<BILLREPORT>
    {
        //入库单报表打印
       bool   StockinPrint(string flag, string username, string PrintCount, string BILLNO, string BILLDATEFROM, string BILLDATETO, string BTYPECODE, string BILLMETHOD, string STATE, string CIGARETTECODE, string FORMULACODE);
       //出库单报表打印
       bool StockoutPrint(string BILLNO, string BILLDATEFROM, string BILLDATETO, string BTYPECODE, string LINENO, string STATE, string CIGARETTECODE, string FORMULACODE);
    }
}
