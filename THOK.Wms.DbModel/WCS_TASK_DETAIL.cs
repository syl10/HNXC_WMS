using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
    public partial class WCS_TASK_DETAIL
    {
        public string TASK_ID { get; set; }
        public decimal ITEM_NO { get; set; }
        public string TASK_NO { get; set; }
        public string ASSIGNMENT_ID { get; set; }
        public string CRANE_NO { get; set; }
        public string CAR_NO { get; set; }
        public string FROM_STATION { get; set; }
        public string TO_STATION { get; set; }
        public string STATE { get; set; }
        public string SERVICE_NAME { get; set; }
        public string ITEM_NAME { get; set; }
        public string ITEM_VALUE { get; set; }
        public string DESCRIPTION { get; set; }
        public string BILL_NO { get; set; }
        public string SQUENCE_NO { get; set; }
        public string ERR_CODE { get; set; }
    }
}
