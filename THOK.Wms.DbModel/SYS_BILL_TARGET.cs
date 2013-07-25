using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class SYS_BILL_TARGET
    {
        public SYS_BILL_TARGET()
        {
            this.WMS_BILL_MASTER = new List<WMS_BILL_MASTER>();
        }

        public string TARGET_CODE { get; set; }
        public string TARGET_NAME { get; set; }
        public virtual ICollection<WMS_BILL_MASTER> WMS_BILL_MASTER { get; set; }
    }
}
