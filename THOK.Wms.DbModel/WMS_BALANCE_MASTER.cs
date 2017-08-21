using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
    public partial class WMS_BALANCE_MASTER
    {
        public string BALANCE_NO { get; set; }
        public Nullable<System.DateTime> BALANCE_DATE { get; set; }
        public string STATE { get; set; }
        public string OPERATER { get; set; }
        public string CHECKER { get; set; }
        public Nullable<System.DateTime> CHECK_DATE { get; set; }
    }
}
