using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class CMD_CELL
    {
        public string CELL_CODE { get; set; }
        public string CELL_NAME { get; set; }
        public string AREA_CODE { get; set; }
        public string SHELF_CODE { get; set; }
        public decimal CELL_COLUMN { get; set; }
        public decimal CELL_ROW { get; set; }
        public string IS_ACTIVE { get; set; }
        public string PRODUCT_CODE { get; set; }
        public Nullable<decimal> REAL_WEIGHT { get; set; }
        public string SCHEDULE_NO { get; set; }
        public string IS_LOCK { get; set; }
        public string PALLET_CODE { get; set; }
        public string BILL_NO { get; set; }
        public Nullable<System.DateTime> IN_DATE { get; set; }
        public string MEMO { get; set; }
        public Nullable<decimal> PRIORITY_LEVEL { get; set; }
        public string ERROR_FLAG { get; set; }
        public string WAREHOUSE_CODE { get; set; }
        public virtual CMD_AREA CMD_AREA { get; set; }
        public virtual CMD_PRODUCT CMD_PRODUCT { get; set; }
        public virtual CMD_SHELF CMD_SHELF { get; set; }
        public virtual CMD_WAREHOUSE CMD_WAREHOUSE { get; set; }
    }
}
