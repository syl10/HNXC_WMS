using System;
using System.Collections.Generic;

namespace THOK.Authority.DbModel
{
    public partial class AUTH_LOGIN_LOG
    {
        public string LOG_ID { get; set; }
        public string LOGIN_PC { get; set; }
        public string LOGIN_TIME { get; set; }
        public string LOGOUT_TIME { get; set; }
        public string USER_USER_ID { get; set; }
        public string SYSTEM_SYSTEM_ID { get; set; }
        public virtual AUTH_SYSTEM AUTH_SYSTEM { get; set; }
        public virtual AUTH_USER AUTH_USER { get; set; }
    }
}
