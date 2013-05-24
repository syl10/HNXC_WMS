using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Util;
using System.Data;

namespace THOK.Wms.DownloadWms.Dao
{
    public class DownSortingInfoDao : BaseDao
    {
        //下载主单
        public DataTable GetSortingOrder(string parameter)
        {
            string sql = string.Format(@"SELECT S.*,C.COMPANY_CODE,C.SALE_REGION_CODE,C.SALE_SCOPE,C.DELIVER_ORDER FROM SORTORDER S
                                            LEFT JOIN WMS_CUSTOMER C ON S.CUSTOMERCODE=C.CUSTOM_CODE WHERE {0}", parameter);
            return this.ExecuteQuery(sql).Tables[0];
        }

        //下载细单
        public DataTable GetSortingDetail(string parameter)
        {
            string sql = string.Format(@"SELECT S.*,P.TRADE_PRICE,(P.TRADE_PRICE*REALQUANTITY) AS AMOUNT_PRICE,
                                                    U.UNIT_CODE02,U.QUANTITY01 FROM SORTORDERDETAIL S
                                                    LEFT JOIN WMS_PRODUCT P ON S.PRODUCTCODE=P.PRODUCT_CODE
                                                    LEFT JOIN WMS_UNIT_LIST U ON P.UNIT_LIST_CODE=U.UNIT_LIST_CODE WHERE {0}", parameter);
            return this.ExecuteQuery(sql).Tables[0];
        }

        //查询调度信息
        public DataTable GetDispatchLine(string parameter)
        {
            string sql = string.Format(@"SELECT ORDERDATE,DIST_BILL_ID,DELIVERYMAN_CODE,DELIVERYMAN_NAME,DELIVERLINECODE,DELIVERLINENAME FROM SORTORDER WHERE {0}
                                        GROUP BY ORDERDATE,DIST_BILL_ID,DELIVERYMAN_CODE,DELIVERYMAN_NAME,DELIVERLINECODE,DELIVERLINENAME", parameter);
            return this.ExecuteQuery(sql).Tables[0];
        }

        //添加主表
        public void InsertSortingOrder(DataSet ds)
        {
            BatchInsert(ds.Tables["WMS_SORT_ORDER"], "WMS_SORT_ORDER");
        }
       
        //添加细表
        public void InsertSortingOrderDetail(DataSet ds)
        {
            BatchInsert(ds.Tables["WMS_SORT_ORDER_DETAIL"], "WMS_SORT_ORDER_DETAIL");
        }

        //添加调度表
        public void InsertDispatch(DataSet ds)
        {
            BatchInsert(ds.Tables["WMS_SORT_ORDER_DISPATCH"], "WMS_SORT_ORDER_DISPATCH");
        }

        //查出3天之内的订单
        public DataTable GetOrderId(string statDate,string endDate)
        {
            string sql = " SELECT ORDER_ID FROM WMS_SORT_ORDER WHERE ORDER_DATE>='" + statDate + "' AND ORDER_DATE<='" + endDate + "'";
            return this.ExecuteQuery(sql).Tables[0];
        }

        //查出单位系列表
        public DataTable GetUnitList()
        {
            string sql = "SELECT * FROM WMS_UNIT_LIST";
            return this.ExecuteQuery(sql).Tables[0];
        }
    }
}
