using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IShelfService : IService<Shelf>
    {
        object GetDetails(string warehouseCode, string areaCode, string shelfCode);

        object GetDetail(string type, string id);

        bool Add(Shelf shelf);

        bool Delete(string shelfCode);

        bool Save(Shelf shelf);

        object FindShelf(string parameter);

        object GetShelfCode(string areaCode);
    }
}
