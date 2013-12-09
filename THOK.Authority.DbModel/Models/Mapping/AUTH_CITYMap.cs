using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Authority.DbModel.Mapping
{
    public class AUTH_CITYMap : EntityTypeConfiguration<AUTH_CITY>
    {
        public AUTH_CITYMap()
        {
            // Primary Key
            this.HasKey(t => t.CITY_ID);

            // Properties
            this.Property(t => t.CITY_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.CITY_NAME)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.DESCRIPTION)
                .HasMaxLength(1000);

            this.Property(t => t.IS_ACTIVE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("AUTH_CITY","HNXC");
            this.Property(t => t.CITY_ID).HasColumnName("CITY_ID");
            this.Property(t => t.CITY_NAME).HasColumnName("CITY_NAME");
            this.Property(t => t.DESCRIPTION).HasColumnName("DESCRIPTION");
            this.Property(t => t.IS_ACTIVE).HasColumnName("IS_ACTIVE");
        }
    }
}
