using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
    public partial class WMS_PRODUCTION_DETAIL
    {
        public string BILL_NO { get; set; }
        public decimal ITEM_NO { get; set; }
        public string PRODUCT_CODE { get; set; }
        public decimal WEIGHT { get; set; }
        public decimal REAL_WEIGHT { get; set; }
        public decimal PACKAGE_COUNT { get; set; }
        public decimal NC_COUNT { get; set; }
    }
}
