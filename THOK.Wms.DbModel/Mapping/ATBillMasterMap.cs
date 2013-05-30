using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Common.Ef.MappingStrategy;
namespace THOK.Wms.DbModel.Mapping
{
   public  class ATBillMasterMap :EntityMappingBase<ATBillMaster>
    {
       public ATBillMasterMap()
           : base("Wms")
       {
           this.HasKey(t => t.WMS_BILL_MASTER_ID);

           this.Property(t => t.WMS_BILL_MASTER_ID)
               .IsRequired()
               .HasMaxLength(40);
           this.Property(t => t.BILL_NO)
               .IsRequired()
               .HasMaxLength(20);
           this.Property(t => t.BILL_DATE);
           this.Property(t => t.BILL_TYPE)
               .IsRequired()
               .HasMaxLength(20);
               //.IsRequired()  类型是(nchar(1) not null）
               //.IsFixedLength()
               //.HasMaxLength(1);
           this.Property(t => t.BIZ_TYPE_CODE)
               .IsRequired()
               .HasMaxLength(20);
               //.IsRequired()
               //.IsFixedLength()
               //.HasMaxLength(1);
           this.Property(t => t.WAREHOUSE_CODE)
               .HasMaxLength(10);
           this.Property(t => t.STATE)
               .IsRequired()
               .IsFixedLength()
               .HasMaxLength(1);
           this.Property(t => t.OPERATER)
               .HasMaxLength(10);
           this.Property(t => t.OPERATER);
           this.Property(t => t.CHECKER)
               .HasMaxLength(10);
           this.Property(t => t.CHECK_DATE);
           this.Property(t => t.TASKER)
               .HasMaxLength(10);
           this.Property(t => t.TASK_DATE);

           // Table & Column Mappings
           this.Property(t => t.WMS_BILL_MASTER_ID).HasColumnName(ColumnMap.Value.To("WMS_BILL_MASTER_ID"));
           this.Property(t => t.BILL_NO).HasColumnName(ColumnMap.Value.To("BILL_NO"));
           this.Property(t => t.BILL_DATE).HasColumnName(ColumnMap.Value.To("BILL_DATE"));
           this.Property(t => t.BILL_TYPE).HasColumnName(ColumnMap.Value.To("BILL_TYPE"));
           this.Property(t => t.BIZ_TYPE_CODE).HasColumnName(ColumnMap.Value.To("BIZ_TYPE_CODE"));
           this.Property(t => t.WAREHOUSE_CODE).HasColumnName(ColumnMap.Value.To("WAREHOUSE_CODE"));
           this.Property(t => t.STATE).HasColumnName(ColumnMap.Value.To("STATE"));
           this.Property(t => t.OPERATER).HasColumnName(ColumnMap.Value.To("OPERATER"));
           this.Property(t => t.OPERATE_DATE).HasColumnName(ColumnMap.Value.To("OPERATE_DATE"));
           this.Property(t => t.CHECKER).HasColumnName(ColumnMap.Value.To("CHECKER"));

           this.Property(t => t.CHECK_DATE).HasColumnName(ColumnMap.Value.To("CHECK_DATE"));
           this.Property(t => t.TASKER).HasColumnName(ColumnMap.Value.To("TASKER"));
           this.Property(t => t.TASK_DATE).HasColumnName(ColumnMap.Value.To("TASK_DATE"));
           // Relationships
           //this.HasRequired(t => t.Warehouse)
           //    .WithMany(t => t.Areas)
           //    .HasForeignKey(d => d.WarehouseCode)
           //    .WillCascadeOnDelete(false);
       }
       
    }
}
