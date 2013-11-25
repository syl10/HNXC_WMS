using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class PRINTREPORT
    {
        public string PRODUCT_BARCODE { get; set; }
        public string BILL_NO { get; set; }
        public string FORMULA_NAME { get; set; }
        public string CIGARETTE_NAME { get; set; }
        public string PRODUCT_CODE { get; set; }
        public decimal REAL_WEIGHT { get; set; }
        public Nullable<System.DateTime> BILL_DATE { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string YEARS { get; set; }
        public string ORIGINAL_NAME { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string GRADE_NAME { get; set; }
        public string STYLE_NAME { get; set; }
        public string MODULES { get; set; }
        public string PACKAGECOUNT { get; set; }
        public string IS_MIX { get; set; }
    }
}
