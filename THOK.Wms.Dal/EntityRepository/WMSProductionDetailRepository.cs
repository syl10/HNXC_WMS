﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Common.Ef.EntityRepository;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.DbModel;

namespace THOK.Wms.Dal.EntityRepository
{
    class WMSProductionDetailRepository : RepositoryBase<WMS_PRODUCTION_DETAIL >, IWMSProductionDetailRepository 
    {
    }
}
