using System;
using System.Collections.Generic;
using System.Text;
using THOK.WMS.Upload.Dao;
using System.Data;
using THOK.Util;

namespace THOK.WMS.Upload.Bll
{
    public class UploadBll
    {
        #region 上报数据
        /// <summary>
        /// 上报卷烟信息数据
        /// </summary>
        /// <param name="brandSet"></param>
        public void InsertProduct(DataSet brandSet)
        {
            using (PersistentManager pm = new PersistentManager("ZYDB2Connection"))
            {
                UploadDao dao = new UploadDao();
                dao.SetPersistentManager(pm);
                dao.InsertProduct(brandSet);
            }
        }
        /// <summary>
        /// 组织机构表
        /// </summary>
        /// <param name="companySet"></param>
        public void UploadOrganization(DataSet companySet)
        {
            using (PersistentManager pm = new PersistentManager("ZYDB2Connection"))
            {
                UploadDao dao = new UploadDao();
                dao.SetPersistentManager(pm);
                dao.InsertCompany(companySet);
            }
        }
        //人员信息表
        public void UploadEmployee(DataSet employee)
        {
            using (PersistentManager pm = new PersistentManager("ZYDB2Connection"))
            {
                UploadDao dao = new UploadDao();
                dao.SetPersistentManager(pm);
                dao.InsertEmployee(employee);
            }
        }
        /// <summary>
        /// 上报客户信息数据
        /// </summary>
        /// <param name="costomerSet"></param>
        public void InsertCustom(DataSet costomerSet)
        {
            using (PersistentManager pm = new PersistentManager("ZYDB2Connection"))
            {
                UploadDao dao = new UploadDao();
                dao.SetPersistentManager(pm);
                dao.InsertCustom(costomerSet);
            }
        }
        //仓储属性表
        public void UploadCell(DataSet cell)
        {
            using (PersistentManager pm = new PersistentManager("ZYDB2Connection"))
            {
                UploadDao dao = new UploadDao();
                dao.SetPersistentManager(pm);
                dao.InsertCell(cell);
            }
        }
        //仓库库存表
        public void QueryStoreStock(DataSet storeStock)
        {
            using (PersistentManager pm = new PersistentManager("ZYDB2Connection"))
            {
                UploadDao dao = new UploadDao();
                dao.SetPersistentManager(pm);
                dao.InsertStoreStock(storeStock);
            }
        }
        //业务库存表
        public void QueryBusiStock(DataSet busiStock)
        {
            using (PersistentManager pm = new PersistentManager("ZYDB2Connection"))
            {
                UploadDao dao = new UploadDao();
                dao.SetPersistentManager(pm);
                dao.InsertBustStock(busiStock);
            }
        }
        //入库主表
        public void InsertInMasterBill(DataSet busiStock)
        {
            using (PersistentManager pm = new PersistentManager("ZYDB2Connection"))
            {
                UploadDao dao = new UploadDao();
                dao.SetPersistentManager(pm);
                dao.InsertInMasterBill(busiStock);
            }
        }
        //入库细表
        public void InsertInDetailBill(DataSet busiStock)
        {
            using (PersistentManager pm = new PersistentManager("ZYDB2Connection"))
            {
                UploadDao dao = new UploadDao();
                dao.SetPersistentManager(pm);
                dao.InsertInDetailBill(busiStock);
            }
        }
        //入库业务表
        public void InsertInBusiBill(DataSet busiStock)
        {
            using (PersistentManager pm = new PersistentManager("ZYDB2Connection"))
            {
                UploadDao dao = new UploadDao();
                dao.SetPersistentManager(pm);
                dao.InsertInBusiBill(busiStock);
            }
        }
        //出库主表
        public void InsertOutMasterBill(DataSet busiStock)
        {
            using (PersistentManager pm = new PersistentManager("ZYDB2Connection"))
            {
                UploadDao dao = new UploadDao();
                dao.SetPersistentManager(pm);
                dao.InsertOutMasterBill(busiStock);
            }
        }
        //出库单据细表
        public void InsertOutDetailBill(DataSet busiStock)
        {
            using (PersistentManager pm = new PersistentManager("ZYDB2Connection"))
            {
                UploadDao dao = new UploadDao();
                dao.SetPersistentManager(pm);
                dao.InsertOutDetailBill(busiStock);
            }
        }
        //出库业务单据表
        public void InsertOutBusiBill(DataSet busiStock)
        {
            using (PersistentManager pm = new PersistentManager("ZYDB2Connection"))
            {
                UploadDao dao = new UploadDao();
                dao.SetPersistentManager(pm);
                dao.InsertOutBusiBill(busiStock);
            }
        }

        // 分拣订单上报(主表细表)
        public void uploadSort(DataSet masterds, DataSet detailds)
        {
            using (PersistentManager pm = new PersistentManager("ZYDB2Connection"))
            {
                UploadDao dao = new UploadDao();
                dao.SetPersistentManager(pm);
                if (masterds.Tables["DWV_OUT_ORDER"].Rows.Count > 0)
                {
                    dao.UploadIordOrder(masterds);
                }
                if (detailds.Tables["DWV_OUT_ORDER_DETAIL"].Rows.Count > 0)
                {
                    dao.UploadIordOrderDetail(detailds);
                }
            }
        }

        //同步状态表
        public void InsertSynchro()
        {
            using (PersistentManager pm = new PersistentManager("ZYDB2Connection"))
            {
                UploadDao dao = new UploadDao();
                dao.SetPersistentManager(pm);
                dao.InsertSynchro();
            }
        }
        #endregion
    }
}
