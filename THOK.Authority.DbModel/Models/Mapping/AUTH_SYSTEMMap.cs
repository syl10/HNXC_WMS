using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Authority.DbModel.Mapping
{
    public class AUTH_SYSTEMMap : EntityTypeConfiguration<AUTH_SYSTEM>
    {
        public AUTH_SYSTEMMap()
        {
            // Primary Key
            this.HasKey(t => t.SYSTEM_ID);

            // Properties
            this.Property(t => t.SYSTEM_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.SYSTEM_NAME)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.DESCRIPTION)
                .HasMaxLength(1000);

            this.Property(t => t.STATUS)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("AUTH_SYSTEM","HNXC");
            this.Property(t => t.SYSTEM_ID).HasColumnName("SYSTEM_ID");
            this.Property(t => t.SYSTEM_NAME).HasColumnName("SYSTEM_NAME");
            this.Property(t => t.DESCRIPTION).HasColumnName("DESCRIPTION");
            this.Property(t => t.STATUS).HasColumnName("STATUS");
        }
    }
}
