using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
   public class MoveBillDetailHistory
    {
        public int ID { get; set; }
        public string BillNo { get; set; }
        public int? PalletTag { get; set; }
        public string ProductCode { get; set; }
        public string OutCellCode { get; set; }
        public string OutStorageCode { get; set; }
        public string InCellCode { get; set; }
        public string InStorageCode { get; set; }
        public string UnitCode { get; set; }
        public decimal RealQuantity { get; set; }
        public Guid? OperatePersonID { get; set; }
        public string Operator { get; set; }
        public string CanRealOperate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public string Status { get; set; }

        public virtual MoveBillMasterHistory MoveBillMasterHistory { get; set; }       
    }
}
