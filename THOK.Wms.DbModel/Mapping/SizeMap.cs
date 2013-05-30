using THOK.Common.Ef.MappingStrategy;
using System.ComponentModel.DataAnnotations;

namespace THOK.Wms.DbModel.Mapping
{
    public class SizeMap : EntityMappingBase<Size>
    {
        public SizeMap()
            : base("Wcs")
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.SizeName)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.SizeNo)
               .IsRequired();
            this.Property(t => t.Length)
                .IsRequired();
            this.Property(t => t.Width)
                .IsRequired();
            this.Property(t => t.Height)
                .IsRequired();

            // Table & Column Mappings
            this.Property(t => t.ID).HasColumnName(ColumnMap.Value.To("ID"));
            this.Property(t => t.SizeName).HasColumnName(ColumnMap.Value.To("SizeName"));
            this.Property(t => t.SizeNo).HasColumnName(ColumnMap.Value.To("SizeNo"));
            this.Property(t => t.Length).HasColumnName(ColumnMap.Value.To("Length"));
            this.Property(t => t.Width).HasColumnName(ColumnMap.Value.To("Width"));
            this.Property(t => t.Height).HasColumnName(ColumnMap.Value.To("Height"));
        }
    }
}
