using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
    public class Region
    {
        public Region()
        {
            this.Positions = new List<Position>();
            this.OriginRegionPath = new List<Path>();
            this.OriginRegionPath = new List<Path>();
        }

        public int ID { get; set; }
        public string RegionName { get; set; }
        public string Description { get; set; }
        public string State { get; set; }

        public virtual ICollection<Position> Positions { get; set; }
        public virtual ICollection<Path> OriginRegionPath { get; set; }
        public virtual ICollection<Path> TargetRegionPath { get; set; }
    }
}
