using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using THOK.Util;
using THOK.Wms.DownloadWms.Dao;
using THOK.WMS.DownloadWms;

namespace THOK.Wms.DownloadWms.Bll
{
   public class DownDistStationBll
    {
       /// <summary>
       /// 下载配送区域表
       /// </summary>
       /// <returns></returns>
       public bool DownDistStationInfo()
       {
           bool tag = true;
           try
           {
               DataTable distStationTable = this.GetDistStationCode();
               string distList = UtinString.MakeString(distStationTable, "dist_code");
               DataTable distTable = this.GetDistInfo(distList);
               if (distTable.Rows.Count > 0)
               {
                   DataSet distds = Insert(distTable);
                   this.Insert(distds);
               }
               else
                   tag = false;
           }
           catch (Exception e)
           {
               throw new Exception("下载区域表失败！原因：" + e.Message);
           }
           return tag;
       }

       /// <summary>
       /// 下载配送信息
       /// </summary>
       /// <param name="routeCodeList"></param>
       /// <returns></returns>
       public DataTable GetDistInfo(string routeCodeList)
       {
           using (PersistentManager dbPm = new PersistentManager("YXConnection"))
           {
               DownDistStationBao dao = new DownDistStationBao();
               dao.SetPersistentManager(dbPm);
               return dao.GetDistStationInfo(routeCodeList);
           }
       }

       /// <summary>
       /// 查询本地配送编码
       /// </summary>
       /// <returns></returns>
       public DataTable GetDistStationCode()
       {
           using (PersistentManager dbPm = new PersistentManager())
           {
               DownDistStationBao dao = new DownDistStationBao();
               return dao.GetDistStationCode();
           }
       }

       public void Insert(DataSet ds)
       {
           using (PersistentManager dbPm = new PersistentManager())
           {
               DownDistStationBao dao = new DownDistStationBao();
               dao.Insert(ds);
           }
       }

       private DataSet Insert(DataTable routeCodeTable)
       {
           DataSet ds = this.GenerateEmptyTables();
           foreach (DataRow row in routeCodeTable.Rows)
           {
               DataRow routeDr = ds.Tables["WMS_DELIVER_DIST"].NewRow();
               routeDr["dist_code"] = row["DIST_STA_CODE"].ToString().Trim();
               routeDr["custom_code"] = row["DIST_STA_N"].ToString().Trim();
               routeDr["dist_name"] = row["DIST_STA_NAME"].ToString().Trim();
               routeDr["dist_center_code"] = row["DIST_CTR_CODE"];
               routeDr["company_code"] = row["ORG_CODE"];
               routeDr["uniform_code"] = row["N_ORG_CODE"];
               routeDr["description"] = "";
               routeDr["is_active"] = row["ISACTIVE"];
               routeDr["update_time"] = DateTime.Now;
               ds.Tables["WMS_DELIVER_DIST"].Rows.Add(routeDr);
           }
           return ds;
       }

       public DataSet GenerateEmptyTables()
       {
           DataSet ds = new DataSet();
           DataTable routeDt = ds.Tables.Add("WMS_DELIVER_DIST");
           routeDt.Columns.Add("dist_code");
           routeDt.Columns.Add("custom_code");
           routeDt.Columns.Add("dist_name");
           routeDt.Columns.Add("dist_center_code");
           routeDt.Columns.Add("company_code");
           routeDt.Columns.Add("uniform_code");
           routeDt.Columns.Add("description");
           routeDt.Columns.Add("is_active");
           routeDt.Columns.Add("update_time");
           return ds;
       }
    }
}
