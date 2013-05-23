using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Download.Interfaces
{
   public interface ISortingDownService
    {
       SortOrder[] GetSortOrder(string beginDate,string endDate,string sortOrders);
       SortOrderDetail[] GetSortOrderDetail(string sortOrders);
    }
}
