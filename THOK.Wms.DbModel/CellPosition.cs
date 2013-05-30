
namespace THOK.Wms.DbModel
{
    public class CellPosition
    {
        public int ID { get; set; }
        public string CellCode { get; set; }
        public int StockInPositionID { get; set; }
        public int StockOutPositionID { get; set; }

        public virtual Position StockInPosition { get; set; }
        public virtual Position StockOutPosition { get; set; }

    }
}
