using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
   public class TrayInfo
    {
       public TrayInfo()
       {           
       }

       public Guid TaryID { get; set; }
       public string TaryRfid { get; set; }
       public string ProductCode { get; set; }
       public decimal Quantity { get; set; }
    }
}
