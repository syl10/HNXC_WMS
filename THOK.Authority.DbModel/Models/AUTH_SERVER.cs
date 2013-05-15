using System;
using System.Collections.Generic;

namespace THOK.Authority.DbModel
{
    public partial class AUTH_SERVER
    {
        public string SERVER_ID { get; set; }
        public string SERVER_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string URL { get; set; }
        public string IS_ACTIVE { get; set; }
        public string CITY_CITY_ID { get; set; }
        public virtual AUTH_CITY AUTH_CITY { get; set; }
    }
}
