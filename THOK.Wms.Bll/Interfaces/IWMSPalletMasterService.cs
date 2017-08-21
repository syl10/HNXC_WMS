using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
   public   interface IWMSPalletMasterService : IService<WMS_PALLET_MASTER>
    {
          //flag  1表示托盘组入库,0表示托盘组出库
       object Details(int page, int rows, string flag, string BILL_NO, string BILL_DATE, string BTYPE_CODE, string WAREHOUSE_CODE, string TARGET, string STATE, string OPERATER, string OPERATE_DATE, string TASKER, string TASK_DATE);
       //获取单据编号  prefix是单据编号的前缀字符
       object GetBillNo(string userName, DateTime dt, string BILL_NO, string prefix);
       //获取单据明细  
       object GetSubDetails(int page, int rows, string BillNo);
       //单据新增
       bool Add(WMS_PALLET_MASTER  mast, object detail, string prefix);
       //修改单据
       bool Edit(WMS_PALLET_MASTER mast, object detail);
       //删除单据
       bool Delete(string BillNo);
       //单据审核
       bool Audit(string checker, string BillNo);
       //反审
       bool Antitrial(string BillNo);
       //打印
       bool Print(string flag, string BILLNO, string BILLDATEFROM, string BILLDATETO, string BTYPECODE, string STATE);
    }
}
