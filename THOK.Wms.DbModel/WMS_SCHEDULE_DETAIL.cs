using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class WMS_SCHEDULE_DETAIL
    {
        public string SCHEDULE_NO { get; set; }
        public decimal ITEM_NO { get; set; }
        public string CIGARETTE_CODE { get; set; }
        public string FORMULA_CODE { get; set; }
        public string BILL_NO { get; set; }
        public decimal QUANTITY { get; set; }
        public string LINE_NO { get; set; }
        public virtual CMD_CIGARETTE CMD_CIGARETTE { get; set; }
        public virtual CMD_PRODUCTION_LINE CMD_PRODUCTION_LINE { get; set; }
        public virtual WMS_FORMULA_MASTER WMS_FORMULA_MASTER { get; set; }
    }
}
