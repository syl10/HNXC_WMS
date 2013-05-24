using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;

namespace THOK.WMS.DownloadWms.Dao
{
    public class DownUnitDao : BaseDao
    {
        /// <summary>
        /// 下载单位信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetUnitInfo(string unitCode)
        {
            string sql = string.Format(@"SELECT U.*,B.BRAND_N FROM V_WMS_BRAND_UNIT U
                                        LEFT JOIN  V_WMS_BRAND B ON U.BRAND_CODE =B.BRAND_CODE
                                        WHERE (B.BRAND_N <> 'NULL' OR B.BRAND_N !='') and {0}", unitCode);
            return this.ExecuteQuery(sql).Tables[0];
        }
        /// <summary>
        /// 下载单位信息 平顶山
        /// </summary>
        /// <returns></returns>
        public DataTable GetUnitInfos(string unitCode)
        {
            string sql = string.Format("SELECT * FROM IC.V_WMS_BRAND_UNIT WHERE {0}", unitCode);
            return this.ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 查询计量单位系列表
        /// </summary>
        /// <param name="ulistCode"></param>
        /// <returns></returns>
        public DataTable GetBrandUlistInfo(string ulistCode)
        {
            string sql = string.Format("SELECT * FROM IC.V_WMS_BRAND_ULIST WHERE {0}", ulistCode);
            return this.ExecuteQuery(sql).Tables[0];
        }
        /// <summary>
        /// 平顶山插入单位系列表
        /// </summary>
        /// <param name="ds"></param>
        public void InsertUlist(DataTable ulistCodeTable)
        {
            BatchInsert(ulistCodeTable, "wms_unit_list");
        }

        /// <summary>
        /// 查询仓储单位系列编号
        /// </summary>
        /// <returns></returns>
        public DataTable GetUlistCode()
        {
            string sql = "SELECT unit_list_code FROM wms_unit_list";
            return this.ExecuteQuery(sql).Tables[0];
        }
        /// <summary>
        /// 单位表插入数据
        /// </summary>
        /// <param name="ds"></param>
        public void InsertUnit(DataTable unitTable)
        {
            BatchInsert(unitTable, "wms_unit");
        }

        /// <summary>
        /// 插入单位系列表
        /// </summary>
        /// <param name="ds"></param>
        public void InsertLcUnit(DataTable unitTable)
        {
            BatchInsert(unitTable, "wms_unit_list");
        }

        /// <summary>
        /// 查询仓储单位编号
        /// </summary>
        /// <returns></returns>
        public DataTable GetUnitCode()
        {
            string sql = "SELECT unit_code FROM WMS_UNIT";
            return this.ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 查询单位系列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetUnitProduct()
        {
            string sql = "SELECT * FROM WMS_UNIT_LIST";
            return this.ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 根据单位系列编码查询单位系列表
        /// </summary>
        /// <param name="unitListCode"></param>
        /// <returns></returns>
        public DataTable FindUnitListCOde(string unitListCode)
        {
            string sql = string.Format("SELECT * FROM WMS_UNIT_LIST WHERE UNIT_LIST_CODE='{0}'", unitListCode);
            return this.ExecuteQuery(sql).Tables[0];
        }

    }
}
