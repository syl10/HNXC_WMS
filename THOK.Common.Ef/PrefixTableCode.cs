using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Common.Ef
{
    public partial class PrefixTableCode
    {
        public string PREFIX_CODE { get; set; }
        public string TABLE_NAME { get; set; }
        public string FIELD_NAME { get; set; }
        public string MEMO { get; set; }
        public string DATE_FORMAT { get; set; }
        public string  SERIAL_LENGTH { get; set; }

    }
}
