﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using System.Data;

namespace THOK.Wms.Bll.Service
{
    class WMSProductionMasterService : ServiceBase<WMS_PRODUCTION_MASTER>, IWMSProductionMasterService 
    {
        [Dependency]
        public IWMSProductionMasterRepository  ProductionmasterRepository { get; set; }
        [Dependency]
        public IWMSProductionDetailRepository ProeductiondetailRepository { get; set; }
        [Dependency]
        public ISysTableStateRepository SysTableStateRepository { get; set; }
        [Dependency]
        public ICMDProuductRepository ProductRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }



        public object GetDetails(int page, int rows, string BILL_NO, string BILL_DATE,  string WAREHOUSE_CODE,  string CIGARETTE_CODE, string FORMULA_CODE, string STATE, string OPERATER, string OPERATE_DATE, string CHECKER, string CHECK_DATE, string BILL_DATEStar, string BILL_DATEEND)
        {
            IQueryable<WMS_PRODUCTION_MASTER> query = ProductionmasterRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            var detail = from a in query
                       join b in statequery on a.STATE equals b.STATE
                       where b.TABLE_NAME == "WMS_PRODUCTION_MASTER" && b.FIELD_NAME == "STATE"
                       select new { 
                           a.BILL_NO ,
                           a.BILL_DATE,
                           a.SCHEDULE_NO ,
                           a.WAREHOUSE_CODE ,
                           a.CMD_WAREHOUSE .WAREHOUSE_NAME ,
                           a.CIGARETTE_CODE ,
                           a.CMD_CIGARETTE .CIGARETTE_NAME ,
                           a.FORMULA_CODE ,
                           a.WMS_FORMULA_MASTER .FORMULA_NAME,
                           a.BATCH_WEIGHT ,
                           a.IN_BILLNO ,
                           a.OUT_BILLNO ,
                           a.STATE ,
                           STATENAME=b.STATE_DESC ,
                           a.OPERATER ,
                           a.OPERATE_DATE ,
                           a.CHECK_DATE ,
                           a.CHECKER 
                       };
            if (!string.IsNullOrEmpty(BILL_NO))
            {
                detail = detail.Where(i => i.BILL_NO == BILL_NO);
            }
            if (!string.IsNullOrEmpty(BILL_DATE))
            {
                DateTime billdt = DateTime.Parse(BILL_DATE);
                detail = detail.Where(i => i.BILL_DATE.CompareTo(billdt) == 0);
            }
            if (!string.IsNullOrEmpty(WAREHOUSE_CODE))
            {
                detail = detail.Where(i => i.WAREHOUSE_CODE == WAREHOUSE_CODE);
            }
            if (!string.IsNullOrEmpty(CIGARETTE_CODE))
            {
                detail = detail.Where(i => i.CIGARETTE_CODE == CIGARETTE_CODE);
            }
            if (!string.IsNullOrEmpty(FORMULA_CODE))
            {
                detail = detail.Where(i => i.FORMULA_CODE == FORMULA_CODE);
            }
            if (!string.IsNullOrEmpty(STATE))
            {
                detail = detail.Where(i => i.STATE == STATE);
            }
            if (!string.IsNullOrEmpty(OPERATER))
            {
                detail = detail.Where(i => i.OPERATER.Contains(OPERATER));
            }
            if (!string.IsNullOrEmpty(OPERATE_DATE))
            {
                DateTime operatedt = DateTime.Parse(OPERATE_DATE);
                DateTime operatedt2 = operatedt.AddDays(1);
                detail = detail.Where(i => i.OPERATE_DATE.Value.CompareTo(operatedt) >= 0);
                detail = detail.Where(i => i.OPERATE_DATE.Value.CompareTo(operatedt2) < 0);
            }
            if (!string.IsNullOrEmpty(CHECKER))
            {
                detail = detail.Where(i => i.CHECKER.Contains(CHECKER));
            }
            if (!string.IsNullOrEmpty(CHECK_DATE))
            {
                DateTime checkdt = DateTime.Parse(CHECK_DATE);
                DateTime checkdt2 = checkdt.AddDays(1);
                detail = detail.Where(i => i.CHECK_DATE.Value.CompareTo(checkdt) >= 0);
                detail = detail.Where(i => i.CHECK_DATE.Value.CompareTo(checkdt2) < 0);
            }
            if (!string.IsNullOrEmpty(BILL_DATEStar))
            {
                DateTime datestare = DateTime.Parse(BILL_DATEStar);
                detail = detail.Where(i => i.BILL_DATE.CompareTo(datestare) >= 0);
            }
            if (!string.IsNullOrEmpty(BILL_DATEEND))
            {
                DateTime dateend = DateTime.Parse(BILL_DATEEND);
                detail = detail.Where(i => i.BILL_DATE.CompareTo(dateend) <= 0);
            }
           var  temp = detail.ToArray().OrderByDescending (i => i.OPERATE_DATE ).Select(i => new
            {
                i.BILL_NO,
                BILL_DATE = i.BILL_DATE.ToString("yyyy-MM-dd"),
                i.SCHEDULE_NO,
                i.WAREHOUSE_CODE,
                i.WAREHOUSE_NAME,
                i.CIGARETTE_CODE,
                i.CIGARETTE_NAME,
                i.FORMULA_CODE,
                i.FORMULA_NAME,
               i.BATCH_WEIGHT,
                i.IN_BILLNO,
                i.OUT_BILLNO,
                i.STATE,
                STATENAME = i.STATENAME ,
                i.OPERATER,
                OPERATE_DATE = i.OPERATE_DATE == null ? "" : ((DateTime)i.OPERATE_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                CHECK_DATE = i.CHECK_DATE == null ? "" : ((DateTime)i.CHECK_DATE).ToString("yyyy-MM-dd HH:mm:ss"),
                i.CHECKER 

            });
            int total = temp .Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
              
        }

