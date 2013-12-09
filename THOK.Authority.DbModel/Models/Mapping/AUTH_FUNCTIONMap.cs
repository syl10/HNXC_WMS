using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Authority.DbModel.Mapping
{
    public class AUTH_FUNCTIONMap : EntityTypeConfiguration<AUTH_FUNCTION>
    {
        public AUTH_FUNCTIONMap()
        {
            // Primary Key
            this.HasKey(t => t.FUNCTION_ID);

            // Properties
            this.Property(t => t.FUNCTION_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.FUNCTION_NAME)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CONTROL_NAME)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.INDICATE_IMAGE)
                .HasMaxLength(100);

            this.Property(t => t.MODULE_MODULE_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            // Table & Column Mappings
            this.ToTable("AUTH_FUNCTION","HNXC");
            this.Property(t => t.FUNCTION_ID).HasColumnName("FUNCTION_ID");
            this.Property(t => t.FUNCTION_NAME).HasColumnName("FUNCTION_NAME");
            this.Property(t => t.CONTROL_NAME).HasColumnName("CONTROL_NAME");
            this.Property(t => t.INDICATE_IMAGE).HasColumnName("INDICATE_IMAGE");
            this.Property(t => t.MODULE_MODULE_ID).HasColumnName("MODULE_MODULE_ID");

            // Relationships
            this.HasRequired(t => t.AUTH_MODULE)
                .WithMany(t => t.AUTH_FUNCTION)
                .HasForeignKey(d => d.MODULE_MODULE_ID);

        }
    }
}
