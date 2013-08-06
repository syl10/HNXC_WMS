using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
    public partial class WCS_TASK
    {
        public string TASK_ID { get; set; }
        public string TASK_TYPE { get; set; }
        public decimal TASK_LEVEL { get; set; }
        public string BILL_NO { get; set; }
        public string PRODUCT_CODE { get; set; }
        public decimal REAL_WEIGHT { get; set; }
        public string PRODUCT_BARCODE { get; set; }
        public string PALLET_CODE { get; set; }
        public string CELL_CODE { get; set; }
        public string TARGET_CODE { get; set; }
        public string STATE { get; set; }
        public Nullable<System.DateTime> TASK_DATE { get; set; }
        public string TASKER { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string NEWCELL_CODE { get; set; }
        public Nullable<System.DateTime> FINISH_DATE { get; set; }
        public string IS_MIX { get; set; }
    }
}
