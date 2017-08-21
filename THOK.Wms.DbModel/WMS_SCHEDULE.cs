using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class WMS_SCHEDULE
    {
        public decimal QUANTITY { get; set; }
        public System.DateTime SCHEDULE_DATE { get; set; }
        public string SCHEDULE_NO { get; set; }
        public string CIGARETTE_CODE { get; set; }
        public string CIGARETTE_NAME { get; set; }
        public string FORMULA_CODE { get; set; }
        public string SOURCE_BILLNO { get; set; }
        public string STATUS { get; set; }
        public string STATE { get; set; }
        public string OPERATER { get; set; }
        public Nullable<System.DateTime> OPERATE_DATE { get; set; }
        public string CHECKER { get; set; }
        public Nullable<System.DateTime> CHECK_DATE { get; set; }
    }
}
