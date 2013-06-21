using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class CMD_BILL_TYPE
    {
        public string BTYPE_CODE { get; set; }
        public string BTYPE_NAME { get; set; }
        public string BILL_TYPE { get; set; }
        public string TASK_LEVEL { get; set; }
        public string ALLOW_EDIT { get; set; }
        public string MEMO { get; set; }
    }
}
