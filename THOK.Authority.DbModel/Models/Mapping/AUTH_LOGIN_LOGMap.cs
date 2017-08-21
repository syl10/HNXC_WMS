using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Authority.DbModel.Mapping
{
    public class AUTH_LOGIN_LOGMap : EntityTypeConfiguration<AUTH_LOGIN_LOG>
    {
        public AUTH_LOGIN_LOGMap()
        {
            // Primary Key
            this.HasKey(t => t.LOG_ID);

            // Properties
            this.Property(t => t.LOG_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(10);

            this.Property(t => t.LOGIN_PC)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.LOGIN_TIME)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.LOGOUT_TIME)
                .HasMaxLength(30);

            this.Property(t => t.USER_USER_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            this.Property(t => t.SYSTEM_SYSTEM_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            this.ToTable("AUTH_LOGIN_LOG","HNXC");
            this.Property(t => t.LOG_ID).HasColumnName("LOG_ID");
            this.Property(t => t.LOGIN_PC).HasColumnName("LOGIN_PC");
            this.Property(t => t.LOGIN_TIME).HasColumnName("LOGIN_TIME");
            this.Property(t => t.LOGOUT_TIME).HasColumnName("LOGOUT_TIME");
            this.Property(t => t.USER_USER_ID).HasColumnName("USER_USER_ID");
            this.Property(t => t.SYSTEM_SYSTEM_ID).HasColumnName("SYSTEM_SYSTEM_ID");

            // Relationships
            this.HasRequired(t => t.AUTH_SYSTEM)
                .WithMany(t => t.AUTH_LOGIN_LOG)
                .HasForeignKey(d => d.SYSTEM_SYSTEM_ID);
            this.HasRequired(t => t.AUTH_USER)
                .WithMany(t => t.AUTH_LOGIN_LOG)
                .HasForeignKey(d => d.USER_USER_ID);

        }
    }
}
