using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ICMDProductOriginalService : IService<CMD_PRODUCT_ORIGINAL >
    {
        object Detail(int page, int rows, string ORIGINAL_NAME, string DISTRICT_CODE, string MEMO);
        //新增
        bool Add(CMD_PRODUCT_ORIGINAL original);
        //编辑
        bool Edit(CMD_PRODUCT_ORIGINAL original, string ORIGINAL_CODE);
        //删除
        bool Delete(string originalcode);
    }
}
