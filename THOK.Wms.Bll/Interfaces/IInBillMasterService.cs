using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IInBillMasterService:IService<InBillMaster>
    {
        object GetDetails(int page, int rows, string BillNo, string WareHouseCode,string BeginDate,string EndDate, string OperatePersonCode,string CheckPersonCode,string Status, string IsActive);

        bool Add(InBillMaster inBillMaster, string userName, out string strResult);

        bool Delete(string BillNo, out string strResult);

        bool Save(InBillMaster inBillMaster, out string strResult);

        object GenInBillNo(string userName);

        bool Audit(string BillNo, string userName, out string strResult);

        bool AntiTrial(string BillNo, out string strResult);

        object GetBillTypeDetail(string BillClass, string IsActive);

        object GetWareHouseDetail(string IsActive);

        bool Settle(string BillNo,out string strResult);

        bool DownInBillMaster(string BeginDate, string EndDate, out string errorInfo);

        bool uploadInBill();
    }
}
