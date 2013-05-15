using System;
using System.Collections.Generic;

namespace THOK.Authority.DbModel
{
    public partial class AUTH_FUNCTION
    {
        public AUTH_FUNCTION()
        {
            this.AUTH_USER_FUNCTION = new List<AUTH_USER_FUNCTION>();
            this.AUTH_ROLE_FUNCTION = new List<AUTH_ROLE_FUNCTION>();
        }

        public string FUNCTION_ID { get; set; }
        public string FUNCTION_NAME { get; set; }
        public string CONTROL_NAME { get; set; }
        public string INDICATE_IMAGE { get; set; }
        public string MODULE_MODULE_ID { get; set; }
        public virtual ICollection<AUTH_USER_FUNCTION> AUTH_USER_FUNCTION { get; set; }
        public virtual ICollection<AUTH_ROLE_FUNCTION> AUTH_ROLE_FUNCTION { get; set; }
        public virtual AUTH_MODULE AUTH_MODULE { get; set; }
    }
}
