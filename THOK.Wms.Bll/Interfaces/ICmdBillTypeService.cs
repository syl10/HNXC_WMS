using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ICmdBillTypeService : IService<CMD_BILL_TYPE>
    {
        object GetDetails(int page, int rows, string BTYPE_NAME, string BILL_TYPE, string TASK_LEVEL, string Memo, string TARGET_CODE);

        bool Add(CMD_BILL_TYPE BillType);

        bool Delete(string BTYPE_CODE);

        bool Save(CMD_BILL_TYPE BillType);
    }
}
