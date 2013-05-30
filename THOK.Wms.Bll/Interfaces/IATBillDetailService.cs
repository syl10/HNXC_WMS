
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IATBillDetailService:IService<ATBillDetail>
    {
        object GetDetails(int page, int rows, string WMS_BILL_MASTER_ID);

        bool Add(ATBillDetail ATBillDetail, out string strResult);

        bool Delete(string ID, out string strResult);

        bool Save(ATBillDetail ATBillDetail, out string strResult);

        object GetProductDetails(int page, int rows,string QueryString, string Value);

        object GetInBillDetail(int page, int rows, string BillNo);
    }
}
