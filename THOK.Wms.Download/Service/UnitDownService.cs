using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Download.Interfaces;
using THOK.Wms.DbModel;

namespace THOK.Wms.Download.Service
{
    public class UnitDownService : IUnitDownService
    {
        public Unit[] GetUnit(string units)
        {
            string sql = @" SELECT [BRAND_UNIT_CODE]+[COUNT] AS UnitCode
                        ,[BRAND_UNIT_NAME] AS UnitName
                        ,[COUNT] AS COUNT
                        FROM [V_WMS_SORT_ORDER_DETAIL]
                        GROUP BY [BRAND_UNIT_CODE]+[COUNT],[BRAND_UNIT_NAME],[COUNT]";

            using (SortingDbContext sortdb = new SortingDbContext())
            {
                return sortdb.Database.SqlQuery<Unit>(sql).ToArray();
            }
        }

        public UnitList[] GetUnitList(string unitLists)
        {
            string sql = @"SELECT [BRAND_ULIST_CODE] AS UnitListCode
                            ,[N_BRAND_ULIST_CODE] AS UniformCode
                            ,[BRAND_ULIST_NAME] AS UnitListName
                            ,[BRAND_UNIT_CODE_01] AS UnitCode01
                            ,[BRAND_UNIT_NAME_01] AS UnitName01
                            ,[QTY_01] AS Quantity01
                            ,[BRAND_UNIT_CODE_02] AS UnitCode02
                            ,[BRAND_UNIT_NAME_02] AS UnitName02
                            ,[QTY_02] AS Quantity02
                            ,[BRAND_UNIT_CODE_03] AS UnitCode03
                            ,[BRAND_UNIT_NAME_03] AS UnitName03
                            ,[QTY_03] AS Quantity03
                            ,[BRAND_UNIT_CODE_04] AS UnitCode04
                            ,[BRAND_UNIT_NAME_04] AS UnitName04
                            ,[ISACTIVE] AS IsActive
                            ,getdate() AS UpdateTime
                        FROM [V_WMS_BRAND_ULIST]";
            using (SortingDbContext sortdb = new SortingDbContext())
            {
                return sortdb.Database.SqlQuery<UnitList>(sql).ToArray();
            }
        }
    }
}
