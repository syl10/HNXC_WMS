using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Common.Ef.MappingStrategy;

namespace THOK.Wms.DbModel.Mapping
{
    public class TrayInfoMap : EntityMappingBase<TrayInfo>
    {
        public TrayInfoMap()
            : base("Wms")
        {
            // Primary Key
            this.HasKey(t => t.TaryID);

            this.Property(t => t.TaryRfid)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.ProductCode)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Quantity)
                .IsRequired()
                .HasPrecision(18, 0);
        }
    }
}
