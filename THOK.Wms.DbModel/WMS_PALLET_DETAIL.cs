using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
    public partial class WMS_PALLET_DETAIL
    {
        public string BILL_NO { get; set; }
        public decimal ITEM_NO { get; set; }
        public string PRODUCT_CODE { get; set; }
        public Nullable<decimal> QUANTITY { get; set; }
        public Nullable<decimal> PACKAGES { get; set; }
    }
}
