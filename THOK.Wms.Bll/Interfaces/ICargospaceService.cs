using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ICargospaceService : IService<Storage>
    {
        object GetCellDetails(int page, int rows, string type, string id);

        object GetCellDetails(string type, string id);

        object GetInCellDetail(string type, string id);

        object GetOutCellDetail(string type, string id, string productCode);

        System.Data.DataTable GetCargospace(int page, int rows, string type, string id);
    }
}
