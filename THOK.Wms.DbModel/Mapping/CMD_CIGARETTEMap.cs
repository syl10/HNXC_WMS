using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class CMD_CIGARETTEMap : EntityTypeConfiguration<CMD_CIGARETTE>
    {
        public CMD_CIGARETTEMap()
        {
            // Primary Key
            this.HasKey(t => t.CIGARETTE_CODE);

            // Properties
            this.Property(t => t.CIGARETTE_CODE)
                .IsRequired()
                .HasMaxLength(3);

            this.Property(t => t.CIGARETTE_NAME)
                .IsRequired()
                .HasMaxLength(40);

            this.Property(t => t.CIGARETTE_MEMO)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("CMD_CIGARETTE", "THOK");
            this.Property(t => t.CIGARETTE_CODE).HasColumnName("CIGARETTE_CODE");
            this.Property(t => t.CIGARETTE_NAME).HasColumnName("CIGARETTE_NAME");
            this.Property(t => t.CIGARETTE_MEMO).HasColumnName("CIGARETTE_MEMO");
        }
    }
}
