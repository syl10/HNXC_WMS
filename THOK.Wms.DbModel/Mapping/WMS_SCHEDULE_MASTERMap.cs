using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class WMS_SCHEDULE_MASTERMap : EntityTypeConfiguration<WMS_SCHEDULE_MASTER>
    {
        public WMS_SCHEDULE_MASTERMap()
        {
            // Primary Key
            this.HasKey(t => new { t.SCHEDULE_NO });

            // Properties
            this.Property(t => t.SCHEDULE_NO)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.SOURCE_BILLNO)
                .HasMaxLength(20);

            this.Property(t => t.STATUS)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.STATE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.OPERATER)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.CHECKER)
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("WMS_SCHEDULE_MASTER","HNXC");
            this.Property(t => t.SCHEDULE_DATE).HasColumnName("SCHEDULE_DATE");
            this.Property(t => t.SCHEDULE_NO).HasColumnName("SCHEDULE_NO");
            this.Property(t => t.SOURCE_BILLNO).HasColumnName("SOURCE_BILLNO");
            this.Property(t => t.STATUS).HasColumnName("STATUS");
            this.Property(t => t.STATE).HasColumnName("STATE");
            this.Property(t => t.OPERATER).HasColumnName("OPERATER");
            this.Property(t => t.OPERATE_DATE).HasColumnName("OPERATE_DATE");
            this.Property(t => t.CHECKER).HasColumnName("CHECKER");
            this.Property(t => t.CHECK_DATE).HasColumnName("CHECK_DATE");
        }
    }
}
