using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using System.Data;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;

namespace THOK.Wms.Bll.Service
{
    class WMSPalletMasterService : ServiceBase<WMS_PALLET_MASTER >, IWMSPalletMasterService 
    {
        [Dependency]
        public IWMSPalletMasterRepository  PalletmasterRepository { get; set; }
        [Dependency]
        public IWMSPalletDetailRepository PalletdetailRepository { get; set; }
        [Dependency]
        public ISysTableStateRepository SysTableStateRepository { get; set; }
        [Dependency]
        public ICmdBillTypeRepository CmdBillTypeRepository { get; set; }
        [Dependency]
        public ICMDWarehouseRepository CMDWarehouseRepository { get; set; }
        [Dependency]
        public ISysBillTargetRepository SysBillTargetRepository { get; set; }
        [Dependency]
        public ICMDProuductRepository ProductRepository { get; set; }
        [Dependency]
        public IUserRepository UserRepository { get; set; }
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        //
        public object Details(int page, int rows, string flag,string BILL_NO, string BILL_DATE, string BTYPE_CODE, string WAREHOUSE_CODE, string TARGET, string STATE, string OPERATER, string OPERATE_DATE, string TASKER, string TASK_DATE)
        {
            IQueryable<WMS_PALLET_MASTER> palletquery = PalletmasterRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> tatequery = SysTableStateRepository.GetQueryable();
            IQueryable<CMD_BILL_TYPE> btypequery = CmdBillTypeRepository.GetQueryable();
            IQueryable<SYS_BILL_TARGET> billtargetquery = SysBillTargetRepository.GetQueryable();
            IQueryable<CMD_WAREHOUSE> warehousequery = CMDWarehouseRepository.GetQueryable();
            IQueryable<AUTH_USER> userquery = UserRepository.GetQueryable();
            var master = from a in palletquery
                       join b in tatequery on a.STATUS equals b.STATE
                       join c in tatequery on a.STATE equals c.STATE
                       join d in btypequery on a.BTYPE_CODE equals d.BTYPE_CODE 
                       join h in userquery on a.OPERATER equals h.USER_ID 
                       join  j in userquery on a.TASKER equals j.USER_ID into k from j in k.DefaultIfEmpty ()
                      join f in warehousequery on a.WAREHOUSE_CODE equals f.WAREHOUSE_CODE
                       join e in billtargetquery on a.TARGET equals e.TARGET_CODE into g
                       from e in g.DefaultIfEmpty ()
                       where b.TABLE_NAME == "WMS_PALLET_MASTER" && b.FIELD_NAME == "STATUS" && c.TABLE_NAME == "WMS_PALLET_MASTER" & c.FIELD_NAME == "STATE"
                       select new { 
                           a.BILL_NO ,
                           a.BILL_DATE ,
                           a.BTYPE_CODE ,
                           d.BTYPE_NAME ,
                           d.BILL_TYPE,
                           a.WAREHOUSE_CODE ,
                           f.WAREHOUSE_NAME ,
                           a.TARGET ,
                           e.TARGET_NAME ,
                           a.STATUS ,
                           STATUSNAME= b.STATE_DESC ,
                           a.STATE ,
                           STATENAME= c.STATE_DESC ,
                           OPERATER=h.USER_NAME  ,
                           a.OPERATE_DATE ,
                           TASKER=j.USER_NAME  ,
                           a.TASK_DATE 
                       };
            if (!string.IsNullOrEmpty(BILL_NO))
            {
                master = master.Where(i => i.BILL_NO == BILL_NO);
            }
            if (!string.IsNullOrEmpty(BILL_DATE))
            {
                DateTime dt = DateTime.Parse(BILL_DATE);
                master = master.Where(i => i.BILL_DATE.CompareTo(dt) == 0);
            }
            if (!string.IsNullOrEmpty(BTYPE_CODE))
            {
                master = master.Where(i => i.BTYPE_CODE == BTYPE_CODE);
            }
            if (!string.IsNullOrEmpty(WAREHOUSE_CODE))
            {
                master = master.Where(i => i.WAREHOUSE_CODE == WAREHOUSE_CODE);
            }
            if (!string.IsNullOrEmpty(TARGET))
            {
                master = master.Where(i => i.TARGET == TARGET);
            }
            if (!string.IsNullOrEmpty(STATE))
            {
                master = master.Where(i => i.STATE == STATE);
            }
            if (!string.IsNullOrEmpty(OPERATER))
            {
                master = master.Where(i => i.OPERATER.Contains (OPERATER));
            }
            if (!string.IsNullOrEmpty(OPERATE_DATE))
            {
                DateTime dt=DateTime .Parse (OPERATE_DATE );
                DateTime dt2=dt.AddDays (1);
                master = master.Where(i => i.OPERATE_DATE.Value.CompareTo(dt) >= 0);
                master = master.Where(i => i.OPERATE_DATE.Value.CompareTo(dt2) < 0);
            }
            if (!string.IsNullOrEmpty(TASKER))
            {
                master = master.Where(i => i.TASKER.Contains (TASKER));
            }
            if (!string.IsNullOrEmpty(TASK_DATE))
            {
                DateTime dt = DateTime.Parse(TASK_DATE);
                DateTime dt2 = dt.AddDays(1);
                master = master.Where(i => i.TASK_DATE.Value.CompareTo(dt) >= 0);
                master = master.Where(i => i.TASK_DATE.Value.CompareTo(dt2) < 0);
            }
            var temp = master.ToArray ().OrderByDescending(i => i.OPERATE_DATE  ).Select(i => new { 
                i.BILL_NO ,
               BILL_DATE= i.BILL_DATE .ToString ("yyyy-MM-dd"),
               i.BTYPE_CODE ,
               i.BTYPE_NAME ,
               i.BILL_TYPE ,
               i.WAREHOUSE_CODE ,
               i.WAREHOUSE_NAME ,
               i.TARGET ,
               i.TARGET_NAME ,
               i.STATUS ,
               i.STATUSNAME ,
               i.STATE ,
               i.STATENAME ,
               i.OPERATER ,
               OPERATE_DATE = i.OPERATE_DATE == null ? "" : ((DateTime)i.OPERATE_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
               i.TASKER ,
               TASK_DATE = i.TASK_DATE == null ? "" : ((DateTime)i.TASK_DATE).ToString("yyyy-MM-dd HH:mm:ss")
            });
            if (flag == "1") //入库类型
            {
                temp = temp.Where(i => i.BILL_TYPE != "9");
            }
            else
            {
                temp = temp.Where(i => i.BILL_TYPE == "9");
            }
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }
        //获取单据编号
        public object GetBillNo(string userName, DateTime dt, string BILL_NO, string prefix)
        {
            var strCode = PalletmasterRepository.GetNewID(prefix, dt, BILL_NO);
            var BillnoInfo =
                new
                {
                    userName = userName,
                    BillNo = strCode
                };
            return BillnoInfo;
        }


