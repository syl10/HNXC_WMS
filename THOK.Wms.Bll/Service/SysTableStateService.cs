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
    public class SysTableStateService:ServiceBase<SYS_TABLE_STATE>,ISysTableStateService
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public ISysTableStateRepository SysTableStateRepository { get; set; }

        public object GetDetails(string TABLE_NAME, string FIELD_NAME)
        {
            IQueryable<SYS_TABLE_STATE> query = SysTableStateRepository.GetQueryable();

            var CMDCigarettes = query.OrderBy(i => i.STATE)
                    .Where(i => i.TABLE_NAME == TABLE_NAME && i.FIELD_NAME == FIELD_NAME).Select(i => new { id = i.STATE, text = i.STATE_DESC });
            return  CMDCigarettes.ToArray();
        }

        public string GetDescByState(string TABLE_NAME, string FIELD_NAME, string State)
        {
            IQueryable<SYS_TABLE_STATE> query = SysTableStateRepository.GetQueryable();

            var CMDCigarettes = query.Where(i => i.TABLE_NAME == TABLE_NAME && i.FIELD_NAME == FIELD_NAME && i.STATE == State);
            return CMDCigarettes.First().STATE_DESC;
            
        }
    }
}
