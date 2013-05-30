using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Common.Ef.MappingStrategy;
using System.ComponentModel.DataAnnotations;

namespace THOK.Wms.DbModel.Mapping
{
    public class CheckBillDetailHistoryMap : EntityMappingBase<CheckBillDetailHistory>
    {
        public CheckBillDetailHistoryMap()
            : base("Wms")
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .IsRequired()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.BillNo)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.CellCode)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.StorageCode)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ProductCode)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.UnitCode)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Quantity)
                .IsRequired()
                .HasPrecision(18, 2);

            this.Property(t => t.RealProductCode)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.RealUnitCode)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.RealQuantity)
                .IsRequired()
                .HasPrecision(18, 2);

            this.Property(t => t.Operator)
                .HasMaxLength(20);

            this.Property(t => t.Status)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);


            // Table & Column Mappings
            this.Property(t => t.ID).HasColumnName(ColumnMap.Value.To("ID"));
            this.Property(t => t.BillNo).HasColumnName(ColumnMap.Value.To("BillNo"));
            this.Property(t => t.CellCode).HasColumnName(ColumnMap.Value.To("CellCode"));
            this.Property(t => t.StorageCode).HasColumnName(ColumnMap.Value.To("StorageCode"));
            this.Property(t => t.ProductCode).HasColumnName(ColumnMap.Value.To("ProductCode"));
            this.Property(t => t.UnitCode).HasColumnName(ColumnMap.Value.To("UnitCode"));
            this.Property(t => t.Quantity).HasColumnName(ColumnMap.Value.To("Quantity"));
            this.Property(t => t.RealProductCode).HasColumnName(ColumnMap.Value.To("RealProductCode"));
            this.Property(t => t.RealUnitCode).HasColumnName(ColumnMap.Value.To("RealUnitCode"));
            this.Property(t => t.RealQuantity).HasColumnName(ColumnMap.Value.To("RealQuantity"));
            this.Property(t => t.OperatePersonID).HasColumnName(ColumnMap.Value.To("OperatePersonID"));
            this.Property(t => t.Operator).HasColumnName(ColumnMap.Value.To("Operator"));
            this.Property(t => t.StartTime).HasColumnName(ColumnMap.Value.To("StartTime"));
            this.Property(t => t.FinishTime).HasColumnName(ColumnMap.Value.To("FinishTime"));
            this.Property(t => t.Status).HasColumnName(ColumnMap.Value.To("Status"));

            // Relationships
            this.HasRequired(t => t.CheckBillMasterHistory)
                .WithMany(t => t.CheckBillDetailHistorys)
                .HasForeignKey(d => d.BillNo)
                .WillCascadeOnDelete(false);

        }
    }
}
