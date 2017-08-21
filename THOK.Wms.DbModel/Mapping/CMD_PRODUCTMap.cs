using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class CMD_PRODUCTMap : EntityTypeConfiguration<CMD_PRODUCT>
    {
        public CMD_PRODUCTMap()
        {
            // Primary Key
            this.HasKey(t => t.PRODUCT_CODE);

            // Properties
            this.Property(t => t.PRODUCT_CODE)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.PRODUCT_NAME)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ORIGINAL_CODE)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.YEARS)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.GRADE_CODE)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.STYLE_NO)
                .IsFixedLength()
                .HasMaxLength(2);

            this.Property(t => t.MEMO)
                .HasMaxLength(200);

            this.Property(t => t.CATEGORY_CODE)
                .IsRequired()
                .HasMaxLength(3);

            // Table & Column Mappings
            this.ToTable("CMD_PRODUCT","HNXC");
            this.Property(t => t.PRODUCT_CODE).HasColumnName("PRODUCT_CODE");
            this.Property(t => t.PRODUCT_NAME).HasColumnName("PRODUCT_NAME");
            this.Property(t => t.ORIGINAL_CODE).HasColumnName("ORIGINAL_CODE");
            this.Property(t => t.YEARS).HasColumnName("YEARS");
            this.Property(t => t.GRADE_CODE).HasColumnName("GRADE_CODE");
            this.Property(t => t.STYLE_NO).HasColumnName("STYLE_NO");
            this.Property(t => t.WEIGHT).HasColumnName("WEIGHT");
            this.Property(t => t.MEMO).HasColumnName("MEMO");
            this.Property(t => t.CATEGORY_CODE).HasColumnName("CATEGORY_CODE");

            // Relationships
            this.HasOptional(t => t.CMD_PRODUCT_GRADE)
                .WithMany(t => t.CMD_PRODUCT)
                .HasForeignKey(d => d.GRADE_CODE);
            this.HasOptional(t => t.CMD_PRODUCT_ORIGINAL)
                .WithMany(t => t.CMD_PRODUCT)
                .HasForeignKey(d => d.ORIGINAL_CODE);
            this.HasRequired(t => t.CMD_PRODUCT_CATEGORY)
                .WithMany(t => t.CMD_PRODUCT)
                .HasForeignKey(d => d.CATEGORY_CODE);
            this.HasOptional(t => t.CMD_PRODUCT_STYLE)
                .WithMany(t => t.CMD_PRODUCT)
                .HasForeignKey(d => d.STYLE_NO);

        }
    }
}
