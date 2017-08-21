using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class WMS_FORMULA_DETAIL
    {
        public string FORMULA_CODE { get; set; }
        public string PRODUCT_CODE { get; set; }
        public decimal FORDER { get; set; }
        public Nullable<decimal> PERCENT { get; set; }
        public string OTHER { get; set; }
        public decimal WEIGHT { get; set; }
        public string MODULES { get; set; }
        public virtual CMD_PRODUCT CMD_PRODUCT { get; set; }
    }
}
