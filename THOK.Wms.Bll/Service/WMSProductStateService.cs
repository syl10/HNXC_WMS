using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;

namespace THOK.Wms.Bll.Service
{
    class WMSProductStateService : ServiceBase<WMS_PRODUCT_STATE >, IWMSProductStateService
    {
        protected override Type LogPrefix
        {
            get { throw new NotImplementedException(); }
        }
        //public bool test() {
        //    OracleConnection ora = new OracleConnection(); 
        //}
    }
}
