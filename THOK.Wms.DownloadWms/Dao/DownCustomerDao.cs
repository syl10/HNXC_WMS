using System;
using System.Collections.Generic;
using System.Text;
using THOK.Util;
using System.Data;

namespace THOK.WMS.DownloadWms.Dao
{
    public class DownCustomerDao : BaseDao
    {

        /// <summary>
        /// 根据客户编码下载库户信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetCustomerInfo(string customerCode)
        {
            string sql = string.Format("SELECT * FROM V_WMS_CUSTOMER WHERE {0}", customerCode);
            return this.ExecuteQuery(sql).Tables[0];
        }


        /// <summary>
        /// 查询客户编码
        /// </summary>
        /// <returns></returns>
        public DataTable GetCustomerCode()
        {
            string sql = "SELECT customer_code FROM wms_customer";
            return this.ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 添加数据到数据库
        /// </summary>
        /// <param name="customerDt"></param>
        public void Insert(DataSet customerDs)
        {
            this.BatchInsert(customerDs.Tables["DWV_IORG_CUSTOMER"], "wms_customer");
        }
    }
}
