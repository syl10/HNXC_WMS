using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Authority.DbModel.Mapping
{
    public class AUTH_ROLE_FUNCTIONMap : EntityTypeConfiguration<AUTH_ROLE_FUNCTION>
    {
        public AUTH_ROLE_FUNCTIONMap()
        {
            // Primary Key
            this.HasKey(t => t.ROLE_FUNCTION_ID);

            // Properties
            this.Property(t => t.ROLE_FUNCTION_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            this.Property(t => t.IS_ACTIVE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.ROLE_MODULE_ROLE_MODULE_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            this.Property(t => t.FUNCTION_FUNCTION_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            this.ToTable("AUTH_ROLE_FUNCTION","HNXC");
            this.Property(t => t.ROLE_FUNCTION_ID).HasColumnName("ROLE_FUNCTION_ID");
            this.Property(t => t.IS_ACTIVE).HasColumnName("IS_ACTIVE");
            this.Property(t => t.ROLE_MODULE_ROLE_MODULE_ID).HasColumnName("ROLE_MODULE_ROLE_MODULE_ID");
            this.Property(t => t.FUNCTION_FUNCTION_ID).HasColumnName("FUNCTION_FUNCTION_ID");

            // Relationships
            this.HasRequired(t => t.AUTH_FUNCTION)
                .WithMany(t => t.AUTH_ROLE_FUNCTION)
                .HasForeignKey(d => d.FUNCTION_FUNCTION_ID);
            this.HasRequired(t => t.AUTH_ROLE_MODULE)
                .WithMany(t => t.AUTH_ROLE_FUNCTION)
                .HasForeignKey(d => d.ROLE_MODULE_ROLE_MODULE_ID);

        }
    }
}
