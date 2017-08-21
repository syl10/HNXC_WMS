using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class CMD_UNIT_CATEGORY
    {
        public CMD_UNIT_CATEGORY()
        {
            this.CMD_UNIT = new List<CMD_UNIT>();
        }

        public string CATEGORY_CODE { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string MEMO { get; set; }
        public virtual ICollection<CMD_UNIT> CMD_UNIT { get; set; }
    }
}
