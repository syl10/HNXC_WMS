using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
   public class ProfitLossBillDetailHistory
    {
       public ProfitLossBillDetailHistory()
        {
        }
        public int ID { get; set; }
        public string BillNo { get; set; }
        public string CellCode { get; set; }
        public string StorageCode { get; set; }
        public string ProductCode { get; set; }
        public string UnitCode { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public string Description { get; set; }

        public virtual ProfitLossBillMasterHistory ProfitLossBillMasterHistory { get; set; }
    }
}
