using System;
using System.Collections.Generic;

namespace THOK.Authority.DbModel
{
    public partial class AUTH_MODULE
    {
        public AUTH_MODULE()
        {
            this.AUTH_FUNCTION = new List<AUTH_FUNCTION>();
            this.AUTH_HELP_CONTENT = new List<AUTH_HELP_CONTENT>();
            this.AUTH_USER_MODULE = new List<AUTH_USER_MODULE>();
            this.AUTH_ROLE_MODULE = new List<AUTH_ROLE_MODULE>();
            this.AUTH_MODULES = new List<AUTH_MODULE>();
        }

        public string MODULE_ID { get; set; }
        public string MODULE_NAME { get; set; }
        public decimal SHOW_ORDER { get; set; }
        public string MODULE_URL { get; set; }
        public string INDICATE_IMAGE { get; set; }
        public string DESK_TOP_IMAGE { get; set; }
        public string SYSTEM_SYSTEM_ID { get; set; }
        public string PARENT_MODULE_MODULE_ID { get; set; }
        public virtual ICollection<AUTH_FUNCTION> AUTH_FUNCTION { get; set; }
        public virtual ICollection<AUTH_HELP_CONTENT> AUTH_HELP_CONTENT { get; set; }
        public virtual ICollection<AUTH_USER_MODULE> AUTH_USER_MODULE { get; set; }
        public virtual ICollection<AUTH_ROLE_MODULE> AUTH_ROLE_MODULE { get; set; }
        public virtual ICollection<AUTH_MODULE> AUTH_MODULES { get; set; }
        public virtual AUTH_MODULE PARENT_AUTH_MODULE { get; set; }
        public virtual AUTH_SYSTEM AUTH_SYSTEM { get; set; }
    }
}
