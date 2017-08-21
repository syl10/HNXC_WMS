using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class WORKSELECTMap : EntityTypeConfiguration<WORKSELECT>
    {
        public WORKSELECTMap()
        {
            // Primary Key
            this.HasKey(t => new { t.BILL_NO, t.PRODUCT_CODE, t.REAL_WEIGHT, t.TASK_ID });

            // Properties
            this.Property(t => t.BILL_NO)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.PRODUCT_CODE)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.PRODUCT_BARCODE)
                .HasMaxLength(40);

            this.Property(t => t.REAL_WEIGHT)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TARGET_CODE)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.STATE)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.STATENAME)
                .HasMaxLength(50);

            this.Property(t => t.TASKER)
                .HasMaxLength(10);

            this.Property(t => t.IS_MIX)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.MIXNAME)
                .HasMaxLength(50);

            this.Property(t => t.SOURCE_BILLNO)
                .HasMaxLength(20);

            this.Property(t => t.TASK_ID)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.CELL_CODE)
                .IsFixedLength()
                .HasMaxLength(8);

            this.Property(t => t.PRODUCT_NAME)
                .HasMaxLength(50);

            this.Property(t => t.CATEGORY_NAME)
                .HasMaxLength(50);

            this.Property(t => t.GRADE_NAME)
                .HasMaxLength(20);

            this.Property(t => t.ORIGINAL_NAME)
                .HasMaxLength(20);

            this.Property(t => t.STYLE_NAME)
                .HasMaxLength(20);

            this.Property(t => t.YEARS)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.BTYPE_CODE)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.BTYPE_NAME)
                .HasMaxLength(20);

            this.Property(t => t.BILL_METHOD)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.BILLMETHOD)
                .HasMaxLength(50);

            this.Property(t => t.FORMULA_CODE)
                .HasMaxLength(20);

            this.Property(t => t.FORMULA_NAME)
                .HasMaxLength(50);

            this.Property(t => t.CIGARETTE_CODE)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.CIGARETTE_NAME)
                .HasMaxLength(40);
            this.Property(t => t.USER_NAME).HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("WORKSELECT","HNXC");
            this.Property(t => t.BILL_NO).HasColumnName("BILL_NO");
            this.Property(t => t.PRODUCT_CODE).HasColumnName("PRODUCT_CODE");
            this.Property(t => t.PRODUCT_BARCODE).HasColumnName("PRODUCT_BARCODE");
            this.Property(t => t.REAL_WEIGHT).HasColumnName("REAL_WEIGHT");
            this.Property(t => t.TARGET_CODE).HasColumnName("TARGET_CODE");
            this.Property(t => t.STATE).HasColumnName("STATE");
            this.Property(t => t.STATENAME).HasColumnName("STATENAME");
            this.Property(t => t.TASK_DATE).HasColumnName("TASK_DATE");
            this.Property(t => t.TASKER).HasColumnName("TASKER");
            this.Property(t => t.IS_MIX).HasColumnName("IS_MIX");
            this.Property(t => t.MIXNAME).HasColumnName("MIXNAME");
            this.Property(t => t.SOURCE_BILLNO).HasColumnName("SOURCE_BILLNO");
            this.Property(t => t.TASK_ID).HasColumnName("TASK_ID");
            this.Property(t => t.CELL_CODE).HasColumnName("CELL_CODE");
            this.Property(t => t.PRODUCT_NAME).HasColumnName("PRODUCT_NAME");
            this.Property(t => t.CATEGORY_NAME).HasColumnName("CATEGORY_NAME");
            this.Property(t => t.GRADE_NAME).HasColumnName("GRADE_NAME");
            this.Property(t => t.ORIGINAL_NAME).HasColumnName("ORIGINAL_NAME");
            this.Property(t => t.STYLE_NAME).HasColumnName("STYLE_NAME");
            this.Property(t => t.YEARS).HasColumnName("YEARS");
            this.Property(t => t.BTYPE_CODE).HasColumnName("BTYPE_CODE");
            this.Property(t => t.BTYPE_NAME).HasColumnName("BTYPE_NAME");
            this.Property(t => t.BILL_METHOD).HasColumnName("BILL_METHOD");
            this.Property(t => t.BILLMETHOD).HasColumnName("BILLMETHOD");
            this.Property(t => t.FORMULA_CODE).HasColumnName("FORMULA_CODE");
            this.Property(t => t.FORMULA_NAME).HasColumnName("FORMULA_NAME");
            this.Property(t => t.CIGARETTE_CODE).HasColumnName("CIGARETTE_CODE");
            this.Property(t => t.CIGARETTE_NAME).HasColumnName("CIGARETTE_NAME");
            this.Property(t => t.BATCH_WEIGHT).HasColumnName("BATCH_WEIGHT");
            this.Property(t => t.IN_DATE).HasColumnName("IN_DATE");
            this.Property(t => t.USER_NAME).HasColumnName("USER_NAME");
        }
    }
}
