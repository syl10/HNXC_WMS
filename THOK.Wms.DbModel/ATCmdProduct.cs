using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
    public class ATCmdProduct
    {
        public string CMD_PRODUCT_ID { get; set; }
        public string PRODUCT_CODE { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string BAR_CODE { get; set; }
        public decimal PALLET_QUANTITY { get; set; }
        public decimal QUANTITY { get; set; }
  
    }
}
