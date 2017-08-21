using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class CMD_CELLMap : EntityTypeConfiguration<CMD_CELL>
    {
        public CMD_CELLMap()
        {
            // Primary Key
            this.HasKey(t => t.CELL_CODE);

            // Properties
            this.Property(t => t.CELL_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(9);

            this.Property(t => t.CELL_NAME)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.AREA_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.SHELF_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(9);

            this.Property(t => t.IS_ACTIVE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PRODUCT_CODE)
                .HasMaxLength(40);

            this.Property(t => t.PRODUCT_BARCODE)
                .HasMaxLength(200);

            this.Property(t => t.SCHEDULE_NO)
                .HasMaxLength(20);

            this.Property(t => t.IS_LOCK)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.PALLET_CODE)
                .HasMaxLength(100);

            this.Property(t => t.BILL_NO)
                .HasMaxLength(20);

            this.Property(t => t.MEMO)
                .HasMaxLength(100);

            this.Property(t => t.ERROR_FLAG)
                .IsFixedLength()
                .HasMaxLength(1);
            this.Property(t => t.NEW_PALLET_CODE)
                .IsFixedLength()
                .HasMaxLength(100);
            this.Property(t => t.WAREHOUSE_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            this.ToTable("CMD_CELL","HNXC");
            this.Property(t => t.CELL_CODE).HasColumnName("CELL_CODE");
            this.Property(t => t.CELL_NAME).HasColumnName("CELL_NAME");
            this.Property(t => t.AREA_CODE).HasColumnName("AREA_CODE");
            this.Property(t => t.SHELF_CODE).HasColumnName("SHELF_CODE");
            this.Property(t => t.CELL_COLUMN).HasColumnName("CELL_COLUMN");
            this.Property(t => t.CELL_ROW).HasColumnName("CELL_ROW");
            this.Property(t => t.IS_ACTIVE).HasColumnName("IS_ACTIVE");
            this.Property(t => t.PRODUCT_CODE).HasColumnName("PRODUCT_CODE");
            this.Property(t => t.REAL_WEIGHT).HasColumnName("REAL_WEIGHT");
            this.Property(t => t.SCHEDULE_NO).HasColumnName("SCHEDULE_NO");
            this.Property(t => t.IS_LOCK).HasColumnName("IS_LOCK");
            this.Property(t => t.PALLET_CODE).HasColumnName("PALLET_CODE");
            this.Property(t => t.BILL_NO).HasColumnName("BILL_NO");
            this.Property(t => t.IN_DATE).HasColumnName("IN_DATE");
            this.Property(t => t.MEMO).HasColumnName("MEMO");
            this.Property(t => t.PRIORITY_LEVEL).HasColumnName("PRIORITY_LEVEL");
            this.Property(t => t.ERROR_FLAG).HasColumnName("ERROR_FLAG");
            this.Property(t => t.NEW_PALLET_CODE).HasColumnName("NEW_PALLET_CODE");
            this.Property(t => t.WAREHOUSE_CODE).HasColumnName("WAREHOUSE_CODE");

            // Relationships
            this.HasRequired(t => t.CMD_AREA)
                .WithMany(t => t.CMD_CELL)
                .HasForeignKey(d => d.AREA_CODE);
            this.HasOptional(t => t.CMD_PRODUCT)
                .WithMany(t => t.CMD_CELL)
                .HasForeignKey(d => d.PRODUCT_CODE);
            this.HasRequired(t => t.CMD_SHELF)
                .WithMany(t => t.CMD_CELL)
                .HasForeignKey(d => d.SHELF_CODE);
            this.HasRequired(t => t.CMD_WAREHOUSE)
                .WithMany(t => t.CMD_CELL)
                .HasForeignKey(d => d.WAREHOUSE_CODE);
        }
    }
}
