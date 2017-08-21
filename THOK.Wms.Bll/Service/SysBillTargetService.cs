
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    public class SysBillTargetService:ServiceBase<SYS_BILL_TARGET>,ISysBillTargetService
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public ISysBillTargetRepository SysBillTargetRepository { get; set; }

        public object GetDetails()
        {
            IQueryable<SYS_BILL_TARGET> query = SysBillTargetRepository.GetQueryable();

            var BillTargets = query.OrderBy(i => i.TARGET_CODE)
                                .Select(i => new { id = i.TARGET_CODE, text = i.TARGET_NAME });
            return BillTargets.ToArray();
        }
    }
}
