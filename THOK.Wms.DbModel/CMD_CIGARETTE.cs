using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class CMD_CIGARETTE
    {
        public CMD_CIGARETTE()
        {
            this.WMS_FORMULA_MASTER = new List<WMS_FORMULA_MASTER>();
        }

        public string CIGARETTE_CODE { get; set; }
        public string CIGARETTE_NAME { get; set; }
        public string CIGARETTE_MEMO { get; set; }
        public virtual ICollection<WMS_FORMULA_MASTER> WMS_FORMULA_MASTER { get; set; }
    }
}
