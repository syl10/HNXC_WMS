using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
   public  interface IWMSScheduleDetailService : IService<WMS_SCHEDULE_DETAIL>
    {
        //获取序号
        object GetSerial(string SCHEDULE_NO);
    }
}
