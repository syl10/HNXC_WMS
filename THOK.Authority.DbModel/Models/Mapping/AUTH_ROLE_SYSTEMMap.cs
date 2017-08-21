using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Authority.DbModel.Mapping
{
    public class AUTH_ROLE_SYSTEMMap : EntityTypeConfiguration<AUTH_ROLE_SYSTEM>
    {
        public AUTH_ROLE_SYSTEMMap()
        {
            // Primary Key
            this.HasKey(t => t.ROLE_SYSTEM_ID);

            // Properties
            this.Property(t => t.ROLE_SYSTEM_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.IS_ACTIVE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.ROLE_ROLE_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            this.Property(t => t.CITY_CITY_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.SYSTEM_SYSTEM_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            this.ToTable("AUTH_ROLE_SYSTEM","HNXC");
            this.Property(t => t.ROLE_SYSTEM_ID).HasColumnName("ROLE_SYSTEM_ID");
            this.Property(t => t.IS_ACTIVE).HasColumnName("IS_ACTIVE");
            this.Property(t => t.ROLE_ROLE_ID).HasColumnName("ROLE_ROLE_ID");
            this.Property(t => t.CITY_CITY_ID).HasColumnName("CITY_CITY_ID");
            this.Property(t => t.SYSTEM_SYSTEM_ID).HasColumnName("SYSTEM_SYSTEM_ID");

            // Relationships
            this.HasRequired(t => t.AUTH_CITY)
                .WithMany(t => t.AUTH_ROLE_SYSTEM)
                .HasForeignKey(d => d.CITY_CITY_ID);
            this.HasRequired(t => t.AUTH_ROLE)
                .WithMany(t => t.AUTH_ROLE_SYSTEM)
                .HasForeignKey(d => d.ROLE_ROLE_ID);
            this.HasRequired(t => t.AUTH_SYSTEM)
                .WithMany(t => t.AUTH_ROLE_SYSTEM)
                .HasForeignKey(d => d.SYSTEM_SYSTEM_ID);

        }
    }
}
