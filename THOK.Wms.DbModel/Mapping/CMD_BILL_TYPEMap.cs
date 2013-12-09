using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Wms.DbModel.Mapping
{
    public class CMD_BILL_TYPEMap : EntityTypeConfiguration<CMD_BILL_TYPE>
    {
        public CMD_BILL_TYPEMap()
        {
            // Primary Key
            this.HasKey(t => t.BTYPE_CODE);

            // Properties
            this.Property(t => t.BTYPE_CODE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.BTYPE_NAME)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.BILL_TYPE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.TASK_LEVEL)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.ALLOW_EDIT)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.MEMO)
                .HasMaxLength(100);

            this.Property(t => t.TARGET_CODE)
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.TASK_TYPE)
                .IsFixedLength()
                .HasMaxLength(2);

            // Table & Column Mappings
            this.ToTable("CMD_BILL_TYPE","HNXC");
            this.Property(t => t.BTYPE_CODE).HasColumnName("BTYPE_CODE");
            this.Property(t => t.BTYPE_NAME).HasColumnName("BTYPE_NAME");
            this.Property(t => t.BILL_TYPE).HasColumnName("BILL_TYPE");
            this.Property(t => t.TASK_LEVEL).HasColumnName("TASK_LEVEL");
            this.Property(t => t.ALLOW_EDIT).HasColumnName("ALLOW_EDIT");
            this.Property(t => t.MEMO).HasColumnName("MEMO");
            this.Property(t => t.TARGET_CODE).HasColumnName("TARGET_CODE");
            this.Property(t => t.TASK_TYPE).HasColumnName("TASK_TYPE");
        }
    }
}
