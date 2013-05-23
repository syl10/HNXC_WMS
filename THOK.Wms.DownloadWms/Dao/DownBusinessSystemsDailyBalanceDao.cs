using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Util;
using System.Data;

namespace THOK.Wms.DownloadWms.Dao
{
   public class DownBusinessSystemsDailyBalanceDao:BaseDao
   {
       #region 下载日表
       public DataTable FindDayEnd(string parameter)
       {
           string sql = string.Format(@"SELECT D.* , B.BRAND_N FROM V_WMS_DAILYBALANCE D
                                        LEFT JOIN V_WMS_BRAND B ON D.PRODUCTCODE=B.BRAND_CODE WHERE {0}", parameter);
           return this.ExecuteQuery(sql).Tables[0];
       }

       public void InsertDayEnd(DataSet ds)
       {
           foreach (DataRow row in ds.Tables["WMS_BUSINESS_SYSTEMS_DAILY_BALANCE"].Rows)
           {
               string sql = "INSERT INTO WMS_BUSINESS_SYSTEMS_DAILY_BALANCE(settle_date,warehouse_code,product_code,unit_code,beginning,entry_amount,delivery_amount,profit_amount,loss_amount,ending" +
                  ") VALUES('" + row["settle_date"] + "','" + "0101" + "','" + row["product_code"] + "'," +
                  "'" + row["unit_code"] + "'," + row["beginning"] + "," + row["entry_amount"] + "," + row["delivery_amount"] + "," + row["profit_amount"] + "," + row["loss_amount"] + "," + row["ending"] + ")";
               this.ExecuteNonQuery(sql);
           }
       }

       public void Delete(string settledate)
       {
           string sql = "DELETE WMS_BUSINESS_SYSTEMS_DAILY_BALANCE WHERE SETTLE_DATE ='" + settledate + "'";
           this.ExecuteNonQuery(sql);
       }

       public DataTable GetUnitProduct()
       {
           string sql = "SELECT * FROM WMS_UNIT_LIST";
           return this.ExecuteQuery(sql).Tables[0];
       }

       #endregion 
   }
}
