using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Authority.DbModel.Mapping
{
    public class AUTH_SYSTEM_EVENT_LOGMap : EntityTypeConfiguration<AUTH_SYSTEM_EVENT_LOG>
    {
        public AUTH_SYSTEM_EVENT_LOGMap()
        {
            // Primary Key
            this.HasKey(t => t.EVENT_LOG_ID);

            // Properties
            this.Property(t => t.EVENT_LOG_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(10);

            this.Property(t => t.EVENT_LOG_TIME)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.EVENT_TYPE)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.EVENT_NAME)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.EVENT_DESCRIPTION)
                .IsRequired()
                .HasMaxLength(400);

            this.Property(t => t.FROM_PC)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.OPERATE_USER)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.TARGET_SYSTEM)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("AUTH_SYSTEM_EVENT_LOG","HNXC");
            this.Property(t => t.EVENT_LOG_ID).HasColumnName("EVENT_LOG_ID");
            this.Property(t => t.EVENT_LOG_TIME).HasColumnName("EVENT_LOG_TIME");
            this.Property(t => t.EVENT_TYPE).HasColumnName("EVENT_TYPE");
            this.Property(t => t.EVENT_NAME).HasColumnName("EVENT_NAME");
            this.Property(t => t.EVENT_DESCRIPTION).HasColumnName("EVENT_DESCRIPTION");
            this.Property(t => t.FROM_PC).HasColumnName("FROM_PC");
            this.Property(t => t.OPERATE_USER).HasColumnName("OPERATE_USER");
            this.Property(t => t.TARGET_SYSTEM).HasColumnName("TARGET_SYSTEM");
        }
    }
}
