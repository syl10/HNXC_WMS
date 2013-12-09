using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class CMD_CRANEMap : EntityTypeConfiguration<CMD_CRANE>
    {
        public CMD_CRANEMap()
        {
            // Primary Key
            this.HasKey(t => t.CRANE_NO);

            // Properties
            this.Property(t => t.CRANE_NO)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2);

            this.Property(t => t.CRANE_NAME)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.IS_ACTIVE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.MEMO)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("CMD_CRANE","HNXC");
            this.Property(t => t.CRANE_NO).HasColumnName("CRANE_NO");
            this.Property(t => t.CRANE_NAME).HasColumnName("CRANE_NAME");
            this.Property(t => t.IS_ACTIVE).HasColumnName("IS_ACTIVE");
            this.Property(t => t.MEMO).HasColumnName("MEMO");
        }
    }
}
