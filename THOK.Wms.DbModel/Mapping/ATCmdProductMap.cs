using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Common.Ef.MappingStrategy;

namespace THOK.Wms.DbModel.Mapping
{
    public class ATCmdProductMap : EntityMappingBase<ATCmdProduct>
    {
        public ATCmdProductMap()
            : base("Wms")
        {

            // Primary Key
            this.HasKey(t => t.CMD_PRODUCT_ID);

            // Properties
            this.Property(t => t.CMD_PRODUCT_ID)
                .IsRequired()
                .HasMaxLength(40);
            this.Property(t => t.PRODUCT_CODE)
                .IsRequired()
                .HasMaxLength(10);
            this.Property(t => t.PRODUCT_NAME)
                .IsRequired()
                .HasMaxLength(100);               
            this.Property(t => t.BAR_CODE)
                .HasMaxLength(50);
            this.Property(t => t.PALLET_QUANTITY)
                .IsRequired()
                .HasPrecision(3, 0);
            this.Property(t => t.QUANTITY)
                .HasPrecision(8, 0);               
            // Table & Column Mappings
            this.Property(t => t.CMD_PRODUCT_ID).HasColumnName(ColumnMap.Value.To("CMD_PRODUCT_ID"));
            this.Property(t => t.PRODUCT_CODE).HasColumnName(ColumnMap.Value.To("PRODUCT_CODE"));
            this.Property(t => t.PRODUCT_NAME).HasColumnName(ColumnMap.Value.To("PRODUCT_NAME"));
            this.Property(t => t.BAR_CODE).HasColumnName(ColumnMap.Value.To("BAR_CODE"));
            this.Property(t => t.PALLET_QUANTITY).HasColumnName(ColumnMap.Value.To("PALLET_QUANTITY"));
            this.Property(t => t.QUANTITY).HasColumnName(ColumnMap.Value.To("QUANTITY"));

            // Relationships
        }
    }
}
