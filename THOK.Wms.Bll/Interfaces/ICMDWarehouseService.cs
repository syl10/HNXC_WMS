using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ICMDWarehouseService : IService<CMD_WAREHOUSE>
    {
        object GetDetails(int page, int rows, string warehouseCode);

        object GetDetail(int page, int rows, string type,string id);

        bool Add(CMD_WAREHOUSE warehouse);

        bool Delete(string warehouseCode);

        bool Save(CMD_WAREHOUSE warehouse);

        object FindWarehouse(string parameter);

        object GetWareCode();
    }
}
