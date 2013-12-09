using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class WMS_BALANCE_MASTERMap : EntityTypeConfiguration<WMS_BALANCE_MASTER>
    {
        public WMS_BALANCE_MASTERMap()
        {
            // Primary Key
            this.HasKey(t => t.BALANCE_NO);

            // Properties
            this.Property(t => t.BALANCE_NO)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            this.Property(t => t.STATE)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.OPERATER)
                .HasMaxLength(20);

            this.Property(t => t.CHECKER)
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("WMS_BALANCE_MASTER","HNXC");
            this.Property(t => t.BALANCE_NO).HasColumnName("BALANCE_NO");
            this.Property(t => t.BALANCE_DATE).HasColumnName("BALANCE_DATE");
            this.Property(t => t.STATE).HasColumnName("STATE");
            this.Property(t => t.OPERATER).HasColumnName("OPERATER");
            this.Property(t => t.CHECKER).HasColumnName("CHECKER");
            this.Property(t => t.CHECK_DATE).HasColumnName("CHECK_DATE");
        }
    }
}
