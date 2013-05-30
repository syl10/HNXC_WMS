using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class CMD_PRODUCT
    {
        public CMD_PRODUCT()
        {
            this.CMD_CELL = new List<CMD_CELL>();
        }

        public string PRODUCT_CODE { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string ORIGINAL { get; set; }
        public string YEARS { get; set; }
        public string GRADE { get; set; }
        public string STYLE { get; set; }
        public decimal WEIGHT { get; set; }
        public string MEMO { get; set; }
        public virtual ICollection<CMD_CELL> CMD_CELL { get; set; }
    }
}
