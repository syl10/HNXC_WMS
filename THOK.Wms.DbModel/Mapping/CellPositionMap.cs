using THOK.Common.Ef.MappingStrategy;
using System.ComponentModel.DataAnnotations;

namespace THOK.Wms.DbModel.Mapping
{
    public class CellPositionMap : EntityMappingBase<CellPosition>
    {
        public CellPositionMap()
            : base("Wcs")
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.CellCode)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.Property(t => t.ID).HasColumnName(ColumnMap.Value.To("ID"));
            this.Property(t => t.CellCode).HasColumnName(ColumnMap.Value.To("CellCode"));
            this.Property(t => t.StockInPositionID).HasColumnName(ColumnMap.Value.To("StockInPositionID"));
            this.Property(t => t.StockOutPositionID).HasColumnName(ColumnMap.Value.To("StockOutPositionID"));

            // Relationships
            this.HasRequired(t => t.StockInPosition)
                .WithMany(t => t.StockInCellPosition)
                .HasForeignKey(d => d.StockInPositionID)
                .WillCascadeOnDelete(false);
            this.HasRequired(t => t.StockOutPosition)
                .WithMany(t => t.StockOutCellPosition)
                .HasForeignKey(d => d.StockOutPositionID)
                .WillCascadeOnDelete(false);
        }
    }
}
