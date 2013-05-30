using THOK.Common.Ef.MappingStrategy;
using System.ComponentModel.DataAnnotations;

namespace THOK.Wms.DbModel.Mapping
{
    public class PositionMap : EntityMappingBase<Position>
    {
        public PositionMap()
            : base("Wcs")
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.PositionName)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.PositionType)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2);
            this.Property(t => t.SRMName)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.TravelPos)
                .IsRequired();
            this.Property(t => t.LiftPos)
                .IsRequired();
            this.Property(t => t.Extension)
                .IsRequired();
            this.Property(t => t.HasGoods)
                .IsRequired();
            this.Property(t => t.AbleStockOut)
                .IsRequired();
            this.Property(t => t.AbleStockInPallet)
                .IsRequired();
            this.Property(t => t.TagAddress)
                .IsRequired();
            this.Property(t => t.ChannelCode)
                .HasMaxLength(50);
            this.Property(t => t.State)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2);

            // Table & Column Mappings
            this.Property(t => t.ID).HasColumnName(ColumnMap.Value.To("ID"));
            this.Property(t => t.PositionName).HasColumnName(ColumnMap.Value.To("PositionName"));
            this.Property(t => t.PositionType).HasColumnName(ColumnMap.Value.To("PositionType"));
            this.Property(t => t.RegionID).HasColumnName(ColumnMap.Value.To("RegionID"));
            this.Property(t => t.SRMName).HasColumnName(ColumnMap.Value.To("SRMName"));
            this.Property(t => t.TravelPos).HasColumnName(ColumnMap.Value.To("TravelPos"));
            this.Property(t => t.LiftPos).HasColumnName(ColumnMap.Value.To("LiftPos"));
            this.Property(t => t.Extension).HasColumnName(ColumnMap.Value.To("Extension"));
            this.Property(t => t.Description).HasColumnName(ColumnMap.Value.To("Description"));
            this.Property(t => t.HasGoods).HasColumnName(ColumnMap.Value.To("HasGoods"));
            this.Property(t => t.AbleStockOut).HasColumnName(ColumnMap.Value.To("AbleStockOut"));
            this.Property(t => t.AbleStockInPallet).HasColumnName(ColumnMap.Value.To("AbleStockInPallet"));
            this.Property(t => t.TagAddress).HasColumnName(ColumnMap.Value.To("TagAddress"));
            this.Property(t => t.CurrentTaskID).HasColumnName(ColumnMap.Value.To("CurrentTaskID"));
            this.Property(t => t.CurrentOperateQuantity).HasColumnName(ColumnMap.Value.To("CurrentOperateQuantity"));
            this.Property(t => t.ChannelCode).HasColumnName(ColumnMap.Value.To("ChannelCode"));
            this.Property(t => t.State).HasColumnName(ColumnMap.Value.To("State"));

            // Relationships
            this.HasRequired(t => t.Region)
                .WithMany(t => t.Positions)
                .HasForeignKey(d => d.RegionID)
                .WillCascadeOnDelete(false);
        }
    }
}
