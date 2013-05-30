using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
   public interface IJobService:IService<Job>
    {
       object GetDetails(int page, int rows, string JobCode, string JobName, string IsActive);

       bool Add(Job job,out string strResult);

       bool Delete(string JobId, out string strResult);

       bool Save(Job job, out string strResult);

       object GetJob(int page, int rows, string queryString, string value);

       System.Data.DataTable GetJob(int page, int rows, string jobCode, string jobName, string isActive);
    }
}
