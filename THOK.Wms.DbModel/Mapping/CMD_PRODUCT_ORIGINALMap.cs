using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class CMD_PRODUCT_ORIGINALMap : EntityTypeConfiguration<CMD_PRODUCT_ORIGINAL>
    {
        public CMD_PRODUCT_ORIGINALMap()
        {
            // Primary Key
            this.HasKey(t => t.ORIGINAL_CODE);

            // Properties
            this.Property(t => t.ORIGINAL_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.ORIGINAL_NAME)
                .HasMaxLength(20);

            this.Property(t => t.DISTRICT_CODE)
                .HasMaxLength(6);

            this.Property(t => t.MEMO)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("CMD_PRODUCT_ORIGINAL","HNXC");
            this.Property(t => t.ORIGINAL_CODE).HasColumnName("ORIGINAL_CODE");
            this.Property(t => t.ORIGINAL_NAME).HasColumnName("ORIGINAL_NAME");
            this.Property(t => t.DISTRICT_CODE).HasColumnName("DISTRICT_CODE");
            this.Property(t => t.MEMO).HasColumnName("MEMO");
        }
    }
}
