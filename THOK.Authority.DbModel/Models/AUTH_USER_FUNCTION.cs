using System;
using System.Collections.Generic;

namespace THOK.Authority.DbModel
{
    public partial class AUTH_USER_FUNCTION
    {
        public string USER_FUNCTION_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string USER_MODULE_USER_MODULE_ID { get; set; }
        public string FUNCTION_FUNCTION_ID { get; set; }
        public virtual AUTH_FUNCTION AUTH_FUNCTION { get; set; }
        public virtual AUTH_USER_MODULE AUTH_USER_MODULE { get; set; }
    }
}
