using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class PRINTREPORTMap : EntityTypeConfiguration<PRINTREPORT>
    {
        public PRINTREPORTMap()
        {
            // Primary Key
            this.HasKey(t => new { t.BILL_NO, t.PRODUCT_CODE, t.REAL_WEIGHT });

            // Properties
            this.Property(t => t.PRODUCT_BARCODE)
                .HasMaxLength(40);

            this.Property(t => t.BILL_NO)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.FORMULA_NAME)
                .HasMaxLength(50);

            this.Property(t => t.CIGARETTE_NAME)
                .HasMaxLength(40);

            this.Property(t => t.PRODUCT_CODE)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.REAL_WEIGHT)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PRODUCT_NAME)
                .HasMaxLength(50);

            this.Property(t => t.YEARS)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.ORIGINAL_NAME)
                .HasMaxLength(20);

            this.Property(t => t.CATEGORY_NAME)
                .HasMaxLength(50);

            this.Property(t => t.GRADE_NAME)
                .HasMaxLength(20);

            this.Property(t => t.STYLE_NAME)
                .HasMaxLength(20);

            this.Property(t => t.MODULES)
                .HasMaxLength(10);
            this.Property(t => t.PACKAGECOUNT)
                .HasMaxLength(3);
            this.Property(t => t.IS_MIX)
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("PRINTREPORT","HNXC");
            this.Property(t => t.PRODUCT_BARCODE).HasColumnName("PRODUCT_BARCODE");
            this.Property(t => t.BILL_NO).HasColumnName("BILL_NO");
            this.Property(t => t.FORMULA_NAME).HasColumnName("FORMULA_NAME");
            this.Property(t => t.CIGARETTE_NAME).HasColumnName("CIGARETTE_NAME");
            this.Property(t => t.PRODUCT_CODE).HasColumnName("PRODUCT_CODE");
            this.Property(t => t.REAL_WEIGHT).HasColumnName("REAL_WEIGHT");
            this.Property(t => t.BILL_DATE).HasColumnName("BILL_DATE");
            this.Property(t => t.PRODUCT_NAME).HasColumnName("PRODUCT_NAME");
            this.Property(t => t.YEARS).HasColumnName("YEARS");
            this.Property(t => t.ORIGINAL_NAME).HasColumnName("ORIGINAL_NAME");
            this.Property(t => t.CATEGORY_NAME).HasColumnName("CATEGORY_NAME");
            this.Property(t => t.GRADE_NAME).HasColumnName("GRADE_NAME");
            this.Property(t => t.STYLE_NAME).HasColumnName("STYLE_NAME");
            this.Property(t => t.MODULES).HasColumnName("MODULES");
            this.Property(t => t.PACKAGECOUNT).HasColumnName("PACKAGECOUNT");
            this.Property(t => t.IS_MIX).HasColumnName("IS_MIX");
        }
    }
}
