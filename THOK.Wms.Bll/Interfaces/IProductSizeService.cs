using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IProductSizeService : IService<ProductSize>
    {
        object GetDetails(int page, int rows, string ProductCode, string SizeNo, string AreaNo);

        bool Add(ProductSize productSize, out string strResult);

        bool Save(ProductSize productSize, out string strResult);

        bool Delete(int productSizeId, out string strResult);

        object GetProductSize(int page, int rows, string queryString, string value);

        System.Data.DataTable GetProductSize(int page, int rows, string productCode);
    }
}
