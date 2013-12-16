using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.Bll.Interfaces;
using THOK.Wms.DbModel;
using THOK.Wms.Dal.Interfaces;
using Microsoft.Practices.Unity;

namespace THOK.Wms.Bll.Service
{
    public class CMDProductStyleService : ServiceBase<CMD_PRODUCT_STYLE>, ICMDPorductStyleService
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public ICMDProductStyleRepository   Productstylerepository { get; set; }
        public object GetDetails(int page, int rows)
        {
            IQueryable<CMD_PRODUCT_STYLE > query = Productstylerepository.GetQueryable();
            var Styles = query.OrderBy(i => i.STYLE_NO).Select(i => new { i.STYLE_NO ,i.STYLE_NAME});
            if (THOK.Common.PrintHandle.isbase)
            {
                THOK.Common.PrintHandle.baseinfoprint = THOK.Common.ConvertData.LinqQueryToDataTable(Styles);
            }
            int total = Styles.Count();
            Styles = Styles.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = Styles.ToArray() };
        }

        public bool Add(string STYLE_NAME, string SORT_LEVEL)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string STYLE_CODE)
        {
            throw new NotImplementedException();
        }

        public bool Save(string STYLE_CODE, string STYLE_NAME, string SORT_LEVEL)
        {
            throw new NotImplementedException();
        }
    }
}
