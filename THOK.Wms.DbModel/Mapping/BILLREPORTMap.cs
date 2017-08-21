using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class BILLREPORTMap : EntityTypeConfiguration<BILLREPORT>
    {
        public BILLREPORTMap()
        {
            // Primary Key
            this.HasKey(t => new { t.MBILL_NO, t.BILL_DATE, t.BTYPE_CODE, t.BILLMETHODCODE, t.SCHEDULE_ITEMNO, t.BATCH_WEIGHT });

            // Properties
            this.Property(t => t.MBILL_NO)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.BTYPE_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.BTYPE_NAME)
                .HasMaxLength(20);

            this.Property(t => t.BILL_TYPE)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SCHEDULE_NO)
                .HasMaxLength(20);

            this.Property(t => t.WAREHOUSE_CODE)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.WAREHOUSE_NAME)
                .HasMaxLength(20);

            this.Property(t => t.TARGET_CODE)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.TARGET_NAME)
                .HasMaxLength(20);

            this.Property(t => t.STATUS)
                .HasMaxLength(50);

            this.Property(t => t.STATE)
                .HasMaxLength(50);

            this.Property(t => t.SOURCE_BILLNO)
                .HasMaxLength(20);

            this.Property(t => t.OPERATER)
                .HasMaxLength(50);

            this.Property(t => t.CHECKER)
                .HasMaxLength(50);

            this.Property(t => t.TASKER)
                .HasMaxLength(50);

            this.Property(t => t.BILL_METHOD)
                .HasMaxLength(50);

            this.Property(t => t.BILLMETHODCODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SCHEDULE_ITEMNO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.LINE_NO)
                .IsFixedLength()
                .HasMaxLength(2);

            this.Property(t => t.LINE_NAME)
                .HasMaxLength(30);

            this.Property(t => t.CIGARETTE_CODE)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.CIGARETTE_NAME)
                .HasMaxLength(40);

            this.Property(t => t.FORMULA_CODE)
                .HasMaxLength(20);

            this.Property(t => t.FORMULA_NAME)
                .HasMaxLength(50);

            this.Property(t => t.BATCH_WEIGHT)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DBILL_NO)
                .HasMaxLength(20);

            this.Property(t => t.PRODUCT_CODE)
                .HasMaxLength(20);

            this.Property(t => t.PRODUCT_NAME)
                .HasMaxLength(50);

            this.Property(t => t.YEARS)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.ORIGINAL_NAME)
                .HasMaxLength(20);

            this.Property(t => t.GRADE_NAME)
                .HasMaxLength(20);

            this.Property(t => t.CATEGORY_NAME)
                .HasMaxLength(50);

            this.Property(t => t.STYLE_NAME)
                .HasMaxLength(20);

            this.Property(t => t.IS_MIX)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.ISMIX)
                .HasMaxLength(50);

            this.Property(t => t.FPRODUCT_CODE)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("BILLREPORT","HNXC");
            this.Property(t => t.MBILL_NO).HasColumnName("MBILL_NO");
            this.Property(t => t.BILL_DATE).HasColumnName("BILL_DATE");
            this.Property(t => t.BTYPE_CODE).HasColumnName("BTYPE_CODE");
            this.Property(t => t.BTYPE_NAME).HasColumnName("BTYPE_NAME");
            this.Property(t => t.BILL_TYPE).HasColumnName("BILL_TYPE");
            this.Property(t => t.SCHEDULE_NO).HasColumnName("SCHEDULE_NO");
            this.Property(t => t.WAREHOUSE_CODE).HasColumnName("WAREHOUSE_CODE");
            this.Property(t => t.WAREHOUSE_NAME).HasColumnName("WAREHOUSE_NAME");
            this.Property(t => t.TARGET_CODE).HasColumnName("TARGET_CODE");
            this.Property(t => t.TARGET_NAME).HasColumnName("TARGET_NAME");
            this.Property(t => t.STATUS).HasColumnName("STATUS");
            this.Property(t => t.STATE).HasColumnName("STATE");
            this.Property(t => t.SOURCE_BILLNO).HasColumnName("SOURCE_BILLNO");
            this.Property(t => t.OPERATER).HasColumnName("OPERATER");
            this.Property(t => t.OPERATE_DATE).HasColumnName("OPERATE_DATE");
            this.Property(t => t.CHECKER).HasColumnName("CHECKER");
            this.Property(t => t.CHECK_DATE).HasColumnName("CHECK_DATE");
            this.Property(t => t.TASKER).HasColumnName("TASKER");
            this.Property(t => t.TASK_DATE).HasColumnName("TASK_DATE");
            this.Property(t => t.BILL_METHOD).HasColumnName("BILL_METHOD");
            this.Property(t => t.BILLMETHODCODE).HasColumnName("BILLMETHODCODE");
            this.Property(t => t.SCHEDULE_ITEMNO).HasColumnName("SCHEDULE_ITEMNO");
            this.Property(t => t.LINE_NO).HasColumnName("LINE_NO");
            this.Property(t => t.LINE_NAME).HasColumnName("LINE_NAME");
            this.Property(t => t.CIGARETTE_CODE).HasColumnName("CIGARETTE_CODE");
            this.Property(t => t.CIGARETTE_NAME).HasColumnName("CIGARETTE_NAME");
            this.Property(t => t.FORMULA_CODE).HasColumnName("FORMULA_CODE");
            this.Property(t => t.FORMULA_NAME).HasColumnName("FORMULA_NAME");
            this.Property(t => t.BATCH_WEIGHT).HasColumnName("BATCH_WEIGHT");
            this.Property(t => t.DBILL_NO).HasColumnName("DBILL_NO");
            this.Property(t => t.ITEM_NO).HasColumnName("ITEM_NO");
            this.Property(t => t.PRODUCT_CODE).HasColumnName("PRODUCT_CODE");
            this.Property(t => t.PRODUCT_NAME).HasColumnName("PRODUCT_NAME");
            this.Property(t => t.YEARS).HasColumnName("YEARS");
            this.Property(t => t.ORIGINAL_NAME).HasColumnName("ORIGINAL_NAME");
            this.Property(t => t.GRADE_NAME).HasColumnName("GRADE_NAME");
            this.Property(t => t.CATEGORY_NAME).HasColumnName("CATEGORY_NAME");
            this.Property(t => t.STYLE_NAME).HasColumnName("STYLE_NAME");
            this.Property(t => t.WEIGHT).HasColumnName("WEIGHT");
            this.Property(t => t.REAL_WEIGHT).HasColumnName("REAL_WEIGHT");
            this.Property(t => t.PACKAGE_COUNT).HasColumnName("PACKAGE_COUNT");
            this.Property(t => t.NC_COUNT).HasColumnName("NC_COUNT");
            this.Property(t => t.IS_MIX).HasColumnName("IS_MIX");
            this.Property(t => t.ISMIX).HasColumnName("ISMIX");
            this.Property(t => t.FPRODUCT_CODE).HasColumnName("FPRODUCT_CODE");
            this.Property(t => t.TOTALWEIGHT).HasColumnName("TOTALWEIGHT");
        }
    }
}
