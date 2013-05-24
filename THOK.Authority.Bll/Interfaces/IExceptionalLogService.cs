using THOK.Authority.DbModel;

namespace THOK.Authority.Bll.Interfaces
{
    public interface IExceptionalLogService : IService<AUTH_EXCEPTIONAL_LOG>
    {
       bool Add(string ModuleName,string FunctionName,System.Exception ex);
       object GetDetails(int page, int rows, string CatchTime, string ModuleName, string FunctionName, string ExceptionalType);
    }
}
