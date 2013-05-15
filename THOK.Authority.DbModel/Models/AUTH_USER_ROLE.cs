using System;
using System.Collections.Generic;

namespace THOK.Authority.DbModel
{
    public partial class AUTH_USER_ROLE
    {
        public string USER_ROLE_ID { get; set; }
        public string ROLE_ROLE_ID { get; set; }
        public string USER_USER_ID { get; set; }
        public virtual AUTH_ROLE AUTH_ROLE { get; set; }
        public virtual AUTH_USER AUTH_USER { get; set; }
    }
}
