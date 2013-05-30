using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    public class JobService : ServiceBase<Job>, IJobService
    {
        [Dependency]
        public IJobRepository JobRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IJobService 成员

        public object GetDetails(int page, int rows, string JobCode, string JobName, string IsActive)
        {
            IQueryable<Job> jobQuery = JobRepository.GetQueryable();
            var job = jobQuery.Where(j => j.JobCode.Contains(JobCode) && j.JobName.Contains(JobName))
                .OrderByDescending(j => j.UpdateTime).AsEnumerable()
                .Select(j => new
                {
                    j.ID,
                    j.JobCode,
                    j.JobName,
                    j.Description,
                    IsActive = j.IsActive == "1" ? "可用" : "不可用",
                    UpdateTime = j.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
                });
            if (!IsActive.Equals(""))
            {
                job = jobQuery.Where(j => j.JobCode.Contains(JobCode) && j.JobName.Contains(JobName) && j.IsActive.Contains(IsActive))
                    .OrderByDescending(j => j.UpdateTime).AsEnumerable()
                    .Select(j => new
                    {
                        j.ID,
                        j.JobCode,
                        j.JobName,
                        j.Description,
                        IsActive = j.IsActive == "1" ? "可用" : "不可用",
                        UpdateTime = j.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
                    });
            }
            int total = job.Count();
            job = job.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = job.ToArray() };

        }

        public bool Add(Job job, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var jo = new Job();
            var jobExist = JobRepository.GetQueryable().FirstOrDefault(j => j.JobCode == job.JobCode);
            if (jobExist == null)
            {
                if (jo != null)
                {
                    try
                    {
                        jo.ID = Guid.NewGuid();
                        jo.JobCode = job.JobCode;
                        jo.JobName = job.JobName;
                        jo.Description = job.Description;
                        jo.IsActive = job.IsActive;
                        jo.UpdateTime = DateTime.Now;

                        JobRepository.Add(jo);
                        JobRepository.SaveChanges();
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        strResult = "原因：" + ex.Message;
                    }
                }
                else
                {
                    strResult = "原因：找不到当前登陆用户！请重新登陆！";
                }
            }
            else
            {
                strResult = "原因：该编号已存在！";
            }
            return result;
        }

        public bool Delete(string JobId, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            Guid joId = new Guid(JobId);
            var job = JobRepository.GetQueryable().FirstOrDefault(j => j.ID == joId);
            if (job != null)
            {
                try
                {
                    JobRepository.Delete(job);
                    JobRepository.SaveChanges();
                    result = true;
                }
                catch (Exception)
                {
                    strResult = "原因：已在使用";
                }
            }
            else
            {
                strResult = "原因：未找到当前需要删除的数据！";
            }
            return result;
        }

        public bool Save(Job job, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var jo = JobRepository.GetQueryable().FirstOrDefault(j => j.ID == job.ID);

            if (jo != null)
            {
                try
                {
                    jo.JobCode = job.JobCode;
                    jo.JobName = job.JobName;
                    jo.Description = job.Description;
                    jo.IsActive = job.IsActive;
                    jo.UpdateTime = DateTime.Now;

                    JobRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    strResult = "原因：" + ex.Message;
                }
            }
            else
            {
                strResult = "原因：未找到当前需要修改的数据！";
            }
            return result;
        }

        #endregion


        public object GetJob(int page, int rows, string queryString, string value)
        {
            string jobCode = "", jobName = "";

            if (queryString == "JobCode")
            {
                jobCode = value;
            }
            else
            {
                jobName = value;
            }
            IQueryable<Job> jobQuery = JobRepository.GetQueryable();
            var job = jobQuery.Where(j => j.JobCode.Contains(jobCode) && j.JobName.Contains(jobName) && j.IsActive == "1")
                .OrderBy(j => j.JobCode).AsEnumerable().
                Select(j => new
                {
                    j.ID,
                    j.JobCode,
                    j.JobName,
                    j.Description,
                    IsActive = j.IsActive == "1" ? "可用" : "不可用"
                });
            int total = job.Count();
            job = job.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = job.ToArray() };
        }

        public System.Data.DataTable GetJob(int page, int rows, string jobCode, string jobName, string isActive)
        {
            IQueryable<Job> jobQuery = JobRepository.GetQueryable();
            var job = jobQuery.Where(j => j.JobCode.Contains(jobCode) && j.JobName.Contains(jobName))
                .OrderByDescending(j => j.UpdateTime).AsEnumerable()
                .Select(j => new
                {
                    j.ID,
                    j.JobCode,
                    j.JobName,
                    j.Description,
                    IsActive = j.IsActive == "1" ? "可用" : "不可用",
                    UpdateTime = j.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
                });
            if (!isActive.Equals(""))
            {
                job = jobQuery.Where(j => j.JobCode.Contains(jobCode) && j.JobName.Contains(jobName) && j.IsActive.Contains(isActive))
                    .OrderByDescending(j => j.UpdateTime).AsEnumerable()
                    .Select(j => new
                    {
                        j.ID,
                        j.JobCode,
                        j.JobName,
                        j.Description,
                        IsActive = j.IsActive == "1" ? "可用" : "不可用",
                        UpdateTime = j.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
                    });
            }
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("岗位编码", typeof(string));
            dt.Columns.Add("岗位名称", typeof(string));
            dt.Columns.Add("描述", typeof(string));
            dt.Columns.Add("是否可用", typeof(string));
            dt.Columns.Add("更新时间", typeof(string));
            foreach (var item in job)
            {
                dt.Rows.Add
                    (
                        item.JobCode,
                        item.JobName,
                        item.Description,
                        item.IsActive,
                        item.UpdateTime
                    );
            }
            return dt;
        }
    }
}
