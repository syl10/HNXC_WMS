using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Authority.DbModel.Mapping
{
    public class AUTH_SYSTEM_PARAMETERMap : EntityTypeConfiguration<AUTH_SYSTEM_PARAMETER>
    {
        public AUTH_SYSTEM_PARAMETERMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.PARAMETER_NAME)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.PARAMETER_VALUE)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.REMARK)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.USER_NAME)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.SYSTEM_ID)
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            this.ToTable("AUTH_SYSTEM_PARAMETER","HNXC");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.PARAMETER_NAME).HasColumnName("PARAMETER_NAME");
            this.Property(t => t.PARAMETER_VALUE).HasColumnName("PARAMETER_VALUE");
            this.Property(t => t.REMARK).HasColumnName("REMARK");
            this.Property(t => t.USER_NAME).HasColumnName("USER_NAME");
            this.Property(t => t.SYSTEM_ID).HasColumnName("SYSTEM_ID");

            // Relationships
            this.HasOptional(t => t.AUTH_SYSTEM)
                .WithMany(t => t.AUTH_SYSTEM_PARAMETER)
                .HasForeignKey(d => d.SYSTEM_ID);

        }
    }
}
