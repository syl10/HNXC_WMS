using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ICMDProductService:IService<CMD_PRODUCT>
    {
        object GetDetails(int page, int rows, string ProductName, string ProductCode, string CustomCode, string BrandCode, string UniformCode, string AbcTypeCode, string ShortCode, string PriceLevelCode, string SupplierCode);

        bool Add(CMD_PRODUCT product);
        
        bool Delete(string ProductCode);

        bool Save(CMD_PRODUCT product);


        //object FindProduct(int page, int rows, string QueryString, string value);

        //object checkFindProduct(string QueryString, string value);

        object FindProduct();

        object LoadProduct(int page, int rows);

        object GetProductBy(int page, int rows, string QueryString, string Value);

        System.Data.DataTable GetProduct(int page, int rows, string ProductName, string ProductCode, string CustomCode, string BrandCode, string UniformCode, string AbcTypeCode, string ShortCode, string PriceLevelCode, string SupplierCode);
    }
}
