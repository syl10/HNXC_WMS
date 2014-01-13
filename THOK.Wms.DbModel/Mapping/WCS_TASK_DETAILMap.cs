using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations;

namespace THOK.Wms.DbModel.Mapping
{
    public class WCS_TASK_DETAILMap : EntityTypeConfiguration<WCS_TASK_DETAIL>
    {
        public WCS_TASK_DETAILMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TASK_ID, t.ITEM_NO });

            // Properties
            this.Property(t => t.TASK_ID)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.ITEM_NO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TASK_NO)
                .IsFixedLength()
                .HasMaxLength(4);

            this.Property(t => t.ASSIGNMENT_ID)
                .IsFixedLength()
                .HasMaxLength(8);

            this.Property(t => t.CRANE_NO)
                .IsFixedLength()
                .HasMaxLength(2);

            this.Property(t => t.CAR_NO)
                .IsFixedLength()
                .HasMaxLength(2);

            this.Property(t => t.FROM_STATION)
                .HasMaxLength(12);

            this.Property(t => t.TO_STATION)
                .HasMaxLength(12);

            this.Property(t => t.STATE)
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.SERVICE_NAME)
                .HasMaxLength(50);

            this.Property(t => t.ITEM_NAME)
                .HasMaxLength(50);

            this.Property(t => t.ITEM_VALUE)
                .HasMaxLength(100);

            this.Property(t => t.DESCRIPTION)
                .HasMaxLength(200);

            this.Property(t => t.BILL_NO)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.SQUENCE_NO)
                .HasMaxLength(12);

            this.Property(t => t.ERR_CODE)
                .IsFixedLength()
                .HasMaxLength(3);

            // Table & Column Mappings
            this.ToTable("WCS_TASK_DETAIL", "HNXC");
            this.Property(t => t.TASK_ID).HasColumnName("TASK_ID");
            this.Property(t => t.ITEM_NO).HasColumnName("ITEM_NO");
            this.Property(t => t.TASK_NO).HasColumnName("TASK_NO");
            this.Property(t => t.ASSIGNMENT_ID).HasColumnName("ASSIGNMENT_ID");
            this.Property(t => t.CRANE_NO).HasColumnName("CRANE_NO");
            this.Property(t => t.CAR_NO).HasColumnName("CAR_NO");
            this.Property(t => t.FROM_STATION).HasColumnName("FROM_STATION");
            this.Property(t => t.TO_STATION).HasColumnName("TO_STATION");
            this.Property(t => t.STATE).HasColumnName("STATE");
            this.Property(t => t.SERVICE_NAME).HasColumnName("SERVICE_NAME");
            this.Property(t => t.ITEM_NAME).HasColumnName("ITEM_NAME");
            this.Property(t => t.ITEM_VALUE).HasColumnName("ITEM_VALUE");
            this.Property(t => t.DESCRIPTION).HasColumnName("DESCRIPTION");
            this.Property(t => t.BILL_NO).HasColumnName("BILL_NO");
            this.Property(t => t.SQUENCE_NO).HasColumnName("SQUENCE_NO");
            this.Property(t => t.ERR_CODE).HasColumnName("ERR_CODE");
        }
    }
}
