using System.Linq;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;
using THOK.Common;
using THOK.Common.Ef.EntityRepository;

namespace THOK.Authority.Dal.EntityRepository
{
    public class CityRepository : RepositoryBase<AUTH_CITY>, ICityRepository
    {
        public new void Delete(AUTH_CITY city)
        {
            Delete(city.AUTH_SERVER.ToArray());

            city.AUTH_ROLE_SYSTEM.Do(rs => rs.AUTH_ROLE_MODULE.Do(rm =>
                Delete(rm.AUTH_ROLE_FUNCTION.ToArray())));
            city.AUTH_ROLE_SYSTEM.Do(rs => Delete(rs.AUTH_ROLE_MODULE.ToArray()));
            Delete(city.AUTH_ROLE_SYSTEM.ToArray());

            //city.UserSystems.Do(us => us.UserModules.Do(um => Delete(um.UserFunctions.ToArray())));
            //city.UserSystems.Do(us => Delete(us.UserModules.ToArray()));
            //Delete(city.UserSystems.ToArray());

            this.dbSet.Remove(city);
        }
    }
}
