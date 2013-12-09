using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations;

namespace THOK.Wms.DbModel.Mapping
{
    public class WMS_PRODUCT_STATEMap : EntityTypeConfiguration<WMS_PRODUCT_STATE>
    {
        public WMS_PRODUCT_STATEMap()
        {
            // Primary Key
            this.HasKey(t => new { t.BILL_NO, t.ITEM_NO, t.PRODUCT_CODE, t.WEIGHT, t.REAL_WEIGHT, t.PACKAGE_COUNT });

            // Properties
            this.Property(t => t.BILL_NO)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.ITEM_NO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SCHEDULE_NO)
                .HasMaxLength(20);

            this.Property(t => t.PRODUCT_CODE)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.WEIGHT)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.REAL_WEIGHT)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PACKAGE_COUNT)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.OUT_BILLNO)
                .HasMaxLength(20);

            this.Property(t => t.CELL_CODE)
                .IsFixedLength()
                .HasMaxLength(8);

            this.Property(t => t.NEWCELL_CODE)
                .IsFixedLength()
                .HasMaxLength(8);

            this.Property(t => t.PRODUCT_BARCODE)
                .HasMaxLength(40);

            this.Property(t => t.PALLET_CODE)
                .HasMaxLength(40);

            this.Property(t => t.IS_MIX)
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("WMS_PRODUCT_STATE","HNXC");
            this.Property(t => t.BILL_NO).HasColumnName("BILL_NO");
            this.Property(t => t.ITEM_NO).HasColumnName("ITEM_NO");
            this.Property(t => t.SCHEDULE_NO).HasColumnName("SCHEDULE_NO");
            this.Property(t => t.PRODUCT_CODE).HasColumnName("PRODUCT_CODE");
            this.Property(t => t.WEIGHT).HasColumnName("WEIGHT");
            this.Property(t => t.REAL_WEIGHT).HasColumnName("REAL_WEIGHT");
            this.Property(t => t.PACKAGE_COUNT).HasColumnName("PACKAGE_COUNT");
            this.Property(t => t.OUT_BILLNO).HasColumnName("OUT_BILLNO");
            this.Property(t => t.CELL_CODE).HasColumnName("CELL_CODE");
            this.Property(t => t.NEWCELL_CODE).HasColumnName("NEWCELL_CODE");
            this.Property(t => t.PRODUCT_BARCODE).HasColumnName("PRODUCT_BARCODE");
            this.Property(t => t.PALLET_CODE).HasColumnName("PALLET_CODE");
            this.Property(t => t.IS_MIX).HasColumnName("IS_MIX");
        }
    }
}
