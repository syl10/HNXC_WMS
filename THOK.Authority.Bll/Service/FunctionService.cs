using System;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Authority.Bll.Interfaces;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;

namespace THOK.Authority.Bll.Service
{
    public class FunctionService :ServiceBase<AUTH_FUNCTION>, IFunctionService
    {
        [Dependency]
        public IFunctionRepository FunctionRepository { get; set; }
        [Dependency]
        public IModuleRepository ModuleRepository { get; set; }

        #region IFunctionService 成员

        public bool Save(string FunctionId, string FunctionName, string ControlName, string IndicateImage)
        {
            try
            {
                //Guid fid = new Guid(FunctionId);
                var function = FunctionRepository.GetQueryable().FirstOrDefault(m => m.FUNCTION_ID == FunctionId);
                function.FUNCTION_NAME = FunctionName;
                function.CONTROL_NAME = ControlName;
                function.INDICATE_IMAGE = IndicateImage;
                FunctionRepository.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool Delete(string FunctionId)
        {
            try
            {
                //Guid fid = new Guid(FunctionId);
                string fid = FunctionId;
                var function = FunctionRepository.GetQueryable().FirstOrDefault(f => f.FUNCTION_ID == fid);
                if (function != null)
                {
                    FunctionRepository.Delete(function);
                    FunctionRepository.SaveChanges();
                }
                else
                    return false;
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool Add(string ModuleId, string FunctionName, string ControlName, string IndicateImage)
        {
            try
            {
                //var module = ModuleRepository.GetQueryable().FirstOrDefault(m => m.ModuleID == new Guid(ModuleId));
                var module = ModuleRepository.GetQueryable().FirstOrDefault(m => m.MODULE_ID == ModuleId);
                var function = new AUTH_FUNCTION();
                function.FUNCTION_ID = Guid.NewGuid().ToString();
                function.FUNCTION_NAME = FunctionName;
                function.CONTROL_NAME = ControlName;
                function.AUTH_MODULE = module;
                function.INDICATE_IMAGE = IndicateImage;
                FunctionRepository.Add(function);
                FunctionRepository.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region IFunctionService 成员

        public object GetDetails(string ModuleId)
        {
            //Guid mid = new Guid(ModuleId);
            string mid = ModuleId;
            var function = FunctionRepository.GetQueryable().Where(f => f.AUTH_MODULE.MODULE_ID == mid).Select(f => new
            {
                f.FUNCTION_ID,
                f.FUNCTION_NAME,
                f.CONTROL_NAME,
                f.INDICATE_IMAGE
            });
            return function.ToArray();
        }

        #endregion
        
        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }
    }
}
