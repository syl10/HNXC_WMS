using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Authority.DbModel.Mapping
{
    public class AUTH_SERVERMap : EntityTypeConfiguration<AUTH_SERVER>
    {
        public AUTH_SERVERMap()
        {
            // Primary Key
            this.HasKey(t => t.SERVER_ID);

            // Properties
            this.Property(t => t.SERVER_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.SERVER_NAME)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.DESCRIPTION)
                .HasMaxLength(1000);

            this.Property(t => t.URL)
                .HasMaxLength(200);

            this.Property(t => t.IS_ACTIVE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.CITY_CITY_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            this.ToTable("AUTH_SERVER","HNXC");
            this.Property(t => t.SERVER_ID).HasColumnName("SERVER_ID");
            this.Property(t => t.SERVER_NAME).HasColumnName("SERVER_NAME");
            this.Property(t => t.DESCRIPTION).HasColumnName("DESCRIPTION");
            this.Property(t => t.URL).HasColumnName("URL");
            this.Property(t => t.IS_ACTIVE).HasColumnName("IS_ACTIVE");
            this.Property(t => t.CITY_CITY_ID).HasColumnName("CITY_CITY_ID");

            // Relationships
            this.HasRequired(t => t.AUTH_CITY)
                .WithMany(t => t.AUTH_SERVER)
                .HasForeignKey(d => d.CITY_CITY_ID);

        }
    }
}
