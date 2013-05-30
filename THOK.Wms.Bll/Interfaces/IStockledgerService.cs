using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IStockledgerService
    {
        object GetInfoDetails(int page, int rows, string warehouseCode, string productCode, string settleDate);

        object GetDetails(int page, int rows, string warehouseCode, string productCode, string beginDate, string endDate, string unitType);

        System.Data.DataTable GetInfoDetail(int page, int rows, string warehouseCode, string productCode, string settleDate);
    }
}
