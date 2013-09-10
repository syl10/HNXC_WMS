using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class WCS_TASKMap : EntityTypeConfiguration<WCS_TASK>
    {
        public WCS_TASKMap()
        {
            // Primary Key
            this.HasKey(t => t.TASK_ID);

            // Properties
            this.Property(t => t.TASK_ID)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.TASK_TYPE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2);

            this.Property(t => t.BILL_NO)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.PRODUCT_CODE)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.PRODUCT_BARCODE)
                .HasMaxLength(40);

            this.Property(t => t.PALLET_CODE)
                .HasMaxLength(40);

            this.Property(t => t.CELL_CODE)
                .IsFixedLength()
                .HasMaxLength(8);

            this.Property(t => t.TARGET_CODE)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.STATE)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.TASKER)
                .HasMaxLength(10);

            this.Property(t => t.PRODUCT_TYPE)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.NEWCELL_CODE)
                .IsFixedLength()
                .HasMaxLength(8);

            this.Property(t => t.IS_MIX)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.RFID_CHECK)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.CHECK_PALLET_CODE)
                .HasMaxLength(40);

            this.Property(t => t.BARCODE_CHECK)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.CHECK_PRODUCT_BARCODE)
                .HasMaxLength(40);

            this.Property(t => t.SOURCE_BILLNO)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("WCS_TASK", "THOK");
            this.Property(t => t.TASK_ID).HasColumnName("TASK_ID");
            this.Property(t => t.TASK_TYPE).HasColumnName("TASK_TYPE");
            this.Property(t => t.TASK_LEVEL).HasColumnName("TASK_LEVEL");
            this.Property(t => t.BILL_NO).HasColumnName("BILL_NO");
            this.Property(t => t.PRODUCT_CODE).HasColumnName("PRODUCT_CODE");
            this.Property(t => t.REAL_WEIGHT).HasColumnName("REAL_WEIGHT");
            this.Property(t => t.PRODUCT_BARCODE).HasColumnName("PRODUCT_BARCODE");
            this.Property(t => t.PALLET_CODE).HasColumnName("PALLET_CODE");
            this.Property(t => t.CELL_CODE).HasColumnName("CELL_CODE");
            this.Property(t => t.TARGET_CODE).HasColumnName("TARGET_CODE");
            this.Property(t => t.STATE).HasColumnName("STATE");
            this.Property(t => t.TASK_DATE).HasColumnName("TASK_DATE");
            this.Property(t => t.TASKER).HasColumnName("TASKER");
            this.Property(t => t.PRODUCT_TYPE).HasColumnName("PRODUCT_TYPE");
            this.Property(t => t.NEWCELL_CODE).HasColumnName("NEWCELL_CODE");
            this.Property(t => t.FINISH_DATE).HasColumnName("FINISH_DATE");
            this.Property(t => t.IS_MIX).HasColumnName("IS_MIX");
            this.Property(t => t.RFID_CHECK).HasColumnName("RFID_CHECK");
            this.Property(t => t.CHECK_PALLET_CODE).HasColumnName("CHECK_PALLET_CODE");
            this.Property(t => t.BARCODE_CHECK).HasColumnName("BARCODE_CHECK");
            this.Property(t => t.CHECK_PRODUCT_BARCODE).HasColumnName("CHECK_PRODUCT_BARCODE");
            this.Property(t => t.SOURCE_BILLNO).HasColumnName("SOURCE_BILLNO");
        }
    }
}
