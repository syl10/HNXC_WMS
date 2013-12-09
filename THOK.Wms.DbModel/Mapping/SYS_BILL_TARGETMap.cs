using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class SYS_BILL_TARGETMap : EntityTypeConfiguration<SYS_BILL_TARGET>
    {
        public SYS_BILL_TARGETMap()
        {
            // Primary Key
            this.HasKey(t => t.TARGET_CODE);

            // Properties
            this.Property(t => t.TARGET_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.TARGET_NAME)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("SYS_BILL_TARGET","HNXC");
            this.Property(t => t.TARGET_CODE).HasColumnName("TARGET_CODE");
            this.Property(t => t.TARGET_NAME).HasColumnName("TARGET_NAME");
        }
    }
}
