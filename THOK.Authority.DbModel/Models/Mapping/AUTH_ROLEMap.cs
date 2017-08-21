using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Authority.DbModel.Mapping
{
    public class AUTH_ROLEMap : EntityTypeConfiguration<AUTH_ROLE>
    {
        public AUTH_ROLEMap()
        {
            // Primary Key
            this.HasKey(t => t.ROLE_ID);

            // Properties
            this.Property(t => t.ROLE_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            this.Property(t => t.ROLE_NAME)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.IS_LOCK)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.MEMO)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("AUTH_ROLE","HNXC");
            this.Property(t => t.ROLE_ID).HasColumnName("ROLE_ID");
            this.Property(t => t.ROLE_NAME).HasColumnName("ROLE_NAME");
            this.Property(t => t.IS_LOCK).HasColumnName("IS_LOCK");
            this.Property(t => t.MEMO).HasColumnName("MEMO");
        }
    }
}
