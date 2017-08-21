using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class CMD_PRODUCT_CATEGORYMap : EntityTypeConfiguration<CMD_PRODUCT_CATEGORY>
    {
        public CMD_PRODUCT_CATEGORYMap()
        {
            // Primary Key
            this.HasKey(t => t.CATEGORY_CODE);

            // Properties
            this.Property(t => t.CATEGORY_CODE)
                .IsRequired()
                .HasMaxLength(3);

            this.Property(t => t.CATEGORY_NAME)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.MEMO)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("CMD_PRODUCT_CATEGORY","HNXC");
            this.Property(t => t.CATEGORY_CODE).HasColumnName("CATEGORY_CODE");
            this.Property(t => t.CATEGORY_NAME).HasColumnName("CATEGORY_NAME");
            this.Property(t => t.MEMO).HasColumnName("MEMO");
        }
    }
}
