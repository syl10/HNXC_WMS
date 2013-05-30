using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.AutomotiveSystems.Models
{
    public class TaskParameter
    {
        public string Method { get; set; }
        public string[] BillTypes { get; set; }
        public string productCode{ get; set; }
        public string OperateType { get; set; }
        public string OperateArea { get; set; }
        public string Operator { get; set; }
        public BillMaster[] BillMasters { get; set; }
        public BillDetail[] BillDetails { get; set; }
        public string UseTag { get; set; }
        public string RfidId { get; set; }
    }
}
