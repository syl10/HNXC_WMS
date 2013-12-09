using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Authority.DbModel.Mapping
{
    public class AUTH_USER_ROLEMap : EntityTypeConfiguration<AUTH_USER_ROLE>
    {
        public AUTH_USER_ROLEMap()
        {
            // Primary Key
            this.HasKey(t => t.USER_ROLE_ID);

            // Properties
            this.Property(t => t.USER_ROLE_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            this.Property(t => t.ROLE_ROLE_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            this.Property(t => t.USER_USER_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            // Table & Column Mappings
            this.ToTable("AUTH_USER_ROLE","HNXC");
            this.Property(t => t.USER_ROLE_ID).HasColumnName("USER_ROLE_ID");
            this.Property(t => t.ROLE_ROLE_ID).HasColumnName("ROLE_ROLE_ID");
            this.Property(t => t.USER_USER_ID).HasColumnName("USER_USER_ID");

            // Relationships
            this.HasRequired(t => t.AUTH_ROLE)
                .WithMany(t => t.AUTH_USER_ROLE)
                .HasForeignKey(d => d.ROLE_ROLE_ID);
            this.HasRequired(t => t.AUTH_USER)
                .WithMany(t => t.AUTH_USER_ROLE)
                .HasForeignKey(d => d.USER_USER_ID);

        }
    }
}
