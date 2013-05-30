using THOK.Common.Ef.MappingStrategy;
using System.ComponentModel.DataAnnotations;

namespace THOK.Wms.DbModel.Mapping
{
    public class SRMMap : EntityMappingBase<SRM>
    {
        public SRMMap()
            : base("Wcs")
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.SRMName)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.OPCServiceName)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.GetRequest)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.GetAllow)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.GetComplete)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.PutRequest)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.PutAllow)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.PutComplete)
                .IsRequired()
                .HasMaxLength(50);
            this.Property(t => t.State)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2);

            // Table & Column Mappings
            this.Property(t => t.ID).HasColumnName(ColumnMap.Value.To("ID"));
            this.Property(t => t.SRMName).HasColumnName(ColumnMap.Value.To("SRMName"));
            this.Property(t => t.Description).HasColumnName(ColumnMap.Value.To("Description"));
            this.Property(t => t.OPCServiceName).HasColumnName(ColumnMap.Value.To("OPCServiceName"));
            this.Property(t => t.GetRequest).HasColumnName(ColumnMap.Value.To("GetRequest"));
            this.Property(t => t.GetAllow).HasColumnName(ColumnMap.Value.To("GetAllow"));
            this.Property(t => t.GetComplete).HasColumnName(ColumnMap.Value.To("GetComplete"));
            this.Property(t => t.PutRequest).HasColumnName(ColumnMap.Value.To("PutRequest"));
            this.Property(t => t.PutAllow).HasColumnName(ColumnMap.Value.To("PutAllow"));
            this.Property(t => t.PutComplete).HasColumnName(ColumnMap.Value.To("PutComplete"));
            this.Property(t => t.State).HasColumnName(ColumnMap.Value.To("State"));
        }
    }
}