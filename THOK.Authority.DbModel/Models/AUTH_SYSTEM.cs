using System;
using System.Collections.Generic;

namespace THOK.Authority.DbModel
{
    public partial class AUTH_SYSTEM
    {
        public AUTH_SYSTEM()
        {
            this.AUTH_LOGIN_LOG = new List<AUTH_LOGIN_LOG>();
            this.AUTH_MODULE = new List<AUTH_MODULE>();
            this.AUTH_ROLE_SYSTEM = new List<AUTH_ROLE_SYSTEM>();
            this.AUTH_SYSTEM_PARAMETER = new List<AUTH_SYSTEM_PARAMETER>();
            this.AUTH_USER_SYSTEM = new List<AUTH_USER_SYSTEM>();
        }

        public string SYSTEM_ID { get; set; }
        public string SYSTEM_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string STATUS { get; set; }
        public virtual ICollection<AUTH_LOGIN_LOG> AUTH_LOGIN_LOG { get; set; }
        public virtual ICollection<AUTH_MODULE> AUTH_MODULE { get; set; }
        public virtual ICollection<AUTH_ROLE_SYSTEM> AUTH_ROLE_SYSTEM { get; set; }
        public virtual ICollection<AUTH_SYSTEM_PARAMETER> AUTH_SYSTEM_PARAMETER { get; set; }
        public virtual ICollection<AUTH_USER_SYSTEM> AUTH_USER_SYSTEM { get; set; }
    }
}
