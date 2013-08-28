using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
    public partial class WMS_PALLET_MASTER
    {
        public string BILL_NO { get; set; }
        public System.DateTime BILL_DATE { get; set; }
        public string BTYPE_CODE { get; set; }
        public string WAREHOUSE_CODE { get; set; }
        public string TARGET { get; set; }
        public string STATUS { get; set; }
        public string STATE { get; set; }
        public string OPERATER { get; set; }
        public Nullable<System.DateTime> OPERATE_DATE { get; set; }
        public string TASKER { get; set; }
        public Nullable<System.DateTime> TASK_DATE { get; set; }
    }
}
