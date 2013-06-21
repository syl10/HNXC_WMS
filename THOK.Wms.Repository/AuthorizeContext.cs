using System.Data.Entity;
using THOK.Wms.Repository.Migrations;
using THOK.Authority.DbModel.Mapping;
using THOK.Authority.DbModel;
using THOK.Wms.DbModel.Mapping;

namespace THOK.Wms.Repository
{
    public class AuthorizeContext : DbContext
    {
        static AuthorizeContext()
        {
            //Database.SetInitializer<AuthorizeContext>(new MigrateDatabaseToLatestVersion<AuthorizeContext, Configuration>());
        }

		public AuthorizeContext()
			: base("Name=AuthorizeContext")
		{
                 
		}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region auth
            modelBuilder.Configurations.Add(new AUTH_MODULEMap());
            modelBuilder.Configurations.Add(new AUTH_CITYMap());
            modelBuilder.Configurations.Add(new AUTH_FUNCTIONMap());
            modelBuilder.Configurations.Add(new AUTH_LOGIN_LOGMap());

            modelBuilder.Configurations.Add(new AUTH_ROLEMap());
            modelBuilder.Configurations.Add(new AUTH_ROLE_FUNCTIONMap());
            modelBuilder.Configurations.Add(new AUTH_ROLE_MODULEMap());
            modelBuilder.Configurations.Add(new AUTH_ROLE_SYSTEMMap());
            modelBuilder.Configurations.Add(new AUTH_SERVERMap());
            modelBuilder.Configurations.Add(new AUTH_SYSTEMMap());
            modelBuilder.Configurations.Add(new AUTH_SYSTEM_EVENT_LOGMap());
            modelBuilder.Configurations.Add(new AUTH_USERMap());
            modelBuilder.Configurations.Add(new AUTH_USER_FUNCTIONMap());
            modelBuilder.Configurations.Add(new AUTH_USER_MODULEMap());
            modelBuilder.Configurations.Add(new AUTH_USER_ROLEMap());
            modelBuilder.Configurations.Add(new AUTH_USER_SYSTEMMap());
            modelBuilder.Configurations.Add(new AUTH_HELP_CONTENTMap());
            modelBuilder.Configurations.Add(new AUTH_EXCEPTIONAL_LOGMap());
            #endregion

            #region wms
            modelBuilder.Configurations.Add(new CMD_AREAMap());
            modelBuilder.Configurations.Add(new CMD_CELLMap());
            modelBuilder.Configurations.Add(new CMD_PRODUCTMap());
            modelBuilder.Configurations.Add(new CMD_SHELFMap());
            modelBuilder.Configurations.Add(new CMD_WAREHOUSEMap());
            modelBuilder.Configurations.Add(new CMD_CRANEMap());
            modelBuilder.Configurations.Add(new CMD_CIGARETTEMap());
            modelBuilder.Configurations.Add(new CMD_CARMap());
            modelBuilder.Configurations.Add(new SYS_TABLE_STATEMap());
            modelBuilder.Configurations.Add(new CMD_BILL_TYPEMap());
            #endregion

            #region wcs

            #endregion

            modelBuilder.Configurations.Add(new AUTH_SYSTEM_PARAMETERMap());            
        }
    }
}
