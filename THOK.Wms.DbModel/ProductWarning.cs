using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
    public class ProductWarning
    {
        public ProductWarning()
        {
        }

        public string ProductCode { get; set; }
        public string UnitCode { get; set; }
        public decimal ?MinLimited { get; set; }
        public decimal ?MaxLimited { get; set; }
        public decimal ?AssemblyTime { get;set;}
        public string Memo { get; set; }

        public virtual Unit Unit { get; set; }

    }
}
