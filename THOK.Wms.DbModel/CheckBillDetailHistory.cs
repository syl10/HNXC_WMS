using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
   public class CheckBillDetailHistory
    {
        public int ID { get; set; }
        public string BillNo { get; set; }
        public string CellCode { get; set; }
        public string StorageCode { get; set; }
        public string ProductCode { get; set; }
        public string UnitCode { get; set; }
        public decimal Quantity { get; set; }
        public string RealProductCode { get; set; }
        public string RealUnitCode { get; set; }
        public decimal RealQuantity { get; set; }
        public Guid? OperatePersonID { get; set; }
        public string Operator { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public string Status { get; set; }

        public virtual CheckBillMasterHistory CheckBillMasterHistory { get; set; }        
    }
}
