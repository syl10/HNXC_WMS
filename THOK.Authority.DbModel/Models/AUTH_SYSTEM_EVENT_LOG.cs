using System;
using System.Collections.Generic;

namespace THOK.Authority.DbModel
{
    public partial class AUTH_SYSTEM_EVENT_LOG
    {
        public string EVENT_LOG_ID { get; set; }
        public string EVENT_LOG_TIME { get; set; }
        public string EVENT_TYPE { get; set; }
        public string EVENT_NAME { get; set; }
        public string EVENT_DESCRIPTION { get; set; }
        public string FROM_PC { get; set; }
        public string OPERATE_USER { get; set; }
        public string TARGET_SYSTEM { get; set; }
    }
}
