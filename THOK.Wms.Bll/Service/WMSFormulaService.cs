using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.Bll.Models;
using System.Data;


namespace THOK.Wms.Bll.Service
{
    public class WMSFormulaService:ServiceBase<WMS_FORMULA_MASTER>, IWMSFormulaService
    {
        [Dependency]
        public IWMSFormulaDetailRepository DetailRepository { get; set; }

        [Dependency]
        public IWMSFormulaMasterRepository MasterRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }



        public object GetDetails(int page, int rows, string BTYPE_NAME, string BILL_TYPE, string TASK_LEVEL, string Memo, string TARGET_CODE)
        {
            IQueryable<WMS_FORMULA_MASTER> masterQuery = MasterRepository.GetQueryable();
            var masters = masterQuery.OrderByDescending(i => i.FORMULA_DATE).Select(i => i);

            int total = masters.Count();
            masters = masters.Skip((page - 1) * rows).Take(rows);
            var tmp = masters.ToArray().AsEnumerable().Select(i => new {
                i.FORMULA_CODE,
                i.FORMULA_DATE,
                FORMULADATE = i.FORMULA_DATE.ToString("yyyy-MM-dd HH:mm:ss"),
                i.FORMULA_NAME,
                i.CIGARETTE_CODE,
                i.CMD_CIGARETTE.CIGARETTE_NAME,
                i.IS_ACTIVE,
                ISACTIVE=i.IS_ACTIVE=="1"?"可用":"禁用",
                i.OPERATEDATE,
                 OPERATE_DATE=i.OPERATEDATE.ToString("yyyy-MM-dd HH:mm:ss"),
                i.OPERATER,
                i.USE_COUNT,
                i.FORMULANO
            });
            return new { total, rows = tmp.ToArray() };
        }

        public object GetSubDetails(int page, int rows, string FORMULA_CODE)
        {
            IQueryable<WMS_FORMULA_DETAIL> DetailQuery = DetailRepository.GetQueryable();
            var Details = DetailQuery.OrderBy(i => i.FORMULA_CODE).Select(i => new { i.FORMULA_CODE,i.PRODUCT_CODE,i.CMD_PRODUCT.GRADE_CODE ,i.CMD_PRODUCT.YEARS,i.CMD_PRODUCT.STYLE_NO,i.CMD_PRODUCT.PRODUCT_NAME,i.PERCENT,i.OTHER});
            Details = Details.Where(i => i.FORMULA_CODE == FORMULA_CODE);

