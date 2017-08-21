using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;


namespace THOK.Wms.Bll.Interfaces
{
    public interface ICMDUnitService:IService<CMD_UNIT>
    {
        object GetDetails(int page, int rows, string UNIT_NAME, string CATEGORY_CODE,  string MEMO);

        bool Add(CMD_UNIT Unit);

        bool Delete(string UNIT_CODE);

        bool Save(CMD_UNIT Unit);

    }
}
