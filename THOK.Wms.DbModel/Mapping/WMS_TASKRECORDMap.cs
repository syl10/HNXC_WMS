using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations;

namespace THOK.Wms.DbModel.Mapping
{
    public class WMS_TASKRECORDMap : EntityTypeConfiguration<WMS_TASKRECORD>
    {
        public WMS_TASKRECORDMap()
        {
            // Primary Key
            this.HasKey(t => new { t.BILL_NO, t.ITEM_NO });

            // Properties
            this.Property(t => t.BILL_NO)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.ITEM_NO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PRODUCT_CODE)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.PRODUCT_BARCODE)
                .HasMaxLength(200);

            this.Property(t => t.PALLET_CODE)
                .HasMaxLength(100);

            this.Property(t => t.PRODUCT_TYPE)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.CELL_CODE)
                .IsFixedLength()
                .HasMaxLength(8);

            this.Property(t => t.ACTION)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.TASKER)
                .HasMaxLength(10);

            this.Property(t => t.IS_MIX)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.INBILL_NO)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("WMS_TASKRECORD", "HNXC");
            this.Property(t => t.BILL_NO).HasColumnName("BILL_NO");
            this.Property(t => t.ITEM_NO).HasColumnName("ITEM_NO");
            this.Property(t => t.PRODUCT_CODE).HasColumnName("PRODUCT_CODE");
            this.Property(t => t.REAL_WEIGHT).HasColumnName("REAL_WEIGHT");
            this.Property(t => t.PRODUCT_BARCODE).HasColumnName("PRODUCT_BARCODE");
            this.Property(t => t.PALLET_CODE).HasColumnName("PALLET_CODE");
            this.Property(t => t.PRODUCT_TYPE).HasColumnName("PRODUCT_TYPE");
            this.Property(t => t.CELL_CODE).HasColumnName("CELL_CODE");
            this.Property(t => t.ACTION).HasColumnName("ACTION");
            this.Property(t => t.TASK_DATE).HasColumnName("TASK_DATE");
            this.Property(t => t.TASKER).HasColumnName("TASKER");
            this.Property(t => t.IS_MIX).HasColumnName("IS_MIX");
            this.Property(t => t.INBILL_NO).HasColumnName("INBILL_NO");
        }
    }
}
