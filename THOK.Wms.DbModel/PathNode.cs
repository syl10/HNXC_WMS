
namespace THOK.Wms.DbModel
{
    public class PathNode
    {
        public int ID { get; set; }
        public int PathID { get; set; }
        public int PositionID { get; set; }
        public int PathNodeOrder { get; set; }

        public virtual Path Path { get; set; }
        public virtual Position Position { get; set; }
    }
}
