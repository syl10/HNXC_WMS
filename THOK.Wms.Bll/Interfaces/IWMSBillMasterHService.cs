using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IWMSBillMasterHService : IService<WMS_BILL_MASTERH>
    {
        //获取单据记录  billtype 单据类型比如入库,出库等,flag为1时是入库 出库作业,为0不是入库 出库作业,为2是属于抽检补料入库单.
        object GetDetails(int page, int rows, string billtype, string flag, string BILL_NO, string BILL_DATE, string BTYPE_CODE, string WAREHOUSE_CODE, string BILL_METHOD, string CIGARETTE_CODE, string FORMULA_CODE, string STATE, string OPERATER, string OPERATE_DATE, string CHECKER, string CHECK_DATE, string STATUS, string BILL_DATEStar, string BILL_DATEEND, string SOURCE_BILLNO, string LINENO, string BILLNOFROM, string BILLNOTO);
        //获取单据明细   flag  为0表示获取该单据的所有明细,为1表示获取该单据下的混装产品信息(即在设置混转第一产品时下拉框的数据).
        object GetSubDetails(int page, int rows, string BillNo, int flag);
    }
}
