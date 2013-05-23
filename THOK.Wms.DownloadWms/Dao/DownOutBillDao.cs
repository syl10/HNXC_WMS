using System;
using System.Collections.Generic;
using System.Text;
using THOK.Util;
using System.Data;

namespace THOK.WMS.DownloadWms.Dao
{
    public class DownOutBillDao : BaseDao
    {
        /// <summary>
        /// 从营销系统下载出库单主表单据数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetOutBillMaster(string outBillNo)
        {
            string sql = string.Format("SELECT * FROM V_WMS_OUT_ORDER WHERE {0} AND ORDER_TYPE='70'", outBillNo);
            return this.ExecuteQuery(sql).Tables[0];
        }
        /// <summary>
        /// 从营销系统下载出库单主表单据数据  创联
        /// </summary>
        /// <returns></returns>
        public DataTable GetOutBillMasters(string outBillNo)
        {
            string sql = string.Format("SELECT * FROM V_WMS_OUT_ORDER WHERE {0} AND ORDER_TYPE='1003'", outBillNo);
            return this.ExecuteQuery(sql).Tables[0];
        }
        /// <summary>
        /// 从营销系统下载出库单明细表数据
        /// </summary>
        /// <returns></returns>
        public DataTable OutBillDetail(string outBillNo)
        {
            string sql = string.Format("SELECT * FROM V_WMS_OUT_ORDER_DETAIL WHERE {0}", outBillNo);
            return this.ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 从营销系统下载出库单明细表数据,合单数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetOutBillDetail(string outBillNo)
        {
            string sql = string.Format(@"SELECT BRAND_CODE,SUM(QUANTITY) AS QUANTITY,PRICE FROM V_WMS_OUT_ORDER_DETAIL 
                                        WHERE {0} GROUP BY BRAND_CODE,PRICE ORDER BY BRAND_CODE", outBillNo);
            return this.ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 添加主表数据到表 WMS_OUT_BILLMASTER
        /// </summary>
        /// <param name="ds"></param>
        public void InsertOutBillMaster(DataSet ds)
        {
            foreach (DataRow row in ds.Tables["WMS_OUT_BILLMASTER"].Rows)
            {
                string sql = "INSERT INTO WMS_OUT_BILL_MASTER(bill_no,bill_date,bill_type_code,warehouse_code,status,is_active,update_time,operate_person_id,origin" +
                   ") VALUES('" + row["bill_no"] + "','" + row["bill_date"] + "','" + row["bill_type_code"] + "'," +
                   "'" + row["warehouse_code"] + "','" + row["status"] + "','" + row["is_active"] + "','" + row["update_time"] + "','" + row["operate_person_id"] + "','" + row["origin"] + "')";
                this.ExecuteNonQuery(sql);
            }
        }

        /// <summary>
        /// 查询当前登陆的操作员
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public DataTable FindEmployee(string userName)
        {
            string sql = "SELECT * FROM WMS_EMPLOYEE WHERE USER_NAME='" + userName + "'";
            return this.ExecuteQuery(sql).Tables[0];
        } 
        /// <summary>
        /// 添加明细表数据到表 WMS_OUT_BILLDETAIL
        /// </summary>
        /// <param name="ds"></param>
        public void InsertOutBillDetail(DataSet ds)
        {
            if (ds.Tables["WMS_OUT_BILLDETAILA"].Rows.Count > 0)
            {
                BatchInsert(ds.Tables["WMS_OUT_BILLDETAILA"], "wms_out_bill_detail");
            }
        }

        /// <summary>
        /// 插入数据到中间表
        /// </summary>
        /// <param name="ds"></param>
        public void InsertMiddle(DataSet ds)
        {
            BatchInsert(ds.Tables["WMS_MIDDLE_OUT_BILLDETAIL"], "WMS_MIDDLE_OUT_BILL");
        }

        /// <summary>
        /// 获取单号
        /// </summary>
        /// <returns></returns>
        public DataSet FindOutBillNo(string orderDate)
        {
            string sql = string.Format("SELECT TOP 1 BILL_NO FROM WMS_OUT_BILL_MASTER WHERE BILL_NO LIKE '{0}%' ORDER BY BILL_NO DESC", orderDate);
            return this.ExecuteQuery(sql);
        }


        /// <summary>
        /// 根据时间查询仓库的出库单据号
        /// </summary>
        /// <returns></returns>
        public DataTable GetOutBillNo(string orderDate)
        {
            string sql = "SELECT BILL_NO FROM WMS_MIDDLE_OUT_BILL WHERE BILL_DATE='" + orderDate + "'";
            return this.ExecuteQuery(sql).Tables[0];
        }

    }
}
