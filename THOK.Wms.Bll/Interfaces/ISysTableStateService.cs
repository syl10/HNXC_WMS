using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
namespace THOK.Wms.Bll.Interfaces
{
    public interface ISysTableStateService:IService<SYS_TABLE_STATE>
    {
        object GetDetails(string TABLE_NAME,string FIELD_NAME);

        string GetDescByState(string TABLE_NAME, string FIELD_NAME, string State);
    }
}
