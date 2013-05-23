using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Download.Interfaces;
using THOK.Wms.DbModel;

namespace THOK.Wms.Download.Service
{
    public class InBillMasterDownService : IInBillMasterDownService
    {
        public InBillMaster[] GetInBillMaster(string inBillMasters)
        {
            string sql = @"SELECT [ORDER_ID] AS BillNo,
                                      [ORDER_DATE] AS BillDate,
                                      [BILL_TYPE] AS BillTypeCode,
                                      ['1'] AS Status,
                                      [''] AS Description ,
                                      [ISACTIVE] AS IsActive ,
                                      [UPDATE_DATE] AS UpdateTime FROM [V_WMS_IN_ORDER]";
            sql = sql + "WHERE ORDER_ID NOT IN('" + inBillMasters + "')";
            using (SortingDbContext sortdb = new SortingDbContext())
            {
                return sortdb.Database.SqlQuery<InBillMaster>(sql).ToArray();
            }
        }

        public InBillDetail[] GetInBillDetail(string inBillMasters)
        {
            string sql = @"SELECT [ORDER_ID] AS BillNo
                            ,[BRAND_CODE] AS ProductCode
                            ,[PRICE] AS Price
                            ,[QUANTITY] AS BillQuantity
                            ,[0] AS AllotQuantity
                            ,[0] AS RealQuantity
                            ,[''] AS Description
                        FROM [V_WMS_IN_ORDER_DETAIL]";

            sql = sql + "WHERE ORDER_ID IN ('" + inBillMasters + "')";
            using (SortingDbContext sortdb = new SortingDbContext())
            {
                return sortdb.Database.SqlQuery<InBillDetail>(sql).ToArray();
            }
        }
    }
}
