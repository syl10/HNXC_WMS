using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Common.Ef.EntityRepository;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Dal.EntityRepository
{
    public class OutBillDetailHistoryRepository : RepositoryBase<OutBillDetailHistory>, IOutBillDetailHistoryRepository
    {
    }
}
