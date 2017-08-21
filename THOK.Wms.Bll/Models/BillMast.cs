using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.Bll.Models
{
    public  class BillMast
    {
        public string BILL_NO { get; set; }
        public System.DateTime BILL_DATE { get; set; }
        public string BTYPE_CODE { get; set; }
        public string SCHEDULE_NO { get; set; }
        public string WAREHOUSE_CODE { get; set; }
        public string TARGET_CODE { get; set; }
        public string STATUS { get; set; }
        public string STATE { get; set; }
        public string SOURCE_BILLNO { get; set; }
        public string OPERATER { get; set; }
        public Nullable<System.DateTime> OPERATE_DATE { get; set; }
        public string CHECKER { get; set; }
        public Nullable<System.DateTime> CHECK_DATE { get; set; }
        public string TASKER { get; set; }
        public Nullable<System.DateTime> TASK_DATE { get; set; }
        public string BILL_METHOD { get; set; }
        public decimal SCHEDULE_ITEMNO { get; set; }
        public string LINE_NO { get; set; }
        public string CIGARETTE_CODE { get; set; }
        public string FORMULA_CODE { get; set; }
        public decimal BATCH_WEIGHT { get; set; }
    }
}
