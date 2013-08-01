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
        object GetDetails(int page, int rows,string billtype);
        //获取单据明细
        object GetSubDetails(int page, int rows, string BillNo);
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
        //批次入库时,载入配方.
        object LoadFormulaDetail(int page, int rows, string Formulacode, decimal BATCH_WEIGHT);
    }
}
