using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class WORKSELECT
    {
        public string BILL_NO { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string PRODUCT_BARCODE { get; set; }
        public decimal REAL_WEIGHT { get; set; }
        public string TARGET_CODE { get; set; }
        public string STATE { get; set; }
        public string STATENAME { get; set; }
        public Nullable<System.DateTime> TASK_DATE { get; set; }
        public string TASKER { get; set; }
        public string IS_MIX { get; set; }
        public string MIXNAME { get; set; }
        public string SOURCE_BILLNO { get; set; }
        public string TASK_ID { get; set; }
        public string CELL_CODE { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string GRADE_NAME { get; set; }
        public string ORIGINAL_NAME { get; set; }
        public string STYLE_NAME { get; set; }
        public string YEARS { get; set; }
        public string BTYPE_CODE { get; set; }
        public string BTYPE_NAME { get; set; }
        public string BILL_METHOD { get; set; }
        public string BILLMETHOD { get; set; }
        public string FORMULA_CODE { get; set; }
        public string FORMULA_NAME { get; set; }
        public string CIGARETTE_CODE { get; set; }
        public string CIGARETTE_NAME { get; set; }
        public string USER_NAME { get; set; }
        public Nullable<decimal> BATCH_WEIGHT { get; set; }
        public Nullable<System.DateTime> IN_DATE { get; set; }
    }
}
