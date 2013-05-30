using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IDistributionService : IService<Storage>
    {
        object GetProductTree(string productCode, string unitType);

        object GetCellDetails(int page, int rows, string type, string id, string unitType, string productCode);

        object GetAreaDetails(int page, int rows, string productCode, string ware, string area, string unitType);

        System.Data.DataTable GetDistribution(int page, int rows, string type, string id, string unitType, string productCode);
    }
}
