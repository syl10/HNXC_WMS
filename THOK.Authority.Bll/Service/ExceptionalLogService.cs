using System;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Authority.Bll.Interfaces;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;
namespace THOK.Authority.Bll.Service
{
    public class ExceptionalLogService : ServiceBase<AUTH_EXCEPTIONAL_LOG>,IExceptionalLogService
    {
        [Dependency]
        public IExceptionalLogRepository ExceptionalLogRepository { get; set; }
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
        public bool Add(string ModuleName, string FunctionName, System.Exception ex)
        {
            AUTH_EXCEPTIONAL_LOG ExLog = new AUTH_EXCEPTIONAL_LOG();
            ExLog.EXCEPTIONAL_LOG_ID = ExceptionalLogRepository.GetNewID("AUTH_EXCEPTIONAL_LOG", "EXCEPTIONAL_LOG_ID");
            ExLog.MODULE_NAME = ModuleName;
            ExLog.FUNCTION_NAME = FunctionName;
            ExLog.CATCH_TIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string ExcMsg = "";
            if (ex is System.Data.Entity.Validation.DbEntityValidationException)
            {
                ExcMsg = ((System.Data.Entity.Validation.DbEntityValidationException)ex).EntityValidationErrors.First().ValidationErrors.First().ErrorMessage;
            }
            else if (ex is System.Data.Entity.Validation.DbUnexpectedValidationException)
            {
 
            }
            else if (ex is System.Data.Entity.ModelConfiguration.ModelValidationException)
            {

            }
            else
            {
                ExcMsg = ex.Message;
            }


            ExLog.EXCEPTIONAL_TYPE = ex.GetType().ToString();
            ExLog.EXCEPTIONAL_DESCRIPTION = ExcMsg;
            ExLog.STATE = "";
            ExceptionalLogRepository.Add(ExLog);
            ExceptionalLogRepository.SaveChanges();
           
            return true;
        }

        public object GetDetails(int page, int rows, string CatchTime, string ModuleName, string FunctionName, string ExceptionalType)
        {
            IQueryable<AUTH_EXCEPTIONAL_LOG> LOGINLOGQuery = ExceptionalLogRepository.GetQueryable();
            var HelpContent = LOGINLOGQuery.Where(c => c.CATCH_TIME.Contains(CatchTime) &&
                                                          c.MODULE_NAME.Contains(ModuleName) &&
                                                          c.FUNCTION_NAME.Contains(FunctionName) &&
                                                          c.EXCEPTIONAL_TYPE.Contains(ExceptionalType));

            HelpContent = HelpContent.OrderBy(h => h.EXCEPTIONAL_LOG_ID);
            if (THOK.Common.PrintHandle.isbase)
            {
                THOK.Common.PrintHandle.baseinfoprint = THOK.Common.ConvertData.LinqQueryToDataTable(HelpContent);
            }
            int total = HelpContent.Count();
            HelpContent = HelpContent.Skip((page - 1) * rows).Take(rows);

            var temp = HelpContent.ToArray().Select(c => new
            {
                ID = c.EXCEPTIONAL_LOG_ID,
                CATCH_TIME = c.CATCH_TIME,
                MODULE_NAME = c.MODULE_NAME,
                FUNCTION_NAME = c.FUNCTION_NAME,
                EXCEPTIONAL_TYPE = c.EXCEPTIONAL_TYPE,
                EXCEPTIONAL_DESCRIPTION = c.EXCEPTIONAL_DESCRIPTION,
               STATE= c.STATE
            });
            return new { total, rows = temp.ToArray() };
        }
    }
}
