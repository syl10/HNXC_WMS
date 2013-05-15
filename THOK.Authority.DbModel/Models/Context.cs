using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using THOK.Authority.DbModel.Mapping;

namespace THOK.Authority.DbModel
{
    public partial class Context : DbContext
    {
        static Context()
        {
            Database.SetInitializer<Context>(null);
        }

        public Context()
            : base("Name=Context")
        {
        }

        public DbSet<AUTH_CITY> AUTH_CITY { get; set; }
        public DbSet<AUTH_EXCEPTIONAL_LOG> AUTH_EXCEPTIONAL_LOG { get; set; }
        public DbSet<AUTH_FUNCTION> AUTH_FUNCTION { get; set; }
        public DbSet<AUTH_HELP_CONTENT> AUTH_HELP_CONTENT { get; set; }
        public DbSet<AUTH_LOGIN_LOG> AUTH_LOGIN_LOG { get; set; }
        public DbSet<AUTH_MODULE> AUTH_MODULE { get; set; }
        public DbSet<AUTH_ROLE> AUTH_ROLE { get; set; }
        public DbSet<AUTH_ROLE_FUNCTION> AUTH_ROLE_FUNCTION { get; set; }
        public DbSet<AUTH_ROLE_MODULE> AUTH_ROLE_MODULE { get; set; }
        public DbSet<AUTH_ROLE_SYSTEM> AUTH_ROLE_SYSTEM { get; set; }
        public DbSet<AUTH_SERVER> AUTH_SERVER { get; set; }
        public DbSet<AUTH_SYSTEM> AUTH_SYSTEM { get; set; }
        public DbSet<AUTH_SYSTEM_EVENT_LOG> AUTH_SYSTEM_EVENT_LOG { get; set; }
        public DbSet<AUTH_SYSTEM_PARAMETER> AUTH_SYSTEM_PARAMETER { get; set; }
        public DbSet<AUTH_USER> AUTH_USER { get; set; }
        public DbSet<AUTH_USER_FUNCTION> AUTH_USER_FUNCTION { get; set; }
        public DbSet<AUTH_USER_MODULE> AUTH_USER_MODULE { get; set; }
        public DbSet<AUTH_USER_ROLE> AUTH_USER_ROLE { get; set; }
        public DbSet<AUTH_USER_SYSTEM> AUTH_USER_SYSTEM { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AUTH_CITYMap());
            modelBuilder.Configurations.Add(new AUTH_EXCEPTIONAL_LOGMap());
            modelBuilder.Configurations.Add(new AUTH_FUNCTIONMap());
            modelBuilder.Configurations.Add(new AUTH_HELP_CONTENTMap());
            modelBuilder.Configurations.Add(new AUTH_LOGIN_LOGMap());
            modelBuilder.Configurations.Add(new AUTH_MODULEMap());
            modelBuilder.Configurations.Add(new AUTH_ROLEMap());
            modelBuilder.Configurations.Add(new AUTH_ROLE_FUNCTIONMap());
            modelBuilder.Configurations.Add(new AUTH_ROLE_MODULEMap());
            modelBuilder.Configurations.Add(new AUTH_ROLE_SYSTEMMap());
            modelBuilder.Configurations.Add(new AUTH_SERVERMap());
            modelBuilder.Configurations.Add(new AUTH_SYSTEMMap());
            modelBuilder.Configurations.Add(new AUTH_SYSTEM_EVENT_LOGMap());
            modelBuilder.Configurations.Add(new AUTH_SYSTEM_PARAMETERMap());
            modelBuilder.Configurations.Add(new AUTH_USERMap());
            modelBuilder.Configurations.Add(new AUTH_USER_FUNCTIONMap());
            modelBuilder.Configurations.Add(new AUTH_USER_MODULEMap());
            modelBuilder.Configurations.Add(new AUTH_USER_ROLEMap());
            modelBuilder.Configurations.Add(new AUTH_USER_SYSTEMMap());
        }
    }
}
