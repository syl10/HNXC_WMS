using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public  interface IWMSBillMasterService:IService<WMS_BILL_MASTER >
    {
        //获取单据记录
        object GetDetails(int page, int rows, string billtype, string BILL_NO, string BILL_DATE, string BTYPE_CODE, string WAREHOUSE_CODE, string BILL_METHOD, string CIGARETTE_CODE, string FORMULA_CODE, string STATE, string OPERATER, string OPERATE_DATE, string CHECKER, string CHECK_DATE);
        //获取单据明细   flag  为0表示获取该单据的所有明细,为1表示获取该单据下的混装产品信息.
        object GetSubDetails(int page, int rows, string BillNo,int flag);
        //单据审核
        bool Audit(string checker, string BillNo);
        //反审
        bool Antitrial(string BillNo);
        //获取单据编号
        object GetBillNo(string userName, DateTime dt, string BILL_NO);
        //单据新增
        bool Add(WMS_BILL_MASTER  mast, object detail);
        //修改单据
        bool Edit(WMS_BILL_MASTER mast, object detail);
        //删除单据
        bool Delete(string BillNo);
        //批次入库时,载入配方.
        object LoadFormulaDetail(int page, int rows, string Formulacode, decimal BATCH_WEIGHT);
        //获取序号
        object GetSerial(string BILLNO);
        //设置混装
        bool SetMIX(string BillNo, object detail);
    }
}
