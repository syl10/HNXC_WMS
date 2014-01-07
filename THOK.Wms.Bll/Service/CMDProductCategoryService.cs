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
    public class CMDProductCategoryService : ServiceBase<CMD_PRODUCT_CATEGORY>, ICMDProductCategoryService
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public ICMDProductCategoryRepository ProductCategoryRepository { get; set; }

        public object GetDetails(int page, int rows, string CATEGORY_NAME, string MEMO)
        {
            IQueryable<CMD_PRODUCT_CATEGORY> query = ProductCategoryRepository.GetQueryable();
            var Categorys = query.OrderBy(i => i.CATEGORY_CODE).Select(i => new { i.CATEGORY_CODE, i.CATEGORY_NAME, i.MEMO });
            if (!string.IsNullOrEmpty(CATEGORY_NAME))
            {
                Categorys = Categorys.Where(i => i.CATEGORY_NAME.Contains(CATEGORY_NAME));
            }
            if (!string.IsNullOrEmpty(MEMO))
            {
                Categorys = Categorys.Where(i => i.MEMO.Contains(MEMO));
            }
            if (THOK.Common.PrintHandle.isbase)
            {
                THOK.Common.PrintHandle.baseinfoprint = THOK.Common.ConvertData.LinqQueryToDataTable(Categorys);
            }
            int total = Categorys.Count();
            Categorys = Categorys.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = Categorys.ToArray() };
        }

        public bool Add(string CATEGORY_NAME, string MEMO)
        {
            var city = new THOK.Wms.DbModel.CMD_PRODUCT_CATEGORY()
            {
                CATEGORY_CODE = ProductCategoryRepository.GetNewID("CMD_PRODUCT_CATEGORY", "CATEGORY_CODE"),
                CATEGORY_NAME = CATEGORY_NAME,
                MEMO = MEMO
            };
            ProductCategoryRepository.Add(city);
            ProductCategoryRepository.SaveChanges();
            return true;
        }

        public bool Delete(string CATEGORY_CODE)
        {
            var Cigarette = ProductCategoryRepository.GetQueryable()
            .FirstOrDefault(i => i.CATEGORY_CODE == CATEGORY_CODE);
            ProductCategoryRepository.Delete(Cigarette);
            int rejust= ProductCategoryRepository.SaveChanges();
            if (rejust == -1) return false;
            else return true;

        }

        public bool Save(string CATEGORY_CODE, string CATEGORY_NAME, string MEMO)
        {
            var Cigarette = ProductCategoryRepository.GetQueryable()
             .FirstOrDefault(i => i.CATEGORY_CODE == CATEGORY_CODE);
            Cigarette.CATEGORY_NAME = CATEGORY_NAME;
            Cigarette.MEMO = MEMO;
            ProductCategoryRepository.SaveChanges();
            return true;
        }
    }
}
