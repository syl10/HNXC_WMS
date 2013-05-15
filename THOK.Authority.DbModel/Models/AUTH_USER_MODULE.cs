using System;
using System.Collections.Generic;

namespace THOK.Authority.DbModel
{
    public partial class AUTH_USER_MODULE
    {
        public AUTH_USER_MODULE()
        {
            this.AUTH_USER_FUNCTION = new List<AUTH_USER_FUNCTION>();
        }

        public string USER_MODULE_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string USER_SYSTEM_USER_SYSTEM_ID { get; set; }
        public string MODULE_MODULE_ID { get; set; }
        public virtual AUTH_MODULE AUTH_MODULE { get; set; }
        public virtual ICollection<AUTH_USER_FUNCTION> AUTH_USER_FUNCTION { get; set; }
        public virtual AUTH_USER_SYSTEM AUTH_USER_SYSTEM { get; set; }
    }
}
