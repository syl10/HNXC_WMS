using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class SYS_TABLE_STATEMap : EntityTypeConfiguration<SYS_TABLE_STATE>
    {
        public SYS_TABLE_STATEMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TABLE_NAME, t.FIELD_NAME, t.STATE });

            // Properties
            this.Property(t => t.TABLE_NAME)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.FIELD_NAME)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.STATE)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.STATE_DESC)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SYS_TABLE_STATE","HNXC");
            this.Property(t => t.TABLE_NAME).HasColumnName("TABLE_NAME");
            this.Property(t => t.FIELD_NAME).HasColumnName("FIELD_NAME");
            this.Property(t => t.STATE).HasColumnName("STATE");
            this.Property(t => t.STATE_DESC).HasColumnName("STATE_DESC");
        }
    }
}
