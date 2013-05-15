using System.Linq;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;
using THOK.Common;
using THOK.Common.Ef.EntityRepository;

namespace THOK.Authority.Dal.EntityRepository
{
    public class HelpContentRepository : RepositoryBase<AUTH_HELP_CONTENT>, IHelpContentRepository
    {
    }
}
