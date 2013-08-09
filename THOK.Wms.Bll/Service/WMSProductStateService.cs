using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Data.Objects;

namespace THOK.Wms.Bll.Service
{
    class WMSProductStateService : ServiceBase<WMS_PRODUCT_STATE >, IWMSProductStateService
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public IWMSProductStateRepository  ProductStateRepository { get; set; }
        [Dependency]
        public ISysTableStateRepository SysTableStateRepository { get; set; }
        //public bool test() {
        //    OracleConnection ora = new OracleConnection(); 
        //}

        public object Details(int page, int rows,string billno)
        {
            IQueryable<WMS_PRODUCT_STATE> query = ProductStateRepository.GetQueryable();
            IQueryable<SYS_TABLE_STATE> statequery = SysTableStateRepository.GetQueryable();
            var details = from a in query
                          join b in statequery on a.IS_MIX equals b.STATE
                          where b.TABLE_NAME == "WMS_PRODUCT_STATE" && b.FIELD_NAME == "IS_MIX"
                          select new { 
                              a.BILL_NO ,
                              a.ITEM_NO,
                              a.SCHEDULE_NO,
                              a.PRODUCT_CODE ,
                              a.WEIGHT,
                              a.REAL_WEIGHT ,
                              a.PACKAGE_COUNT ,
                              a.OUT_BILLNO ,
                              a.CELL_CODE ,
                              a.NEWCELL_CODE ,
                              a.PRODUCT_BARCODE ,
                              a.PALLET_CODE ,
                              a.IS_MIX, //是否混装代码
                             IS_MIXDESC= b.STATE_DESC  //是否混装,文字显示
                          };

            var temp = details.Where(i => i.BILL_NO == billno).OrderBy(i => i.ITEM_NO).Select(i => i);
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };

        }

        //单据拆分为作业
        public bool Task(string billno, string btypecode, string tasker)
        {
            string sqlstr = "begin stockinwork('"+billno+"','"+btypecode+"','"+tasker+"');end;";
            int result = ProductStateRepository.Exeprocedure(sqlstr , null );
            //return ((ObjectContext)RepositoryContext).ExecuteStoreCommand("","");
            if (result < 0)
                return false;
            else
                return true;
           //((ObjectContext)RepositoryContext).ExecuteStoreCommand("", "");
        }
    }
}
