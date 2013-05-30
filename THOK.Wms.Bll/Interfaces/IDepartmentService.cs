using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IDepartmentService : IService<Department>
    {
        object GetDetails(int page, int rows, string DepartmentCode, string DepartmentName, string DepartmentLeaderID, string CompanyID);

        bool Add(Department department, out string strResult);

        bool Delete(string departmentId, out string strResult);

        bool Save(Department department, out string strResult);

        object GetDepartment(int page, int rows, string queryString, string value);

        System.Data.DataTable GetDepartment(int page, int rows, string departmentCode, string departmentName, string departmentLeaderID, string companyID);
    }
}