        //审核
        public bool Audit(string checker, string billno)
        {
            var temp = ProductionmasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO == billno);
            if (temp != null)
            {
                temp.CHECK_DATE = DateTime.Now;
                temp.CHECKER = checker;
                temp.STATE = "2";
                ProductionmasterRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }
        //反审
        public bool Antitrial(string billno)
        {
            var temp = ProductionmasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO == billno);
            if (temp != null)
            {
                temp.CHECK_DATE = DateTime.Now;
                temp.CHECKER = "";
                temp.STATE = "1";
                ProductionmasterRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        //删除
        public bool Delete(string billno)
        {
            var editmaster = ProductionmasterRepository.GetQueryable().Where(i => i.BILL_NO  == billno ).FirstOrDefault();


            var details = ProeductiondetailRepository.GetQueryable().Where(i => i.BILL_NO  == billno );
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_PRODUCTION_DETAIL  sub in tmp)
            {
                ProeductiondetailRepository.Delete(sub);
            }
            ProductionmasterRepository.Delete(editmaster);
            ProductionmasterRepository.SaveChanges();
            return true;
        }

        //单据编号
        public object GetBillNo(string userName, DateTime dt, string BILL_NO)
        {
            var strCode = ProductionmasterRepository.GetNewID("DP", dt, BILL_NO);
            var BillnoInfo =
                new
                {
                    userName = userName,
                    BillNo = strCode
                };
            return BillnoInfo;
        }

