
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IATBillMasterService : IService<ATBillMaster>
    {
        object GetDetails(int page, int rows, string BeginDate, string EndDate, string WMS_BILL_MASTER_ID, string BILL_NO, string BILL_DATE, string BILL_TYPE, string BIZ_TYPE_CODE, string WAREHOUSE_CODE, string STATE, string OPERATER, string OPERATE_DATE, string CHECKER, string CHECK_DATE, string TASKER, string TASK_DATE);

        bool Add(ATBillMaster inBillMaster, string userName, out string strResult);

        bool Delete(string BillNo, out string strResult);

        bool Save(ATBillMaster inBillMaster, out string strResult);

        object GenInBillNo(string userName);

        //bool Audit(string BillNo, string userName, out string strResult);

        //bool AntiTrial(string BillNo, out string strResult);

        //object GetBillTypeDetail(string BillClass, string IsActive);

        //object GetWareHouseDetail(string IsActive);

        //bool Settle(string BillNo, out string strResult);

        //bool DownInBillMaster(string BeginDate, string EndDate, out string errorInfo);

        //bool uploadInBill();
    }
}
