using System;
using System.Collections.Generic;

namespace THOK.Wms.DbModel
{
    public partial class CMD_CRANE
    {
        public CMD_CRANE()
        {
            this.CMD_SHELF = new List<CMD_SHELF>();
        }

        public string CRANE_NO { get; set; }
        public string CRANE_NAME { get; set; }
        public string IS_ACTIVE { get; set; }
        public string MEMO { get; set; }
        public virtual ICollection<CMD_SHELF> CMD_SHELF { get; set; }
    }
}
