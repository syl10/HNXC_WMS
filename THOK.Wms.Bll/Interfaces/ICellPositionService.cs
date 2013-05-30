using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ICellPositionService : IService<CellPosition>
    {
        object GetDetails(int page, int rows, string CellCode, string StockInPosition, string StockOutPosition);

        bool Add(CellPosition cellPosition, out string strResult);

        bool Save(CellPosition cellPosition, out string strResult);

        bool Delete(int sizeId, out string strResult);

        System.Data.DataTable GetCellPosition(int page, int rows, string cellCode);
    }
}
