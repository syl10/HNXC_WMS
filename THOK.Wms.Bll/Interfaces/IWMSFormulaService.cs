using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IWMSFormulaService:IService<WMS_FORMULA_MASTER>
    {
        object GetDetails(int page, int rows, string BTYPE_NAME, string BILL_TYPE, string TASK_LEVEL, string Memo, string TARGET_CODE);
        object GetSubDetails(int page, int rows, string FORMULA_CODE);

        bool Add(WMS_FORMULA_MASTER master, object detail);

        bool Edit(WMS_FORMULA_MASTER master, object detail);

        bool Delete(string FORMULA_CODE);

        object GetFormulaCode(string userName,DateTime dt, string FORMULA_CODE);

    }
}
