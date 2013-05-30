using System;
using System.Collections.Generic;

namespace  THOK.Wms.DbModel
{
    public partial class CMD_WAREHOUSE
    {
        public CMD_WAREHOUSE()
        {
            this.CMD_AREA = new List<CMD_AREA>();
            this.CMD_CELL = new List<CMD_CELL>();
            this.CMD_SHELF = new List<CMD_SHELF>();
        }

        public string WAREHOUSE_CODE { get; set; }
        public string WAREHOUSE_NAME { get; set; }
        public string MEMO { get; set; }
        public virtual ICollection<CMD_AREA> CMD_AREA { get; set; }
        public virtual ICollection<CMD_CELL> CMD_CELL { get; set; }
        public virtual ICollection<CMD_SHELF> CMD_SHELF { get; set; }
    }
}
