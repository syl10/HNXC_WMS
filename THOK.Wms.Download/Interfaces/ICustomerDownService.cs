using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Download.Interfaces
{
   public interface ICustomerDownService
    {
       Customer[] GetCustomer(string customers);
    }
}