        public bool Add(WMS_PALLET_MASTER  mast, object detail, string prefix)
        {
            var targetcode = CmdBillTypeRepository.GetQueryable().FirstOrDefault(i => i.BTYPE_CODE == mast.BTYPE_CODE);
            bool rejust = false;
            int serial = 1;
            try
            {
                mast.BILL_NO = PalletmasterRepository.GetNewID(prefix, mast.BILL_DATE, mast.BILL_NO);
                mast.OPERATE_DATE = DateTime.Now;
                //mast.BILL_DATE = DateTime.Now;
                mast.STATE = "1"; //默认保存状态
                mast.STATUS = "0"; //默认手工输入
                mast.TARGET = targetcode.TARGET_CODE=="null"?"":targetcode .TARGET_CODE;
                PalletmasterRepository.Add(mast);

                DataTable dt = THOK.Common.JsonData.JsonToDataTable(((System.String[])detail)[0]);
                foreach (DataRow dr in dt.Rows)
                {

                    WMS_PALLET_DETAIL subdetail = new WMS_PALLET_DETAIL();
                    THOK.Common.JsonData.DataBind(subdetail, dr);
                    subdetail.ITEM_NO = serial;
                    subdetail.BILL_NO = mast.BILL_NO;
                    PalletdetailRepository.Add(subdetail);
                    serial++;
                }

                int brs = PalletmasterRepository.SaveChanges();
                if (brs == -1) rejust = false;
                else
                    rejust = true;
            }
            catch (Exception ex)
            {

            }
            return rejust;
        }


