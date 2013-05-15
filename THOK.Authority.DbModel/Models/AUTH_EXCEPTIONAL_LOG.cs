using System;
using System.Collections.Generic;

namespace THOK.Authority.DbModel
{
    public partial class AUTH_EXCEPTIONAL_LOG
    {
        public string EXCEPTIONAL_LOG_ID { get; set; }
        public string CATCH_TIME { get; set; }
        public string MODULE_NAME { get; set; }
        public string FUNCTION_NAME { get; set; }
        public string EXCEPTIONAL_TYPE { get; set; }
        public string EXCEPTIONAL_DESCRIPTION { get; set; }
        public string STATE { get; set; }
    }
}
