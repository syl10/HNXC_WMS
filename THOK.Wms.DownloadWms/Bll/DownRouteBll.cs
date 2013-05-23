using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;
using THOK.WMS.DownloadWms.Dao;

namespace THOK.WMS.DownloadWms.Bll
{
    public class DownRouteBll
    {
        #region 从营销系统下载线路信息

        /// <summary>
        /// 从营销系统下载线路信息
        /// </summary>
        /// <returns></returns>
        public bool DownRouteInfo()
        {
            bool tag = true;
            try
            {
                DataTable RouteCodeDt = this.GetRouteCode();
                string routeCodeList = UtinString.StringMake(RouteCodeDt, "deliver_line_code");
                routeCodeList = UtinString.StringMake(routeCodeList);
                DataTable RouteDt = this.GetRouteInfo("");

                if (RouteDt.Rows.Count > 0)
                {
                    DataTable routeTable = this.InsertRouteCode(RouteDt).Tables["DWV_OUT_DELIVER_LINE"];
                    DataRow[] line = routeTable.Select("DELIVER_LINE_CODE NOT IN(" + routeCodeList + ")");
                    if (line.Length > 0)
                    {
                        DataSet lineds = this.InsertRouteCode(line);
                        this.Insert(lineds);
                    }
                }
                else
                    tag = false;
            }
            catch (Exception e)
            {
                throw new Exception("从营销下载线路失败！原因：" + e.Message);
            }
            
            return tag;
        }
        /// <summary>
        /// 从营销系统下载线路信息 创联
        /// </summary>
        /// <returns></returns>
        public bool DownRouteInfos()
        {
            bool tag = true;
            DataTable RouteCodeDt = this.GetRouteCode();
            string routeCodeList = UtinString.StringMake(RouteCodeDt, "deliver_line_code");
            routeCodeList = UtinString.StringMake(routeCodeList);
            DataTable RouteDt = this.GetRouteInfos("");

            if (RouteDt.Rows.Count > 0)
            {
                DataTable routeTable = this.InsertRouteCodes(RouteDt).Tables["DWV_OUT_DELIVER_LINE"];
                DataRow[] line = routeTable.Select("DELIVER_LINE_CODE NOT IN(" + routeCodeList + ")");
                if (line.Length > 0)
                {
                    DataSet lineds = this.InsertRouteCode(line);
                    this.Insert(lineds);
                }
            }
            else
                tag = false;
            return tag;
        }
       /// <summary>
       /// 下载线路信息
       /// </summary>
       /// <returns></returns>
       public DataTable GetRouteInfo(string routeCodeList)
       {
           using (PersistentManager dbPm = new PersistentManager("YXConnection"))
           {
               DownRouteDao dao = new DownRouteDao();
               dao.SetPersistentManager(dbPm);
               return dao.GetRouteInfo(routeCodeList);
           }
       }
       /// <summary>
       /// 下载线路信息
       /// </summary>
       /// <returns></returns>
       public DataTable GetRouteInfos(string routeCodeList)
       {
           using (PersistentManager dbPm = new PersistentManager("YXConnection"))
           {
               DownRouteDao dao = new DownRouteDao();
               dao.SetPersistentManager(dbPm);
               return dao.GetRouteInfos(routeCodeList);
           }
       }

       /// <summary>
       /// 查询仓库线路编号
       /// </summary>
       /// <returns></returns>
       public DataTable GetRouteCode()
       {
           using (PersistentManager dbPm = new PersistentManager())
           {
               DownRouteDao dao = new DownRouteDao();
               return dao.GetRouteCode();
           }
       }

       /// <summary>
       /// 把虚拟表的数据添加到数据库
       /// </summary>
       /// <param name="ds"></param>
       public void Insert(DataSet ds)
       {
           using (PersistentManager dbPm = new PersistentManager())
           {
               DownRouteDao dao = new DownRouteDao();
               dao.Insert(ds);
           }
       }

