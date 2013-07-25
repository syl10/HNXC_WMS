using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    class WMSScheduleDetailService : ServiceBase<WMS_SCHEDULE_DETAIL>, IWMSScheduleDetailService 
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public IWMSScheduleDetailRepository ScheduleDetailRepository { get; set; }

        public object GetSerial(string SCHEDULE_NO)
        {
            IQueryable<WMS_SCHEDULE_DETAIL> query = ScheduleDetailRepository.GetQueryable();
            var Serial = query.OrderByDescending(i => i.ITEM_NO).FirstOrDefault(i => i.SCHEDULE_NO == SCHEDULE_NO);
            var newSerial = new { 
                Itemno=Serial.ITEM_NO 
            };
            return newSerial;
                  
        }
    }
}
