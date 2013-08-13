using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class CMD_CIGARETTE
    {
        public CMD_CIGARETTE()
        {
            this.WMS_BILL_MASTER = new List<WMS_BILL_MASTER>();
            this.WMS_FORMULA_MASTER = new List<WMS_FORMULA_MASTER>();
            this.WMS_PRODUCTION_MASTER = new List<WMS_PRODUCTION_MASTER>();
            this.WMS_SCHEDULE = new List<WMS_SCHEDULE>();
            this.WMS_SCHEDULE_DETAIL = new List<WMS_SCHEDULE_DETAIL>();
        }

        public string CIGARETTE_CODE { get; set; }
        public string CIGARETTE_NAME { get; set; }
        public string CIGARETTE_MEMO { get; set; }
        public virtual ICollection<WMS_BILL_MASTER> WMS_BILL_MASTER { get; set; }
        public virtual ICollection<WMS_FORMULA_MASTER> WMS_FORMULA_MASTER { get; set; }
        public virtual ICollection<WMS_PRODUCTION_MASTER> WMS_PRODUCTION_MASTER { get; set; }
        public virtual ICollection<WMS_SCHEDULE> WMS_SCHEDULE { get; set; }
        public virtual ICollection<WMS_SCHEDULE_DETAIL> WMS_SCHEDULE_DETAIL { get; set; }
    }
}
