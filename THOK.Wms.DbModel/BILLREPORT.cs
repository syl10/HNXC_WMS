using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
    public partial class BILLREPORT
    {
        public string MBILL_NO { get; set; }
        public System.DateTime BILL_DATE { get; set; }
        public string BTYPE_CODE { get; set; }
        public string BTYPE_NAME { get; set; }
        public string BILL_TYPE { get; set; }
        public string SCHEDULE_NO { get; set; }
        public string WAREHOUSE_CODE { get; set; }
        public string WAREHOUSE_NAME { get; set; }
        public string TARGET_CODE { get; set; }
        public string TARGET_NAME { get; set; }
        public string STATUS { get; set; }
        public string STATE { get; set; }
        public string SOURCE_BILLNO { get; set; }
        public string OPERATER { get; set; }
        public Nullable<System.DateTime> OPERATE_DATE { get; set; }
        public string CHECKER { get; set; }
        public Nullable<System.DateTime> CHECK_DATE { get; set; }
        public string TASKER { get; set; }
        public Nullable<System.DateTime> TASK_DATE { get; set; }
        public string BILL_METHOD { get; set; }
        public string BILLMETHODCODE { get; set; }
        public decimal SCHEDULE_ITEMNO { get; set; }
        public string LINE_NO { get; set; }
        public string LINE_NAME { get; set; }
        public string CIGARETTE_CODE { get; set; }
        public string CIGARETTE_NAME { get; set; }
        public string FORMULA_CODE { get; set; }
        public string FORMULA_NAME { get; set; }
        public decimal BATCH_WEIGHT { get; set; }
        public string DBILL_NO { get; set; }
        public Nullable<decimal> ITEM_NO { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string YEARS { get; set; }
        public string ORIGINAL_NAME { get; set; }
        public string GRADE_NAME { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string STYLE_NAME { get; set; }
        public Nullable<decimal> WEIGHT { get; set; }
        public Nullable<decimal> REAL_WEIGHT { get; set; }
        public Nullable<decimal> PACKAGE_COUNT { get; set; }
        public Nullable<decimal> NC_COUNT { get; set; }
        public string IS_MIX { get; set; }
        public string ISMIX { get; set; }
        public string FPRODUCT_CODE { get; set; }
        public Nullable<decimal> TOTALWEIGHT { get; set; }
    }
}
