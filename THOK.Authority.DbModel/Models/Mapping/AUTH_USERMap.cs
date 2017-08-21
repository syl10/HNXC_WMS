using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace THOK.Authority.DbModel.Mapping
{
    public class AUTH_USERMap : EntityTypeConfiguration<AUTH_USER>
    {
        public AUTH_USERMap()
        {
            // Primary Key
            this.HasKey(t => t.USER_ID);

            // Properties
            this.Property(t => t.USER_ID)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            this.Property(t => t.USER_NAME)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.PWD)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CHINESE_NAME)
                .HasMaxLength(50);

            this.Property(t => t.IS_LOCK)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.IS_ADMIN)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(1);

            this.Property(t => t.LOGIN_PC)
                .HasMaxLength(50);

            this.Property(t => t.MEMO)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("AUTH_USER","HNXC");
            this.Property(t => t.USER_ID).HasColumnName("USER_ID");
            this.Property(t => t.USER_NAME).HasColumnName("USER_NAME");
            this.Property(t => t.PWD).HasColumnName("PWD");
            this.Property(t => t.CHINESE_NAME).HasColumnName("CHINESE_NAME");
            this.Property(t => t.IS_LOCK).HasColumnName("IS_LOCK");
            this.Property(t => t.IS_ADMIN).HasColumnName("IS_ADMIN");
            this.Property(t => t.LOGIN_PC).HasColumnName("LOGIN_PC");
            this.Property(t => t.MEMO).HasColumnName("MEMO");
        }
    }
}
