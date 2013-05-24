using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DownloadWms.Dao;
using THOK.Util;
using System.Data;
using THOK.WMS.DownloadWms;
using THOK.WMS.DownloadWms.Dao;

namespace THOK.Wms.DownloadWms.Bll
{
    public class DownDecidePlanBll 
    {
        private string Employee = "";
        #region 选择日期从决策系统下载入库数据

        /// <summary>
        /// 根据日期下载入库数据 zxl   2012-09-14 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public bool GetInBillMiddle(string startDate, string endDate, string EmployeeCode, string wareCode, string billtype, out string errorInfo)
        {
            bool tag = false;
            Employee = EmployeeCode;
            errorInfo = string.Empty;
            using (PersistentManager pm = new PersistentManager())
            {
                try
                {
                    DownDecidePlanDao dao = new DownDecidePlanDao();
                    DataTable emply = dao.FindEmployee(EmployeeCode);
                    DataTable inMasterBillNo = this.GetMiddleBillNo();
                    string billnolist = UtinString.MakeString(inMasterBillNo, "bill_no");
                    billnolist = string.Format("BB_INPUT_DATE >='{0}' AND BB_UUID NOT IN({1})", startDate, billnolist);
                    DataTable masterdt = this.GetMiddleInBillMaster(billnolist);

                    string inDetailList = UtinString.MakeString(masterdt, "BILL_NO");
                    inDetailList = "BD_BB_UUID IN(" + inDetailList + ")";
                    DataTable detaildt = this.GetMiddleInBillDetail(inDetailList);

                    if (masterdt.Rows.Count > 0 && detaildt.Rows.Count > 0)
                    {
                        try
                        {
                            DataSet middleds = this.MiddleTable(masterdt);
                            DataSet masterds = this.MiddleInBillMaster(masterdt, emply.Rows[0]["employee_id"].ToString(), wareCode, billtype);
                            DataSet detailds = this.MiddleInBillDetail(detaildt);
                            this.Insert(masterds, detailds, middleds);
                            tag = true;
                        }
                        catch (Exception e)
                        {
                            errorInfo = "保存错误：" + e.Message;
                        }
                    }
                    else
                        errorInfo = "没有新的入库单下载！";
                }
                catch (Exception ex)
                {
                    errorInfo ="下载入库数据失败！原因："+ ex.Message;
                }
            }
            return tag;
        }

        /// <summary>
        /// 下载入库单主表数据zxl   2012-09-14 
        /// </summary>
        /// <returns></returns>
        public DataTable GetMiddleInBillMaster(string inBillNoList)
        {
            using (PersistentManager dbpm = new PersistentManager("ZYJCConnection"))
            {
                DownDecidePlanDao dao = new DownDecidePlanDao();
                dao.SetPersistentManager(dbpm);
                return dao.GetMiddleInBillMaster(inBillNoList);
            }
        }

        /// <summary>
        /// 下载入库单明细表数据zxl   2012-09-14 
        /// </summary>
        /// <returns></returns>
        public DataTable GetMiddleInBillDetail(string inBillNoList)
        {
            using (PersistentManager dbpm = new PersistentManager("ZYJCConnection"))
            {
                DownDecidePlanDao dao = new DownDecidePlanDao();
                dao.SetPersistentManager(dbpm);
                return dao.GetMiddleInBillDetail(inBillNoList);
            }
        }

        /// <summary>
        /// 把入库主表数据保存在虚拟表中zxl   2012-09-14 
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public DataSet MiddleInBillMaster(DataTable inBillMasterdr, string employeeId, string wareCode, string billType)
        {
            DataSet ds = this.GenerateEmptyTables();
            foreach (DataRow row in inBillMasterdr.Rows)
            {
                Guid eid = new Guid(employeeId);
                string bill = row["BILL_NO"].ToString().Trim();
                bill = bill.Substring(2, 4) + bill.Substring(11, 4) + bill.Substring(22, 4);
                DataRow masterrow = ds.Tables["WMS_IN_BILLMASTER"].NewRow();
                masterrow["bill_no"] = bill;
                masterrow["bill_date"] = Convert.ToDateTime(row["BILL_DATE"]);
                masterrow["bill_type_code"] = billType;//row["ORDER_TYPE"].ToString().Trim();
                masterrow["warehouse_code"] = wareCode;//row["DIST_CTR_CODE"].ToString().Trim();
                masterrow["status"] = "1";
                masterrow["verify_date"] = null;
                masterrow["is_active"] = "1";
                masterrow["update_time"] = DateTime.Now;
                masterrow["operate_person_id"] = eid;
                //masterrow["verify_person_id"] = null;
                masterrow["description"] = "";
                masterrow["lock_tag"] = "";
                masterrow["target_cell_code"] = null;
                ds.Tables["WMS_IN_BILLMASTER"].Rows.Add(masterrow);
            }
            return ds;
        }

