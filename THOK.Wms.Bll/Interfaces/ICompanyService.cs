using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ICompanyService : IService<Company>
    {
        object GetDetails(int page, int rows, string CompanyCode, string CompanyName, string CompanyType, string IsActive);

        bool Add(Company company, out string strResult);

        bool Delete(string companyID, out string strResult);

        bool Save(Company company, out string strResult);

        object GetParentName(int page, int rows, string queryString, string value);

        System.Data.DataTable GetCompany(int page, int rows, string companyCode, string companyName, string companyType, string isActive);
    }
}
