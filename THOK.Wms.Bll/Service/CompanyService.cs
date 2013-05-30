using System;
using System.Linq;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;
using System.Data;
using THOK.WMS.Upload.Bll;

namespace THOK.Wms.Bll.Service
{
    public class CompanyService : ServiceBase<Company>, ICompanyService
    {
        [Dependency]
        public ICompanyRepository CompanyRepository { get; set; }

        UploadBll Upload = new UploadBll();

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region ICompanyService 增，删，改，查等方法

        public object GetDetails(int page, int rows, string CompanyCode, string CompanyName, string CompanyType, string IsActive)
        {
            IQueryable<Company> companyQuery = CompanyRepository.GetQueryable();
            var company = companyQuery.Where(c => c.CompanyCode.Contains(CompanyCode)
                && c.CompanyName.Contains(CompanyName)
                && c.CompanyType.Contains(CompanyType))
                .OrderByDescending(c => c.UpdateTime).AsEnumerable()
                .Select(c => new
                {
                    c.ID,
                    c.CompanyCode,
                    c.CompanyName,
                    c.UniformCode,
                    c.Description,
                    CompanyType = c.CompanyType == "1" ? "配送中心" : c.CompanyType == "2" ? "市公司" : "县公司",
                    c.WarehouseCapacity,
                    c.WarehouseCount,
                    c.WarehouseSpace,
                    c.SortingCount,
                    ParentCompanyName = c.ParentCompany.CompanyName,
                    c.ParentCompanyID,
                    IsActive = c.IsActive == "1" ? "可用" : "不可用",
                    UpdateTime = c.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
                });
            if (!IsActive.Equals(""))
            {
                company = companyQuery.Where(c => c.CompanyCode.Contains(CompanyCode)
                    && c.CompanyName.Contains(CompanyName)
                    && c.CompanyType.Contains(CompanyType)
                    && c.IsActive.Contains(IsActive))
                .OrderByDescending(c => c.UpdateTime).AsEnumerable()
                .Select(c => new
                {
                    c.ID,
                    c.CompanyCode,
                    c.CompanyName,
                    c.UniformCode,
                    c.Description,
                    CompanyType = c.CompanyType == "1" ? "配送中心" : c.CompanyType == "2" ? "市公司" : "县公司",
                    c.WarehouseCapacity,
                    c.WarehouseCount,
                    c.WarehouseSpace,
                    c.SortingCount,
                    ParentCompanyName = c.ParentCompany.CompanyName,
                    c.ParentCompanyID,
                    IsActive = c.IsActive == "1" ? "可用" : "不可用",
                    UpdateTime = c.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            int total = company.Count();
            company = company.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = company.ToArray() };
        }

        public bool Add(Company company, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var comp = new Company();
            var parent = CompanyRepository.GetQueryable().FirstOrDefault(p => p.ID == company.ParentCompanyID);

            var comExist = CompanyRepository.GetQueryable().FirstOrDefault(c => c.CompanyCode == company.CompanyCode);
            if (comExist == null)
            {
                if (comp != null)
                {
                    try
                    {
                        comp.ID = Guid.NewGuid();
                        comp.CompanyCode = company.CompanyCode;
                        comp.CompanyName = company.CompanyName;
                        comp.CompanyType = company.CompanyType;
                        comp.Description = company.Description;
                        comp.ParentCompany = parent ?? comp;
                        comp.UniformCode = company.UniformCode;
                        comp.WarehouseCapacity = company.WarehouseCapacity;
                        comp.WarehouseCount = company.WarehouseCount;
                        comp.WarehouseSpace = company.WarehouseSpace;
                        comp.SortingCount = company.SortingCount;
                        comp.IsActive = company.IsActive;
                        comp.UpdateTime = DateTime.Now;

                        CompanyRepository.Add(comp);
                        CompanyRepository.SaveChanges();
                        //组织机构上报
                        //DataSet ds = this.Insert(comp);
                        //Upload.UploadOrganization (ds);
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

        public bool Delete(string companyID, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            Guid cid = new Guid(companyID);
            var com = CompanyRepository.GetQueryable().FirstOrDefault(c => c.ID == cid);
            if (com != null)
            {
                try
                {
                    Del(CompanyRepository, com.Companies);
                    CompanyRepository.Delete(com);
                    CompanyRepository.SaveChanges();
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

        public bool Save(Company company, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var comp = CompanyRepository.GetQueryable().FirstOrDefault(c => c.ID == company.ID);
            var par = CompanyRepository.GetQueryable().FirstOrDefault(c => c.ID == company.ParentCompanyID);

            if (comp != null)
            {
                try
                {
                    comp.CompanyCode = company.CompanyCode;
                    comp.CompanyName = company.CompanyName;
                    comp.CompanyType = company.CompanyType;
                    comp.Description = company.Description;
                    comp.ParentCompany = par;
                    comp.SortingCount = company.SortingCount;
                    comp.UniformCode = company.UniformCode;
                    comp.UpdateTime = DateTime.Now;
                    comp.WarehouseCapacity = company.WarehouseCapacity;
                    comp.WarehouseCount = company.WarehouseCount;
                    comp.WarehouseSpace = company.WarehouseSpace;
                    comp.IsActive = company.IsActive;
                    CompanyRepository.SaveChanges();
                    //组织机构上报
                    //DataSet ds = this.Insert(comp);
                    //Upload.UploadOrganization (ds);
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

        #region 插入数据到虚拟表
        public DataSet Insert(Company company)
        {
            DataSet ds = this.GenerateEmptyTables();
            DataRow inbrddr = ds.Tables["wms_company"].NewRow();
            inbrddr["company_code"] = company.CompanyCode;
            inbrddr["company_name"] = company.CompanyName;
            inbrddr["company_type"] = company.CompanyType;
            inbrddr["description"] = company.Description;
            inbrddr["parent_company_id"] = company.ParentCompany.CompanyCode;
            inbrddr["uniform_code"] = company.UniformCode;
            inbrddr["warehouse_space"] = company.WarehouseSpace;
            inbrddr["warehouse_count"] = company.WarehouseCount;
            inbrddr["warehouse_capacity"] = company.WarehouseCapacity;
            inbrddr["sorting_count"] = company.SortingCount;
            inbrddr["is_active"] = company.IsActive;
            inbrddr["update_time"] = DateTime.Now;
            ds.Tables["wms_company"].Rows.Add(inbrddr);
            return ds;
        }
        #endregion

        #region 创建一个空的产品表
        private DataSet GenerateEmptyTables()
        {
            DataSet ds = new DataSet();
            DataTable inbrtable = ds.Tables.Add("wms_company");
            inbrtable.Columns.Add("company_code");
            inbrtable.Columns.Add("company_name");
            inbrtable.Columns.Add("company_type");
            inbrtable.Columns.Add("description");
            inbrtable.Columns.Add("parent_company_id");
            inbrtable.Columns.Add("uniform_code");
            inbrtable.Columns.Add("warehouse_space");
            inbrtable.Columns.Add("warehouse_count");
            inbrtable.Columns.Add("warehouse_capacity");
            inbrtable.Columns.Add("sorting_count");
            inbrtable.Columns.Add("is_active");
            inbrtable.Columns.Add("update_time");
            return ds;
        }
        #endregion

        #endregion

        /// <summary>
        /// 查找上级名称
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="CompanyCode"></param>
        /// <param name="CompanyName"></param>
        /// <returns></returns>
        public object GetParentName(int page, int rows, string queryString, string value)
        {
            string companyCode = "", companyName = "";

            if (queryString == "CompanyCode")
            {
                companyCode = value;
            }
            else
            {
                companyName = value;
            }
            IQueryable<Company> companyQuery = CompanyRepository.GetQueryable();
            var company = companyQuery.Where(c => c.CompanyCode.Contains(companyCode) && c.CompanyName.Contains(companyName) && c.IsActive == "1")
                .OrderBy(c => c.CompanyCode).AsEnumerable()
                .Select(c => new
                {
                    c.ID,
                    c.CompanyCode,
                    c.CompanyName,
                    ParentCompanyID = c.ParentCompany.ParentCompanyID,
                    ParentCompanyName = c.ParentCompany.CompanyName,
                    IsActive = c.IsActive == "1" ? "可用" : "不可用",
                    UpdateTime = c.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
                });
            int total = company.Count();
            company = company.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = company.ToArray() };
        }

        public System.Data.DataTable GetCompany(int page, int rows, string companyCode, string companyName, string companyType, string isActive)
        {
            IQueryable<Company> companyQuery = CompanyRepository.GetQueryable();
            var company = companyQuery.Where(c => c.CompanyCode.Contains(companyCode)
                && c.CompanyName.Contains(companyName)
                && c.CompanyType.Contains(companyType))
                .OrderByDescending(c => c.UpdateTime).AsEnumerable()
                .Select(c => new
                {
                    c.CompanyCode,
                    c.CompanyName,
                    c.UniformCode,
                    c.Description,
                    CompanyType = c.CompanyType == "1" ? "配送中心" : c.CompanyType == "2" ? "市公司" : "县公司",
                    c.WarehouseCapacity,
                    c.WarehouseCount,
                    c.WarehouseSpace,
                    c.SortingCount,
                    ParentCompanyName = c.ParentCompany.CompanyName,
                    c.ParentCompanyID,
                    IsActive = c.IsActive == "1" ? "可用" : "不可用",
                    UpdateTime = c.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
                });
            if (!isActive.Equals(""))
            {
                company = companyQuery.Where(c => c.CompanyCode.Contains(companyCode)
                    && c.CompanyName.Contains(companyName)
                    && c.CompanyType.Contains(companyType)
                    && c.IsActive.Contains(isActive))
                .OrderByDescending(c => c.UpdateTime).AsEnumerable()
                .Select(c => new
                {
                    c.CompanyCode,
                    c.CompanyName,
                    c.UniformCode,
                    c.Description,
                    CompanyType = c.CompanyType == "1" ? "配送中心" : c.CompanyType == "2" ? "市公司" : "县公司",
                    c.WarehouseCapacity,
                    c.WarehouseCount,
                    c.WarehouseSpace,
                    c.SortingCount,
                    ParentCompanyName = c.ParentCompany.CompanyName,
                    c.ParentCompanyID,
                    IsActive = c.IsActive == "1" ? "可用" : "不可用",
                    UpdateTime = c.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("公司编码", typeof(string));
            dt.Columns.Add("公司名称", typeof(string));
            dt.Columns.Add("公司类型", typeof(string));
            dt.Columns.Add("上级名称", typeof(string));
            dt.Columns.Add("仓库面积", typeof(string));
            dt.Columns.Add("仓库个数", typeof(string));
            dt.Columns.Add("仓库容量", typeof(string));
            dt.Columns.Add("分拣线数", typeof(string));
            dt.Columns.Add("状态", typeof(string));
            dt.Columns.Add("更新时间", typeof(string));
            foreach (var item in company)
            {
                dt.Rows.Add
                    (
                        item.CompanyCode,
                        item.CompanyName,
                        item.CompanyType,
                        item.ParentCompanyName,
                        item.WarehouseSpace,
                        item.WarehouseCount,
                        item.WarehouseCapacity,
                        item.SortingCount,
                        item.IsActive,
                        item.UpdateTime
                    );

            }
            return dt;
        }
    }
}
