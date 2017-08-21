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
  
    public interface ICMDUnitCategoryService : IService<CMD_UNIT_CATEGORY>
    {
        object GetDetails(int page, int rows, string CATEGORY_NAME, string MEMO);

        bool Add(CMD_UNIT_CATEGORY UnitGategory);

        bool Delete(string CIGARETTE_CODE);

        bool Save(CMD_UNIT_CATEGORY UnitGategory);
    }
}
