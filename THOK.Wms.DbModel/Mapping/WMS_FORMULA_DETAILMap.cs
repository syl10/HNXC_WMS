using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class WMS_FORMULA_DETAILMap : EntityTypeConfiguration<WMS_FORMULA_DETAIL>
    {
        public WMS_FORMULA_DETAILMap()
        {
            // Primary Key
            this.HasKey(t => new { t.FORMULA_CODE, t.PRODUCT_CODE });

            // Properties
            this.Property(t => t.FORMULA_CODE)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.PRODUCT_CODE)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.OTHER)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("WMS_FORMULA_DETAIL", "THOK");
            this.Property(t => t.FORMULA_CODE).HasColumnName("FORMULA_CODE");
            this.Property(t => t.PRODUCT_CODE).HasColumnName("PRODUCT_CODE");
            this.Property(t => t.FORDER).HasColumnName("FORDER");
            this.Property(t => t.PERCENT).HasColumnName("PERCENT");
            this.Property(t => t.OTHER).HasColumnName("OTHER");

            // Relationships
            this.HasRequired(t => t.CMD_PRODUCT)
                .WithMany(t => t.WMS_FORMULA_DETAIL)
                .HasForeignKey(d => d.PRODUCT_CODE);

        }
    }
}
