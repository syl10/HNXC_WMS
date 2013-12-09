using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class CMD_UNITMap : EntityTypeConfiguration<CMD_UNIT>
    {
        public CMD_UNITMap()
        {
            // Primary Key
            this.HasKey(t => t.UNIT_CODE);

            // Properties
            this.Property(t => t.UNIT_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            this.Property(t => t.MEMO)
                .HasMaxLength(100);

            this.Property(t => t.UNIT_NAME)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.CATEGORY_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            this.ToTable("CMD_UNIT","HNXC");
            this.Property(t => t.UNIT_CODE).HasColumnName("UNIT_CODE");
            this.Property(t => t.MEMO).HasColumnName("MEMO");
            this.Property(t => t.UNIT_NAME).HasColumnName("UNIT_NAME");
            this.Property(t => t.CATEGORY_CODE).HasColumnName("CATEGORY_CODE");
            // Relationships
            this.HasRequired(t => t.CMD_UNIT_CATEGORY)
                .WithMany(t => t.CMD_UNIT)
                .HasForeignKey(d => d.CATEGORY_CODE);
        }
    }
}
