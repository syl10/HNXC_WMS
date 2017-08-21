using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class CMD_PRODUCT_STYLE
    {
        public CMD_PRODUCT_STYLE()
        {
            this.CMD_PRODUCT = new List<CMD_PRODUCT>();
        }

        public string STYLE_NO { get; set; }
        public string STYLE_NAME { get; set; }
        public string SORT_LEVEL { get; set; }
        public virtual ICollection<CMD_PRODUCT> CMD_PRODUCT { get; set; }
    }
}
