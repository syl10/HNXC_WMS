using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ICMDShelfService : IService<CMD_SHELF>
    {
        object GetDetails(string warehouseCode, string areaCode, string shelfCode);

        object GetDetail(string type, string id);

        bool Add(CMD_SHELF shelf);

        bool Delete(string shelfCode);

        bool Save(CMD_SHELF shelf);

        object FindShelf(string parameter);

        object GetShelfCode(string areaCode);
    }
}
