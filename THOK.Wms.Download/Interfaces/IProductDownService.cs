using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Download.Interfaces
{
    public interface IProductDownService
    {
        Product[] GetProduct(string productCodes);
        Supplier[] GetSupplier(string SupplierCodes);
    }
}
