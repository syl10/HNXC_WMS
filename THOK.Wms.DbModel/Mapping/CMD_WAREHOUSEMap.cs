using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace  THOK.Wms.DbModel.Mapping
{
    public class CMD_WAREHOUSEMap : EntityTypeConfiguration<CMD_WAREHOUSE>
    {
        public CMD_WAREHOUSEMap()
        {
            // Primary Key
            this.HasKey(t => t.WAREHOUSE_CODE);

            // Properties
            this.Property(t => t.WAREHOUSE_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.WAREHOUSE_NAME)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.MEMO)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("CMD_WAREHOUSE","HNXC");
            this.Property(t => t.WAREHOUSE_CODE).HasColumnName("WAREHOUSE_CODE");
            this.Property(t => t.WAREHOUSE_NAME).HasColumnName("WAREHOUSE_NAME");
            this.Property(t => t.MEMO).HasColumnName("MEMO");
        }
    }
}
