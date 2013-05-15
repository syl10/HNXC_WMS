using System;
using System.Collections.Generic;

namespace THOK.Authority.DbModel
{
    public partial class AUTH_ROLE_MODULE
    {
        public AUTH_ROLE_MODULE()
        {
            this.AUTH_ROLE_FUNCTION = new List<AUTH_ROLE_FUNCTION>();
        }

        public string ROLE_MODULE_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string ROLE_SYSTEM_ROLE_SYSTEM_ID { get; set; }
        public string MODULE_MODULE_ID { get; set; }
        public virtual AUTH_MODULE AUTH_MODULE { get; set; }
        public virtual ICollection<AUTH_ROLE_FUNCTION> AUTH_ROLE_FUNCTION { get; set; }
        public virtual AUTH_ROLE_SYSTEM AUTH_ROLE_SYSTEM { get; set; }
    }
}
