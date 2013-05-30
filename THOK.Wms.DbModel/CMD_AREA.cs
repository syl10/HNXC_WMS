using System;
using System.Collections.Generic;

namespace  THOK.Wms.DbModel
{
    public partial class CMD_AREA
    {
        public CMD_AREA()
        {
            this.CMD_CELL = new List<CMD_CELL>();
            this.CMD_SHELF = new List<CMD_SHELF>();
        }

        public string AREA_CODE { get; set; }
        public string AREA_NAME { get; set; }
        public string WAREHOUSE_CODE { get; set; }
        public string MEMO { get; set; }
        public virtual ICollection<CMD_CELL> CMD_CELL { get; set; }
        public virtual ICollection<CMD_SHELF> CMD_SHELF { get; set; }
        public virtual CMD_WAREHOUSE CMD_WAREHOUSE { get; set; }
    }
}