       /// <summary>
       /// 添加数据到虚拟表中
       /// </summary>
       /// <param name="dr"></param>
       /// <returns></returns>
       private DataSet InsertRouteCode(DataTable routeCodeTable)
       {
           DataSet ds = this.GenerateEmptyTables();
           foreach (DataRow row in routeCodeTable.Rows)
           {
               DataRow routeDr = ds.Tables["DWV_OUT_DELIVER_LINE"].NewRow();
               routeDr["deliver_line_code"] = row["DIST_BILL_ID"].ToString();// +"_" + row["DELIVER_LINE_CODE"].ToString();
               routeDr["custom_code"] = row["LINE_TYPE"];
               routeDr["deliver_line_name"] = row["DELIVER_LINE_NAME"].ToString().Trim() + "--" + row["DELIVERYMAN_NAME"].ToString() + "";//row["DELIVER_LINE_NAME"].ToString();
               routeDr["dist_code"] = row["DIST_STA_CODE"];
               routeDr["deliver_order"] = row["DELIVER_LINE_ORDER"];
               routeDr["description"] = "";
               routeDr["is_active"] = row["ISACTIVE"];
               routeDr["update_time"] = DateTime.Now;
               routeDr["new_deliver_line_code"] = row["DELIVER_LINE_CODE"].ToString();
               ds.Tables["DWV_OUT_DELIVER_LINE"].Rows.Add(routeDr);
           }
           return ds;
       }
       /// <summary>
       /// 添加数据到虚拟表中 创联
       /// </summary>
       /// <param name="dr"></param>
       /// <returns></returns>
       private DataSet InsertRouteCodes(DataTable routeCodeTable)
       {
           DataSet ds = this.GenerateEmptyTables();
           foreach (DataRow row in routeCodeTable.Rows)
           {
               DataRow routeDr = ds.Tables["DWV_OUT_DELIVER_LINE"].NewRow();
               routeDr["deliver_line_code"] = row["DELIVER_LINE_CODE"].ToString();
               routeDr["custom_code"] = row["LINE_TYPE"];
               routeDr["deliver_line_name"] = row["DELIVER_LINE_NAME"].ToString();//row["DELIVER_LINE_NAME"].ToString().Trim() + "--" + row["DELIVERYMAN_NAME"].ToString() + "";
               routeDr["dist_code"] = row["DIST_STA_CODE"];
               routeDr["deliver_order"] = row["DELIVER_LINE_ORDER"];
               routeDr["description"] = "";
               routeDr["is_active"] = row["ISACTIVE"];
               routeDr["update_time"] = DateTime.Now;
               routeDr["new_deliver_line_code"] = row["DELIVER_LINE_CODE"].ToString();
               ds.Tables["DWV_OUT_DELIVER_LINE"].Rows.Add(routeDr);
           }
           return ds;
       }
       /// <summary>
       /// 添加数据到虚拟表中
       /// </summary>
       /// <param name="dr"></param>
       /// <returns></returns>
       private DataSet InsertRouteCode(DataRow[] routeCodeTable)
       {
           DataSet ds = this.GenerateEmptyTables();
           foreach (DataRow row in routeCodeTable)
           {
               DataRow routeDr = ds.Tables["DWV_OUT_DELIVER_LINE"].NewRow();
               routeDr["deliver_line_code"] = row["deliver_line_code"].ToString().Trim();
               routeDr["custom_code"] = row["custom_code"].ToString().Trim();
               routeDr["deliver_line_name"] = row["deliver_line_name"].ToString().Trim();
               routeDr["dist_code"] = row["dist_code"];
               routeDr["deliver_order"] = row["deliver_order"];
               routeDr["description"] = "";
               routeDr["is_active"] = row["is_active"];
               routeDr["update_time"] = DateTime.Now;
               routeDr["new_deliver_line_code"] = row["new_deliver_line_code"].ToString().Trim();
               ds.Tables["DWV_OUT_DELIVER_LINE"].Rows.Add(routeDr);
           }
           return ds;
       }

