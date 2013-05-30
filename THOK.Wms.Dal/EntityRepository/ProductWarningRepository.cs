using THOK.Wms.DbModel;
using THOK.Wms.Dal.Interfaces;
using THOK.Common.Ef.EntityRepository;
using System.Linq;

namespace THOK.Wms.Dal.EntityRepository
{
    public class ProductWarningRepository:RepositoryBase<ProductWarning>, IProductWarningRepository
    {
    }
}
