using System;
using System.Collections.Generic;

namespace THOK.Authority.DbModel
{
    public partial class AUTH_USER
    {
        public AUTH_USER()
        {
            this.AUTH_LOGIN_LOG = new List<AUTH_LOGIN_LOG>();
            this.AUTH_USER_ROLE = new List<AUTH_USER_ROLE>();
            this.AUTH_USER_SYSTEM = new List<AUTH_USER_SYSTEM>();
        }

        public string USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public string PWD { get; set; }
        public string CHINESE_NAME { get; set; }
        public string IS_LOCK { get; set; }
        public string IS_ADMIN { get; set; }
        public string LOGIN_PC { get; set; }
        public string MEMO { get; set; }
        public virtual ICollection<AUTH_LOGIN_LOG> AUTH_LOGIN_LOG { get; set; }
        public virtual ICollection<AUTH_USER_ROLE> AUTH_USER_ROLE { get; set; }
        public virtual ICollection<AUTH_USER_SYSTEM> AUTH_USER_SYSTEM { get; set; }
    }
}