            int total = Details.Count();
            Details = Details.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = Details.ToArray() };
        }

        public bool Add(WMS_FORMULA_MASTER master, object detail)
        {
            master.FORMULA_CODE = MasterRepository.GetNewID("FM", master.FORMULA_DATE, master.FORMULA_CODE);
            master.USE_COUNT = 0;
            master.FORMULANO = 10;
            master.OPERATEDATE = DateTime .Now ;
            MasterRepository.Add(master);

            DataTable dt = THOK.Common.JsonData.JsonToDataTable(((System.String[])detail)[0]);
            foreach (DataRow dr in dt.Rows)
            {
                WMS_FORMULA_DETAIL subdetail = new WMS_FORMULA_DETAIL();
                THOK.Common.JsonData.DataBind(subdetail, dr);
                subdetail.FORMULA_CODE = master.FORMULA_CODE;
                DetailRepository.Add(subdetail);
            }
            MasterRepository.SaveChanges();


            return true;
        }
        public bool Edit(WMS_FORMULA_MASTER master, object detail)
        {
            var editmaster = MasterRepository.GetQueryable().Where(i => i.FORMULA_CODE == master.FORMULA_CODE).FirstOrDefault();
            editmaster.FORMULA_NAME = master.FORMULA_NAME;
            editmaster.FORMULA_DATE = master.FORMULA_DATE;
            editmaster.IS_ACTIVE = master.IS_ACTIVE;
            editmaster.CIGARETTE_CODE = master.CIGARETTE_CODE;
            var details = DetailRepository.GetQueryable().Where(i => i.FORMULA_CODE == master.FORMULA_CODE);
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_FORMULA_DETAIL sub in tmp)
            {
                DetailRepository.Delete(sub);
            }

            DataTable dt = THOK.Common.JsonData.JsonToDataTable(((System.String[])detail)[0]); //修改
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    WMS_FORMULA_DETAIL subdetail = new WMS_FORMULA_DETAIL();
                    THOK.Common.JsonData.DataBind(subdetail, dr);
                    subdetail.FORMULA_CODE = master.FORMULA_CODE;
                    DetailRepository.Add(subdetail);
                }
            }
           
            MasterRepository.SaveChanges();


            return true;
        }

        public bool Delete(string FORMULA_CODE)
        {
            var editmaster = MasterRepository.GetQueryable().Where(i => i.FORMULA_CODE == FORMULA_CODE).FirstOrDefault();
            

            var details = DetailRepository.GetQueryable().Where(i => i.FORMULA_CODE == FORMULA_CODE);
            var tmp = details.ToArray().AsEnumerable().Select(i => i);
            foreach (WMS_FORMULA_DETAIL sub in tmp)
            {
                DetailRepository.Delete(sub);
            }
            MasterRepository.Delete(editmaster);
            MasterRepository.SaveChanges();
            return true;
          
        }

        public object GetFormulaCode(string userName, DateTime dt, string FORMULA_CODE)
        {
            var strCode = MasterRepository.GetNewID("FM", dt, FORMULA_CODE);
            var FormulaInfo =
                new
                {
                    userName = userName,
                    FormulaCode = strCode
                };
            return FormulaInfo;

        }

        //根据牌号获取配方.
        public object GetSubDetailbyCigarettecode(int page, int rows, string CIGARETTE_CODE)
        {
            IQueryable<WMS_FORMULA_MASTER> masterQuery = MasterRepository.GetQueryable();
            var masters = masterQuery.OrderByDescending(i => i.FORMULA_DATE).Select(i => i);
            masters = masters.Where(i => i.CIGARETTE_CODE == CIGARETTE_CODE);
            int total = masters.Count();
            masters = masters.Skip((page - 1) * rows).Take(rows);
            var tmp = masters.ToArray().AsEnumerable().Select(i => new
            {
                i.FORMULA_CODE,
                i.FORMULA_DATE,
                FORMULADATE = i.FORMULA_DATE.ToString("yyyy-MM-dd HH:mm:ss"),
                i.FORMULA_NAME,
                i.CIGARETTE_CODE,
                i.CMD_CIGARETTE.CIGARETTE_NAME,
                ISACTIVE = i.IS_ACTIVE == "1" ? "可用" : "禁用",
                OPERATE_DATE = i.OPERATEDATE.ToString("yyyy-MM-dd HH:mm:ss"),
                i.OPERATER,
                i.USE_COUNT,
                i.FORMULANO
            });
            return new { total, rows = tmp.ToArray() };
        }


        public object Getusefull(int page, int rows, string CIGARETTE_CODE)
        {
            IQueryable<WMS_FORMULA_MASTER> masterQuery = MasterRepository.GetQueryable();
            var masters = masterQuery.OrderByDescending(i => i.FORMULA_DATE).Select(i => i);
            masters = masters.Where(i => i.CIGARETTE_CODE == CIGARETTE_CODE);
            masters = masters.Where(i => i.IS_ACTIVE == "1");
            int total = masters.Count();
            masters = masters.Skip((page - 1) * rows).Take(rows);
            var tmp = masters.ToArray().AsEnumerable().Select(i => new
            {
                i.FORMULA_CODE,
                i.FORMULA_DATE,
                FORMULADATE = i.FORMULA_DATE.ToString("yyyy-MM-dd HH:mm:ss"),
                i.FORMULA_NAME,
                i.CMD_CIGARETTE.CIGARETTE_NAME,
                ISACTIVE = i.IS_ACTIVE == "1" ? "可用" : "禁用",
                OPERATE_DATE = i.OPERATEDATE.ToString("yyyy-MM-dd HH:mm:ss"),
                i.OPERATER,
                i.USE_COUNT,
                i.FORMULANO
            });
            return new { total, rows = tmp.ToArray() };
        }
    }
}
