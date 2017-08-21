using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.DbModel;
using THOK.Common.Ef.EntityRepository;

namespace THOK.Wms.Dal.EntityRepository
{
    class WMSScheduleRespository : RepositoryBase<WMS_SCHEDULE >, IWMSScheduleRepository
    {
    }
}
