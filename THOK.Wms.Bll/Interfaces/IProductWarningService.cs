using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using System.Data;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IProductWarningService:IService<ProductWarning>
    {
        object GetDetail(int page, int rows, string productCode, decimal minLimited, decimal maxLimited, decimal assemblyTime);

        object GetStorageByTime();

        bool Add(ProductWarning productWarning);

        bool Delete(string productCode);

        bool Save(ProductWarning productWarning);

        object GetQtyLimitsDetail(int page, int rows, string productCode, decimal minLimited, decimal maxLimited, string unitCode);


        object GetProductDetails(int page, int rows, string productCode, decimal assemblyTime);

        object GetWarningPrompt();
        object GetCellInfo();

        object GetCell();
        System.Data.DataTable GetProductWarning(int page, int rows, string productCode, decimal minLimited, decimal maxLimited, decimal assemblyTime);

        System.Data.DataTable GetQuantityLimitsDetail(int page, int rows, string productCode, decimal minLimited, decimal maxLimited, string unitCode);

        System.Data.DataTable GetProductTimeOut(int page, int rows, string productCode, decimal assemblyTime);
    }
}
