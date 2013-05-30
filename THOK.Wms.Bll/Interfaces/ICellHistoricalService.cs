using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ICellHistoricalService : IService<InBillAllot>
    {
        //object GetCellDetails(int page, int rows, string type, string id);

        object GetCellDetails(int page, int rows, string beginDate, string endDate, string type, string id);

        System.Data.DataTable GetCellHistory(int page, int rows, string beginDate, string endDate, string type, string id);
    }
}
