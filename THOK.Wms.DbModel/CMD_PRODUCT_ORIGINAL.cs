using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
    public partial class CMD_PRODUCT_ORIGINAL
    {
        public CMD_PRODUCT_ORIGINAL()
        {
            this.CMD_PRODUCT = new List<CMD_PRODUCT>();
        }

        public string ORIGINAL_CODE { get; set; }
        public string ORIGINAL_NAME { get; set; }
        public string DISTRICT_CODE { get; set; }
        public string MEMO { get; set; }
        public virtual ICollection<CMD_PRODUCT> CMD_PRODUCT { get; set; }
    }
}
