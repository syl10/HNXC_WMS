using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class WMS_PALLET_MASTERMap : EntityTypeConfiguration<WMS_PALLET_MASTER>
    {
        public WMS_PALLET_MASTERMap()
        {
            // Primary Key
            this.HasKey(t => t.BILL_NO);

            // Properties
            this.Property(t => t.BILL_NO)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.BTYPE_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.WAREHOUSE_CODE)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.TARGET)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.STATUS)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.STATE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.OPERATER)
                .HasMaxLength(10);

            this.Property(t => t.TASKER)
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("WMS_PALLET_MASTER","HNXC");
            this.Property(t => t.BILL_NO).HasColumnName("BILL_NO");
            this.Property(t => t.BILL_DATE).HasColumnName("BILL_DATE");
            this.Property(t => t.BTYPE_CODE).HasColumnName("BTYPE_CODE");
            this.Property(t => t.WAREHOUSE_CODE).HasColumnName("WAREHOUSE_CODE");
            this.Property(t => t.TARGET).HasColumnName("TARGET");
            this.Property(t => t.STATUS).HasColumnName("STATUS");
            this.Property(t => t.STATE).HasColumnName("STATE");
            this.Property(t => t.OPERATER).HasColumnName("OPERATER");
            this.Property(t => t.OPERATE_DATE).HasColumnName("OPERATE_DATE");
            this.Property(t => t.TASKER).HasColumnName("TASKER");
            this.Property(t => t.TASK_DATE).HasColumnName("TASK_DATE");
        }
    }
}
