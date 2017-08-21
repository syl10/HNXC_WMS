using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public  interface ICMDPorductStyleService : IService<CMD_PRODUCT_STYLE>
    {
        object GetDetails(int page, int rows);

        bool Add(string STYLE_NAME, string SORT_LEVEL);

        bool Delete(string STYLE_CODE);

        bool Save(string STYLE_CODE, string STYLE_NAME, string SORT_LEVEL);
    }
}
