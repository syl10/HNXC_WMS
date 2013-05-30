using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THOK.Wms.DbModel
{
    public  class ATBillMaster
    {
        public ATBillMaster()
        {
            this.BillDetailes = new List<ATBillDetail>();
         }
        public string WMS_BILL_MASTER_ID { get; set; }
        public string BILL_NO { get; set; }
        public DateTime BILL_DATE { get; set; }
        public string BILL_TYPE { get; set; }
        public string BIZ_TYPE_CODE { get; set; }
        public string WAREHOUSE_CODE { get; set; }
        public string STATE { get; set; }
        public string OPERATER { get; set; }
        public DateTime OPERATE_DATE { get; set; }
        public string CHECKER { get; set; }
        public DateTime  CHECK_DATE { get; set; }
        public string TASKER { get; set; }
        public DateTime TASK_DATE { get; set; }

       public virtual ICollection<ATBillDetail> BillDetailes { get; set; }
    }
}
