using System;
using System.Collections.Generic;

namespace THOK.Authority.DbModel
{
    public partial class AUTH_ROLE_FUNCTION
    {
        public string ROLE_FUNCTION_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string ROLE_MODULE_ROLE_MODULE_ID { get; set; }
        public string FUNCTION_FUNCTION_ID { get; set; }
        public virtual AUTH_FUNCTION AUTH_FUNCTION { get; set; }
        public virtual AUTH_ROLE_MODULE AUTH_ROLE_MODULE { get; set; }
    }
}
