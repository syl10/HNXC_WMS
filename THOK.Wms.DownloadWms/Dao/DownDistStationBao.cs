using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Util;
using System.Data;

namespace THOK.Wms.DownloadWms.Dao
{
    public class DownDistStationBao : BaseDao
    {
        public DataTable GetDistStationInfo(string distCode)
        {
            string sql = string.Format("SELECT * FROM V_WMS_DIST_STATION WHERE DIST_STA_CODE NOT IN({0})", distCode);
            return this.ExecuteQuery(sql).Tables[0];
        }

        public DataTable GetDistStationCode()
        {
            string sql = " SELECT DIST_CODE FROM WMS_DELIVER_DIST";
            return this.ExecuteQuery(sql).Tables[0];
        }

        public void Insert(DataSet ds)
        {
            BatchInsert(ds.Tables["WMS_DELIVER_DIST"], "WMS_DELIVER_DIST");
        }
    }
}
