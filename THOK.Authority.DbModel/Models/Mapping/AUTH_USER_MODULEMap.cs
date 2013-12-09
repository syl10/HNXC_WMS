using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Authority.DbModel.Mapping
{
    public class AUTH_USER_MODULEMap : EntityTypeConfiguration<AUTH_USER_MODULE>
    {
        public AUTH_USER_MODULEMap()
        {
            // Primary Key
            this.HasKey(t => t.USER_MODULE_ID);

            // Properties
            this.Property(t => t.USER_MODULE_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            this.Property(t => t.IS_ACTIVE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.USER_SYSTEM_USER_SYSTEM_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.MODULE_MODULE_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            // Table & Column Mappings
            this.ToTable("AUTH_USER_MODULE","HNXC");
            this.Property(t => t.USER_MODULE_ID).HasColumnName("USER_MODULE_ID");
            this.Property(t => t.IS_ACTIVE).HasColumnName("IS_ACTIVE");
            this.Property(t => t.USER_SYSTEM_USER_SYSTEM_ID).HasColumnName("USER_SYSTEM_USER_SYSTEM_ID");
            this.Property(t => t.MODULE_MODULE_ID).HasColumnName("MODULE_MODULE_ID");

            // Relationships
            this.HasRequired(t => t.AUTH_MODULE)
                .WithMany(t => t.AUTH_USER_MODULE)
                .HasForeignKey(d => d.MODULE_MODULE_ID);
            this.HasRequired(t => t.AUTH_USER_SYSTEM)
                .WithMany(t => t.AUTH_USER_MODULE)
                .HasForeignKey(d => d.USER_SYSTEM_USER_SYSTEM_ID);

        }
    }
}
