using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class WMS_PRODUCTION_MASTERMap : EntityTypeConfiguration<WMS_PRODUCTION_MASTER>
    {
        public WMS_PRODUCTION_MASTERMap()
        {
            // Primary Key
            this.HasKey(t => t.BILL_NO);

            // Properties
            this.Property(t => t.BILL_NO)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.SCHEDULE_NO)
                .HasMaxLength(20);

            this.Property(t => t.WAREHOUSE_CODE)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.CIGARETTE_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.FORMULA_CODE)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.IN_BILLNO)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.OUT_BILLNO)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.STATE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.OPERATER)
                .HasMaxLength(10);

            this.Property(t => t.CHECKER)
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("WMS_PRODUCTION_MASTER","HNXC");
            this.Property(t => t.BILL_NO).HasColumnName("BILL_NO");
            this.Property(t => t.BILL_DATE).HasColumnName("BILL_DATE");
            this.Property(t => t.SCHEDULE_NO).HasColumnName("SCHEDULE_NO");
            this.Property(t => t.WAREHOUSE_CODE).HasColumnName("WAREHOUSE_CODE");
            this.Property(t => t.CIGARETTE_CODE).HasColumnName("CIGARETTE_CODE");
            this.Property(t => t.FORMULA_CODE).HasColumnName("FORMULA_CODE");
            this.Property(t => t.BATCH_WEIGHT).HasColumnName("BATCH_WEIGHT");
            this.Property(t => t.IN_BILLNO).HasColumnName("IN_BILLNO");
            this.Property(t => t.OUT_BILLNO).HasColumnName("OUT_BILLNO");
            this.Property(t => t.STATE).HasColumnName("STATE");
            this.Property(t => t.OPERATER).HasColumnName("OPERATER");
            this.Property(t => t.OPERATE_DATE).HasColumnName("OPERATE_DATE");
            this.Property(t => t.CHECKER).HasColumnName("CHECKER");
            this.Property(t => t.CHECK_DATE).HasColumnName("CHECK_DATE");

            // Relationships
            this.HasRequired(t => t.CMD_CIGARETTE)
                .WithMany(t => t.WMS_PRODUCTION_MASTER)
                .HasForeignKey(d => d.CIGARETTE_CODE);
            this.HasOptional(t => t.CMD_WAREHOUSE)
                .WithMany(t => t.WMS_PRODUCTION_MASTER)
                .HasForeignKey(d => d.WAREHOUSE_CODE);
            this.HasRequired(t => t.WMS_FORMULA_MASTER)
                .WithMany(t => t.WMS_PRODUCTION_MASTER)
                .HasForeignKey(d => d.FORMULA_CODE);

        }
    }
}
