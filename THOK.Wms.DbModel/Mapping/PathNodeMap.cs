using THOK.Common.Ef.MappingStrategy;
using System.ComponentModel.DataAnnotations;

namespace THOK.Wms.DbModel.Mapping
{
    public class PathNodeMap : EntityMappingBase<PathNode>
    {
        public PathNodeMap()
            : base("Wcs")
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.PathNodeOrder)
                .IsRequired();

            // Table & Column Mappings
            this.Property(t => t.ID).HasColumnName(ColumnMap.Value.To("ID"));
            this.Property(t => t.PathID).HasColumnName(ColumnMap.Value.To("PathID"));
            this.Property(t => t.PositionID).HasColumnName(ColumnMap.Value.To("PositionID"));
            this.Property(t => t.PathNodeOrder).HasColumnName(ColumnMap.Value.To("PathNodeOrder"));

            // Relationships
            this.HasRequired(t => t.Path)
                .WithMany(t => t.PathNodes)
                .HasForeignKey(d => d.PathID)
                .WillCascadeOnDelete(false);
            this.HasRequired(t => t.Position)
                .WithMany(t => t.PathNodes)
                .HasForeignKey(d => d.PositionID)
                .WillCascadeOnDelete(false);
        }
    }
}
