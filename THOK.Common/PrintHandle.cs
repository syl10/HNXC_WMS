using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace THOK.Common
{
    public   class PrintHandle
    {
        public static DataTable dt;
        public  delegate void StockinprintEventHandler( DataTable dt,string soursename);
        public static  event StockinprintEventHandler Stockinprint;
        public static  void Onstockinprint(DataTable dt, string soursename)
        {
            if (Stockinprint != null) {
                Stockinprint(dt,soursename);
            }
        }
    }
}
