using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class CMD_PRODUCTION_LINE
    {
        public CMD_PRODUCTION_LINE()
        {
            this.WMS_BILL_MASTER = new List<WMS_BILL_MASTER>();
            this.WMS_SCHEDULE_DETAIL = new List<WMS_SCHEDULE_DETAIL>();
        }

        public string LINE_NO { get; set; }
        public string LINE_NAME { get; set; }
        public string MEMO { get; set; }
        public virtual ICollection<WMS_BILL_MASTER> WMS_BILL_MASTER { get; set; }
        public virtual ICollection<WMS_SCHEDULE_DETAIL> WMS_SCHEDULE_DETAIL { get; set; }
    }
}
