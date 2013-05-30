using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ISizeService : IService<Size>
    {
        object GetDetails(int page, int rows,string SizeName, string SizeNo);

        bool Add(Size size, out string strResult);

        bool Save(Size size, out string strResult);

        bool Delete(int sizeId, out string strResult);

        object GetSize(int page, int rows, string queryString, string value);

        System.Data.DataTable GetSize(int page, int rows, string sizeName);
    }
}
