using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Download.Interfaces;

namespace THOK.Wms.Download.Service
{
    public class SortingDownService : ISortingDownService
    {
        public SortOrder[] GetSortOrder(string beginDate, string endDate, string sortOrders)
        {
            string sql = @" SELECT [ORDER_ID] AS OrderID
                            ,[ORG_CODE] AS CompanyCode
                            ,[SALE_REG_CODE] AS SaleRegionCode
                            ,[ORDER_DATE] AS OrderDate
                            ,[ORDER_TYPE] AS OrderType
                            ,[CUST_CODE] AS CustomerCode
                            ,[CUST_NAME] AS CustomerName
                            ,[QUANTITY_SUM] AS QuantitySum
                            ,[AMOUNT_SUM] AS AmountSum
                            ,[DETAIL_NUM] AS DetailNum
                            ,[DELIVER_ORDER] AS DeliverOrder
                            ,getdate() AS DeliverDate
                            ,'' AS Description
                            ,[ISACTIVE] AS IsActive
                            ,[UPDATE_DATE] AS UpdateTime
                            ,[DELIVER_LINE_CODE] AS DeliverLineCode
                        FROM [V_WMS_SORT_ORDER]";
            sql = sql + " WHERE ORDER_DATE>='" + beginDate + "' AND ORDER_DATE<='" + endDate + "'";
            using (SortingDbContext sortdb = new SortingDbContext())
            {
                return sortdb.Database.SqlQuery<SortOrder>(sql).ToArray();
            }
        }

        public SortOrderDetail[] GetSortOrderDetail(string sortOrders)
        {
            string sql = @"SELECT [ORDER_DETAIL_ID] AS OrderDetailID
                            ,[ORDER_ID] AS OrderID
                            ,[BRAND_CODE] AS ProductCode
                            ,[BRAND_NAME] AS ProductName
                            ,['200'] AS UnitCode
                            ,[BRAND_UNIT_NAME] AS UnitName
                            ,[QUANTITY] AS DemandQuantity
                            ,[QUANTITY] AS RealQuantity
                            ,[PRICE] AS Price
                            ,[AMOUNT] AS Amount
                            ,[50] AS UnitQuantity                           
                        FROM [V_WMS_SORT_ORDER_DETAIL]";
            sql = sql + " WHERE ORDER_ID IN('" + sortOrders + "')";
            using (SortingDbContext sortdb = new SortingDbContext())
            {
                return sortdb.Database.SqlQuery<SortOrderDetail>(sql).ToArray();
            }
        }
    }
}
