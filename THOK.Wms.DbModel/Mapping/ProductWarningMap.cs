using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Common.Ef.MappingStrategy;

namespace THOK.Wms.DbModel.Mapping
{
    public class ProductWarningMap:EntityMappingBase<ProductWarning>
    {
        public ProductWarningMap()
            : base("Wms")
        {
            // Primary Key
            this.HasKey(t => t.ProductCode);

            // Properties
            this.Property(t => t.ProductCode)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UnitCode)
               .IsRequired()
               .HasMaxLength(20);

            this.Property(t => t.MinLimited)
                .HasPrecision(18, 2);

            this.Property(t => t.MaxLimited)
                .HasPrecision(18,2);

            this.Property(t => t.AssemblyTime)
                .HasPrecision(18,2);

            this.Property(t => t.Memo)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.Property(t => t.ProductCode).HasColumnName(ColumnMap.Value.To("ProductCode"));
            this.Property(t => t.UnitCode).HasColumnName(ColumnMap.Value.To("UnitCode"));
            this.Property(t => t.MinLimited).HasColumnName(ColumnMap.Value.To("MinLimited"));
            this.Property(t => t.MaxLimited).HasColumnName(ColumnMap.Value.To("MaxLimited"));
            this.Property(t => t.AssemblyTime).HasColumnName(ColumnMap.Value.To("AssemblyTime"));
            this.Property(t => t.Memo).HasColumnName(ColumnMap.Value.To("Memo"));

            this.HasRequired(t => t.Unit)
               .WithMany()
               .HasForeignKey(d => d.UnitCode)
               .WillCascadeOnDelete(false);
        }
    }
}
