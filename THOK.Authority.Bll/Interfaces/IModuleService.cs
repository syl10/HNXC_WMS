using THOK.Authority.DbModel;

namespace THOK.Authority.Bll.Interfaces
{
    public interface IModuleService : IService<AUTH_MODULE>
    {
        object GetDetails();//测试数据

        object GetDetails(string p);

        bool Add(string moduleName, int showOrder, string moduleUrl, string indicateImage, string desktopImage, string systemId, string p);

        bool Delete(string moduleId);

        bool Save(string moduleID, string moduleName, int showOrder, string moduleUrl, string indicateImage, string deskTopImage);

        object GetUserMenus(string userName,string cityID,string systemID);

        object GetModuleFuns(string userName, string cityID, string moduleID);

        bool InitUserSystemInfo(string userID, string cityID, string systemID);

        void InitRoleSys(string roleID,string cityID,string systemID);

        object GetRoleSystemDetails(string roleID,string cityID,string systemID);

        bool ProcessRolePermissionStr(string rolePermissionStr);

        object GetUserSystemDetails(string userID, string cityID, string systemID);

        bool ProcessUserPermissionStr(string userPermissionStr);

        object GetDetails2(int page, int rows, string QueryString, string Value);
    }
}
