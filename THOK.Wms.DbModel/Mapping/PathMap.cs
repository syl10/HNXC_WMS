using THOK.Common.Ef.MappingStrategy;
using System.ComponentModel.DataAnnotations;

namespace THOK.Wms.DbModel.Mapping
{
    public class PathMap : EntityMappingBase<Path>
    {
        public PathMap()
            : base("Wcs")
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.PathName)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.State)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2);
           

            // Table & Column Mappings
            this.Property(t => t.ID).HasColumnName(ColumnMap.Value.To("ID"));
            this.Property(t => t.PathName).HasColumnName(ColumnMap.Value.To("PathName"));
          
            this.Property(t => t.OriginRegionID).HasColumnName(ColumnMap.Value.To("OriginRegionID"));
            this.Property(t => t.TargetRegionID).HasColumnName(ColumnMap.Value.To("TargetRegionID"));
            this.Property(t => t.Description).HasColumnName(ColumnMap.Value.To("Description"));
            this.Property(t => t.State).HasColumnName(ColumnMap.Value.To("State"));

            // Relationships
            this.HasRequired(t => t.OriginRegion)
                .WithMany(t => t.OriginRegionPath)
                .HasForeignKey(d => d.OriginRegionID)
                .WillCascadeOnDelete(false);
            this.HasRequired(t => t.TargetRegion)
                .WithMany(t => t.TargetRegionPath)
                .HasForeignKey(d => d.TargetRegionID)
                .WillCascadeOnDelete(false);
        }
    }
}
