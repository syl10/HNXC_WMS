using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class WMS_BILL_MASTER
    {
        public string BILL_NO { get; set; }
        public System.DateTime BILL_DATE { get; set; }
        public string BTYPE_CODE { get; set; }
        public string SCHEDULE_NO { get; set; }
        public string WAREHOUSE_CODE { get; set; }
        public string TARGET_CODE { get; set; }
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
        public decimal SCHEDULE_ITEMNO { get; set; }
        public string LINE_NO { get; set; }
        public string CIGARETTE_CODE { get; set; }
        public string FORMULA_CODE { get; set; }
        public decimal BATCH_WEIGHT { get; set; }
        public virtual CMD_BILL_TYPE CMD_BILL_TYPE { get; set; }
        public virtual CMD_CIGARETTE CMD_CIGARETTE { get; set; }
        public virtual CMD_PRODUCTION_LINE CMD_PRODUCTION_LINE { get; set; }
        public virtual CMD_WAREHOUSE CMD_WAREHOUSE { get; set; }
        public virtual SYS_BILL_TARGET SYS_BILL_TARGET { get; set; }
        public virtual WMS_FORMULA_MASTER WMS_FORMULA_MASTER { get; set; }
    }
}
