using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    public class DeliverDistService : ServiceBase<DeliverDist>, IDeliverDistService
    {
        [Dependency]
        public IDeliverDistRepository DeliverDistRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IDeliverDistService 成员

        public object GetDetails(int page, int rows, string DistCode, string CustomCode, string DistName, string CompanyCode, string UniformCode, string IsActive)
        {
            IQueryable<DeliverDist> DeliverDistQuery = DeliverDistRepository.GetQueryable();
            var DeliverDist = DeliverDistQuery.Where(c => c.DistCode.Contains(DistCode) &&
                                                          c.DistName.Contains(DistName) &&
                                                          c.IsActive.Contains(IsActive) &&
                                                          c.UniformCode.Contains(UniformCode));
            if (!CustomCode.Equals(string.Empty))
            {
                DeliverDist = DeliverDist.Where(d => d.CustomCode == CustomCode);
            }
            if (!CompanyCode.Equals(string.Empty))
            {
                DeliverDist = DeliverDist.Where(d => d.DistCenterCode == CompanyCode);
            }
            DeliverDist = DeliverDist.OrderBy(h => h.DistCode);
            int total = DeliverDist.Count();
            DeliverDist = DeliverDist.Skip((page - 1) * rows).Take(rows);

            var temp = DeliverDist.ToArray().Select(c => new
            {
                DistCode = c.DistCode,
                CustomCode = c.CustomCode,
                DistName=c.DistName,
                DistCenterCode=c.DistCenterCode,
                CompanyCode = c.CompanyCode,
                UniformCode=c.UniformCode,
                Description=c.Description,
                IsActive = c.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = c.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
            return new { total, rows = temp.ToArray() };
        }

        #endregion

        #region IDeliverDistService 成员


        public bool Add(DeliverDist deliverDist, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var deliver = new DeliverDist();
            if (deliverDist != null)
            {
                try
                {
                    deliver.DistCode = deliverDist.DistCode;
                    deliver.CustomCode = deliverDist.CustomCode;
                    deliver.DistName = deliverDist.DistName;
                    deliver.DistCenterCode = deliverDist.DistCenterCode;
                    deliver.CompanyCode = deliverDist.CompanyCode;
                    deliver.UniformCode = deliverDist.UniformCode;
                    deliver.Description = deliverDist.Description;
                    deliver.IsActive = deliverDist.IsActive;
                    deliver.UpdateTime = DateTime.Now;

                    DeliverDistRepository.Add(deliver);
                    DeliverDistRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    strResult = "原因：" + ex.InnerException;
                }
            }
            return result;
        }

        #endregion

        #region IDeliverDistService 成员


        public object S_Details(int page, int rows, string QueryString, string Value)
        {
            string DistName = "";
            string CompanyCode = "";
            if (QueryString == "DistName")
            {
                DistName = Value;
            }
            else
            {
                CompanyCode = Value;
            }
            IQueryable<DeliverDist> deliverQuery = DeliverDistRepository.GetQueryable();
            var deliver = deliverQuery.Where(c => c.DistName.Contains(DistName) && c.CompanyCode.Contains(CompanyCode))
                .OrderBy(c => c.CompanyCode)
                .Select(c => c);
            if (!DistName.Equals(string.Empty))
            {
                deliver = deliver.Where(p => p.DistName == DistName);
            }
            int total = deliver.Count();
            deliver = deliver.Skip((page - 1) * rows).Take(rows);

            var temp = deliver.ToArray().Select(c => new
            {

                DistCode = c.DistCode,
                DistName = c.DistName,
                CompanyCode = c.CompanyCode,
                DistCenterCode = c.DistCenterCode,
                IsActive = c.IsActive == "1" ? "可用" : "不可用"
            });
            return new { total, rows = temp.ToArray() };
        }

        #endregion

        #region IDeliverDistService 成员


        public bool Save(string DistCode, string CustomCode, string DistName, string DistCenterCode, string CompanyCode, string UniformCode, string Description, string IsActive, out string strResult)
        {
            strResult = string.Empty;
            try
            {
                var deliver = DeliverDistRepository.GetQueryable()
                    .FirstOrDefault(i => i.DistCode == DistCode);
                deliver.CustomCode = CustomCode;
                deliver.DistName = DistName;
                deliver.DistCenterCode = DistCenterCode;
                deliver.CompanyCode = CompanyCode;
                deliver.UniformCode = UniformCode;
                deliver.Description = Description;
                deliver.IsActive = IsActive;
                DeliverDistRepository.SaveChanges();

            }
            catch (Exception ex)
            {
                strResult = "原因：" + ex.InnerException;
            }
            return true;
        }

        #endregion

        #region IDeliverDistService 成员


        public bool Delete(string DistCode)
        {
            var deliver = DeliverDistRepository.GetQueryable()
              .FirstOrDefault(i => i.DistCode == DistCode);
            if (DistCode != null)
            {
                DeliverDistRepository.Delete(deliver);
                DeliverDistRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        #endregion
    }
}
