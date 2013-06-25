using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class CMD_UNIT
    {
        
        public string UNIT_CODE { get; set; }
        public string MEMO { get; set; }
        public string UNIT_NAME { get; set; }
        public string CATEGORY_CODE { get; set; }
        public virtual CMD_UNIT_CATEGORY CMD_UNIT_CATEGORY { get; set; }
    }
}
