using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Authority.DbModel.Mapping
{
    public class AUTH_HELP_CONTENTMap : EntityTypeConfiguration<AUTH_HELP_CONTENT>
    {
        public AUTH_HELP_CONTENTMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.CONTENT_CODE)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.CONTENT_NAME)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CONTENT_TEXT)
                .HasMaxLength(4000);

            this.Property(t => t.CONTENT_PATH)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.NODE_TYPE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.FATHER_NODE_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(3);

            this.Property(t => t.MODULE_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            this.Property(t => t.IS_ACTIVE)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            // Table & Column Mappings
            this.ToTable("AUTH_HELP_CONTENT","HNXC");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CONTENT_CODE).HasColumnName("CONTENT_CODE");
            this.Property(t => t.CONTENT_NAME).HasColumnName("CONTENT_NAME");
            this.Property(t => t.CONTENT_TEXT).HasColumnName("CONTENT_TEXT");
            this.Property(t => t.CONTENT_PATH).HasColumnName("CONTENT_PATH");
            this.Property(t => t.NODE_TYPE).HasColumnName("NODE_TYPE");
            this.Property(t => t.FATHER_NODE_ID).HasColumnName("FATHER_NODE_ID");
            this.Property(t => t.MODULE_ID).HasColumnName("MODULE_ID");
            this.Property(t => t.NODE_ORDER).HasColumnName("NODE_ORDER");
            this.Property(t => t.IS_ACTIVE).HasColumnName("IS_ACTIVE");
            this.Property(t => t.UPDATE_TIME).HasColumnName("UPDATE_TIME");

            // Relationships
            this.HasRequired(t => t.FATHER_NODE)
                .WithMany(t => t.AUTH_HELP_CONTENTS)
                .HasForeignKey(d => d.FATHER_NODE_ID);
            this.HasRequired(t => t.AUTH_MODULE)
                .WithMany(t => t.AUTH_HELP_CONTENT)
                .HasForeignKey(d => d.MODULE_ID);

        }
    }
}