        //新增
        public bool Add(WMS_PRODUCTION_MASTER mast, object detail)
        {
            bool rejust = false;
            int serial = 1;
            try
            {
                mast.BILL_NO = ProductionmasterRepository.GetNewID("DP", mast.BILL_DATE, mast.BILL_NO);
                mast.OPERATE_DATE = DateTime.Now;
                //mast.BILL_DATE = DateTime.Now;
                mast.STATE = "1"; //默认保存状态
                mast.IN_BILLNO = " ";
                mast.OUT_BILLNO = " ";
                ProductionmasterRepository.Add(mast);

                DataTable dt = THOK.Common.JsonData.JsonToDataTable(((System.String[])detail)[0]);
                foreach (DataRow dr in dt.Rows)
                {

                    WMS_PRODUCTION_DETAIL subdetail = new WMS_PRODUCTION_DETAIL();
                    THOK.Common.JsonData.DataBind(subdetail, dr);
                    subdetail.ITEM_NO = serial;
                    subdetail.BILL_NO = mast.BILL_NO;
                    //subdetail.IS_MIX = "0";
                    //subdetail.FPRODUCT_CODE = "";
                    ProeductiondetailRepository.Add(subdetail);
                    serial++;
                }

                ProductionmasterRepository.SaveChanges();
                rejust = true;
            }
            catch (Exception ex)
            {

            }
            return rejust;
        }

        //获取某个单据下明细
        public object GetSubDetails(int page, int rows, string BillNo)
        {
            IQueryable<WMS_PRODUCTION_DETAIL> detailquery =ProeductiondetailRepository.GetQueryable();
            //IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            IQueryable<CMD_PRODUCT> productquery = ProductRepository.GetQueryable();
            var billdetail = from a in detailquery
                             join c in productquery on a.PRODUCT_CODE equals c.PRODUCT_CODE
                             select new
                             {
                                 a.ITEM_NO,
                                 a.BILL_NO,
                                 a.PRODUCT_CODE,
                                 c.PRODUCT_NAME,
                                 c.YEARS ,
                                 c.CMD_PRODUCT_GRADE .GRADE_NAME ,
                                 c.CMD_PRODUCT_STYLE .STYLE_NAME ,
                                 c.CMD_PRODUCT_ORIGINAL .ORIGINAL_NAME ,
                                 c.CMD_PRODUCT_CATEGORY .CATEGORY_NAME ,
                                 a.WEIGHT,
                                 a.REAL_WEIGHT,
                                 a.PACKAGE_COUNT,
                                 a.NC_COUNT,
                                 TOTAL_WEIGHT = a.PACKAGE_COUNT * a.REAL_WEIGHT
                             };
            var temp = billdetail.ToArray().Where(i => i.BILL_NO == BillNo).OrderBy(i => i.ITEM_NO).Select(i => i);
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }

        //修改
        public bool Edit(WMS_PRODUCTION_MASTER mast, object detail)
        {
            var billmast = ProductionmasterRepository.GetQueryable().FirstOrDefault(i => i.BILL_NO == mast.BILL_NO);
            billmast.BILL_DATE = mast.BILL_DATE;
            billmast.WAREHOUSE_CODE = mast.WAREHOUSE_CODE;
                billmast.CIGARETTE_CODE = mast.CIGARETTE_CODE;
                billmast.FORMULA_CODE = mast.FORMULA_CODE;
                billmast.BATCH_WEIGHT = mast.BATCH_WEIGHT;
                var details = ProeductiondetailRepository.GetQueryable().Where(i => i.BILL_NO == mast.BILL_NO);
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_PRODUCTION_DETAIL  sub in tmp)
            {
                ProeductiondetailRepository.Delete(sub);
            }

            DataTable dt = THOK.Common.JsonData.JsonToDataTable(((System.String[])detail)[0]); //修改
            if (dt != null)
            {
                int serial = 1;
                foreach (DataRow dr in dt.Rows)
                {
                    WMS_PRODUCTION_DETAIL subdetail = new WMS_PRODUCTION_DETAIL();
                    THOK.Common.JsonData.DataBind(subdetail, dr);
                    subdetail.ITEM_NO = serial;
                    subdetail.BILL_NO = mast.BILL_NO;
                    ProeductiondetailRepository.Add(subdetail);
                    serial++;
                }
            }
            ProductionmasterRepository.SaveChanges();

            return true;
        }
    }
}