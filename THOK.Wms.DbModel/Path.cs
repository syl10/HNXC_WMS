
using System.Collections.Generic;
namespace THOK.Wms.DbModel
{
    public class Path
    {
        public Path()
        {
            this.PathNodes = new List<PathNode>();
        }
        public int ID { get; set; }
        public string PathName { get; set; }
        public int OriginRegionID { get; set; }
        public int TargetRegionID { get; set; }
        public string Description { get; set; }
        public string State { get; set; }

        public virtual Region OriginRegion { get; set; }
        public virtual Region TargetRegion { get; set; }

        public virtual ICollection<PathNode> PathNodes { get; set; }
    }
}
