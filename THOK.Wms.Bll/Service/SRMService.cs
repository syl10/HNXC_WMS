using System;
using Microsoft.Practices.Unity;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.DbModel;
using System.Linq;

namespace THOK.Wms.Bll.Service
{
    public class SRMService : ServiceBase<SRM>, ISRMService
    {
        [Dependency]
        public ISRMRepository SRMRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows,SRM srms)
        {
            IQueryable<SRM> sRMQuery = SRMRepository.GetQueryable();

            var sRMDetail = sRMQuery.Where(s =>
                s.SRMName.Contains(srms.SRMName)
                //&& s.OPCServiceName.Contains(srms.OPCServiceName)
                //&& s.GetRequest.Contains(srms.GetRequest)
                //&& s.GetAllow.Contains(srms.GetAllow)
                //&& s.GetComplete.Contains(srms.GetComplete)
                //&& s.PutRequest.Contains(srms.PutRequest)
                //&& s.PutAllow.Contains(srms.PutAllow)
                //&& s.PutComplete.Contains(srms.PutComplete)
                && s.State.Contains(srms.State)).OrderBy(ul => ul.SRMName);
            int total = sRMDetail.Count();
            var sRMDetails = sRMDetail.Skip((page - 1) * rows).Take(rows);
            var sRM_Detail = sRMDetails.ToArray().Select(s => new
            {
                s.ID,
                s.SRMName,
                s.OPCServiceName,
                s.GetRequest,
                s.GetAllow,
                s.GetComplete,
                s.PutRequest,
                s.PutAllow,
                s.PutComplete,
                s.Description,
                State = s.State == "01" ? "可用" : "不可用"
            });
            return new { total, rows = sRM_Detail.ToArray() };
        }

        public bool Add(SRM srm)
        {
            var s = new SRM();
            s.SRMName = srm.SRMName;
            s.Description = srm.Description;
            s.OPCServiceName = srm.OPCServiceName;
            s.GetRequest = srm.GetRequest;
            s.GetAllow = srm.GetAllow;
            s.GetComplete = srm.GetComplete;
            s.PutRequest = srm.PutRequest;
            s.PutAllow = srm.PutAllow;
            s.PutComplete = srm.PutComplete;
            s.State = srm.State;
            SRMRepository.Add(s);
            SRMRepository.SaveChanges();
            return true;
        }

        public bool Save(SRM srm)
        {
            var sr = SRMRepository.GetQueryable().FirstOrDefault(s => s.ID == srm.ID);
            sr.SRMName = srm.SRMName;
            sr.Description = srm.Description;
            sr.OPCServiceName = srm.OPCServiceName;
            sr.GetRequest = srm.GetRequest;
            sr.GetAllow = srm.GetAllow;
            sr.GetComplete = srm.GetComplete;
            sr.PutRequest = srm.PutRequest;
            sr.PutAllow = srm.PutAllow;
            sr.PutComplete = srm.PutComplete;
            sr.State = srm.State;
            SRMRepository.SaveChanges();
            return true;
        }

        public bool Delete(int srmId)
        {
            var srm = SRMRepository.GetQueryable().FirstOrDefault(s => s.ID == srmId);
            if (srm != null)
            {
                SRMRepository.Delete(srm);
                SRMRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public object GetSRM(int page, int rows, string queryString, string value)
        {
            string id = "", srmName = "";

            if (queryString == "id")
            {
                id = value;
            }
            else
            {
                srmName = value;
            }
            IQueryable<SRM> srmQuery = SRMRepository.GetQueryable();
            int Id = Convert.ToInt32(id);
            var srm = srmQuery.Where(s => s.ID == Id && s.SRMName.Contains(srmName) && s.State == "01")
                .OrderBy(s => s.ID).AsEnumerable().
                Select(s => new
                {
                    s.SRMName,
                    s.OPCServiceName,
                    s.GetRequest,
                    s.GetAllow,
                    s.GetComplete,
                    s.PutRequest,
                    s.PutAllow,
                    s.PutComplete,
                    s.Description,
                    State = s.State == "01" ? "可用" : "不可用"
                });
            int total = srm.Count();
            srm = srm.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = srm.ToArray() };
        }

        public System.Data.DataTable GetSRM(int page, int rows, SRM srms)
        {
            IQueryable<SRM> sRMQuery = SRMRepository.GetQueryable();

            var sRMDetail = sRMQuery.Where(s =>
                s.SRMName.Contains(srms.SRMName)
                && s.OPCServiceName.Contains(srms.OPCServiceName)
                && s.GetRequest.Contains(srms.GetRequest)
                && s.GetAllow.Contains(srms.GetAllow)
                && s.GetComplete.Contains(srms.GetComplete)
                && s.PutRequest.Contains(srms.PutRequest)
                && s.PutAllow.Contains(srms.PutAllow)
                && s.PutComplete.Contains(srms.PutComplete)
                && s.State.Contains(srms.State)).OrderBy(ul => ul.SRMName);
            int total = sRMDetail.Count();
            var sRMDetails = sRMDetail.Skip((page - 1) * rows).Take(rows);
            var sRM_Detail = sRMDetails.ToArray().Select(s => new
            {
                s.ID,
                s.SRMName,
                s.OPCServiceName,
                s.GetRequest,
                s.GetAllow,
                s.GetComplete,
                s.PutRequest,
                s.PutAllow,
                s.PutComplete,
                s.Description,
                State = s.State == "01" ? "可用" : "不可用"
            });

            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add("堆垛机编码", typeof(string));
            dt.Columns.Add("堆垛机名称", typeof(string));
            dt.Columns.Add("描述", typeof(string));
            dt.Columns.Add("OPC服务名", typeof(string));
            dt.Columns.Add("取货请求数据项名", typeof(string));
            dt.Columns.Add("充许取货数据项名", typeof(string));
            dt.Columns.Add("取货完成数据项名", typeof(string));
            dt.Columns.Add("放货请求数据项名", typeof(string));
            dt.Columns.Add("充许放货数据项名", typeof(string));
            dt.Columns.Add("放货完成数据项名", typeof(string));
            dt.Columns.Add("状态", typeof(string));
            foreach (var s in sRM_Detail)
            {
                dt.Rows.Add
                    (
                        s.ID,
                        s.SRMName,
                        s.OPCServiceName,
                        s.GetRequest,
                        s.GetAllow,
                        s.GetComplete,
                        s.PutRequest,
                        s.PutAllow,
                        s.PutComplete,
                        s.Description,
                        s.State
                    );
            }
            return dt;
        }
    }
}
