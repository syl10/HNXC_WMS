using THOK.Common.Ef.MappingStrategy;
using System.ComponentModel.DataAnnotations;

namespace THOK.Wms.DbModel.Mapping
{
    public class TaskMap : EntityMappingBase<Task>
    {
        public TaskMap()
            : base("Wcs")
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.TaskType)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2);
            this.Property(t => t.TaskLevel)
                .IsRequired();
            this.Property(t => t.ProductCode)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.ProductName)
                .IsRequired()
                .HasMaxLength(20);
            this.Property(t => t.OriginStorageCode)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.TargetStorageCode)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.OriginPositionID)
                .IsRequired();
            this.Property(t => t.TargetPositionID)
                .IsRequired();
            this.Property(t => t.CurrentPositionID)
                .IsRequired();
            this.Property(t => t.CurrentPositionState)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2);
            this.Property(t => t.State)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2);
            this.Property(t => t.TagState)
               .IsRequired()
               .IsFixedLength()
               .HasMaxLength(2);
            this.Property(t => t.Quantity)
               .IsRequired();
            this.Property(t => t.TaskQuantity)
               .IsRequired();
            this.Property(t => t.OperateQuantity)
               .IsRequired();
            this.Property(t => t.OrderID)
               .HasMaxLength(20);
            this.Property(t => t.OrderType)
              .IsFixedLength()
              .HasMaxLength(2);
            this.Property(t => t.DownloadState)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2);
            
            // Table & Column Mappings
            this.Property(t => t.ID).HasColumnName(ColumnMap.Value.To("ID"));
            this.Property(t => t.TaskType).HasColumnName(ColumnMap.Value.To("TaskType"));
            this.Property(t => t.TaskLevel).HasColumnName(ColumnMap.Value.To("TaskLevel"));
            this.Property(t => t.PathID).HasColumnName(ColumnMap.Value.To("PathID"));
            this.Property(t => t.ProductCode).HasColumnName(ColumnMap.Value.To("ProductCode"));
            this.Property(t => t.ProductName).HasColumnName(ColumnMap.Value.To("ProductName"));
            this.Property(t => t.OriginStorageCode).HasColumnName(ColumnMap.Value.To("OriginStorageCode"));
            this.Property(t => t.TargetStorageCode).HasColumnName(ColumnMap.Value.To("TargetStorageCode"));
            this.Property(t => t.OriginPositionID).HasColumnName(ColumnMap.Value.To("OriginPositionID"));
            this.Property(t => t.TargetPositionID).HasColumnName(ColumnMap.Value.To("TargetPositionID"));
            this.Property(t => t.CurrentPositionID).HasColumnName(ColumnMap.Value.To("CurrentPositionID"));
            this.Property(t => t.CurrentPositionState).HasColumnName(ColumnMap.Value.To("CurrentPositionState"));
            this.Property(t => t.State).HasColumnName(ColumnMap.Value.To("State"));
            this.Property(t => t.TagState).HasColumnName(ColumnMap.Value.To("TagState"));
            this.Property(t => t.Quantity).HasColumnName(ColumnMap.Value.To("Quantity"));
            this.Property(t => t.TaskQuantity).HasColumnName(ColumnMap.Value.To("TaskQuantity"));
            this.Property(t => t.OperateQuantity).HasColumnName(ColumnMap.Value.To("OperateQuantity"));
            this.Property(t => t.OrderID).HasColumnName(ColumnMap.Value.To("OrderID"));
            this.Property(t => t.OrderType).HasColumnName(ColumnMap.Value.To("OrderType"));
            this.Property(t => t.AllotID).HasColumnName(ColumnMap.Value.To("AllotID"));
            this.Property(t => t.DownloadState).HasColumnName(ColumnMap.Value.To("DownloadState"));
        }
    }
}
