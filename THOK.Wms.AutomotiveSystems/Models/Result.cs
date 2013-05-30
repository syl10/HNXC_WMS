using System;
using System.Collections.Generic;
using System.Text;

namespace THOK.Wms.AutomotiveSystems.Models
{
    public class Result
    {
        public bool IsSuccess = false;
        public string Message = string.Empty;
        public BillMaster[] BillMasters = new BillMaster[] { };
        public BillDetail[] BillDetails = new BillDetail[] { };
    }
}
