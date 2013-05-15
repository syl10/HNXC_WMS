using System;
using System.Collections.Generic;

namespace THOK.Authority.DbModel
{
    public partial class AUTH_CITY
    {
        public AUTH_CITY()
        {
            this.AUTH_ROLE_SYSTEM = new List<AUTH_ROLE_SYSTEM>();
            this.AUTH_SERVER = new List<AUTH_SERVER>();
            this.AUTH_USER_SYSTEM = new List<AUTH_USER_SYSTEM>();
        }

        public string CITY_ID { get; set; }
        public string CITY_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string IS_ACTIVE { get; set; }
        public virtual ICollection<AUTH_ROLE_SYSTEM> AUTH_ROLE_SYSTEM { get; set; }
        public virtual ICollection<AUTH_SERVER> AUTH_SERVER { get; set; }
        public virtual ICollection<AUTH_USER_SYSTEM> AUTH_USER_SYSTEM { get; set; }
    }
}
