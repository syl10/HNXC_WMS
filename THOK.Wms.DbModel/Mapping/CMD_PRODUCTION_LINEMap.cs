using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class CMD_PRODUCTION_LINEMap : EntityTypeConfiguration<CMD_PRODUCTION_LINE>
    {
        public CMD_PRODUCTION_LINEMap()
        {
            // Primary Key
            this.HasKey(t => t.LINE_NO);

            // Properties
            this.Property(t => t.LINE_NO)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2);

            this.Property(t => t.LINE_NAME)
                .HasMaxLength(30);

            this.Property(t => t.MEMO)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("CMD_PRODUCTION_LINE", "THOK");
            this.Property(t => t.LINE_NO).HasColumnName("LINE_NO");
            this.Property(t => t.LINE_NAME).HasColumnName("LINE_NAME");
            this.Property(t => t.MEMO).HasColumnName("MEMO");
        }
    }
}
