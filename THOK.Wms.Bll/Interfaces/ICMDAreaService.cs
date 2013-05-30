using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ICMDAreaService : IService<CMD_AREA>
    {
        object GetDetails(string warehouseCode, string areaCode);
        object GetDetail(string type, string id);

        bool Add(CMD_AREA area);

        bool Delete(string areaCode);

        bool Save(CMD_AREA area);

        object FindArea(string parameter);

        object GetWareArea();

        object GetAreaCode(string WareCode);
    }
}
