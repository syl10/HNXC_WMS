using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using System.Data;
using THOK.WMS.Upload.Bll;

namespace THOK.Wms.Bll.Service
{
    public class EmployeeService : ServiceBase<Employee>, IEmployeeService
    {
        [Dependency]
        public IEmployeeRepository EmployeeRepository { get; set; }

        [Dependency]
        public IJobRepository JobRepository { get; set; }

        [Dependency]
        public IDepartmentRepository DepartmentRepository { get; set; }


        UploadBll Upload = new UploadBll();

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IEmployeeService 成员

        public object GetDetails(int page, int rows, string EmployeeCode, string EmployeeName, string DepartmentID, string JobID, string Status, string IsActive)
        {
            IQueryable<Employee> employeeQuery = EmployeeRepository.GetQueryable();
            var employee = employeeQuery.Where(e => e.EmployeeCode.Contains(EmployeeCode)
                && e.EmployeeName.Contains(EmployeeName)
                && e.Status.Contains(Status)
                && e.IsActive.Contains(IsActive));

            if (!DepartmentID.Equals(string.Empty))
            {
                Guid departID = new Guid(DepartmentID);
                employee = employee.Where(e => e.DepartmentID == departID);
            }
            if (!JobID.Equals(string.Empty))
            {
                Guid jobID = new Guid(JobID);
                employee = employee.Where(e => e.JobID == jobID);
            }

