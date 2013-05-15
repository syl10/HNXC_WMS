using System;
using System.Collections.Generic;

namespace THOK.Authority.DbModel
{
    public partial class AUTH_SYSTEM_PARAMETER
    {
        public decimal ID { get; set; }
        public string PARAMETER_NAME { get; set; }
        public string PARAMETER_VALUE { get; set; }
        public string REMARK { get; set; }
        public string USER_NAME { get; set; }
        public string SYSTEM_ID { get; set; }
        public virtual AUTH_SYSTEM AUTH_SYSTEM { get; set; }
    }
}
