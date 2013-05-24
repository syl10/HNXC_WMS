using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Util;
using THOK.Wms.DownloadWms.Dao;
using System.Data;
using THOK.WMS.DownloadWms;
using THOK.WMS.Upload.Bll;

namespace THOK.Wms.DownloadWms.Bll
{
    public class DownSortingInfoBll
    {
        UploadBll upload = new UploadBll();
        //选择时间下载分拣数据
        public bool GetSortingOrderDate(string startDate, string endDate,string sortingLine,string batch, out string errorInfo)
        {
            bool tag = false;
            errorInfo = string.Empty;
            using (PersistentManager dbpm = new PersistentManager())
            {
                DownSortingInfoDao dao = new DownSortingInfoDao();
                try
                {
                    string sort = string.Empty;
                    if (sortingLine != string.Empty || sortingLine != null)
                    {
                        sort = " AND SORTINGLINECODE='" + sortingLine + "' AND BATCHID='" + batch + "'";
                    }
                    //查询仓库7天内的订单号
                    DataTable orderdt = this.GetOrderId(startDate, endDate);
                    string orderlist = UtinString.MakeString(orderdt, "order_id");
                    string orderlistDate = "ORDERDATE >='" + startDate + "' AND ORDERDATE <='" + endDate + "'" + sort;
                    DataTable masterdt = this.GetSortingOrder(orderlistDate);
                    DataRow[] masterdr = masterdt.Select("ORDERID NOT IN(" + orderlist + ")");

                    string ordermasterlist = UtinString.MakeString(masterdr, "OrderID");                
                    ordermasterlist = "OrderID IN (" + ordermasterlist + ")";
                    DataTable detaildt = this.GetSortingOrderDetail(ordermasterlist);
                    if (masterdr.Count() > 0 && detaildt.Rows.Count > 0)
                    {
                        DataSet masterds = this.SaveSortingOrder(masterdr);
                        DataSet detailds = this.SaveSortingOrderDetail(detaildt);
                        this.Insert(masterds, detailds);
                        //上报分拣订单
                        //upload.uploadSort(masterds, detailds);
                        if (sort != string.Empty)
                        {
                            try
                            {
                                DataTable diapLine = this.GetDispatchLine(ordermasterlist);
                                DataSet dispDs = this.SaveDispatch(diapLine, sortingLine);
                                this.Insert(dispDs);
                                tag = true;
                            }
                            catch (Exception e)
                            {
                                errorInfo = "调度出错,请手动进行线路调度，出错原因：" + e.Message;
                            }
                            
                        }
                        else
                            errorInfo = "没有选择分拣线！下载完成后，请手动进行线路调度！";
                        //tag = true;
                    }
                    else
                        errorInfo = "没有可用的数据下载！";
                }
                catch (Exception e)
                {
                    errorInfo = "下载错误：" + e.Message;
                }
            }
            return tag;
        }

        //查询数仓3天内分拣订单
        public DataTable GetOrderId(string startDate, string endDate)
        {
            using (PersistentManager dbpm = new PersistentManager())
            {
                DownSortingInfoDao dao = new DownSortingInfoDao();
                return dao.GetOrderId(startDate, endDate);
            }
        }

        //下载分拣主表订单
        public DataTable GetSortingOrder(string parameter)
        {
            using (PersistentManager dbpm = new PersistentManager())
            {
                DownSortingInfoDao dao = new DownSortingInfoDao();
                return dao.GetSortingOrder(parameter);
            }
        }

        //下载调度信息
        public DataTable GetDispatchLine(string parameter)
        {
            using (PersistentManager dbpm = new PersistentManager())
            {
                DownSortingInfoDao dao = new DownSortingInfoDao();
                return dao.GetDispatchLine(parameter);
            }
        }

