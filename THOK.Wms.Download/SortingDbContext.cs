using System.Data.Entity;

namespace THOK.Wms.Download
{
    public class SortingDbContext : DbContext
    {
        static SortingDbContext()
        {
           
        }

        public SortingDbContext()
            : base("Name=SortingDbContext")
		{
		}
    }
}
