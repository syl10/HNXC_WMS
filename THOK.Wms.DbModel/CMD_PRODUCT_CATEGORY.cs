using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class CMD_PRODUCT_CATEGORY
    {
        public CMD_PRODUCT_CATEGORY()
        {
            this.CMD_PRODUCT = new List<CMD_PRODUCT>();
        }

        public string CATEGORY_CODE { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string MEMO { get; set; }
        public virtual ICollection<CMD_PRODUCT> CMD_PRODUCT { get; set; }
    }
}