        //下载分拣细表订单
        public DataTable GetSortingOrderDetail(string parameter)
        {
            using (PersistentManager dbpm = new PersistentManager())
            {
                DownSortingInfoDao dao = new DownSortingInfoDao();
                //dao.SetPersistentManager(dbpm);
                return dao.GetSortingDetail(parameter);
            }
        }

        //保存主表信息
        public DataSet SaveSortingOrder(DataRow[] masterdt)
        {
            DataSet ds = this.GenerateEmptyTables();
            foreach (DataRow row in masterdt.ToArray())
            {
                DataRow masterrow = ds.Tables["WMS_SORT_ORDER"].NewRow();
                masterrow["order_id"] = row["OrderID"].ToString().Trim();//订单编号
                masterrow["company_code"] = row["COMPANY_CODE"].ToString().Trim(); //所属单位编号
                masterrow["sale_region_code"] = row["SALE_REGION_CODE"].ToString().Trim();//营销部编号
                masterrow["order_date"] = row["OrderDate"].ToString();//订单日期
                masterrow["order_type"] = row["SALE_SCOPE"].ToString().Trim();//订单类型
                masterrow["customer_code"] = row["CustomerCode"].ToString().Trim();//客户编号
                masterrow["customer_name"] = row["CustomerName"].ToString().Trim();//客户名称
                masterrow["quantity_sum"] = Convert.ToDecimal(row["QuantitySum"].ToString());//总数量
                masterrow["amount_sum"] = 0;
                masterrow["detail_num"] = 0;
                masterrow["deliver_order"] = row["DeliverOrder"];
                masterrow["DeliverDate"] = DateTime.Now.ToString("yyyyMMdd");
                masterrow["description"] = "";
                masterrow["is_active"] = "1";
                masterrow["update_time"] = DateTime.Now;
                masterrow["deliver_line_code"] = row["DIST_BILL_ID"].ToString();// row["DELIVERLINECODE"].ToString();// +"_" + row["DIST_BILL_ID"].ToString();
                masterrow["dist_bill_id"] = row["DIST_BILL_ID"].ToString();
                ds.Tables["WMS_SORT_ORDER"].Rows.Add(masterrow);
            }
            return ds;
        }

        //保存细表信息
        public DataSet SaveSortingOrderDetail(DataTable detaildt)
        {
            DataSet ds = this.GenerateEmptyTables();
            try
            {
                foreach (DataRow row in detaildt.Rows)
                {
                    DataRow detailrow = ds.Tables["WMS_SORT_ORDER_DETAIL"].NewRow();
                    detailrow["order_detail_id"] = row["OrderDetailID"].ToString().Trim();
                    detailrow["order_id"] = row["OrderID"].ToString().Trim().Trim();
                    detailrow["product_code"] = row["ProductCode"].ToString().Trim();
                    detailrow["product_name"] = row["ProductName"].ToString().Trim();
                    detailrow["unit_code"] = row["UNIT_CODE02"].ToString();
                    detailrow["unit_name"] = "条" ;
                    detailrow["demand_quantity"] = Convert.ToDecimal(row["RealQuantity"]);
                    detailrow["real_quantity"] = Convert.ToDecimal(row["RealQuantity"]);
                    detailrow["price"] = Convert.ToDecimal(row["TRADE_PRICE"]);
                    detailrow["amount"] = Convert.ToDecimal(row["AMOUNT_PRICE"]);
                    detailrow["unit_quantity"] = row["QUANTITY01"].ToString();
                    ds.Tables["WMS_SORT_ORDER_DETAIL"].Rows.Add(detailrow);
                }
                return ds;
            }
            catch (Exception e)
            {
                string s = e.Message;
                return null;
            }
        }

