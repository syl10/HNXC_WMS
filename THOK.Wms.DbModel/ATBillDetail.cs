using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
    public  class ATBillDetail
    {
       public ATBillDetail()
       {
          
       }

       public string WMS_BILL_DETAIL_ID { get; set; }
       public string WMS_BILL_MASTER_ID { get; set; }
       public decimal ITEM_ORDER { get; set; }
       public string PRODUCT_CODE { get; set; }
       public decimal QUANTITY { get; set; }
       public decimal REAL_QUANTITY { get; set; }
       public decimal UNIT_PRICE { get; set; }
       public decimal AMOUNT { get; set; }

      // public virtual ATBillMaster ATBillMaster { get; set; }
    }
    
}
