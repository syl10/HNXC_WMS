using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class WMS_FORMULA_MASTERMap : EntityTypeConfiguration<WMS_FORMULA_MASTER>
    {
        public WMS_FORMULA_MASTERMap()
        {
            // Primary Key
            this.HasKey(t => t.FORMULA_CODE);

            // Properties
            this.Property(t => t.FORMULA_CODE)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.FORMULA_NAME)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CIGARETTE_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.OPERATER)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.IS_ACTIVE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("WMS_FORMULA_MASTER","HNXC");
            this.Property(t => t.FORMULA_CODE).HasColumnName("FORMULA_CODE");
            this.Property(t => t.FORMULA_NAME).HasColumnName("FORMULA_NAME");
            this.Property(t => t.FORMULA_DATE).HasColumnName("FORMULA_DATE");
            this.Property(t => t.CIGARETTE_CODE).HasColumnName("CIGARETTE_CODE");
            this.Property(t => t.OPERATER).HasColumnName("OPERATER");
            this.Property(t => t.OPERATEDATE).HasColumnName("OPERATEDATE");
            this.Property(t => t.USE_COUNT).HasColumnName("USE_COUNT");
            this.Property(t => t.FORMULANO).HasColumnName("FORMULANO");
            this.Property(t => t.IS_ACTIVE).HasColumnName("IS_ACTIVE");
            this.Property(t => t.BATCH_WEIGHT).HasColumnName("BATCH_WEIGHT");

            // Relationships
            this.HasRequired(t => t.CMD_CIGARETTE)
                .WithMany(t => t.WMS_FORMULA_MASTER)
                .HasForeignKey(d => d.CIGARETTE_CODE);

        }
    }
}
