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
       bool StockoutPrint(string BILLNO, string BILLDATEFROM, string BILLDATETO, string BTYPECODE, string LINENO, string STATE, string CIGARETTECODE, string FORMULACODE, string SOURSEBILL, string SCHEDULENO);
       //盘点单打印
       bool InventoryPrint(string BILLNO, string BILLDATEFROM, string BILLDATETO, string STATE, string SOURSEBILL, string btypecode);
       //损益单打印
       bool StockdiffPrint(string BILLNO, string BILLDATEFROM, string BILLDATETO, string STATE, string SOURSEBILL, string btypecode);
       //抽检不料入库单打印
       bool FillbillPrint(string BILLNO, string BILLDATEFROM, string BILLDATETO, string BILLMETHOD, string STATE, string CIGARETTECODE, string FORMULACODE);
    }
}
