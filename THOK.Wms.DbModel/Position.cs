using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace THOK.Wms.DbModel
{
    public class Position
    {
        public Position()
        {
            this.PathNodes = new List<PathNode>();
            this.StockInCellPosition = new List<CellPosition>();
            this.StockOutCellPosition = new List<CellPosition>();
        }
        public int ID { get; set; }
        public string PositionName { get; set; }
        public string PositionType { get; set; }
        public int RegionID { get; set; }
        public string SRMName { get; set; }
        public int TravelPos { get; set; }
        public int LiftPos { get; set; }
        public int Extension { get; set; }
        public string Description { get; set; }
        public bool HasGoods { get; set; }
        public bool AbleStockOut { get; set; }
        public bool AbleStockInPallet { get; set; }
        public string TagAddress { get; set; }
        public int CurrentTaskID { get; set; }
        public int CurrentOperateQuantity { get; set; }
        public string ChannelCode { get; set; }
        public string State { get; set; }

        public virtual Region Region { get; set; }

        public virtual ICollection<PathNode> PathNodes { get; set; }
        public virtual ICollection<CellPosition> StockInCellPosition { get; set; }
        public virtual ICollection<CellPosition> StockOutCellPosition { get; set; }
    }
}
