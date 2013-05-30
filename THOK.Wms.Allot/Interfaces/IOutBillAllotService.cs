using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Allot.Interfaces;

namespace THOK.Wms.Allot.Interfaces
{
    public interface IOutBillAllotService:IService<OutBillAllot>
    {
        object Search(string billNo, int page, int rows);

        bool AllotCancelConfirm(string billNo, out string strResult);

        bool AllotConfirm(string billNo, string userName, ref string strResult);

        bool AllotDelete(string billNo, long id, out string strResult);

        bool AllotEdit(string billNo, long id, string cellCode, decimal allotQuantity, out string strResult);

        bool AllotAdd(string billNo, long id,string productCode,string cellCode, decimal allotQuantity, out string strResult);

        bool AllotAdd(string billNo, long id, string productCode, string cellCode, decimal allotQuantity,string productName, out string strResult);

        bool AllotCancel(string billNo,out string strResult);

        System.Data.DataTable AllotSearch(int page, int rows, string billNo);

        object SearchOutBillAllot(string billNo, int page, int rows);

        bool EditAllot(string id, string status, string operater, out string strResult);

        object GetOutBillMaster();
    }
}
