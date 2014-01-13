using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class SYS_ERROR_CODEMap : EntityTypeConfiguration<SYS_ERROR_CODE>
    {
        public SYS_ERROR_CODEMap()
        {
            // Primary Key
            this.HasKey(t => t.CODE);

            // Properties
            this.Property(t => t.CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.DESCRIPTION)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("SYS_ERROR_CODE", "HNXC");
            this.Property(t => t.CODE).HasColumnName("CODE");
            this.Property(t => t.DESCRIPTION).HasColumnName("DESCRIPTION");
        }
    }
}
