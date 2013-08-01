using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class CMD_PRODUCT_GRADEMap : EntityTypeConfiguration<CMD_PRODUCT_GRADE>
    {
        public CMD_PRODUCT_GRADEMap()
        {
            // Primary Key
            this.HasKey(t => t.GRADE_CODE);

            // Properties
            this.Property(t => t.GRADE_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.ENGLISH_CODE)
                .HasMaxLength(6);

            this.Property(t => t.USER_CODE)
                .HasMaxLength(10);

            this.Property(t => t.GRADE_NAME)
                .HasMaxLength(20);

            this.Property(t => t.MEMO)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("CMD_PRODUCT_GRADE", "THOK");
            this.Property(t => t.GRADE_CODE).HasColumnName("GRADE_CODE");
            this.Property(t => t.ENGLISH_CODE).HasColumnName("ENGLISH_CODE");
            this.Property(t => t.USER_CODE).HasColumnName("USER_CODE");
            this.Property(t => t.GRADE_NAME).HasColumnName("GRADE_NAME");
            this.Property(t => t.MEMO).HasColumnName("MEMO");
        }
    }
}
