using System;
using System.Collections.Generic;

namespace THOK.Authority.DbModel
{
    public partial class AUTH_ROLE
    {
        public AUTH_ROLE()
        {
            this.AUTH_USER_ROLE = new List<AUTH_USER_ROLE>();
            this.AUTH_ROLE_SYSTEM = new List<AUTH_ROLE_SYSTEM>();
        }

        public string ROLE_ID { get; set; }
        public string ROLE_NAME { get; set; }
        public string IS_LOCK { get; set; }
        public string MEMO { get; set; }
        public virtual ICollection<AUTH_USER_ROLE> AUTH_USER_ROLE { get; set; }
        public virtual ICollection<AUTH_ROLE_SYSTEM> AUTH_ROLE_SYSTEM { get; set; }
    }
}
