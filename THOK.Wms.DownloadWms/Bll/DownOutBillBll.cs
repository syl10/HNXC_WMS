using System;
using System.Collections.Generic;
using System.Text;
using THOK.Util;
using System.Data;
using System.Threading;
using THOK.WMS.DownloadWms.Dao;

namespace THOK.WMS.DownloadWms.Bll
{
    public class DownOutBillBll
    {
        private string Employee = "";

        #region 日期从营系统据下载数据

        /// <summary>
        /// 选择日期从营销系统下载出库单据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public bool GetOutBill(string startDate, string endDate, string EmployeeCode, out string errorInfo, string wareCode, string billType)
        {
            bool tag = true;
            Employee = EmployeeCode;
            errorInfo = string.Empty;
            using (PersistentManager dbpm = new PersistentManager())
            {
                DownOutBillDao dao = new DownOutBillDao();
                DataTable emply = dao.FindEmployee(EmployeeCode);   
                DataTable outBillNoTable = this.GetOutBillNo(startDate);
                string outBillList = UtinString.MakeString(outBillNoTable, "bill_no");
                outBillList = string.Format("ORDER_DATE ='{0}' AND ORDER_ID NOT IN({1}) ", startDate, outBillList);
                DataTable masterdt = this.GetOutBillMaster(outBillList);

                string outDetailList = UtinString.MakeString(masterdt, "ORDER_ID");
                outDetailList = "ORDER_ID IN(" + outDetailList + ")";
                DataTable detaildt = this.GetOutBillDetail(outDetailList);

                if (masterdt.Rows.Count > 0 && detaildt.Rows.Count > 0)
                {
                    try
                    {
                        string billno = this.GetNewBillNo();
                        DataSet middleds = this.MiddleTable(masterdt,billno);
                        //DataSet masterds = this.OutBillMaster(masterdt, emply.Rows[0]["employee_id"].ToString(), wareCode, billType);
                        DataSet detailds = this.OutBillDetail(detaildt, emply.Rows[0]["employee_id"].ToString(), wareCode, billType, startDate,billno);
                        this.Insert(detailds, middleds);
                    }
                    catch (Exception e)
                    {
                        errorInfo += e.Message;
                        tag = false;
                    }
                }
                else
                {
                    errorInfo = "没有可下载的出库数据！";
                    tag = false;
                }
            }
            return tag;
        }

