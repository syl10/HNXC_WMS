using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
    public partial class CMD_PRODUCT_GRADE
    {
        public CMD_PRODUCT_GRADE()
        {
            this.CMD_PRODUCT = new List<CMD_PRODUCT>();
        }

        public string GRADE_CODE { get; set; }
        public string ENGLISH_CODE { get; set; }
        public string USER_CODE { get; set; }
        public string GRADE_NAME { get; set; }
        public string MEMO { get; set; }
        public virtual ICollection<CMD_PRODUCT> CMD_PRODUCT { get; set; }
    }
}
