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
    class CMDProductOriginalService : ServiceBase<CMD_PRODUCT_ORIGINAL >, ICMDProductOriginalService
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public ICMDProductOriginalRepository ProductOriginalRepository { get; set; }
        public object Detail(int page, int rows, string ORIGINAL_NAME, string DISTRICT_CODE, string MEMO)
        {
            IQueryable<CMD_PRODUCT_ORIGINAL> query = ProductOriginalRepository.GetQueryable();
            var temp = query.OrderBy(i => i.ORIGINAL_CODE).Select(i => new {i.ORIGINAL_CODE ,i.ORIGINAL_NAME ,i.DISTRICT_CODE ,i.MEMO  });
            if (!string.IsNullOrEmpty(ORIGINAL_NAME))
            {
                temp = temp.Where (i => i.ORIGINAL_NAME.Contains(ORIGINAL_NAME));
            }
            if (!string.IsNullOrEmpty(DISTRICT_CODE))
            {
                temp = temp.Where(i => i.DISTRICT_CODE.Contains(DISTRICT_CODE));
            }
            if (!string.IsNullOrEmpty(MEMO)) {
                temp = temp.Where(i => i.MEMO.Contains(MEMO));
            }
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }

        //新增
        public new bool Add(CMD_PRODUCT_ORIGINAL original)
        {
            try
            {
                original.ORIGINAL_CODE = ProductOriginalRepository.GetNewID("CMD_PRODUCT_ORIGINAL", "ORIGINAL_CODE");
                ProductOriginalRepository.Add(original);
                ProductOriginalRepository.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //编辑
        public bool Edit(CMD_PRODUCT_ORIGINAL original, string ORIGINAL_CODE)
        {
            try
            {
                var temp = ProductOriginalRepository.GetQueryable().Where(i => i.ORIGINAL_CODE == ORIGINAL_CODE).FirstOrDefault();
                temp.ORIGINAL_NAME = original.ORIGINAL_NAME;
                temp.DISTRICT_CODE = original.DISTRICT_CODE;
                temp.MEMO = original.MEMO;
                ProductOriginalRepository.SaveChanges();
                return true;
            }
            catch (Exception ex) { return false; }
        }

        //删除
        public bool Delete(string originalcode)
        {
            try
            {
                var temp = ProductOriginalRepository.GetQueryable().FirstOrDefault(i => i.ORIGINAL_CODE == originalcode);
                ProductOriginalRepository.Delete(temp);
                ProductOriginalRepository.SaveChanges();
                return true;
            }
            catch (Exception ex) { return false; }
        }
    }
}
