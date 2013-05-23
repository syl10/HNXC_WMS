using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Download.Interfaces;
using THOK.Wms.DbModel;

namespace THOK.Wms.Download.Service
{
    public class DeliverLineDownService : IDeliverLineDownService
    {
        public DeliverLine[] GetDeliverLine(string deloverLines)
        {
            string sql = @"SELECT [DELIVER_LINE_CODE] AS DeliverLineCode
                            ,[LINE_TYPE] AS CustomCode
                            ,[DELIVER_LINE_NAME] AS DeliverLineName
                            ,[DIST_STA_CODE] AS DistCode
                            ,[DELIVER_LINE_ORDER] AS DeliverOrder
                            ,'' AS Description
                            ,[ISACTIVE] AS IsActive
                            ,getdate() AS UpdateTime
                        FROM [V_WMS_DELIVER_LINE]";
            sql = sql + " WHERE DELIVER_LINE_CODE NOT IN('" + deloverLines + "')";
            using (SortingDbContext sortdb = new SortingDbContext())
            {
                return sortdb.Database.SqlQuery<DeliverLine>(@"
                            SELECT [DELIVER_LINE_CODE] AS DeliverLineCode
                            ,[LINE_TYPE] AS CustomCode
                            ,[DELIVER_LINE_NAME] AS DeliverLineName
                            ,[DIST_STA_CODE] AS DistCode
                            ,[DELIVER_LINE_ORDER] AS DeliverOrder
                            ,'' AS Description
                            ,[ISACTIVE] AS IsActive
                            ,getdate() AS UpdateTime
                        FROM [V_WMS_DELIVER_LINE]").ToArray();
            }
        }


        public DeliverDist[] GetDeliverDist(string deloverDists)
        {
            string sql = @"SELECT [DIST_STA_CODE] AS DistCode
                            ,[DIST_STA_N] AS CustomCode
                            ,[DIST_STA_NAME] AS DistName
                            ,[DIST_CTR_CODE] AS DistCenterCode
                            ,[ORG_CODE] AS CompanyCode
                            ,[N_ORG_CODE] AS UniformCode
                            ,'' AS Description
                            ,[ISACTIVE] AS IsActive
                            ,getdate() AS UpdateTime
                        FROM [V_WMS_DIST_STATION]";
            sql = sql + " WHERE DIST_STA_CODE NOT IN('" + deloverDists + "')";
            using (SortingDbContext sortdb = new SortingDbContext())
            {
                return sortdb.Database.SqlQuery<DeliverDist>(sql).ToArray();
            }
        }
    }
}
