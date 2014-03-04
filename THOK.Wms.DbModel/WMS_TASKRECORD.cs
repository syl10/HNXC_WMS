using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
    public partial class WMS_TASKRECORD
    {
        public string BILL_NO { get; set; }
        public decimal ITEM_NO { get; set; }
        public string PRODUCT_CODE { get; set; }
        public decimal REAL_WEIGHT { get; set; }
        public string PRODUCT_BARCODE { get; set; }
        public string PALLET_CODE { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string CELL_CODE { get; set; }
        public string ACTION { get; set; }
        public Nullable<System.DateTime> TASK_DATE { get; set; }
        public string TASKER { get; set; }
        public string IS_MIX { get; set; }
        public string INBILL_NO { get; set; }
    }
}
