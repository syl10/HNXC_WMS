using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace  THOK.Wms.DbModel.Mapping
{
    public class CMD_SHELFMap : EntityTypeConfiguration<CMD_SHELF>
    {
        public CMD_SHELFMap()
        {
            // Primary Key
            this.HasKey(t => t.SHELF_CODE);

            // Properties
            this.Property(t => t.SHELF_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(9);

            this.Property(t => t.SHELF_NAME)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.WAREHOUSE_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.AREA_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.CRANE_NO)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2);

            this.Property(t => t.MEMO)
                .HasMaxLength(100);

            this.Property(t => t.STATION_NO)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.CRANE_POSITION)
                .IsFixedLength()
                .HasMaxLength(12);

            // Table & Column Mappings
            this.ToTable("CMD_SHELF","HNXC");
            this.Property(t => t.SHELF_CODE).HasColumnName("SHELF_CODE");
            this.Property(t => t.SHELF_NAME).HasColumnName("SHELF_NAME");
            this.Property(t => t.ROW_COUNT).HasColumnName("ROW_COUNT");
            this.Property(t => t.COLUMN_COUNT).HasColumnName("COLUMN_COUNT");
            this.Property(t => t.WAREHOUSE_CODE).HasColumnName("WAREHOUSE_CODE");
            this.Property(t => t.AREA_CODE).HasColumnName("AREA_CODE");
            this.Property(t => t.CRANE_NO).HasColumnName("CRANE_NO");
            this.Property(t => t.MEMO).HasColumnName("MEMO");
            this.Property(t => t.STATION_NO).HasColumnName("STATION_NO");
            this.Property(t => t.CRANE_POSITION).HasColumnName("CRANE_POSITION");

            // Relationships
            this.HasRequired(t => t.CMD_AREA)
                .WithMany(t => t.CMD_SHELF)
                .HasForeignKey(d => d.AREA_CODE);
            this.HasRequired(t => t.CMD_CRANE)
                .WithMany(t => t.CMD_SHELF)
                .HasForeignKey(d => d.CRANE_NO);
            this.HasRequired(t => t.CMD_WAREHOUSE)
                .WithMany(t => t.CMD_SHELF)
                .HasForeignKey(d => d.WAREHOUSE_CODE);

        }
    }
}
