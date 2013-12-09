using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Authority.DbModel.Mapping
{
    public class AUTH_USER_FUNCTIONMap : EntityTypeConfiguration<AUTH_USER_FUNCTION>
    {
        public AUTH_USER_FUNCTIONMap()
        {
            // Primary Key
            this.HasKey(t => t.USER_FUNCTION_ID);

            // Properties
            this.Property(t => t.USER_FUNCTION_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            this.Property(t => t.IS_ACTIVE)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.USER_MODULE_USER_MODULE_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            this.Property(t => t.FUNCTION_FUNCTION_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            this.ToTable("AUTH_USER_FUNCTION","HNXC");
            this.Property(t => t.USER_FUNCTION_ID).HasColumnName("USER_FUNCTION_ID");
            this.Property(t => t.IS_ACTIVE).HasColumnName("IS_ACTIVE");
            this.Property(t => t.USER_MODULE_USER_MODULE_ID).HasColumnName("USER_MODULE_USER_MODULE_ID");
            this.Property(t => t.FUNCTION_FUNCTION_ID).HasColumnName("FUNCTION_FUNCTION_ID");

            // Relationships
            this.HasRequired(t => t.AUTH_FUNCTION)
                .WithMany(t => t.AUTH_USER_FUNCTION)
                .HasForeignKey(d => d.FUNCTION_FUNCTION_ID);
            this.HasRequired(t => t.AUTH_USER_MODULE)
                .WithMany(t => t.AUTH_USER_FUNCTION)
                .HasForeignKey(d => d.USER_MODULE_USER_MODULE_ID);

        }
    }
}
