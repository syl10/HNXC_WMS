using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ICurrentStockService : IService<Storage>
    {
        object GetCellDetails(int page, int rows, string productCode, string ware, string area, string unitType);

        System.Data.DataTable GetCurrentStock(int page, int rows, string productCode, string ware, string area, string unitType, out string areaName, bool isAbnormity);
    }
}
