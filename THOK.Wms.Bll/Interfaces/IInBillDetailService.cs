using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IInBillDetailService:IService<InBillDetail>
    {
        object GetDetails(int page, int rows, string BillNo);

        bool Add(InBillDetail inBillDetail, out string strResult);

        bool Delete(string ID, out string strResult);

        bool Save(InBillDetail inBillDetail, out string strResult);

        object GetProductDetails(int page, int rows,string QueryString, string Value);

        System.Data.DataTable GetInBillDetail(int page, int rows, string BillNo);
    }
}