        public object GetSubDetails(int page, int rows, string BillNo)
        {
            IQueryable<WMS_PALLET_DETAIL> palletdetailquery = PalletdetailRepository.GetQueryable();
            //IQueryable<CMD_PRODUCT> productquery = ProductRepository.GetQueryable();
            //var detail = from a in palletdetailquery
            //             join b in productquery on a.PRODUCT_CODE equals b.PRODUCT_CODE
            //             select new { 
            //                 a.BILL_NO ,
            //                 a.ITEM_NO,
            //                 a.PRODUCT_CODE ,
            //                 b.PRODUCT_NAME ,
            //                 a.QUANTITY ,
            //                 a.PACKAGES 
            //             };
            var temp = palletdetailquery.ToArray().Where(i => i.BILL_NO == BillNo).OrderBy(i => i.ITEM_NO).Select(i => i);
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }


        public bool Edit(WMS_PALLET_MASTER mast, object detail)
        {
            var billmast = PalletmasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO == mast.BILL_NO);
            var targetcode = CmdBillTypeRepository.GetQueryable().FirstOrDefault(i => i.BTYPE_CODE == mast.BTYPE_CODE);
            billmast.BILL_DATE = mast.BILL_DATE;
            billmast.WAREHOUSE_CODE = mast.WAREHOUSE_CODE;
            billmast.BTYPE_CODE = mast.BTYPE_CODE;
            billmast.TARGET = targetcode.TARGET_CODE;

            var details = PalletdetailRepository.GetQueryable().Where(i => i.BILL_NO == mast.BILL_NO);
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_PALLET_DETAIL  sub in tmp)
            {
                PalletdetailRepository.Delete(sub);
            }

            DataTable dt = THOK.Common.JsonData.JsonToDataTable(((System.String[])detail)[0]); //修改
            if (dt != null)
            {
                int serial = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    WMS_PALLET_DETAIL subdetail = new WMS_PALLET_DETAIL();
                    THOK.Common.JsonData.DataBind(subdetail, dr);
                    subdetail.ITEM_NO = serial;
                    subdetail.BILL_NO = mast.BILL_NO;
                    PalletdetailRepository.Add(subdetail);
                    serial++;
                }
            }
            int result = PalletmasterRepository.SaveChanges();
            if (result == -1) return false;
            else
                return true;
        }


        public bool Delete(string BillNo)
        {
            var deletbillno = PalletmasterRepository.GetQueryable().Where(i => i.BILL_NO == BillNo).FirstOrDefault();
            var details = PalletdetailRepository.GetQueryable().Where(i => i.BILL_NO == BillNo);
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_PALLET_DETAIL sub in tmp)
            {
                PalletdetailRepository.Delete(sub);
            }
            PalletmasterRepository.Delete(deletbillno);
            int result = PalletmasterRepository.SaveChanges();
            if (result == -1) return false;
            return true;
        }


        public bool Audit(string checker, string BillNo)
        {
            throw new NotImplementedException();
        }

        public bool Antitrial(string BillNo)
        {
            throw new NotImplementedException();
        }
    }
}
