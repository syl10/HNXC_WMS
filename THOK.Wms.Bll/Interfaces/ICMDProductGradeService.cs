using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ICMDProductGradeService : IService<CMD_PRODUCT_GRADE >
    {
        object Detail(int page, int rows, string ENGLISH_CODE, string USER_CODE,string GRADE_NAME, string MEMO);
        //新增
        bool Add(CMD_PRODUCT_GRADE grade);
        //编辑
        bool Edit(CMD_PRODUCT_GRADE grade, string GRADE_CODE);
        //删除
        bool Delete(string GRADE_CODE);
    }
}
