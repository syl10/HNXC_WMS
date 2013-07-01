using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Common.Ef
{
    public partial class PrefixTableCode
    {
        public string Prefix { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public string Memo { get; set; }
    }
}