        /// <summary>
        /// 把入库明细单数据保存在虚拟表,zxl   2012-09-14 
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public DataSet MiddleInBillDetail(DataTable inBillDetaildr)
        {
            DataSet ds = this.GenerateEmptyTables();
            foreach (DataRow row in inBillDetaildr.Rows)
            {
                string bill = row["BILL_NO"].ToString().Trim();
                bill = bill.Substring(2, 4) + bill.Substring(11, 4) + bill.Substring(22, 4);
                DataTable prodt = FindUnitListCode(row["PRODUCT_CODE"].ToString());//                
                DataRow detailrow = ds.Tables["WMS_IN_BILLDETAIL"].NewRow();
                detailrow["bill_no"] = bill;
                detailrow["product_code"] = row["PRODUCT_CODE"].ToString();
                detailrow["price"] = prodt.Rows[0]["TRADE_PRICE"];
                detailrow["bill_quantity"] = Convert.ToDecimal(Convert.ToDecimal(row["BARQUANTITY"]) * Convert.ToDecimal(prodt.Rows[0]["quantity02"]) * Convert.ToDecimal(prodt.Rows[0]["quantity03"]));
                detailrow["allot_quantity"] = 0;
                detailrow["unit_code"] = prodt.Rows[0]["UNIT_CODE"];
                detailrow["description"] = "";
                detailrow["real_quantity"] = 0;
                ds.Tables["WMS_IN_BILLDETAIL"].Rows.Add(detailrow);
            }
            return ds;
        }


        /// <summary>
        /// 把数据添加到中间表。zxl   2012-09-14 
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public DataSet MiddleTable(DataTable inBillMaster)
        {
            DataSet ds = this.GenerateEmptyTables();
            foreach (DataRow row in inBillMaster.Rows)
            {
                string bill = row["BILL_NO"].ToString().Trim();
                bill = bill.Substring(2, 4) + bill.Substring(11, 4) + bill.Substring(22, 4);
                DataRow detailrow = ds.Tables["WMS_MIDDLE_IN_BILLDETAIL"].NewRow();
                detailrow["bill_no"] = row["bill_no"];
                detailrow["bill_date"] = row["bill_date"];
                detailrow["in_bill_no"] = bill;
                ds.Tables["WMS_MIDDLE_IN_BILLDETAIL"].Rows.Add(detailrow);
            }
            return ds;
        }

       

        /// <summary>
        /// 根据卷烟编码查询单位信息
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
        /// 根据卷烟编码查询计量单位信息
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public DataTable FindUnitListCode(string product)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                DownDecidePlanDao dao = new DownDecidePlanDao();
                return dao.FindUnitListCode(product);
            }
        }

        /// <summary>
        /// 把查询的数据添加到仓储数据库
        /// </summary>
        /// <param name="masterds"></param>
        /// <param name="detailds"></param>
        public void Insert(DataSet masterds, DataSet detailds, DataSet middleds)
        {
            try
            {
                using (PersistentManager pm = new PersistentManager())
                {
                    DownDecidePlanDao dao = new DownDecidePlanDao();
                    if (masterds.Tables["WMS_IN_BILLMASTER"].Rows.Count > 0)
                    {
                        dao.InsertInBillMaster(masterds);
                    }
                    if (detailds.Tables["WMS_IN_BILLDETAIL"].Rows.Count > 0)
                    {
                        dao.InsertInBillDetail(detailds);
                    }
                    if (middleds.Tables["WMS_MIDDLE_IN_BILLDETAIL"].Rows.Count > 0)
                    {
                        dao.InsertMiddle(middleds);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }

        /// <summary>
        /// 把查询的数据添加到仓储数据库
        /// </summary>
        /// <param name="middleds"></param>
        /// <param name="detailds"></param>
        public void InsertMiddle(DataSet middleds)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                DownDecidePlanDao dao = new DownDecidePlanDao();
                
            }
        }

        /// <summary>
        /// 删除中间表数据
        /// </summary>
        /// <param name="billno">单号</param>
        public void DeleteMiddleBill(string billno)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                DownDecidePlanDao dao = new DownDecidePlanDao();
                dao.DeleteMiddleBill(billno);
            }
        }

        /// <summary>
        /// 查询数字仓储中间表4天内入库单 zxl   2012-09-14 
        /// </summary>
        /// <returns></returns>
        public DataTable GetMiddleBillNo()
        {
            using (PersistentManager pm = new PersistentManager())
            {
                DownDecidePlanDao dao = new DownDecidePlanDao();
                return dao.GetMiddleBillNo();
            }
        }

        /// <summary>
        /// 构建入库虚拟表
        /// </summary>
        /// <returns></returns>
        private DataSet GenerateEmptyTables()
        {
            DataSet ds = new DataSet();
            DataTable mastertable = ds.Tables.Add("WMS_IN_BILLMASTER");
            mastertable.Columns.Add("bill_no");
            mastertable.Columns.Add("bill_date");
            mastertable.Columns.Add("bill_type_code");
            mastertable.Columns.Add("warehouse_code");
            mastertable.Columns.Add("status");
            mastertable.Columns.Add("verify_date");
            mastertable.Columns.Add("description");
            mastertable.Columns.Add("is_active");
            mastertable.Columns.Add("update_time");
            mastertable.Columns.Add("operate_person_id");
            mastertable.Columns.Add("verify_person_id");
            mastertable.Columns.Add("lock_tag");
            mastertable.Columns.Add("row_version");
            mastertable.Columns.Add("target_cell_code");

            DataTable detailtable = ds.Tables.Add("WMS_IN_BILLDETAIL");
            detailtable.Columns.Add("id");
            detailtable.Columns.Add("bill_no");
            detailtable.Columns.Add("product_code");
            detailtable.Columns.Add("unit_code");
            detailtable.Columns.Add("price");
            detailtable.Columns.Add("bill_quantity");
            detailtable.Columns.Add("allot_quantity");
            detailtable.Columns.Add("real_quantity");
            detailtable.Columns.Add("description");


            DataTable middletable = ds.Tables.Add("WMS_MIDDLE_IN_BILLDETAIL");
            middletable.Columns.Add("bill_no");
            middletable.Columns.Add("bill_date");
            middletable.Columns.Add("in_bill_no");
            return ds;
        }
        #endregion
    }
}
