using THOK.Wms.Dal.Interfaces;
using THOK.Wms.DbModel;
using THOK.Common.Ef.EntityRepository;
using System.Linq;

namespace THOK.Wms.Dal.EntityRepository
{
    public class OutBillDetailRepository:RepositoryBase<OutBillDetail>,IOutBillDetailRepository
    {
        public IQueryable<OutBillDetail> GetQueryableIncludeProduct()
        {
            return this.dbSet.Include("Product")
                             .AsQueryable<OutBillDetail>();
        }
    }
}
