using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class WMS_BALANCE_DETAILMap : EntityTypeConfiguration<WMS_BALANCE_DETAIL>
    {
        public WMS_BALANCE_DETAILMap()
        {
            // Primary Key
            this.HasKey(t => new { t.BALANCE_NO, t.WAREHOUSE_CODE, t.PRODUCT_CODE });

            // Properties
            this.Property(t => t.BALANCE_NO)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            this.Property(t => t.WAREHOUSE_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.PRODUCT_CODE)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("WMS_BALANCE_DETAIL","HNXC");
            this.Property(t => t.BALANCE_NO).HasColumnName("BALANCE_NO");
            this.Property(t => t.WAREHOUSE_CODE).HasColumnName("WAREHOUSE_CODE");
            this.Property(t => t.PRODUCT_CODE).HasColumnName("PRODUCT_CODE");
            this.Property(t => t.BEGIN_QUANTITY).HasColumnName("BEGIN_QUANTITY");
            this.Property(t => t.IN_QUANTITY).HasColumnName("IN_QUANTITY");
            this.Property(t => t.OUT_QUANTITY).HasColumnName("OUT_QUANTITY");
            this.Property(t => t.DIFF_QUANTITY).HasColumnName("DIFF_QUANTITY");
            this.Property(t => t.ENDQUANTITY).HasColumnName("ENDQUANTITY");
            this.Property(t => t.INSPECTOUT_QUANTITY).HasColumnName("INSPECTOUT_QUANTITY");
            this.Property(t => t.INSPECTIN_QUANTITY).HasColumnName("INSPECTIN_QUANTITY");
            this.Property(t => t.INCOME_QUANTITY).HasColumnName("INCOME_QUANTITY");
            this.Property(t => t.FEEDING_QUANTITY).HasColumnName("FEEDING_QUANTITY");
        }
    }
}
