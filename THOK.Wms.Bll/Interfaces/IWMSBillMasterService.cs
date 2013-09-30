using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public  interface IWMSBillMasterService:IService<WMS_BILL_MASTER >
    {
        //获取单据记录  billtype 单据类型比如入库,出库等,flag为1时是入库,出库作业,为0不是入库,出库作业,为2是属于抽检补料入库单.
        object GetDetails(int page, int rows, string billtype, string flag, string BILL_NO, string BILL_DATE, string BTYPE_CODE, string WAREHOUSE_CODE, string BILL_METHOD, string CIGARETTE_CODE, string FORMULA_CODE, string STATE, string OPERATER, string OPERATE_DATE, string CHECKER, string CHECK_DATE, string STATUS, string BILL_DATEStar, string BILL_DATEEND, string SOURCE_BILLNO);
        //获取单据明细   flag  为0表示获取该单据的所有明细,为1表示获取该单据下的混装产品信息(即在设置混转第一产品时下拉框的数据).
        object GetSubDetails(int page, int rows, string BillNo,int flag);
        //单据审核
        bool Audit(string checker, string BillNo);
        //反审
        bool Antitrial(string BillNo);
        //获取单据编号  prefix是单据编号的前缀字符
        object GetBillNo(string userName, DateTime dt, string BILL_NO, string prefix);
        //单据新增
        bool Add(WMS_BILL_MASTER mast, object detail, string prefix);
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
        //查询需要补料的单据
        object billselect(int page, int rows, string billmethod,string billno);
        //抽检补料入库单添加
        bool FillBillAdd(WMS_BILL_MASTER mast, object detail, string prefix);
        //抽检补料入库单修改
        bool FillBillEdit(WMS_BILL_MASTER mast, object detail);
        //抽检补料入库单删除
        bool FillBillDelete(string BillNo);
        //抽检补料入库单作业
        bool FillBillTask(string BillNo, string tasker);
        //查询所有货位上不为空且处于解锁状态的货位,soursebill为入库批次 ,queryinfo为查询条件的信息
        object Cellselect(int page, int rows, string soursebill, string queryinfo, string selectedcellcodestr);
        //找出空的货位
        object GetNullCell(int page, int rows,string queryinfo);
        //盘点单新增
        bool InventoryAdd(WMS_BILL_MASTER mast, object detail, string prefix);
        //盘点单修改
        bool InventoryEdit(WMS_BILL_MASTER mast, object detail);
        //盘点单删除
        bool InventoryDelete(string BillNo);
        //盘点单作业
        bool InventoryTask(string BillNo, string tasker);
        //抽检单作业
        bool SamplingTask(string BillNo, string tasker);
        //移库单新增
        bool MoveStockAdd(WMS_BILL_MASTER mast, object detail, string prefix,out string error);
        //移库单修改
        bool MoveStockEdit(WMS_BILL_MASTER mast, object detail, out string error);
        //移库单删除
        bool MoveStockDelete(string BillNo);
        //移库单作业
        bool MoveStockTask(string BillNo, string tasker);
        //获取出库批次,用于紧急补料单中的出库批次
        object Outstockbill(int page, int rows,string queryinfo);
        //紧急补料单新增
        bool FeedingAdd(WMS_BILL_MASTER mast, object detail, string prefix);
        //紧急补料单修改
        bool FeedingEdit(WMS_BILL_MASTER mast, object detail);
    }
}
