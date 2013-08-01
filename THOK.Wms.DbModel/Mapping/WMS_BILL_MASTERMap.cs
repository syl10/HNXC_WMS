using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class WMS_BILL_MASTERMap : EntityTypeConfiguration<WMS_BILL_MASTER>
    {
        public WMS_BILL_MASTERMap()
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

            this.Property(t => t.SCHEDULE_NO)
                .HasMaxLength(20);

            this.Property(t => t.WAREHOUSE_CODE)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.TARGET_CODE)
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

            this.Property(t => t.SOURCE_BILLNO)
                .HasMaxLength(20);

            this.Property(t => t.OPERATER)
                .HasMaxLength(10);

            this.Property(t => t.CHECKER)
                .HasMaxLength(10);

            this.Property(t => t.TASKER)
                .HasMaxLength(10);

            this.Property(t => t.BILL_METHOD)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.LINE_NO)
                .IsFixedLength()
                .HasMaxLength(2);

            this.Property(t => t.CIGARETTE_CODE)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.FORMULA_CODE)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("WMS_BILL_MASTER", "THOK");
            this.Property(t => t.BILL_NO).HasColumnName("BILL_NO");
            this.Property(t => t.BILL_DATE).HasColumnName("BILL_DATE");
            this.Property(t => t.BTYPE_CODE).HasColumnName("BTYPE_CODE");
            this.Property(t => t.SCHEDULE_NO).HasColumnName("SCHEDULE_NO");
            this.Property(t => t.WAREHOUSE_CODE).HasColumnName("WAREHOUSE_CODE");
            this.Property(t => t.TARGET_CODE).HasColumnName("TARGET_CODE");
            this.Property(t => t.STATUS).HasColumnName("STATUS");
            this.Property(t => t.STATE).HasColumnName("STATE");
            this.Property(t => t.SOURCE_BILLNO).HasColumnName("SOURCE_BILLNO");
            this.Property(t => t.OPERATER).HasColumnName("OPERATER");
            this.Property(t => t.OPERATE_DATE).HasColumnName("OPERATE_DATE");
            this.Property(t => t.CHECKER).HasColumnName("CHECKER");
            this.Property(t => t.CHECK_DATE).HasColumnName("CHECK_DATE");
            this.Property(t => t.TASKER).HasColumnName("TASKER");
            this.Property(t => t.TASK_DATE).HasColumnName("TASK_DATE");
            this.Property(t => t.BILL_METHOD).HasColumnName("BILL_METHOD");
            this.Property(t => t.SCHEDULE_ITEMNO).HasColumnName("SCHEDULE_ITEMNO");
            this.Property(t => t.LINE_NO).HasColumnName("LINE_NO");
            this.Property(t => t.CIGARETTE_CODE).HasColumnName("CIGARETTE_CODE");
            this.Property(t => t.FORMULA_CODE).HasColumnName("FORMULA_CODE");
            this.Property(t => t.BATCH_WEIGHT).HasColumnName("BATCH_WEIGHT");

            // Relationships
            this.HasRequired(t => t.CMD_BILL_TYPE)
                .WithMany(t => t.WMS_BILL_MASTER)
                .HasForeignKey(d => d.BTYPE_CODE);
            this.HasOptional(t => t.CMD_CIGARETTE)
                .WithMany(t => t.WMS_BILL_MASTER)
                .HasForeignKey(d => d.CIGARETTE_CODE);
            this.HasOptional(t => t.CMD_PRODUCTION_LINE)
                .WithMany(t => t.WMS_BILL_MASTER)
                .HasForeignKey(d => d.LINE_NO);
            this.HasOptional(t => t.CMD_WAREHOUSE)
                .WithMany(t => t.WMS_BILL_MASTER)
                .HasForeignKey(d => d.WAREHOUSE_CODE);
            this.HasOptional(t => t.SYS_BILL_TARGET)
                .WithMany(t => t.WMS_BILL_MASTER)
                .HasForeignKey(d => d.TARGET_CODE);
            this.HasOptional(t => t.WMS_FORMULA_MASTER)
                .WithMany(t => t.WMS_BILL_MASTER)
                .HasForeignKey(d => d.FORMULA_CODE);

        }
    }
}
