using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class CMD_PRODUCT_STYLEMap : EntityTypeConfiguration<CMD_PRODUCT_STYLE>
    {
        public CMD_PRODUCT_STYLEMap()
        {
            // Primary Key
            this.HasKey(t => t.STYLE_NO);

            // Properties
            this.Property(t => t.STYLE_NO)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2);

            this.Property(t => t.STYLE_NAME)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.SORT_LEVEL)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("CMD_PRODUCT_STYLE", "THOK");
            this.Property(t => t.STYLE_NO).HasColumnName("STYLE_NO");
            this.Property(t => t.STYLE_NAME).HasColumnName("STYLE_NAME");
            this.Property(t => t.SORT_LEVEL).HasColumnName("SORT_LEVEL");
        }
    }
}
