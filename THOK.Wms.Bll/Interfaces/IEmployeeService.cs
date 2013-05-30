using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IEmployeeService : IService<Employee>
    {
        object GetDetails(int page, int rows, string EmployeeCode, string EmployeeName, string DepartmentID, string JobID, string Status, string IsActive);

        bool Add(Employee employee, out string strResult);

        bool Delete(string employeeId, out string strResult);

        bool Save(Employee employee, out string strResult);

        object GetEmployee(int page, int rows, string queryString, string value);

        System.Data.DataTable GetEmployee(int page, int rows, string employeeCode, string employeeName, string departmentID, string jobID, string status, string isActive);
    }
}