        /// <summary>
        /// 缓存中构建虚拟表
        /// </summary>
        /// <returns></returns>
        public DataSet GenerateEmptyTables()
        {
            DataSet ds = new DataSet();
            DataTable routeDt = ds.Tables.Add("DWV_OUT_DELIVER_LINE");
            routeDt.Columns.Add("deliver_line_code");
            routeDt.Columns.Add("custom_code");
            routeDt.Columns.Add("deliver_line_name");
            routeDt.Columns.Add("dist_code");
            routeDt.Columns.Add("deliver_order");
            routeDt.Columns.Add("description");
            routeDt.Columns.Add("is_active");
            routeDt.Columns.Add("update_time");
            routeDt.Columns.Add("new_deliver_line_code");
            return ds;
        }
        #endregion


        /// <summary>
        /// 从分拣下载线路信息
        /// </summary>
        /// <returns></returns>
        public bool DownSortRouteInfo()
        {
            bool tag = true;
            try
            {
                DataTable RouteCodeDt = this.GetRouteCode();
                string routeCodeList = UtinString.StringMake(RouteCodeDt, "deliver_line_code");
                routeCodeList = UtinString.StringMake(routeCodeList);
                DataTable RouteDt = this.GetSortRouteInfo("");

                if (RouteDt.Rows.Count > 0)
                {
                    DataTable routeTable = this.InsertSortRouteCode(RouteDt).Tables["DWV_OUT_DELIVER_LINE"];
                    DataRow[] line = routeTable.Select("DELIVER_LINE_CODE NOT IN(" + routeCodeList + ")");
                    if (line.Length > 0)
                    {
                        DataSet lineds = this.InsertRouteCode(line);
                        this.Insert(lineds);
                    }
                }
                else
                    tag = false;
            }
            catch (Exception e)
            {
                throw new Exception("从分拣下载线路信息失败！原因：" + e.Message);
            }           
            return tag;
        }

        /// <summary>
        /// 添加数据到虚拟表中
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private DataSet InsertSortRouteCode(DataTable routeCodeTable)
        {
            DataSet ds = this.GenerateEmptyTables();
            foreach (DataRow row in routeCodeTable.Rows)
            {
                DataRow routeDr = ds.Tables["DWV_OUT_DELIVER_LINE"].NewRow();
                routeDr["deliver_line_code"] = row["DIST_BILL_ID"].ToString().Trim();// +"_" + row["DIST_BILL_ID"].ToString().Trim();
                routeDr["custom_code"] = row["DELIVERLINECODE"].ToString().Trim();
                routeDr["deliver_line_name"] = row["DELIVERLINENAME"].ToString().Trim() + "--" + row["DELIVERYMAN_NAME"].ToString() + "";//row["DELIVERLINENAME"].ToString();
                routeDr["dist_code"] = row["DELIVERLINECODE"];
                routeDr["deliver_order"] = 0;
                routeDr["description"] = "";
                routeDr["is_active"] = "1";
                routeDr["update_time"] = DateTime.Now;
                routeDr["new_deliver_line_code"] = row["DELIVERLINECODE"].ToString().Trim();
                ds.Tables["DWV_OUT_DELIVER_LINE"].Rows.Add(routeDr);
            }
            return ds;
        }


        /// <summary>
        /// 从分拣线下载线路信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetSortRouteInfo(string routeCodeList)
        {
            using (PersistentManager dbPm = new PersistentManager())
            {
                DownRouteDao dao = new DownRouteDao();
                return dao.GetSortRouteInfo(routeCodeList);
            }
        }

        /// <summary>
        /// 删除7天之前的线路表，分拣中间表和分拣表(包含细表)，作业调度表
        /// </summary>
        public void DeleteTable()
        {
            try
            {
                using (PersistentManager dbPm = new PersistentManager())
                {
                    DownRouteDao dao = new DownRouteDao();
                    dao.DeleteTable();
                }
            }
            catch (Exception e)
            {
                throw new Exception("删除数据失败！原因：" + e.Message);
            }
        }
    }
}
