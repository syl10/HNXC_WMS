using System;
using System.Collections.Generic;

namespace  THOK.Wms.DbModel
{
    public partial class CMD_SHELF
    {
        public CMD_SHELF()
        {
            this.CMD_CELL = new List<CMD_CELL>();
        }

        public string SHELF_CODE { get; set; }
        public string SHELF_NAME { get; set; }
        public decimal ROW_COUNT { get; set; }
        public decimal COLUMN_COUNT { get; set; }
        public string WAREHOUSE_CODE { get; set; }
        public string AREA_CODE { get; set; }
        public string CRANE_NO { get; set; }
        public string MEMO { get; set; }
        public string STATION_NO { get; set; }
        public string CRANE_POSITION { get; set; }
        public virtual CMD_AREA CMD_AREA { get; set; }
        public virtual ICollection<CMD_CELL> CMD_CELL { get; set; }
        public virtual CMD_WAREHOUSE CMD_WAREHOUSE { get; set; }
        public virtual CMD_CRANE CMD_CRANE { get; set; }
    }
}
