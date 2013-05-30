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
    public class DepartmentService : ServiceBase<Department>, IDepartmentService
    {
        [Dependency]
        public IDepartmentRepository DepartmentRepository { get; set; }

        [Dependency]
        public IEmployeeRepository EmployeeRepository { get; set; }

        [Dependency]
        public ICompanyRepository CompanyRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region IDepartmentService 增，删，改，查等方法

        public object GetDetails(int page, int rows, string DepartmentCode, string DepartmentName, string DepartmentLeaderID, string CompanyID)
        {
            IQueryable<Department> departQuery = DepartmentRepository.GetQueryable();
            var department = departQuery.Where(d => d.DepartmentCode.Contains(DepartmentCode)
                && d.DepartmentName.Contains(DepartmentName));

            if (!CompanyID.Equals(string.Empty))
            {
                Guid compId = new Guid(CompanyID);
                department = department.Where(d => d.Company.ID == compId);
            }
            if (!DepartmentLeaderID.Equals(string.Empty))
            {
                Guid empId = new Guid(DepartmentLeaderID);
                department = department.Where(d => d.DepartmentLeader.ID == empId);
            }
            var temp = department.AsEnumerable().OrderByDescending(d => d.UpdateTime).AsEnumerable().Select(d => new
            {
                d.ID,
                d.DepartmentCode,
                d.DepartmentName,
                d.Description,
                d.DepartmentLeaderID,
                EmployeeName = d.DepartmentLeaderID == null ? string.Empty : d.DepartmentLeader.EmployeeName,
                CompanyID = d.Company.ID,
                d.Company.CompanyName,
                ParentDepartmentID = d.ParentDepartmentID,
                ParentDepartmentName = d.ParentDepartment.DepartmentName,
                d.UniformCode,
                IsActive = d.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = d.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }

        public bool Add(Department department, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var newDepartment = new Department();
            var depart = DepartmentRepository.GetQueryable().FirstOrDefault(d => d.ID == department.ParentDepartmentID);
            var employee = EmployeeRepository.GetQueryable().FirstOrDefault(e => e.ID == department.DepartmentLeaderID);
            var company = CompanyRepository.GetQueryable().FirstOrDefault(c => c.ID == department.CompanyID);
            var departExist = DepartmentRepository.GetQueryable().FirstOrDefault(d => d.DepartmentCode == department.DepartmentCode);
            if (departExist == null)
            {
                if (newDepartment != null)
                {
                    try
                    {
                        newDepartment.ID = Guid.NewGuid();
                        newDepartment.DepartmentCode = department.DepartmentCode;
                        newDepartment.DepartmentName = department.DepartmentName;
                        newDepartment.ParentDepartment = depart ?? newDepartment;
                        newDepartment.DepartmentLeader = employee;
                        newDepartment.Description = department.Description;
                        newDepartment.Company = company;
                        newDepartment.UniformCode = department.UniformCode;
                        newDepartment.IsActive = department.IsActive;
                        newDepartment.UpdateTime = DateTime.Now;
                        DepartmentRepository.Add(newDepartment);
                        DepartmentRepository.SaveChanges();
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

        public bool Delete(string departmentId, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            Guid deparId = new Guid(departmentId);
            var departemnt = DepartmentRepository.GetQueryable()
                .FirstOrDefault(c => c.ID == deparId);
            if (departemnt != null)
            {
                try
                {
                    Del(DepartmentRepository, departemnt.Departments);
                    DepartmentRepository.Delete(departemnt);
                    DepartmentRepository.SaveChanges();
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

        public bool Save(Department department, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var depart = DepartmentRepository.GetQueryable().FirstOrDefault(d => d.ID == department.ID);
            var parent = DepartmentRepository.GetQueryable().FirstOrDefault(p => p.ID == department.ParentDepartmentID);
            var employee = EmployeeRepository.GetQueryable().FirstOrDefault(e => e.ID == department.DepartmentLeaderID);
            var company = CompanyRepository.GetQueryable().FirstOrDefault(c => c.ID == department.CompanyID);

            if (depart != null)
            {
                try
                {
                    depart.DepartmentCode = department.DepartmentCode;
                    depart.DepartmentName = department.DepartmentName;
                    //depart.ParentDepartment = depart ?? depart;
                    depart.ParentDepartmentID = department.ParentDepartmentID;
                    depart.DepartmentLeader = employee;
                    depart.Description = department.Description;
                    depart.Company = company;
                    depart.UniformCode = department.UniformCode;
                    depart.IsActive = department.IsActive;
                    depart.UpdateTime = DateTime.Now;
                    DepartmentRepository.SaveChanges();
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

        public object GetDepartment(int page, int rows, string queryString, string value)
        {
            string departmentCode = "", departmentName = "";

            if (queryString == "DepartmentCode")
            {
                departmentCode = value;
            }
            else
            {
                departmentName = value;
            }
            IQueryable<Department> departQuery = DepartmentRepository.GetQueryable();
            var department = departQuery.Where(d => d.DepartmentCode.Contains(departmentCode) && d.DepartmentName.Contains(departmentName) && d.IsActive == "1")
                .OrderBy(d => d.DepartmentCode).AsEnumerable()
                .Select(d => new
                {
                    d.ID,
                    d.DepartmentCode,
                    d.DepartmentName,
                    ParentDepartmentID = d.ParentDepartmentID,
                    IsActive = d.IsActive == "1" ? "可用" : "不可用",
                    ParentDepartmentName = d.ParentDepartment.DepartmentName
                });
            int total = department.Count();
            department = department.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = department.ToArray() };
        }

        public System.Data.DataTable GetDepartment(int page, int rows, string departmentCode, string departmentName, string departmentLeaderID, string companyID)
        {
            IQueryable<Department> departQuery = DepartmentRepository.GetQueryable();
            var department = departQuery.Where(d => d.DepartmentCode.Contains(departmentCode)
                && d.DepartmentName.Contains(departmentName));

            if (!companyID.Equals(string.Empty))
            {
                Guid compId = new Guid(companyID);
                department = department.Where(d => d.Company.ID == compId);
            }
            if (!departmentLeaderID.Equals(string.Empty))
            {
                Guid empId = new Guid(departmentLeaderID);
                department = department.Where(d => d.DepartmentLeader.ID == empId);
            }
            var temp = department.AsEnumerable().OrderByDescending(d => d.UpdateTime).AsEnumerable().Select(d => new
            {
                d.DepartmentCode,
                d.DepartmentName,
                d.Description,
                d.DepartmentLeaderID,
                EmployeeName = d.DepartmentLeaderID == null ? string.Empty : d.DepartmentLeader.EmployeeName,
                CompanyID = d.Company.ID,
                d.Company.CompanyName,
                ParentDepartmentID = d.ParentDepartmentID,
                ParentDepartmentName = d.ParentDepartment.DepartmentName,
                d.UniformCode,
                IsActive = d.IsActive == "1" ? "可用" : "不可用",
                UpdateTime = d.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("部门编码", typeof(string));
            dt.Columns.Add("部门名称", typeof(string));
            dt.Columns.Add("负责人名称", typeof(string));
            dt.Columns.Add("描述", typeof(string));
            dt.Columns.Add("公司名称", typeof(string));
            dt.Columns.Add("上级名称", typeof(string));
            dt.Columns.Add("是否可用", typeof(string));
            dt.Columns.Add("更新时间", typeof(string));
            foreach (var item in temp)
            {
                dt.Rows.Add
                    (
                        item.DepartmentCode,
                        item.DepartmentName,
                        item.EmployeeName,
                        item.Description,
                        item.CompanyName,
                        item.ParentDepartmentName,
                        item.IsActive,
                        item.UpdateTime
                    );   
            }
            return dt;
        }
    }
}
