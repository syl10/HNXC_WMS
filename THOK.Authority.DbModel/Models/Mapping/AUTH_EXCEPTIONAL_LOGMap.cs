using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Authority.DbModel.Mapping
{
    public class AUTH_EXCEPTIONAL_LOGMap : EntityTypeConfiguration<AUTH_EXCEPTIONAL_LOG>
    {
        public AUTH_EXCEPTIONAL_LOGMap()
        {
            // Primary Key
            this.HasKey(t => t.EXCEPTIONAL_LOG_ID);

            // Properties
            this.Property(t => t.EXCEPTIONAL_LOG_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(10);

            this.Property(t => t.CATCH_TIME)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.MODULE_NAME)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.FUNCTION_NAME)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.EXCEPTIONAL_TYPE)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.EXCEPTIONAL_DESCRIPTION)
                .IsRequired()
                .HasMaxLength(1000);

            this.Property(t => t.STATE)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("AUTH_EXCEPTIONAL_LOG","HNXC");
            this.Property(t => t.EXCEPTIONAL_LOG_ID).HasColumnName("EXCEPTIONAL_LOG_ID");
            this.Property(t => t.CATCH_TIME).HasColumnName("CATCH_TIME");
            this.Property(t => t.MODULE_NAME).HasColumnName("MODULE_NAME");
            this.Property(t => t.FUNCTION_NAME).HasColumnName("FUNCTION_NAME");
            this.Property(t => t.EXCEPTIONAL_TYPE).HasColumnName("EXCEPTIONAL_TYPE");
            this.Property(t => t.EXCEPTIONAL_DESCRIPTION).HasColumnName("EXCEPTIONAL_DESCRIPTION");
            this.Property(t => t.STATE).HasColumnName("STATE");
        }
    }
}
