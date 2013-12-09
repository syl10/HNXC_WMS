using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class WMS_SCHEDULE_DETAILMap : EntityTypeConfiguration<WMS_SCHEDULE_DETAIL>
    {
        public WMS_SCHEDULE_DETAILMap()
        {
            // Primary Key
            this.HasKey(t => new { t.SCHEDULE_NO, t.ITEM_NO });

            // Properties
            this.Property(t => t.SCHEDULE_NO)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.ITEM_NO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CIGARETTE_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.FORMULA_CODE)
                .HasMaxLength(20);

            this.Property(t => t.BILL_NO)
                .HasMaxLength(20);

            this.Property(t => t.LINE_NO)
                .IsFixedLength()
                .HasMaxLength(2);

            // Table & Column Mappings
            this.ToTable("WMS_SCHEDULE_DETAIL","HNXC");
            this.Property(t => t.SCHEDULE_NO).HasColumnName("SCHEDULE_NO");
            this.Property(t => t.ITEM_NO).HasColumnName("ITEM_NO");
            this.Property(t => t.CIGARETTE_CODE).HasColumnName("CIGARETTE_CODE");
            this.Property(t => t.FORMULA_CODE).HasColumnName("FORMULA_CODE");
            this.Property(t => t.BILL_NO).HasColumnName("BILL_NO");
            this.Property(t => t.QUANTITY).HasColumnName("QUANTITY");
            this.Property(t => t.LINE_NO).HasColumnName("LINE_NO");

            // Relationships
            this.HasRequired(t => t.CMD_CIGARETTE)
                .WithMany(t => t.WMS_SCHEDULE_DETAIL)
                .HasForeignKey(d => d.CIGARETTE_CODE);
            this.HasOptional(t => t.CMD_PRODUCTION_LINE)
                .WithMany(t => t.WMS_SCHEDULE_DETAIL)
                .HasForeignKey(d => d.LINE_NO);
            this.HasOptional(t => t.WMS_FORMULA_MASTER)
                .WithMany(t => t.WMS_SCHEDULE_DETAIL)
                .HasForeignKey(d => d.FORMULA_CODE);

        }
    }
}
