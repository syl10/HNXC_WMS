using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class WMS_FORMULA_MASTER
    {
        public string FORMULA_CODE { get; set; }
        public string FORMULA_NAME { get; set; }
        public System.DateTime FORMULA_DATE { get; set; }
        public string CIGARETTE_CODE { get; set; }
        public string OPERATER { get; set; }
        public System.DateTime OPERATEDATE { get; set; }
        public decimal USE_COUNT { get; set; }
        public decimal FORMULANO { get; set; }
        public string IS_ACTIVE { get; set; }
    }
}
