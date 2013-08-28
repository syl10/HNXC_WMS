using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IWMSFormulaService:IService<WMS_FORMULA_MASTER>
    {
        object GetDetails(int page, int rows, string BTYPE_NAME, string BILL_TYPE, string TASK_LEVEL, string Memo, string TARGET_CODE, string FORMULA_CODE, string FORMULA_NAME, string CIGARETTE_CODE, string ISACTIVE, string FORMULADATE, string OPERATER);
        object GetSubDetails(int page, int rows, string FORMULA_CODE);

        bool Add(WMS_FORMULA_MASTER master, object detail);

        bool Edit(WMS_FORMULA_MASTER master, object detail);

        bool Delete(string FORMULA_CODE);

        object GetFormulaCode(string userName,DateTime dt, string FORMULA_CODE);
        //根据牌号获取配方
        object GetSubDetailbyCigarettecode(int page, int rows, string CIGARETTE_CODE);
        //根据牌号获取已经启用的配方
        object Getusefull(int page, int rows, string CIGARETTE_CODE);
        //验证配方编号是否存在
        bool Checkformulacode(string formulacode);
    }
}
