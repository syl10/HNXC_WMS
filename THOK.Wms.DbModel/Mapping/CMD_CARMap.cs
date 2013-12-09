using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class CMD_CARMap : EntityTypeConfiguration<CMD_CAR>
    {
        public CMD_CARMap()
        {
            // Primary Key
            this.HasKey(t => t.CAR_NO);

            // Properties
            this.Property(t => t.CAR_NO)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2);

            this.Property(t => t.CAR_NAME)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.IS_ACTIVE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.MEMO)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("CMD_CAR","HNXC");
            this.Property(t => t.CAR_NO).HasColumnName("CAR_NO");
            this.Property(t => t.CAR_NAME).HasColumnName("CAR_NAME");
            this.Property(t => t.IS_ACTIVE).HasColumnName("IS_ACTIVE");
            this.Property(t => t.MEMO).HasColumnName("MEMO");
        }
    }
}
