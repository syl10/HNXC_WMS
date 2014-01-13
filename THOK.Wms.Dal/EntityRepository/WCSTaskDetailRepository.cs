using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Common.Ef.EntityRepository;
using THOK.Wms.DbModel;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Dal.EntityRepository
{
    class WCSTaskDetailRepository : RepositoryBase<WCS_TASK_DETAIL >, IWCSTaskDetailRepository  
    {
    }
}
