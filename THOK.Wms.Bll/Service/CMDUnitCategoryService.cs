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
    public class CMDUnitCategoryService:ServiceBase<CMD_UNIT_CATEGORY>,ICMDUnitCategoryService
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public ICMDUnitCategoryRepository UnitCategoryRepository { get; set; }

        public object GetDetails(int page, int rows, string CATEGORY_NAME, string MEMO)
        {
            IQueryable<CMD_UNIT_CATEGORY> query = UnitCategoryRepository.GetQueryable();
            var UnitCategorys = query.OrderBy(i => i.CATEGORY_CODE).Select(i => new { i.CATEGORY_CODE, i.CATEGORY_NAME, i.MEMO });
            if (!string.IsNullOrEmpty(CATEGORY_NAME))
            {
                UnitCategorys = UnitCategorys.Where(i => i.CATEGORY_NAME.Contains(CATEGORY_NAME));
            }
            if (!string.IsNullOrEmpty(MEMO))
            {
                UnitCategorys = UnitCategorys.Where(i => i.MEMO.Contains(MEMO));
            }
            if (THOK.Common.PrintHandle.isbase)
            {
                THOK.Common.PrintHandle.baseinfoprint = THOK.Common.ConvertData.LinqQueryToDataTable(UnitCategorys);
            }
            int total = UnitCategorys.Count();
            UnitCategorys = UnitCategorys.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = UnitCategorys.ToArray() };
          
        }
        public bool Add(CMD_UNIT_CATEGORY UnitCategory)
        {
            UnitCategory.CATEGORY_CODE = UnitCategoryRepository.GetNewID("CMD_UNIT_CATEGORY", "CATEGORY_CODE");
            UnitCategoryRepository.Add(UnitCategory);
            UnitCategoryRepository.SaveChanges();
            return true;
        }

        public bool Delete(string Category_CODE)
        {
            var UnitCategory = UnitCategoryRepository.GetQueryable()
             .FirstOrDefault(i => i.CATEGORY_CODE == Category_CODE);
            UnitCategoryRepository.Delete(UnitCategory);
            int rejust=UnitCategoryRepository.SaveChanges();
            if (rejust == -1) return false;
            else return true;
            
        }

        public bool Save(CMD_UNIT_CATEGORY UnitCategory)
        {
            var FindUnitCategory = UnitCategoryRepository.GetQueryable()
              .FirstOrDefault(i => i.CATEGORY_CODE == UnitCategory.CATEGORY_CODE);
            FindUnitCategory.CATEGORY_NAME = UnitCategory.CATEGORY_NAME;
            FindUnitCategory.MEMO = UnitCategory.MEMO;
            UnitCategoryRepository.SaveChanges();
            return true;
            
        }

        
    }
}
