using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using THOK.Util;
using THOK.Wms.DownloadWms.Dao;

namespace THOK.Wms.DownloadWms.Bll
{
   public class DownBusinessSystemsDailyBalanceBll
    {
        /// <summary>
        /// 下载日结信息
        /// </summary>
        /// <returns></returns>
       public bool DownDayEndInfo(string SettleDate)
        {
            bool tag = true;
            try
            {
                if (SettleDate == string.Empty || SettleDate == null)
                {
                    SettleDate = DateTime.Now.ToString("yyyyMMdd");
                }
                else
                {
                    SettleDate = Convert.ToDateTime(SettleDate).ToString("yyyyMMdd");
                }
                string parameter = " SettleDate='" + SettleDate + "'";
                this.DeleteDayEnd(SettleDate);
                DataTable dayEnd = this.GetDayEndInfo(parameter);
                if (dayEnd.Rows.Count > 0)
                {
                    DataSet dayEndDs = this.SaveDayEnd(dayEnd);
                    this.Insert(dayEndDs);
                }
            }
            catch (Exception e)
            {
                throw new Exception("下载日结失败！原因：" + e.Message);
            }
            return tag;
        }

        /// <summary>
        /// 读取下载日结信息
        /// </summary>
        /// <returns></returns>
       public DataTable GetDayEndInfo(string parameter)
        {
            using (PersistentManager dbPm = new PersistentManager("YXConnection"))
            {
                DownBusinessSystemsDailyBalanceDao dao = new DownBusinessSystemsDailyBalanceDao();
                dao.SetPersistentManager(dbPm);
                return dao.FindDayEnd(parameter);
            }
        }

       public DataSet SaveDayEnd(DataTable eayEndTable)
       {
           DownBusinessSystemsDailyBalanceDao dao = new DownBusinessSystemsDailyBalanceDao();
           DataTable unitList = dao.GetUnitProduct();
           DataSet ds = this.GenerateEmptyTables();
           foreach (DataRow row in eayEndTable.Rows)
           {
               DataRow[] unitRow = unitList.Select(string.Format("unit_list_code='{0}'", row["BRAND_N"].ToString().Trim()));
               DataRow masterrow = ds.Tables["WMS_BUSINESS_SYSTEMS_DAILY_BALANCE"].NewRow();
               //masterrow["id"] = row["DailyBalanceID"].ToString().Trim();
               masterrow["settle_date"] = row["SettleDate"].ToString().Trim();
               masterrow["warehouse_code"] = row["WarehouseCode"].ToString().Trim();
               masterrow["product_code"] = row["BRAND_N"].ToString().Trim();
               masterrow["unit_code"] = unitRow[0]["unit_code04"].ToString();
               masterrow["beginning"] = Convert.ToDecimal(row["Beginning"]);
               masterrow["entry_amount"] = Convert.ToDecimal(row["EntryAmount"]);
               masterrow["delivery_amount"] = Convert.ToDecimal(row["DeliveryAmount"]);
               masterrow["profit_amount"] = Convert.ToDecimal(row["ProfitAmount"]);
               masterrow["loss_amount"] = Convert.ToDecimal(row["LossAmount"]);
               masterrow["ending"] = Convert.ToDecimal(row["Ending"]);
               ds.Tables["WMS_BUSINESS_SYSTEMS_DAILY_BALANCE"].Rows.Add(masterrow);
           }
           return ds;
       }

       public void Insert(DataSet dispatchDs)
       {
           using (PersistentManager pm = new PersistentManager())
           {
               DownBusinessSystemsDailyBalanceDao dao = new DownBusinessSystemsDailyBalanceDao();
               if (dispatchDs.Tables["WMS_BUSINESS_SYSTEMS_DAILY_BALANCE"].Rows.Count > 0)
               {
                   dao.InsertDayEnd(dispatchDs);
               }
           }
       }

       public void DeleteDayEnd(string SettleDate)
       {
           using (PersistentManager pm = new PersistentManager())
           {
               DownBusinessSystemsDailyBalanceDao dao = new DownBusinessSystemsDailyBalanceDao();
               dao.Delete(SettleDate);               
           }
       }

        /// <summary>
        /// 构建一个日结虚拟表
        /// </summary>
        /// <returns></returns>
        private DataSet GenerateEmptyTables()
        {
            DataSet ds = new DataSet();
            DataTable inbrtable = ds.Tables.Add("WMS_BUSINESS_SYSTEMS_DAILY_BALANCE");
            //inbrtable.Columns.Add("id");
            inbrtable.Columns.Add("settle_date");
            inbrtable.Columns.Add("warehouse_code");
            inbrtable.Columns.Add("product_code");
            inbrtable.Columns.Add("unit_code");
            inbrtable.Columns.Add("beginning");
            inbrtable.Columns.Add("entry_amount");
            inbrtable.Columns.Add("delivery_amount");
            inbrtable.Columns.Add("profit_amount");
            inbrtable.Columns.Add("loss_amount");
            inbrtable.Columns.Add("ending");
            return ds;
        }
    }
}
