using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Authority.DbModel.Mapping
{
    public class AUTH_USER_SYSTEMMap : EntityTypeConfiguration<AUTH_USER_SYSTEM>
    {
        public AUTH_USER_SYSTEMMap()
        {
            // Primary Key
            this.HasKey(t => t.USER_SYSTEM_ID);

            // Properties
            this.Property(t => t.USER_SYSTEM_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.IS_ACTIVE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.USER_USER_ID)
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
            this.ToTable("AUTH_USER_SYSTEM","HNXC");
            this.Property(t => t.USER_SYSTEM_ID).HasColumnName("USER_SYSTEM_ID");
            this.Property(t => t.IS_ACTIVE).HasColumnName("IS_ACTIVE");
            this.Property(t => t.USER_USER_ID).HasColumnName("USER_USER_ID");
            this.Property(t => t.CITY_CITY_ID).HasColumnName("CITY_CITY_ID");
            this.Property(t => t.SYSTEM_SYSTEM_ID).HasColumnName("SYSTEM_SYSTEM_ID");

            // Relationships
            this.HasRequired(t => t.AUTH_CITY)
                .WithMany(t => t.AUTH_USER_SYSTEM)
                .HasForeignKey(d => d.CITY_CITY_ID);
            this.HasRequired(t => t.AUTH_SYSTEM)
                .WithMany(t => t.AUTH_USER_SYSTEM)
                .HasForeignKey(d => d.SYSTEM_SYSTEM_ID);
            this.HasRequired(t => t.AUTH_USER)
                .WithMany(t => t.AUTH_USER_SYSTEM)
                .HasForeignKey(d => d.USER_USER_ID);

        }
    }
}
