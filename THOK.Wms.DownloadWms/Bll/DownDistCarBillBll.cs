using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Util;
using THOK.Wms.DownloadWms.Dao;
using System.Data;
using THOK.WMS.DownloadWms;

namespace THOK.Wms.DownloadWms.Bll
{
   public class DownDistCarBillBll
    {
       public bool DownDistCarBillInfo(string orderDate)
       {
           bool tag = true;
           try
           {
               this.Delete();
               //DataTable distCarCodeTable = this.GetOrderInfo(orderDate);
               //string distList = UtinString.MakeString(distCarCodeTable, "DIST_BILL_ID");
               DataTable distCarTable = this.GetDistCarInfo("''");
               if (distCarTable.Rows.Count > 0)
               {
                   DataSet distCarDs = this.Insert(distCarTable);
                   this.Insert(distCarDs);
               }
               else
                   tag = false;
           }
           catch (Exception e)
           {
               throw new Exception("下载配车单表失败！原因：" + e.Message);
           }
           return tag;
       }


       public DataTable GetDistStationCode()
       {
           using (PersistentManager dbPm = new PersistentManager())
           {
               DownDistCarBillDao dao = new DownDistCarBillDao();
               return dao.GetDistStationCode();
           }
       }

       private DataSet Insert(DataTable routeCodeTable)
       {
           DataSet ds = this.GenerateEmptyTables();
           foreach (DataRow row in routeCodeTable.Rows)
           {
               DataRow routeDr = ds.Tables["WMS_ORD_DIST_BILL"].NewRow();
               routeDr["DIST_BILL_ID"] = row["DIST_BILL_ID"].ToString().Trim();
               routeDr["DELIVER_LINE_CODE"] = row["DELIVER_LINE_CODE"].ToString().Trim();
               routeDr["DELIVER_LINE_NAME"] = row["DELIVER_LINE_NAME"].ToString().Trim();
               routeDr["DELIVERYMAN_CODE"] = row["DELIVERYMAN_CODE"];
               routeDr["DELIVERYMAN_NAME"] = row["DELIVERYMAN_NAME"];
               ds.Tables["WMS_ORD_DIST_BILL"].Rows.Add(routeDr);
           }
           return ds;
       }

       /// <summary>
       /// 下载配车单
       /// </summary>
       /// <returns></returns>
       public DataTable GetDistCarInfo(string distCarCode)
       {
           using (PersistentManager dbPm = new PersistentManager("YXConnection"))
           {
               DownDistCarBillDao dao = new DownDistCarBillDao();
               dao.SetPersistentManager(dbPm);
               return dao.GetDistCarBillInfo(distCarCode);
           }
       }


       public DataTable GetOrderInfo(string orderDate)
       {
           using (PersistentManager dbPm = new PersistentManager("YXConnection"))
           {
               DownDistCarBillDao dao = new DownDistCarBillDao();
               dao.SetPersistentManager(dbPm);
               return dao.GetOrderInfo(orderDate);
           }
       }

       /// <summary>
       /// 保存到数据库
       /// </summary>
       /// <param name="distTable"></param>
       public void Insert(DataSet distTable)
       {
           using (PersistentManager dbPm = new PersistentManager())
           {
               DownDistCarBillDao dao = new DownDistCarBillDao();
               dao.Insert(distTable);
           }
       }

       /// <summary>
       /// 删除配车单
       /// </summary>
       public void Delete()
       {
           using (PersistentManager dbPm = new PersistentManager())
           {
               DownDistCarBillDao dao = new DownDistCarBillDao();
               dao.Delete();
           }
       }

       public DataSet GenerateEmptyTables()
       {
           DataSet ds = new DataSet();
           DataTable routeDt = ds.Tables.Add("WMS_ORD_DIST_BILL");
           routeDt.Columns.Add("DIST_BILL_ID");
           routeDt.Columns.Add("DELIVER_LINE_CODE");
           routeDt.Columns.Add("DELIVER_LINE_NAME");
           routeDt.Columns.Add("DELIVERYMAN_CODE");
           routeDt.Columns.Add("DELIVERYMAN_NAME");
           return ds;
       }
    }
}
