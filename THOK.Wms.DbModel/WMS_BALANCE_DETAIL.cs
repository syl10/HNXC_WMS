using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
    public partial class WMS_BALANCE_DETAIL
    {
        public string BALANCE_NO { get; set; }
        public string WAREHOUSE_CODE { get; set; }
        public string PRODUCT_CODE { get; set; }
        public Nullable<decimal> BEGIN_QUANTITY { get; set; }
        public Nullable<decimal> IN_QUANTITY { get; set; }
        public Nullable<decimal> OUT_QUANTITY { get; set; }
        public Nullable<decimal> DIFF_QUANTITY { get; set; }
        public Nullable<decimal> ENDQUANTITY { get; set; }
        public Nullable<decimal> INSPECTOUT_QUANTITY { get; set; }
        public Nullable<decimal> INSPECTIN_QUANTITY { get; set; }
        public Nullable<decimal> INCOME_QUANTITY { get; set; }
        public Nullable<decimal> FEEDING_QUANTITY { get; set; }
    }
}