        /// <summary>
        /// 选择日期从营销系统下载出库单据 创联
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public bool GetOutBills(string startDate, string endDate, string EmployeeCode, out string errorInfo, string wareCode, string billType)
        {
            bool tag = true;
            Employee = EmployeeCode;
            errorInfo = string.Empty;
            using (PersistentManager dbpm = new PersistentManager())
            {
                DownOutBillDao dao = new DownOutBillDao();
                DataTable emply = dao.FindEmployee(EmployeeCode);
                DataTable outBillNoTable = this.GetOutBillNo(startDate);
                string outBillList = UtinString.MakeString(outBillNoTable, "bill_no");
                outBillList = string.Format("ORDER_DATE ='{0}' AND ORDER_ID NOT IN({1}) ", startDate, outBillList);
                DataTable masterdt = this.GetOutBillMasters(outBillList);

                string outDetailList = UtinString.MakeString(masterdt, "ORDER_ID");
                outDetailList = "ORDER_ID IN(" + outDetailList + ")";
                DataTable detaildt = this.GetOutBillDetail(outDetailList);

                if (masterdt.Rows.Count > 0 && detaildt.Rows.Count > 0)
                {
                    try
                    {
                        string billno = this.GetNewBillNo();
                        DataSet middleds = this.MiddleTable(masterdt, billno);
                        //DataSet masterds = this.OutBillMaster(masterdt, emply.Rows[0]["employee_id"].ToString(), wareCode, billType);
                        DataSet detailds = this.OutBillDetail(detaildt, emply.Rows[0]["employee_id"].ToString(), wareCode, billType, startDate, billno);
                        this.Insert(detailds, middleds);
                    }
                    catch (Exception e)
                    {
                        errorInfo += e.Message;
                        tag = false;
                    }
                }
                else
                {
                    errorInfo = "没有可下载的出库数据！";
                    tag = false;
                }
            }
            return tag;
        }
        /// <summary>
        /// 根据时间查询仓库出库单据号
        /// </summary>
        /// <returns></returns>
        public DataTable GetOutBillNo(string orderDate)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                DownOutBillDao dao = new DownOutBillDao();
                return dao.GetOutBillNo(orderDate);
            }
        }

        /// <summary>
        /// 下载出库主表信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetOutBillMaster(string billno)
        {
            using (PersistentManager dbpm = new PersistentManager("YXConnection"))
            {
                DownOutBillDao dao = new DownOutBillDao();
                dao.SetPersistentManager(dbpm);
                return dao.GetOutBillMaster(billno);
            }
        }
        /// <summary>
        /// 下载出库主表信息 创联
        /// </summary>
        /// <returns></returns>
        public DataTable GetOutBillMasters(string billno)
        {
            using (PersistentManager dbpm = new PersistentManager("YXConnection"))
            {
                DownOutBillDao dao = new DownOutBillDao();
                dao.SetPersistentManager(dbpm);
                return dao.GetOutBillMasters(billno);
            }
        }
        /// <summary>
        /// 下载出库明细表信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetOutBillDetail(string billno)
        {
            using (PersistentManager dbpm = new PersistentManager("YXConnection"))
            {
                DownOutBillDao dao = new DownOutBillDao();
                dao.SetPersistentManager(dbpm);
                return dao.GetOutBillDetail(billno);
            }
        }

        /// <summary>
        /// 保存主表信息到虚拟表
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public DataSet OutBillMaster(DataTable dt, string emplcode, string wareCode, string billType)
        {
            DataSet ds = this.GenerateEmptyTables();
            Guid eid = new Guid(emplcode);
            foreach (DataRow row in dt.Rows)
            {
                string createdate = row["ORDER_DATE"].ToString();
                createdate = createdate.Substring(0, 4) + "-" + createdate.Substring(4, 2) + "-" + createdate.Substring(6, 2);
                DataRow masterrow = ds.Tables["WMS_OUT_BILLMASTER"].NewRow();
                masterrow["bill_no"] = row["ORDER_ID"].ToString().Trim();
                masterrow["bill_date"] = Convert.ToDateTime(createdate);
                masterrow["bill_type_code"] = billType;
                masterrow["warehouse_code"] = wareCode;
                masterrow["operate_person_id"] = eid;
                masterrow["status"] = "1";
                masterrow["is_active"] = "1";
                masterrow["update_time"] = DateTime.Now;
                masterrow["origin"] = "1";
                ds.Tables["WMS_OUT_BILLMASTER"].Rows.Add(masterrow);

            }
            return ds;
        }

        /// <summary>
        /// 保存订单主表和细表
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public DataSet OutBillDetail(DataTable outDetailTable,string emplcode, string wareCode, string billType,string orderDate,string billno)
        {
            DataSet ds = this.GenerateEmptyTables();
            Guid eid = new Guid(emplcode);            
            orderDate = orderDate.Substring(0, 4) + "-" + orderDate.Substring(4, 2) + "-" + orderDate.Substring(6, 2);
            DataRow masterrow = ds.Tables["WMS_OUT_BILLMASTER"].NewRow();
            masterrow["bill_no"] = billno;
            masterrow["bill_date"] = Convert.ToDateTime(orderDate);
            masterrow["bill_type_code"] = billType;
            masterrow["warehouse_code"] = wareCode;
            masterrow["operate_person_id"] = eid;
            masterrow["status"] = "1";
            masterrow["is_active"] = "1";
            masterrow["update_time"] = DateTime.Now;
            masterrow["origin"] = "1";
            ds.Tables["WMS_OUT_BILLMASTER"].Rows.Add(masterrow);

            foreach (DataRow row in outDetailTable.Rows)
            {
                DataTable prodt = FindProductCodeInfo(" CUSTOM_CODE='" + row["BRAND_CODE"].ToString() + "'");
                DataRow detailrow = ds.Tables["WMS_OUT_BILLDETAILA"].NewRow();
                detailrow["bill_no"] = billno;
                detailrow["product_code"] = prodt.Rows[0]["product_code"];
                detailrow["price"] = row["PRICE"] == "" ? 0 : row["PRICE"];
                detailrow["bill_quantity"] = Convert.ToDecimal(row["QUANTITY"]);
                detailrow["allot_quantity"] = 0;
                detailrow["unit_code"] = prodt.Rows[0]["unit_code"];
                detailrow["real_quantity"] = 0;
                ds.Tables["WMS_OUT_BILLDETAILA"].Rows.Add(detailrow);
            }
            return ds;
        }


        /// <summary>
        /// 把数据添加到中间表。zxl   2012-09-14 
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public DataSet MiddleTable(DataTable inBillMaster,string billno)
        {
            DataSet ds = this.GenerateEmptyTables();
            foreach (DataRow row in inBillMaster.Rows)
            {
                string orderDate = row["ORDER_DATE"].ToString();
                orderDate = orderDate.Substring(0, 4) + "-" + orderDate.Substring(4, 2) + "-" + orderDate.Substring(6, 2);
                DataRow detailrow = ds.Tables["WMS_MIDDLE_OUT_BILLDETAIL"].NewRow();
                detailrow["bill_no"] = row["ORDER_ID"];
                detailrow["bill_date"] = Convert.ToDateTime(orderDate);
                detailrow["in_bill_no"] = billno;
                ds.Tables["WMS_MIDDLE_OUT_BILLDETAIL"].Rows.Add(detailrow);
            }
            return ds;
        }

        /// <summary>
        /// 根据卷烟查询单位信息
        /// </summary>
        /// <param name="productCode"></param>
        /// <returns></returns>
        public DataTable FindProductCodeInfo(string productCode)
        {
            using (PersistentManager dbPm = new PersistentManager())
            {
                DownProductDao dao = new DownProductDao();
                return dao.FindProductCodeInfo(productCode);
            }
        }

        /// <summary>
        /// 把下载的数据添加到数据库。
        /// </summary>
        /// <param name="masterds"></param>
        /// <param name="detailds"></param>
        public void Insert(DataSet detailds,DataSet middleds)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                DownOutBillDao dao = new DownOutBillDao();
                try
                {
                    if (middleds.Tables["WMS_MIDDLE_OUT_BILLDETAIL"].Rows.Count > 0)
                    {
                        dao.InsertMiddle(middleds);
                    }
                    if (detailds.Tables["WMS_OUT_BILLMASTER"].Rows.Count > 0)
                    {
                        dao.InsertOutBillMaster(detailds);
                    }
                    if (detailds.Tables["WMS_OUT_BILLDETAILA"].Rows.Count > 0)
                    {
                        dao.InsertOutBillDetail(detailds);
                    }
                }
                catch (Exception exp)
                {
                    throw new Exception(exp.Message);
                }
            }
        }

        /// <summary>
        /// 生成出库单号
        /// </summary>
        /// <returns></returns>
        public string GetNewBillNo()
        {
            using (PersistentManager persistentManager = new PersistentManager())
            {
                DownOutBillDao dao = new DownOutBillDao();
                DataSet ds = dao.FindOutBillNo(System.DateTime.Now.ToString("yyMMdd"));
                if (ds.Tables[0].Rows.Count == 0)
                {
                    return System.DateTime.Now.ToString("yyMMdd") + "0001" + "OU";
                }
                else
                {
                    int i = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString().Substring(6, 4));
                    i++;
                    string newcode = i.ToString();
                    for (int j = 0; j < 4 - i.ToString().Length; j++)
                    {
                        newcode = "0" + newcode;
                    }
                    return System.DateTime.Now.ToString("yyMMdd") + newcode + "OU";
                }
            }
        }


        /// <summary>
        /// 创建虚拟表
        /// </summary>
        /// <returns></returns>
        private DataSet GenerateEmptyTables()
        {
            DataSet ds = new DataSet();
            DataTable mastertable = ds.Tables.Add("WMS_OUT_BILLMASTER");
            mastertable.Columns.Add("bill_no");
            mastertable.Columns.Add("bill_date");
            mastertable.Columns.Add("bill_type_code");
            mastertable.Columns.Add("warehouse_code");
            mastertable.Columns.Add("status");
            mastertable.Columns.Add("verify_person_id");
            mastertable.Columns.Add("verify_date");
            mastertable.Columns.Add("description");
            mastertable.Columns.Add("is_active");
            mastertable.Columns.Add("update_time");
            mastertable.Columns.Add("operate_person_id");
            mastertable.Columns.Add("lock_tag");
            mastertable.Columns.Add("row_version");
            mastertable.Columns.Add("move_bill_master_bill_no");
            mastertable.Columns.Add("origin");
            mastertable.Columns.Add("target_cell_code");

            DataTable detailtable = ds.Tables.Add("WMS_OUT_BILLDETAILA");
            detailtable.Columns.Add("id");
            detailtable.Columns.Add("bill_no");
            detailtable.Columns.Add("product_code");
            detailtable.Columns.Add("unit_code");
            detailtable.Columns.Add("price");
            detailtable.Columns.Add("bill_quantity");
            detailtable.Columns.Add("allot_quantity");
            detailtable.Columns.Add("real_quantity");
            detailtable.Columns.Add("description");


            DataTable middletable = ds.Tables.Add("WMS_MIDDLE_OUT_BILLDETAIL");
            middletable.Columns.Add("bill_no");
            middletable.Columns.Add("bill_date");
            middletable.Columns.Add("in_bill_no");
            return ds;
        }

        #endregion

    }
}
