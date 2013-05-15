using System;
using System.Collections.Generic;

namespace THOK.Authority.DbModel
{
    public partial class AUTH_USER_SYSTEM
    {
        public AUTH_USER_SYSTEM()
        {
            this.AUTH_USER_MODULE = new List<AUTH_USER_MODULE>();
        }

        public string USER_SYSTEM_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string USER_USER_ID { get; set; }
        public string CITY_CITY_ID { get; set; }
        public string SYSTEM_SYSTEM_ID { get; set; }
        public virtual AUTH_CITY AUTH_CITY { get; set; }
        public virtual AUTH_SYSTEM AUTH_SYSTEM { get; set; }
        public virtual AUTH_USER AUTH_USER { get; set; }
        public virtual ICollection<AUTH_USER_MODULE> AUTH_USER_MODULE { get; set; }
    }
}
