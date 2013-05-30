using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Common.Ef.MappingStrategy;


namespace THOK.Wms.DbModel.Mapping
{
   public  class ATBillDetailMap :EntityMappingBase<ATBillDetail>
    {
       public ATBillDetailMap()
           : base("Wms")
       {
           this.HasKey(t => t.WMS_BILL_DETAIL_ID);

           this.Property(t => t.WMS_BILL_DETAIL_ID)
               .IsRequired()
               .HasMaxLength(40);
           this.Property(t => t.WMS_BILL_MASTER_ID)
               .HasMaxLength(40);
           this.Property(t => t.ITEM_ORDER)
               .IsRequired()
               .HasPrecision(12, 0);
           this.Property(t => t.PRODUCT_CODE)
               .HasMaxLength(10);
           this.Property(t => t.QUANTITY)
               .IsRequired()
               .HasPrecision(10, 0);
           this.Property(t => t.REAL_QUANTITY)
               .HasPrecision(10, 0);
           this.Property(t => t.UNIT_PRICE)
               .HasPrecision(10, 2);
           this.Property(t => t.AMOUNT)
               .HasPrecision(12, 2);

           // Table & Column Mappings
           this.Property(t => t.WMS_BILL_DETAIL_ID).HasColumnName(ColumnMap.Value.To("WMS_BILL_DETAIL_ID"));
           this.Property(t => t.WMS_BILL_MASTER_ID).HasColumnName(ColumnMap.Value.To("WMS_BILL_MASTER_ID"));
           this.Property(t => t.ITEM_ORDER).HasColumnName(ColumnMap.Value.To("ITEM_ORDER"));
           this.Property(t => t.PRODUCT_CODE).HasColumnName(ColumnMap.Value.To("PRODUCT_CODE"));
           this.Property(t => t.QUANTITY).HasColumnName(ColumnMap.Value.To("QUANTITY"));
           this.Property(t => t.REAL_QUANTITY).HasColumnName(ColumnMap.Value.To("REAL_QUANTITY"));
           this.Property(t => t.UNIT_PRICE).HasColumnName(ColumnMap.Value.To("UNIT_PRICE"));
           this.Property(t => t.AMOUNT).HasColumnName(ColumnMap.Value.To("AMOUNT"));

           //       // Relationships
           //this.HasRequired(t => t.BillType)
           //    .WithMany(t => t.CheckBillMasters)
           //    .HasForeignKey(d => d.BillTypeCode)
           //    .WillCascadeOnDelete(false);
       }
    }
}
