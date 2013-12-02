using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;
using THOK.Wms.Bll.Interfaces;
using Microsoft.Practices.Unity;
using THOK.Wms.Dal.Interfaces;

namespace THOK.Wms.Bll.Service
{
    class CMDProductGradeService : ServiceBase<CMD_PRODUCT_GRADE >, ICMDProductGradeService
    {
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        [Dependency]
        public ICMDProductGradeRepository ProductGradeRepository { get; set; }
        public object Detail(int page, int rows, string ENGLISH_CODE, string USER_CODE, string GRADE_NAME, string MEMO)
        {
            IQueryable <CMD_PRODUCT_GRADE> query = ProductGradeRepository.GetQueryable();
            var temp = query.OrderBy(i => i.GRADE_CODE).Select(i => new {i.GRADE_CODE ,i.ENGLISH_CODE ,i.USER_CODE ,i.GRADE_NAME ,i.MEMO });
            if (!string.IsNullOrEmpty(ENGLISH_CODE))
            {
                temp = temp.Where(i => i.ENGLISH_CODE.Contains(ENGLISH_CODE));
            }
            if (!string.IsNullOrEmpty(USER_CODE))
            {
                temp = temp.Where(i => i.USER_CODE.Contains(USER_CODE));
            }
            if (!string.IsNullOrEmpty(GRADE_NAME))
            {
                temp = temp.Where(i => i.GRADE_NAME.Contains(GRADE_NAME));
            }
            if (!string.IsNullOrEmpty(MEMO))
            {
                temp = temp.Where(i => i.MEMO.Contains(MEMO));
            }
            THOK.Common.PrintHandle.baseinfoprint = THOK.Common.ConvertData.LinqQueryToDataTable(temp);
            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };

        }


        //新增
        public  bool Add(CMD_PRODUCT_GRADE grade)
        {
            try
            {
                grade.GRADE_CODE = ProductGradeRepository.GetNewID("CMD_PRODUCT_GRADE", "GRADE_CODE");
                ProductGradeRepository.Add(grade);
                ProductGradeRepository.SaveChanges();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //编辑
        public bool Edit(CMD_PRODUCT_GRADE grade, string GRADE_CODE)
        {
            try
            {
                var temp = ProductGradeRepository.GetQueryable().FirstOrDefault(i => i.GRADE_CODE == GRADE_CODE);
                temp.ENGLISH_CODE = grade.ENGLISH_CODE;
                temp.USER_CODE = grade.USER_CODE;
                temp.GRADE_NAME = grade.GRADE_NAME;
                temp.MEMO = grade.MEMO;
                ProductGradeRepository.SaveChanges();
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        }
        //删除
        public bool Delete(string GRADE_CODE)
        {
            try
            {
                var temp = ProductGradeRepository.GetQueryable().FirstOrDefault(i => i.GRADE_CODE == GRADE_CODE);
                ProductGradeRepository.Delete(temp);
                ProductGradeRepository.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
