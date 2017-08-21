using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ICMDProductService:IService<CMD_PRODUCT>
    {
        object GetDetails(int page, int rows, string ProductName, string ORIGINAL, string YEARS, string GRADE, string STYLE, string WEIGHT, string MEMO, string CATEGORY_CODE);
        bool Add(CMD_PRODUCT product);
        
        bool Delete(string ProductCode);

        bool Save(CMD_PRODUCT product);
        object Selectprod(int page, int rows, string QueryString, string value);


       
    }
}
