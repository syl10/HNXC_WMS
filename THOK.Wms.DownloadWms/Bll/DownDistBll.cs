using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;
using THOK.WMS.DownloadWms.Dao;

namespace THOK.WMS.DownloadWms.Bll
{
    public class DownDistBll
    {
        #region 从营销系统下载配送中心信息

        /// <summary>
        /// 下载配送中心信息
        /// </summary>
        /// <returns></returns>
        public bool DownDistInfo()
        {
            bool tag = true;
            this.Delete();
            DataTable distTable = this.GetDistInfo();
            if (distTable.Rows.Count > 0)
                this.Insert(distTable);
            else
                tag = false;
            return tag;
        }

        /// <summary>
        /// 清除配送中心信息
        /// </summary>
        public void Delete()
        {
            using (PersistentManager dbPm = new PersistentManager())
            {
                DownDistDao dao = new DownDistDao();
                dao.Delete();
            }
        }

        public void Insert(DataTable distTable)
        {
            using (PersistentManager dbPm = new PersistentManager())
            {
                DownDistDao dao = new DownDistDao();
                dao.Insert(distTable);
            }
        }

        /// <summary>
        /// 下载配送中心信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetDistInfo()
        {
            using (PersistentManager dbPm = new PersistentManager("YXConnection"))
            {
                DownDistDao dao = new DownDistDao();
                dao.SetPersistentManager(dbPm);
                return dao.GetDistInfo();
            }
        }

        /// <summary>
        /// 获取配送中心编码
        /// </summary>
        /// <returns></returns>
        public string GetCompany()
        {
            using (PersistentManager dbpm = new PersistentManager())
            {
                DownDistDao dao = new DownDistDao();
                return dao.GetCompany().ToString();
            }
        }
        #endregion
    }
}
