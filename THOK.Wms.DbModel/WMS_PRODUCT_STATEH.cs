using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
    public partial class WMS_PRODUCT_STATEH
    {
        public string BILL_NO { get; set; }
        public decimal ITEM_NO { get; set; }
        public string SCHEDULE_NO { get; set; }
        public string PRODUCT_CODE { get; set; }
        public decimal WEIGHT { get; set; }
        public decimal REAL_WEIGHT { get; set; }
        public decimal PACKAGE_COUNT { get; set; }
        public string OUT_BILLNO { get; set; }
        public string CELL_CODE { get; set; }
        public string NEWCELL_CODE { get; set; }
        public string PRODUCT_BARCODE { get; set; }
        public string PALLET_CODE { get; set; }
        public string IS_MIX { get; set; }
        public string OLD_PALLET_CODE { get; set; }
        public Nullable<decimal> FORDER { get; set; }
        public string FORDERBILLNO { get; set; }
    }
}
