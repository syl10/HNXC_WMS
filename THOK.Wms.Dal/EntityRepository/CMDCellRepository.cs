using THOK.Wms.DbModel;
using THOK.Wms.Dal.Interfaces;
using THOK.Common.Ef.EntityRepository;
using System.Linq;

namespace THOK.Wms.Dal.EntityRepository
{
    public class CMDCellRepository : RepositoryBase<CMD_CELL>, ICMDCellRepository
    {
        public new IQueryable<CMD_CELL> GetQueryable()
        {
            return this.dbSet.AsQueryable<CMD_CELL>();
        }

        public IQueryable<CMD_CELL> GetQueryableIncludeStorages()
        {
            return this.dbSet.Include("Storages")
                             .AsQueryable<CMD_CELL>();
        }
    }
}
