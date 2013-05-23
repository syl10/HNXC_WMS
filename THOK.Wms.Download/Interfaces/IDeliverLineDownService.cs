using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Download.Interfaces
{
   public interface IDeliverLineDownService
    {
       DeliverLine[] GetDeliverLine(string deloverLines);
       DeliverDist[] GetDeliverDist(string deloverDists);
    }
}