        //保存线路调度表信息
        public DataSet SaveDispatch(DataTable diapTable, string sortingLine)
        {
            DataSet ds = this.GenerateEmptyTables();
            foreach (DataRow row in diapTable.Rows)
            {
                DataRow disprow = ds.Tables["WMS_SORT_ORDER_DISPATCH"].NewRow();
                disprow["order_date"] = row["OrderDate"].ToString();//订单时间
                disprow["sorting_line_code"] = sortingLine;//调度分拣线
                disprow["deliver_line_code"] = row["DIST_BILL_ID"].ToString();// row["DELIVERLINECODE"].ToString();// +"_" ;
                disprow["is_active"] = "1";//是否可用
                disprow["update_time"] = DateTime.Now;//调度时间
                disprow["sort_work_dispatch_id"] = null;//作业调度ID
                disprow["work_status"] = "1";//调度状态
                ds.Tables["WMS_SORT_ORDER_DISPATCH"].Rows.Add(disprow);
            }
            return ds;
        }

        //保存到数据库
        public void Insert(DataSet masterds, DataSet detailds)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                DownSortingInfoDao dao = new DownSortingInfoDao();
                if (masterds.Tables["WMS_SORT_ORDER"].Rows.Count > 0)
                {
                    dao.InsertSortingOrder(masterds);
                }
                if (detailds.Tables["WMS_SORT_ORDER_DETAIL"].Rows.Count > 0)
                {
                    dao.InsertSortingOrderDetail(detailds);
                }
            }
        }

        //保存线路调度结果
        public void Insert(DataSet dispatchDs)
        {
            using (PersistentManager pm = new PersistentManager())
            {
                DownSortingInfoDao dao = new DownSortingInfoDao();
                if (dispatchDs.Tables["WMS_SORT_ORDER_DISPATCH"].Rows.Count > 0)
                {
                    dao.InsertDispatch(dispatchDs);
                }
            }
        }

        //生成虚拟表
        private DataSet GenerateEmptyTables()
        {
            DataSet ds = new DataSet();
            DataTable mastertable = ds.Tables.Add("WMS_SORT_ORDER");
            mastertable.Columns.Add("order_id");
            mastertable.Columns.Add("company_code");
            mastertable.Columns.Add("sale_region_code");
            mastertable.Columns.Add("order_date");
            mastertable.Columns.Add("order_type");
            mastertable.Columns.Add("customer_code");
            mastertable.Columns.Add("customer_name");
            mastertable.Columns.Add("quantity_sum");
            mastertable.Columns.Add("amount_sum");
            mastertable.Columns.Add("detail_num");
            mastertable.Columns.Add("deliver_order");
            mastertable.Columns.Add("DeliverDate");
            mastertable.Columns.Add("description");
            mastertable.Columns.Add("is_active");
            mastertable.Columns.Add("update_time");
            mastertable.Columns.Add("deliver_line_code");
            mastertable.Columns.Add("dist_bill_id");

            DataTable detailtable = ds.Tables.Add("WMS_SORT_ORDER_DETAIL");
            detailtable.Columns.Add("order_detail_id");
            detailtable.Columns.Add("order_id");
            detailtable.Columns.Add("product_code");
            detailtable.Columns.Add("product_name");
            detailtable.Columns.Add("unit_code");
            detailtable.Columns.Add("unit_name");
            detailtable.Columns.Add("demand_quantity");
            detailtable.Columns.Add("real_quantity");
            detailtable.Columns.Add("price");
            detailtable.Columns.Add("amount");
            detailtable.Columns.Add("unit_quantity");

            DataTable dispatchtable = ds.Tables.Add("WMS_SORT_ORDER_DISPATCH");
            dispatchtable.Columns.Add("id");
            dispatchtable.Columns.Add("order_date");
            dispatchtable.Columns.Add("sorting_line_code");
            dispatchtable.Columns.Add("deliver_line_code");
            dispatchtable.Columns.Add("is_active");
            dispatchtable.Columns.Add("update_time");
            dispatchtable.Columns.Add("sort_work_dispatch_id");
            dispatchtable.Columns.Add("work_status");
            return ds;
        }
    }
}
