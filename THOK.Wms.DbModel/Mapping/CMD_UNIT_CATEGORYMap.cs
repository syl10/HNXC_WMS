using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class CMD_UNIT_CATEGORYMap : EntityTypeConfiguration<CMD_UNIT_CATEGORY>
    {
        public CMD_UNIT_CATEGORYMap()
        {
            // Primary Key
            this.HasKey(t => t.CATEGORY_CODE);

            // Properties
            this.Property(t => t.CATEGORY_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.CATEGORY_NAME)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.MEMO)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("CMD_UNIT_CATEGORY","HNXC");
            this.Property(t => t.CATEGORY_CODE).HasColumnName("CATEGORY_CODE");
            this.Property(t => t.CATEGORY_NAME).HasColumnName("CATEGORY_NAME");
            this.Property(t => t.MEMO).HasColumnName("MEMO");
        }
    }
}
