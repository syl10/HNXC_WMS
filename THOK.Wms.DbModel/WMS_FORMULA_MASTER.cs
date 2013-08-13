using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class WMS_FORMULA_MASTER
    {
        public WMS_FORMULA_MASTER()
        {
            this.WMS_BILL_MASTER = new List<WMS_BILL_MASTER>();
            this.WMS_PRODUCTION_MASTER = new List<WMS_PRODUCTION_MASTER>();
            this.WMS_SCHEDULE = new List<WMS_SCHEDULE>();
            this.WMS_SCHEDULE_DETAIL = new List<WMS_SCHEDULE_DETAIL>();
        }

        public string FORMULA_CODE { get; set; }
        public string FORMULA_NAME { get; set; }
        public System.DateTime FORMULA_DATE { get; set; }
        public string CIGARETTE_CODE { get; set; }
        public string OPERATER { get; set; }
        public System.DateTime OPERATEDATE { get; set; }
        public decimal USE_COUNT { get; set; }
        public decimal FORMULANO { get; set; }
        public string IS_ACTIVE { get; set; }
        public virtual CMD_CIGARETTE CMD_CIGARETTE { get; set; }
        public virtual ICollection<WMS_BILL_MASTER> WMS_BILL_MASTER { get; set; }
        public virtual ICollection<WMS_PRODUCTION_MASTER> WMS_PRODUCTION_MASTER { get; set; }
        public virtual ICollection<WMS_SCHEDULE> WMS_SCHEDULE { get; set; }
        public virtual ICollection<WMS_SCHEDULE_DETAIL> WMS_SCHEDULE_DETAIL { get; set; }
    }
}
