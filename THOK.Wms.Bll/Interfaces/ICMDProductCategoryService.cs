using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;


namespace THOK.Wms.Bll.Interfaces
{
    public interface ICMDProductCategoryService : IService<CMD_PRODUCT_CATEGORY>
    {
        object GetDetails(int page, int rows, string CATEGORY_NAME, string MEMO);

        bool Add(string CATEGORY_NAME, string MEMO);

        bool Delete(string CATEGORY_CODE);

        bool Save(string CATEGORY_CODE, string CATEGORY_NAME, string MEMO);
    }
}
