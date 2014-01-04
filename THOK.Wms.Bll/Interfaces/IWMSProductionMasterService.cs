using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IWMSProductionMasterService : IService<WMS_PRODUCTION_MASTER>
    {
        //获取所有直接投料单记录
        object GetDetails(int page, int rows, string BILL_NO, string BILL_DATE, string WAREHOUSE_CODE, string CIGARETTE_CODE, string FORMULA_CODE, string STATE, string OPERATER, string OPERATE_DATE, string CHECKER, string CHECK_DATE, string BILL_DATEStar, string BILL_DATEEND,string BILLNOFROM,string BILLNOTO);
        //获取明细
         object GetSubDetails(int page, int rows, string BillNo);
         //获取单据编号
         object GetBillNo(string userName, DateTime dt, string BILL_NO);
        //新增
         bool Add(WMS_PRODUCTION_MASTER mast, object detail);
         //修改单据
         bool Edit(WMS_PRODUCTION_MASTER  mast, object detail);
         //审核
         bool Audit(string checker, string billno);
         //反审
         bool Antitrial(string billno);
         //删除
         bool Delete(string billno);
        //打印
         bool Print(string BILL_NO,string WAREHOUSE_CODE, string CIGARETTE_CODE, string FORMULA_CODE, string STATE, string BILL_DATEStar, string BILL_DATEEND,string  SCHEDULENO,string  IN_BILLNO,string  OUT_BILLNO);
    }
}
