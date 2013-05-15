using System;
using System.Collections.Generic;

namespace THOK.Authority.DbModel
{
    public partial class AUTH_ROLE_SYSTEM
    {
        public AUTH_ROLE_SYSTEM()
        {
            this.AUTH_ROLE_MODULE = new List<AUTH_ROLE_MODULE>();
        }

        public string ROLE_SYSTEM_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string ROLE_ROLE_ID { get; set; }
        public string CITY_CITY_ID { get; set; }
        public string SYSTEM_SYSTEM_ID { get; set; }
        public virtual AUTH_CITY AUTH_CITY { get; set; }
        public virtual AUTH_ROLE AUTH_ROLE { get; set; }
        public virtual ICollection<AUTH_ROLE_MODULE> AUTH_ROLE_MODULE { get; set; }
        public virtual AUTH_SYSTEM AUTH_SYSTEM { get; set; }
    }
}
