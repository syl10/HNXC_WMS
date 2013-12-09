using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Authority.DbModel.Mapping
{
    public class AUTH_ROLE_MODULEMap : EntityTypeConfiguration<AUTH_ROLE_MODULE>
    {
        public AUTH_ROLE_MODULEMap()
        {
            // Primary Key
            this.HasKey(t => t.ROLE_MODULE_ID);

            // Properties
            this.Property(t => t.ROLE_MODULE_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            this.Property(t => t.IS_ACTIVE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.ROLE_SYSTEM_ROLE_SYSTEM_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.MODULE_MODULE_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            // Table & Column Mappings
            this.ToTable("AUTH_ROLE_MODULE","HNXC");
            this.Property(t => t.ROLE_MODULE_ID).HasColumnName("ROLE_MODULE_ID");
            this.Property(t => t.IS_ACTIVE).HasColumnName("IS_ACTIVE");
            this.Property(t => t.ROLE_SYSTEM_ROLE_SYSTEM_ID).HasColumnName("ROLE_SYSTEM_ROLE_SYSTEM_ID");
            this.Property(t => t.MODULE_MODULE_ID).HasColumnName("MODULE_MODULE_ID");

            // Relationships
            this.HasRequired(t => t.AUTH_MODULE)
                .WithMany(t => t.AUTH_ROLE_MODULE)
                .HasForeignKey(d => d.MODULE_MODULE_ID);
            this.HasRequired(t => t.AUTH_ROLE_SYSTEM)
                .WithMany(t => t.AUTH_ROLE_MODULE)
                .HasForeignKey(d => d.ROLE_SYSTEM_ROLE_SYSTEM_ID);

        }
    }
}