            var temp = employee.AsEnumerable().OrderByDescending(e => e.UpdateTime).Select(e => new
            {
                e.ID,
                e.EmployeeCode,
                e.EmployeeName,
                DepartmentID = e.DepartmentID == null ? string.Empty : e.DepartmentID.ToString(),
                DepartmentName = e.DepartmentID == null ? string.Empty : e.Department.DepartmentName,
                e.Description,
                JobID = e.Job == null ? string.Empty : e.Job.ID.ToString(),
                JobName = e.Job == null ? string.Empty : e.Job.JobName,
                e.Sex,
                e.Tel,
                e.UserName,
                Status = e.Status == "3701" ? "在职" : e.Status == "3702" ? "离职" : e.Status == "3703" ? "退休" : e.Status == "3704" ? "试用" : e.Status == "3705" ? "外调" : e.Status == "3706" ? "停薪留职" : e.Status == "3707" ? "借用" : e.Status == "3708" ? "其他" : "",
                IsActive = e.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = e.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }

        public bool Add(Employee employee, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var emp = new Employee();
            var job = JobRepository.GetQueryable().FirstOrDefault(j => j.ID == employee.JobID);
            var department = DepartmentRepository.GetQueryable().FirstOrDefault(d => d.ID == employee.DepartmentID);
            var empExist = EmployeeRepository.GetQueryable().FirstOrDefault(e => e.EmployeeCode == employee.EmployeeCode);
            if (empExist == null)
            {
                if (emp != null)
                {
                    try
                    {
                        emp.ID = Guid.NewGuid();
                        emp.EmployeeCode = employee.EmployeeCode;
                        emp.EmployeeName = employee.EmployeeName;
                        emp.Description = employee.Description;
                        emp.Department = department;
                        emp.Job = job;
                        emp.Sex = employee.Sex;
                        emp.Tel = employee.Tel;
                        emp.Status = employee.Status;
                        emp.IsActive = employee.IsActive;
                        emp.UserName = employee.UserName;
                        emp.UpdateTime = DateTime.Now;
                        EmployeeRepository.Add(emp);
                        EmployeeRepository.SaveChanges();
                        //人员信息上报
                        //DataSet ds = this.Insert(emp);
                        //Upload.UploadEmployee(ds);
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

        public bool Delete(string employeeId, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            Guid empId = new Guid(employeeId);
            var employee = EmployeeRepository.GetQueryable()
                .FirstOrDefault(e => e.ID == empId);
            if (employee != null)
            {
                try
                {
                    EmployeeRepository.Delete(employee);
                    EmployeeRepository.SaveChanges();
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

        public bool Save(Employee employee, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var emp = EmployeeRepository.GetQueryable().FirstOrDefault(e => e.ID == employee.ID);
            var department = DepartmentRepository.GetQueryable().FirstOrDefault(d => d.ID == employee.DepartmentID);
            var job = JobRepository.GetQueryable().FirstOrDefault(j => j.ID == employee.JobID);

            if (emp != null)
            {
                try
                {
                    emp.EmployeeCode = employee.EmployeeCode;
                    emp.EmployeeName = employee.EmployeeName;
                    emp.Description = employee.Description;
                    emp.Department = department;
                    emp.Job = job;
                    emp.Sex = employee.Sex;
                    emp.Tel = employee.Tel;
                    emp.Status = employee.Status;
                    emp.IsActive = employee.IsActive;
                    emp.UserName = employee.UserName;
                    emp.UpdateTime = DateTime.Now;
                    EmployeeRepository.SaveChanges();
                    //人员信息上报
                    //DataSet ds = this.Insert(emp);
                    //Upload.UploadEmployee (ds);
                    result = true; ;
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

        #region 插入数据到虚拟表
        public DataSet Insert(Employee employee)
        {
            DataSet ds = this.GenerateEmptyTables();
            DataRow inbrddr = ds.Tables["wms_employee"].NewRow();
            inbrddr["employee_code"] = employee.EmployeeCode;
            inbrddr["employee_no"] = employee.EmployeeCode;
            inbrddr["employee_name"] = employee.EmployeeName;
            inbrddr["sex"] = employee.Sex;
            inbrddr["is_active"] = employee.IsActive;
            inbrddr["update_time"] = DateTime.Now;
            ds.Tables["wms_employee"].Rows.Add(inbrddr);
            return ds;
        }
        #endregion

        #region 创建一个空的产品表
        private DataSet GenerateEmptyTables()
        {
            DataSet ds = new DataSet();
            DataTable inbrtable = ds.Tables.Add("wms_employee");
            inbrtable.Columns.Add("employee_code");
            inbrtable.Columns.Add("employee_no");
            inbrtable.Columns.Add("employee_name");
            inbrtable.Columns.Add("sex");
            inbrtable.Columns.Add("is_active");
            inbrtable.Columns.Add("update_time");
            return ds;
        }
        #endregion

        public object GetEmployee(int page, int rows, string queryString, string value)
        {
            string employeeCode = "", employeeName = "";

            if (employeeCode == "employeeCode")
            {
                employeeCode = value;
            }
            else
            {
                employeeName = value;
            }
            IQueryable<Employee> employeeQuery = EmployeeRepository.GetQueryable();
            var employee = employeeQuery.Where(e => e.EmployeeCode.Contains(employeeCode) && e.EmployeeName.Contains(employeeName) && e.IsActive == "1")
                .OrderBy(e => e.EmployeeCode).AsEnumerable()
                .Select(e => new
                {
                    e.ID,
                    e.EmployeeCode,
                    e.EmployeeName,
                    IsActive = e.IsActive == "1" ? "可用" : "不可用",
                });
            int total = employee.Count();
            employee = employee.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = employee.ToArray() };
        }

        public System.Data.DataTable GetEmployee(int page, int rows, string employeeCode, string employeeName, string departmentID, string jobID, string status, string isActive)
        {
            IQueryable<Employee> employeeQuery = EmployeeRepository.GetQueryable();
            var employee = employeeQuery.Where(e => e.EmployeeCode.Contains(employeeCode)
                && e.EmployeeName.Contains(employeeName)
                && e.Status.Contains(status)
                && e.IsActive.Contains(isActive));

            if (!departmentID.Equals(string.Empty))
            {
                Guid gDepartID = new Guid(departmentID);
                employee = employee.Where(e => e.DepartmentID == gDepartID);
            }
            if (!jobID.Equals(string.Empty))
            {
                Guid gJobID = new Guid(jobID);
                employee = employee.Where(e => e.JobID == gJobID);
            }
            var temp = employee.AsEnumerable().OrderByDescending(e => e.UpdateTime).Select(e => new
            {
                e.ID,
                e.EmployeeCode,
                e.EmployeeName,
                DepartmentID = e.DepartmentID == null ? string.Empty : e.DepartmentID.ToString(),
                DepartmentName = e.DepartmentID == null ? string.Empty : e.Department.DepartmentName,
                e.Description,
                JobID = e.Job == null ? string.Empty : e.Job.ID.ToString(),
                JobName = e.Job == null ? string.Empty : e.Job.JobName,
                e.Sex,
                e.Tel,
                e.UserName,
                Status = e.Status == "3701" ? "在职" : e.Status == "3702" ? "离职" : e.Status == "3703" ? "退休" : e.Status == "3704" ? "试用" : e.Status == "3705" ? "外调" : e.Status == "3706" ? "停薪留职" : e.Status == "3707" ? "借用" : e.Status == "3708" ? "其他" : "",
                IsActive = e.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = e.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("员工编码", typeof(string));
            dt.Columns.Add("员工姓名", typeof(string));
            dt.Columns.Add("描述", typeof(string));
            dt.Columns.Add("部门名称", typeof(string));
            dt.Columns.Add("岗位名称", typeof(string));
            dt.Columns.Add("性别", typeof(string));
            dt.Columns.Add("电话", typeof(string));
            dt.Columns.Add("状态", typeof(string));
            dt.Columns.Add("是否可用", typeof(string));
            dt.Columns.Add("更新时间", typeof(string));
            foreach (var item in temp)
            {
                dt.Rows.Add
                    (
                        item.EmployeeCode,
                        item.EmployeeName,
                        item.Description,
                        item.DepartmentName,
                        item.JobName,
                        item.Sex,
                        item.Tel,
                        item.Status,
                        item.IsActive,
                        item.UpdateTime
                    );
            }
            return dt;
        }
    }
}
