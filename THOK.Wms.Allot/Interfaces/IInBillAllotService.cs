using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Allot.Interfaces
{
    public interface IInBillAllotService:IService<InBillAllot>
    {
        object Search(string billNo, int page, int rows);

        bool AllotConfirm(string billNo, out string strResult);

        bool AllotCancelConfirm(string billNo, out string strResult);

        bool AllotDelete(string billNo, long id, out string strResult);

        bool AllotEdit(string billNo, long id, string cellCode, decimal allotQuantity, out string strResult);

        bool AllotCancel(string billNo, out string strResult);

        bool AllotAdd(string billNo, long id, string cellCode, decimal allotQuantity, out string strResult);

        bool AllotAdd(string billNo, long id, string cellCode, decimal allotQuantity,string productname, out string strResult);

        System.Data.DataTable AllotSearch(int page, int rows, string billNo);

        object SearchInBillAllot(string billNo, int page, int rows);

        bool EditAllot(string id, string status, string operater, out string strResult);

        object GetInBillMaster();
    }
}
