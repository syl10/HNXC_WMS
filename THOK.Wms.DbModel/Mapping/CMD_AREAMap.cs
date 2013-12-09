using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace  THOK.Wms.DbModel.Mapping
{
    public class CMD_AREAMap : EntityTypeConfiguration<CMD_AREA>
    {
        public CMD_AREAMap()
        {
            // Primary Key
            this.HasKey(t => t.AREA_CODE);

            // Properties
            this.Property(t => t.AREA_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.AREA_NAME)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.WAREHOUSE_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.MEMO)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("CMD_AREA","HNXC");
            this.Property(t => t.AREA_CODE).HasColumnName("AREA_CODE");
            this.Property(t => t.AREA_NAME).HasColumnName("AREA_NAME");
            this.Property(t => t.WAREHOUSE_CODE).HasColumnName("WAREHOUSE_CODE");
            this.Property(t => t.MEMO).HasColumnName("MEMO");

            // Relationships
            this.HasRequired(t => t.CMD_WAREHOUSE)
                .WithMany(t => t.CMD_AREA)
                .HasForeignKey(d => d.WAREHOUSE_CODE);
        }
    }
}
