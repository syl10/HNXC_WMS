using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Download.Interfaces;
using THOK.Wms.DbModel;

namespace THOK.Wms.Download.Service
{
    public class CustomerDownService : ICustomerDownService
    {
        public Customer[] GetCustomer(string customers)
        {
            string sql = @"SELECT [CUST_CODE] AS CustomerCode
                        ,[CUST_N] AS CustomCode
                        ,[CUST_NAME] AS CustomerName
                        ,[ORG_CODE] AS CompanyCode
                        ,[SALE_REG_CODE] AS SaleRegionCode
                        ,[N_CUST_CODE] AS UniformCode
                        ,[CUST_TYPE] AS CustomerType
                        ,[sale_scope] AS SaleScope
                        ,[RTL_CUST_TYPE_CODE] AS IndustryType
                        ,[CUST_GEO_TYPE_CODE] AS CityOrCountryside
                        ,[DELIVER_LINE_CODE] AS DeliverLineCode
                        ,[DELIVER_ORDER] AS DeliverOrder
                        ,[DIST_ADDRESS] AS Address
                        ,[DIST_PHONE] AS Phone
                        ,[LICENSE_TYPE] AS LicenseType
                        ,[LICENSE_CODE] AS LicenseCode
                        ,[PRINCIPAL_NAME] AS PrincipalName
                        ,[PRINCIPAL_TEL] AS PrincipalPhone
                        ,[PRINCIPAL_ADDRESS] AS PrincipalAddress
                        ,[MANAGEMENT_NAME] AS ManagementName
                        ,[MANAGEMENT_TEL] AS ManagementPhone
                        ,[BANK] AS Bank
                        ,[BANK_ACCOUNTS] AS BankAccounts
                        ,[''] AS Description
                        ,[ISACTIVE] AS IsActive
                        ,[UPDATE_DATE] AS UpdateTime
                        FROM [V_WMS_CUSTOMER]";

            sql = sql + " WHERE CUST_CODE NOT IN('" + customers + "')";
            using (SortingDbContext sortdb = new SortingDbContext())
            {
                return sortdb.Database.SqlQuery<Customer>(sql).ToArray();
            }
        }
    }
}
