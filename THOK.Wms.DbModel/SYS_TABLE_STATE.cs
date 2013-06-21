using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class SYS_TABLE_STATE
    {
        public string TABLE_NAME { get; set; }
        public string FIELD_NAME { get; set; }
        public string STATE { get; set; }
        public string STATE_DESC { get; set; }
    }
}
