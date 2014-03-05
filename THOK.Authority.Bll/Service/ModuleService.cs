using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Authority.Bll.Interfaces;
using THOK.Authority.Bll.Models;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;

namespace THOK.Authority.Bll.Service
{
    public class ModuleService : ServiceBase<AUTH_MODULE>, IModuleService
    {
        [Dependency]
        public IUserRepository UserRepository { get; set; }
        [Dependency]
        public ICityRepository CityRepository { get; set; }
        [Dependency]
        public ISystemRepository SystemRepository { get; set; }
        [Dependency]
        public IModuleRepository ModuleRepository { get; set; }
        [Dependency]
        public IUserSystemRepository UserSystemRepository { get; set; }
        [Dependency]
        public IUserModuleRepository UserModuleRepository { get; set; }
        [Dependency]
        public IUserFunctionRepository UserFunctionRepository { get; set; }
        [Dependency]
        public IRoleSystemRepository RoleSystemRepository { get; set; }
        [Dependency]
        public IRoleModuleRepository RoleModuleRepository { get; set; }
        [Dependency]
        public IRoleFunctionRepository RoleFunctionRepository { get; set; }
        [Dependency]
        public IFunctionRepository FunctionRepository { get; set; }
        [Dependency]
        public IRoleRepository RoleRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        #region 模块信息维护

        public object GetDetails()// 测试数据
        {
            IQueryable<AUTH_MODULE> queryModule = ModuleRepository.GetQueryable();
            int c = queryModule.Count();
            var modules = queryModule.Select(m => m);
            return modules.ToArray();
        }

        public object GetDetails(string systemID)
        {
            IQueryable<AUTH_SYSTEM> querySystem = SystemRepository.GetQueryable();
            IQueryable<AUTH_MODULE> queryModule = ModuleRepository.GetQueryable();
            var systems = querySystem.AsEnumerable();
            if (systemID != null && systemID != string.Empty)
            {
               // Guid gsystemid = new Guid(systemID);
                systems = querySystem.Where(i => i.SYSTEM_ID == systemID)
                                     .Select(i => i);
            }

            HashSet<Menu> systemMenuSet = new HashSet<Menu>();
            foreach (var system in systems)
            {
                Menu systemMenu = new Menu();
                systemMenu.ModuleID = system.SYSTEM_ID.ToString();
                systemMenu.ModuleName = system.SYSTEM_NAME;
                systemMenu.SystemID = system.SYSTEM_ID.ToString();
                systemMenu.SystemName = system.SYSTEM_NAME;

                var modules = queryModule.Where(m => m.SYSTEM_SYSTEM_ID == system.SYSTEM_ID && m.MODULE_ID == m.PARENT_AUTH_MODULE.MODULE_ID)
                                         .OrderBy(m => m.SHOW_ORDER)
                                         .Select(m => m);
                HashSet<Menu> moduleMenuSet = new HashSet<Menu>();
                foreach (var item in modules)
                {
                    Menu moduleMenu = new Menu();
                    moduleMenu.ModuleID = item.MODULE_ID.ToString();
                    moduleMenu.ModuleName = item.MODULE_NAME;
                    moduleMenu.SystemID = item.AUTH_SYSTEM.SYSTEM_ID;
                    moduleMenu.SystemName = item.AUTH_SYSTEM.SYSTEM_NAME;
                    moduleMenu.ParentModuleID = item.PARENT_AUTH_MODULE.MODULE_ID;
                    moduleMenu.ParentModuleName = item.PARENT_AUTH_MODULE.MODULE_NAME;

                    moduleMenu.ModuleURL = item.MODULE_URL;
                    moduleMenu.iconCls = item.INDICATE_IMAGE;
                    moduleMenu.ShowOrder =Convert.ToInt32(item.SHOW_ORDER);
                    moduleMenuSet.Add(moduleMenu);
                    GetChildMenu(moduleMenu, item);
                    moduleMenuSet.Add(moduleMenu);
                }
                systemMenu.children = moduleMenuSet.ToArray();
                systemMenuSet.Add(systemMenu);
            }

            return systemMenuSet.ToArray();
        }

        public bool Add(string moduleName, int showOrder, string moduleUrl, string indicateImage, string desktopImage, string systemID, string moduleID)
        {
            IQueryable<AUTH_SYSTEM> querySystem = SystemRepository.GetQueryable();
            IQueryable<AUTH_MODULE> queryModule = ModuleRepository.GetQueryable();
            moduleID = !String.IsNullOrEmpty(moduleID) ? moduleID : "000001";
            var system = querySystem.FirstOrDefault(i => i.SYSTEM_ID == systemID);
            var parentModule = queryModule.FirstOrDefault(i => i.MODULE_ID == moduleID);
            var module = new AUTH_MODULE();
            //module.MODULE_ID = Guid.NewGuid();
            module.MODULE_ID = ModuleRepository.GetNewID("AUTH_MODULE", "MODULE_ID");
            module.MODULE_NAME = moduleName;
            module.SHOW_ORDER = showOrder;
            module.MODULE_URL = moduleUrl;
            module.INDICATE_IMAGE = indicateImage;
            module.DESK_TOP_IMAGE = desktopImage;
            module.AUTH_SYSTEM = system;
            module.PARENT_AUTH_MODULE = parentModule ?? module;
            ModuleRepository.Add(module);
            ModuleRepository.SaveChanges();
            return true;
        }

        public bool Delete(string moduleID)
        {
            IQueryable<AUTH_MODULE> queryModule = ModuleRepository.GetQueryable();

            //Guid gmoduleId = new Guid(moduleID);
            //var module = queryModule.FirstOrDefault(i => i.ModuleID == gmoduleId);
            var module = queryModule.FirstOrDefault(i => i.MODULE_ID == moduleID);
            if (module != null)
            {
                Del(FunctionRepository, module.AUTH_FUNCTION);
                Del(ModuleRepository, module.AUTH_MODULES);
                Del(RoleModuleRepository, module.AUTH_ROLE_MODULE);
                Del(UserModuleRepository, module.AUTH_USER_MODULE);

                ModuleRepository.Delete(module);
                ModuleRepository.SaveChanges();
            }
            else
                return false;
            return true;
        }

        public bool Save(string moduleID, string moduleName, int showOrder, string moduleUrl, string indicateImage, string deskTopImage)
        {
           // IQueryable<AUTH_MODULE> queryModule = ModuleRepository.GetQueryable();
            //Guid sid = new Guid(moduleID);
            //var module = queryModule.FirstOrDefault(i => i.ModuleID == sid);

            var module = ModuleRepository.GetQueryable().FirstOrDefault(i => i.MODULE_ID == moduleID);
            //var module = queryModule.FirstOrDefault(i => i.MODULE_ID == moduleID);
            module.MODULE_NAME = moduleName;
            module.SHOW_ORDER = showOrder;
            module.MODULE_URL = moduleUrl;
            module.INDICATE_IMAGE = indicateImage;
            module.DESK_TOP_IMAGE = deskTopImage;
            ModuleRepository.SaveChanges();
            return true;
        }

        private void GetChildMenu(Menu menu, AUTH_MODULE module)
        {
            HashSet<Menu> childMenuSet = new HashSet<Menu>();
            var modules = from m in module.AUTH_MODULES
                          orderby m.SHOW_ORDER
                          select m;
            foreach (var item in modules)
            {
                if (item != module)
                {
                    Menu childMenu = new Menu();
                    childMenu.ModuleID = item.MODULE_ID.ToString();
                    childMenu.ModuleName = item.MODULE_NAME;
                    childMenu.SystemID = item.AUTH_SYSTEM.SYSTEM_ID;
                    childMenu.SystemName = item.AUTH_SYSTEM.SYSTEM_NAME;
                    childMenu.ParentModuleID = item.PARENT_AUTH_MODULE.MODULE_ID;
                    childMenu.ParentModuleName = item.PARENT_AUTH_MODULE.MODULE_NAME;
                    childMenu.ModuleURL = item.MODULE_URL;
                    childMenu.iconCls = item.INDICATE_IMAGE;
                    childMenu.ShowOrder = Convert.ToInt32(item.SHOW_ORDER);
                    childMenuSet.Add(childMenu);
                    if (item.AUTH_MODULES.Count > 0)
                    {
                        GetChildMenu(childMenu, item);
                    }
                }
            }
            menu.children = childMenuSet.ToArray();
        }

        #endregion

        #region 页面权限控制

        public object GetUserMenus(string userName, string cityID, string systemID)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("值不能为NULL或为空。", "userName");
            if (String.IsNullOrEmpty(cityID)) throw new ArgumentException("值不能为NULL或为空。", "cityID");
            if (String.IsNullOrEmpty(systemID)) throw new ArgumentException("值不能为NULL或为空。", "systemId");

            IQueryable<AUTH_USER> queryUser = UserRepository.GetQueryable();
            IQueryable<AUTH_CITY> queryCity = CityRepository.GetQueryable();
            IQueryable<AUTH_SYSTEM> querySystem = SystemRepository.GetQueryable();
            IQueryable<AUTH_ROLE_MODULE> queryRoleModule = RoleModuleRepository.GetQueryable();
            IQueryable<AUTH_MODULE> QueryModule=ModuleRepository.GetQueryable();

            //Guid gSystemID = new Guid(systemID);
            //Guid gCityID = new Guid(cityID);
            var user = queryUser.Single(u => u.USER_NAME.ToLower() == userName.ToLower());
            var city = queryCity.Single(c => c.CITY_ID.Trim() == cityID);
            var system = querySystem.Single(s => s.SYSTEM_ID.Trim() == systemID);
            //InitUserSystem(user, city, system);

            HashSet<Menu> systemMenuSet = new HashSet<Menu>();
            Menu systemMenu = new Menu();
            HashSet<Menu> moduleMenuSet = new HashSet<Menu>();

            if (user.IS_ADMIN == "1" || userName == "Admin")
            {

                systemMenu.ModuleID = system.SYSTEM_ID;
                systemMenu.ModuleName = system.SYSTEM_NAME;
                systemMenu.SystemID = system.SYSTEM_ID;
                systemMenu.SystemName = system.SYSTEM_NAME;

                var userModules = from um in QueryModule
                                  where um.SYSTEM_SYSTEM_ID == systemID && um.MODULE_ID == um.PARENT_MODULE_MODULE_ID
                                  orderby um.SHOW_ORDER
                                  select um;

                foreach (var module in userModules)
                {
                    Menu moduleMenu = new Menu();
                    moduleMenu.ModuleID = module.MODULE_ID.ToString();
                    moduleMenu.ModuleName = module.MODULE_NAME;
                    moduleMenu.SystemID = module.AUTH_SYSTEM.SYSTEM_ID;
                    moduleMenu.SystemName = module.AUTH_SYSTEM.SYSTEM_NAME;
                    moduleMenu.ParentModuleID = module.PARENT_AUTH_MODULE.MODULE_ID;
                    moduleMenu.ParentModuleName = module.PARENT_AUTH_MODULE.MODULE_NAME;
                    moduleMenu.ModuleURL = module.MODULE_URL;
                    moduleMenu.iconCls = module.INDICATE_IMAGE;
                    moduleMenu.ShowOrder = Convert.ToInt32(module.SHOW_ORDER);
                    GetChildMenuNew(moduleMenu, module);
                    moduleMenuSet.Add(moduleMenu);

                }
            }
            else
            {
                var userSystem = (from us in user.AUTH_USER_SYSTEM
                                  where us.AUTH_USER.USER_ID == user.USER_ID
                                    && us.AUTH_CITY.CITY_ID == city.CITY_ID
                                    && us.AUTH_SYSTEM.SYSTEM_ID == system.SYSTEM_ID
                                  select us).Single();


                systemMenu.ModuleID = userSystem.AUTH_SYSTEM.SYSTEM_ID;
                systemMenu.ModuleName = userSystem.AUTH_SYSTEM.SYSTEM_NAME;
                systemMenu.SystemID = userSystem.AUTH_SYSTEM.SYSTEM_ID;
                systemMenu.SystemName = userSystem.AUTH_SYSTEM.SYSTEM_NAME;



                var userModules = from um in QueryModule
                                  where um.SYSTEM_SYSTEM_ID == systemID && um.MODULE_ID == um.PARENT_MODULE_MODULE_ID
                                  orderby um.SHOW_ORDER
                                  select um;




                foreach (var module in userModules)
                {

                    var roles = user.AUTH_USER_ROLE.Select(ur => ur.AUTH_ROLE);  //shj 2013/05/28 屏蔽

                    var userModule = (from um in userSystem.AUTH_USER_MODULE
                                      where um.AUTH_MODULE.MODULE_ID == module.MODULE_ID
                                      select um).Single();



                    if (userModule.IS_ACTIVE == "1" ||
                        userModule.AUTH_MODULE.AUTH_ROLE_MODULE.Any(rm => roles.Any(r => r.ROLE_ID == rm.AUTH_ROLE_SYSTEM.AUTH_ROLE.ROLE_ID
                            && rm.IS_ACTIVE == "1")) ||
                        userModule.AUTH_MODULE.AUTH_MODULES
                            .Any(m => m.AUTH_USER_MODULE.Any(um => um.AUTH_USER_SYSTEM.AUTH_USER.USER_ID == userModule.AUTH_USER_SYSTEM.AUTH_USER.USER_ID
                                && (um.IS_ACTIVE == "1" || um.AUTH_USER_FUNCTION.Any(uf => uf.IS_ACTIVE == "1")))) ||
                        userModule.AUTH_MODULE.AUTH_MODULES
                            .Any(m => m.AUTH_ROLE_MODULE.Any(rm => roles.Any(r => r.ROLE_ID == rm.AUTH_ROLE_SYSTEM.AUTH_ROLE.ROLE_ID
                                && (rm.IS_ACTIVE == "1" || rm.AUTH_ROLE_FUNCTION.Any(rf => rf.IS_ACTIVE == "1"))))) ||
                        userName == "Admin" || user.IS_ADMIN == "1"
                        )
                    {

                        Menu moduleMenu = new Menu();
                        moduleMenu.ModuleID = module.MODULE_ID.ToString();
                        moduleMenu.ModuleName = module.MODULE_NAME;
                        moduleMenu.SystemID = module.AUTH_SYSTEM.SYSTEM_ID;
                        moduleMenu.SystemName = module.AUTH_SYSTEM.SYSTEM_NAME;
                        moduleMenu.ParentModuleID = module.PARENT_AUTH_MODULE.MODULE_ID;
                        moduleMenu.ParentModuleName = module.PARENT_AUTH_MODULE.MODULE_NAME;
                        moduleMenu.ModuleURL = module.MODULE_URL;
                        moduleMenu.iconCls = module.INDICATE_IMAGE;
                        moduleMenu.ShowOrder = Convert.ToInt32(module.SHOW_ORDER);
                        GetChildMenu(moduleMenu, userSystem, module);
                        moduleMenuSet.Add(moduleMenu);
                    }
                }
            }
            systemMenu.children = moduleMenuSet.ToArray();
            systemMenuSet.Add(systemMenu);
        
            return systemMenuSet.ToArray();
        }

        public object GetModuleFuns(string userName, string cityID, string moduleID)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("值不能为NULL或为空。", "userName");
            if (String.IsNullOrEmpty(cityID)) throw new ArgumentException("值不能为NULL或为空。", "cityID");
            if (String.IsNullOrEmpty(moduleID)) throw new ArgumentException("值不能为NULL或为空。", "moduleID");

            IQueryable<AUTH_USER> queryUser = UserRepository.GetQueryable();
           
            IQueryable<AUTH_MODULE> queryModule = ModuleRepository.GetQueryable();

            //Guid gCityID = new Guid(cityID);
            //Guid gModuleID = new Guid(moduleID);
            var user = queryUser.Single(u => u.USER_NAME.ToLower() == userName.ToLower());
            var module = queryModule.Single(m => m.MODULE_ID == moduleID);
             Fun fun = new Fun();
             HashSet<Fun> moduleFunctionSet = new HashSet<Fun>();
             if (user.IS_ADMIN == "1")
             {
                 var ModuleFuns = module.AUTH_FUNCTION;

                 foreach (var userFunction in ModuleFuns)
                 {

                     moduleFunctionSet.Add(new Fun()
                     {
                         funid = userFunction.FUNCTION_ID,
                         funname = userFunction.CONTROL_NAME,
                         iconCls = userFunction.INDICATE_IMAGE,
                         isActive = true
                     });
                 }
             }
             else
             {
             
                 // var module = queryModule.Single(m => m.ModuleID == gModuleID);
               

                 var userModule = (from um in module.AUTH_USER_MODULE
                                   where um.AUTH_USER_SYSTEM.AUTH_USER.USER_ID == user.USER_ID
                                     && um.AUTH_USER_SYSTEM.AUTH_CITY.CITY_ID == cityID
                                   select um).Single();



                 var userFunctions = userModule.AUTH_USER_FUNCTION;
                 if (userFunctions.Count == 0)
                 {
                     foreach (var UF in module.AUTH_FUNCTION)  
                     {
                         bool bResult = UF.AUTH_ROLE_FUNCTION.Any(r => r.AUTH_ROLE_MODULE.AUTH_MODULE.MODULE_ID == moduleID && r.IS_ACTIVE == "1");
                         moduleFunctionSet.Add(new Fun()
                         {
                             funid = UF.FUNCTION_ID,
                             funname = UF.CONTROL_NAME,
                             iconCls = UF.INDICATE_IMAGE,
                             isActive = bResult
                         });
                     }
                 }
                 else
                 {
                     foreach (var userFunction in userFunctions)
                     {
                         var roles = user.AUTH_USER_ROLE.Select(ur => ur.AUTH_ROLE);
                         bool bResult = userFunction.AUTH_FUNCTION.AUTH_ROLE_FUNCTION.Any(
                                 rf => roles.Any(
                                     r => r.ROLE_ID == rf.AUTH_ROLE_MODULE.AUTH_ROLE_SYSTEM.AUTH_ROLE.ROLE_ID
                                        && rf.IS_ACTIVE == "1"
                                  )
                             );
                         moduleFunctionSet.Add(new Fun()
                             {
                                 funid = userFunction.AUTH_FUNCTION.FUNCTION_ID,
                                 funname = userFunction.AUTH_FUNCTION.CONTROL_NAME,
                                 iconCls = userFunction.AUTH_FUNCTION.INDICATE_IMAGE,
                                 isActive = userFunction.IS_ACTIVE == "1" || bResult || user.USER_NAME == "Admin"
                             });

                     }
                 }
             }
            fun.funs = moduleFunctionSet.ToArray();
            return fun;
        }

        private void GetChildMenu(Menu moduleMenu, AUTH_USER_SYSTEM userSystem, AUTH_MODULE module)
        {
            IQueryable<AUTH_ROLE_MODULE> queryRoleModule = RoleModuleRepository.GetQueryable();
            HashSet<Menu> childMenuSet = new HashSet<Menu>();
            var userModules = from um in userSystem.AUTH_USER_MODULE
                              where um.AUTH_MODULE.PARENT_AUTH_MODULE == module && um.AUTH_MODULE!=module
                              orderby um.AUTH_MODULE.SHOW_ORDER
                              select um;
            foreach (var userModule in userModules)
            {
                var childModule = userModule.AUTH_MODULE;
                if (childModule != module)
                {
                    var roles = userSystem.AUTH_USER.AUTH_USER_ROLE.Select(ur => ur.AUTH_ROLE);
                    if (userModule.IS_ACTIVE=="1" || userModule.AUTH_USER_FUNCTION.Any(uf =>uf.IS_ACTIVE=="1") ||
                        userModule.AUTH_MODULE.AUTH_ROLE_MODULE.Any(rm => roles.Any(r => r.ROLE_ID == rm.AUTH_ROLE_SYSTEM.AUTH_ROLE.ROLE_ID
                            && (rm.IS_ACTIVE == "1" || rm.AUTH_ROLE_FUNCTION.Any(rf => rf.IS_ACTIVE == "1")))) ||
                        userModule.AUTH_MODULE.AUTH_MODULES
                            .Any(m => m.AUTH_USER_MODULE.Any(um => um.AUTH_USER_SYSTEM.AUTH_USER.USER_ID == userModule.AUTH_USER_SYSTEM.AUTH_USER.USER_ID
                                && (um.IS_ACTIVE == "1" || um.AUTH_USER_FUNCTION.Any(uf => uf.IS_ACTIVE == "1")))) ||
                        userModule.AUTH_MODULE.AUTH_MODULES
                            .Any(m => m.AUTH_ROLE_MODULE.Any(rm => roles.Any(r => r.ROLE_ID == rm.AUTH_ROLE_SYSTEM.AUTH_ROLE.ROLE_ID
                                && (rm.IS_ACTIVE == "1" || rm.AUTH_ROLE_FUNCTION.Any(rf => rf.IS_ACTIVE == "1"))))) ||
                        userModule.AUTH_USER_SYSTEM.AUTH_USER.USER_NAME == "Admin"
                        )
                    {
                        Menu childMenu = new Menu();
                        childMenu.ModuleID = childModule.MODULE_ID;
                        childMenu.ModuleName = childModule.MODULE_NAME;
                        childMenu.SystemID = childModule.AUTH_SYSTEM.SYSTEM_ID;;
                        childMenu.SystemName = childModule.AUTH_SYSTEM.SYSTEM_NAME;
                        childMenu.ParentModuleID = childModule.PARENT_AUTH_MODULE.MODULE_ID;
                        childMenu.ParentModuleName = childModule.PARENT_AUTH_MODULE.MODULE_NAME;
                        childMenu.ModuleURL = childModule.MODULE_URL;
                        childMenu.iconCls = childModule.INDICATE_IMAGE;
                        childMenu.ShowOrder = Convert.ToInt32(childModule.SHOW_ORDER);
                        childMenuSet.Add(childMenu);
                        if (childModule.AUTH_MODULES.Count > 0)
                        {
                            GetChildMenu(childMenu, userSystem, childModule);
                        }
                    }
                }
            }
            moduleMenu.children = childMenuSet.ToArray();
        }

        private void GetChildMenuNew(Menu moduleMenu, AUTH_MODULE module)
        {

            HashSet<Menu> childMenuSet = new HashSet<Menu>();

            var userModules = from um in module.AUTH_MODULES
                              where um.MODULE_ID != module.MODULE_ID
                              orderby um.SHOW_ORDER
                              select um;
            foreach (var userModule in userModules)
            {
                var childModule = userModule;
                if (childModule != module)
                {

                    Menu childMenu = new Menu();
                    childMenu.ModuleID = childModule.MODULE_ID;
                    childMenu.ModuleName = childModule.MODULE_NAME;
                    childMenu.SystemID = childModule.AUTH_SYSTEM.SYSTEM_ID; ;
                    childMenu.SystemName = childModule.AUTH_SYSTEM.SYSTEM_NAME;
                    childMenu.ParentModuleID = childModule.PARENT_AUTH_MODULE.MODULE_ID;
                    childMenu.ParentModuleName = childModule.PARENT_AUTH_MODULE.MODULE_NAME;
                    childMenu.ModuleURL = childModule.MODULE_URL;
                    childMenu.iconCls = childModule.INDICATE_IMAGE;
                    childMenu.ShowOrder = Convert.ToInt32(childModule.SHOW_ORDER);
                    childMenuSet.Add(childMenu);
                    if (childModule.AUTH_MODULES.Count > 0)
                    {
                        GetChildMenuNew(childMenu, childModule);
                    }

                }
            }
            moduleMenu.children = childMenuSet.ToArray();
        }

        #endregion

        #region 初始化角色权限

        private void InitRoleSystem(AUTH_ROLE role, AUTH_CITY city, AUTH_SYSTEM system)
        {
            var roleSystems = role.AUTH_ROLE_SYSTEM.Where(rs => rs.AUTH_CITY.CITY_ID == city.CITY_ID
                && rs.AUTH_SYSTEM.SYSTEM_ID == system.SYSTEM_ID);
            if (roleSystems.Count() == 0)
            {
                AUTH_ROLE_SYSTEM rs = new AUTH_ROLE_SYSTEM()
                {
                    //ROLE_SYSTEM_ID = Guid.NewGuid().ToString(),
                    ROLE_SYSTEM_ID = RoleSystemRepository.GetNewID("AUTH_ROLE_SYSTEM", "ROLE_SYSTEM_ID"),
                    AUTH_ROLE = role,
                    AUTH_CITY = city,
                    AUTH_SYSTEM = system,
                    IS_ACTIVE = "0"
                };
                RoleSystemRepository.Add(rs);
                RoleSystemRepository.SaveChanges();
            }

            var roleSystem = role.AUTH_ROLE_SYSTEM.Single(rs => rs.AUTH_CITY.CITY_ID == city.CITY_ID
                && rs.AUTH_SYSTEM.SYSTEM_ID == system.SYSTEM_ID);
            InitRoleModule(roleSystem);
        }

        private void InitRoleModule(AUTH_ROLE_SYSTEM roleSystem)
        {
            foreach (var module in roleSystem.AUTH_SYSTEM.AUTH_MODULE)
            {
                var roleModules = roleSystem.AUTH_ROLE_MODULE.Where(rm => rm.AUTH_MODULE.MODULE_ID == module.MODULE_ID
                    && rm.AUTH_ROLE_SYSTEM.AUTH_SYSTEM.SYSTEM_ID == roleSystem.AUTH_SYSTEM.SYSTEM_ID);
                if (roleModules.Count() == 0)
                {
                    AUTH_ROLE_MODULE rm = new AUTH_ROLE_MODULE()
                    {
                       // ROLE_MODULE_ID = Guid.NewGuid().ToString(),//之后再修改
                        ROLE_MODULE_ID = RoleModuleRepository.GetNewID("AUTH_ROLE_MODULE", "ROLE_MODULE_ID"),
                        AUTH_ROLE_SYSTEM = roleSystem,
                        AUTH_MODULE = module,
                        IS_ACTIVE = "0"
                    };
                    roleSystem.IS_ACTIVE ="0";
                    SetParentRoleModuleIsActiveFalse(rm);
                    RoleModuleRepository.Add(rm);
                    RoleModuleRepository.SaveChanges();
                }
                var roleModule = roleSystem.AUTH_ROLE_MODULE.Single(rm => rm.AUTH_MODULE.MODULE_ID == module.MODULE_ID
                    && rm.AUTH_ROLE_SYSTEM.AUTH_SYSTEM.SYSTEM_ID == roleSystem.AUTH_SYSTEM.SYSTEM_ID);
                InitRoleFunctions(roleModule);
            }
        }

        private void SetParentRoleModuleIsActiveFalse(AUTH_ROLE_MODULE roleModule)
        {
            var parentRoleModule = roleModule.AUTH_MODULE.PARENT_AUTH_MODULE.AUTH_ROLE_MODULE.FirstOrDefault(prm => prm.AUTH_ROLE_SYSTEM.AUTH_ROLE.ROLE_ID == roleModule.AUTH_ROLE_SYSTEM.AUTH_ROLE.ROLE_ID);
            if (parentRoleModule != null)
            {
                parentRoleModule.IS_ACTIVE ="0";
                if (parentRoleModule.AUTH_MODULE.MODULE_ID != parentRoleModule.AUTH_MODULE.PARENT_AUTH_MODULE.MODULE_ID)
                {
                    SetParentRoleModuleIsActiveFalse(parentRoleModule);
                }
            }
        }

        private void InitRoleFunctions(AUTH_ROLE_MODULE roleModule)
        {
            foreach (var function in roleModule.AUTH_MODULE.AUTH_FUNCTION)
            {
                var roleFunctions = roleModule.AUTH_ROLE_FUNCTION.Where(rf => rf.AUTH_FUNCTION.FUNCTION_ID == function.FUNCTION_ID);
                if (roleFunctions.Count() == 0)
                {
                    AUTH_ROLE_FUNCTION rf = new AUTH_ROLE_FUNCTION()
                    {
                        //ROLE_FUNCTION_ID = Guid.NewGuid().ToString(),
                        ROLE_FUNCTION_ID = RoleFunctionRepository.GetNewID("AUTH_ROLE_FUNCTION", "ROLE_FUNCTION_ID"),
                        AUTH_ROLE_MODULE = roleModule,
                        AUTH_FUNCTION = function,
                       // IS_ACTIVE = false.ToString()
                        IS_ACTIVE = "0"
                    };
                    roleModule.AUTH_ROLE_SYSTEM.IS_ACTIVE ="0";
                    SetParentRoleModuleIsActiveFalse(roleModule);
                    roleModule.IS_ACTIVE ="0";
                    RoleFunctionRepository.Add(rf);
                    RoleFunctionRepository.SaveChanges();
                }
            }
        }

        #endregion

        #region 初始化用户权限

        private void InitUserSystem(AUTH_USER user, AUTH_CITY city, AUTH_SYSTEM system)
        {
            var userSystems = user.AUTH_USER_SYSTEM.Where(us => us.AUTH_CITY.CITY_ID == city.CITY_ID
                && us.AUTH_SYSTEM.SYSTEM_ID == system.SYSTEM_ID);
            if (userSystems.Count() == 0)
            {
                AUTH_USER_SYSTEM us = new AUTH_USER_SYSTEM()
                {
                    //USER_SYSTEM_ID = Guid.NewGuid().ToString(),
                    USER_SYSTEM_ID = UserSystemRepository.GetNewID("AUTH_USER_SYSTEM", "USER_SYSTEM_ID"),
                    AUTH_USER = user,
                    AUTH_CITY = city,
                    AUTH_SYSTEM = system,
                    IS_ACTIVE = user.USER_NAME == "Admin"?"1":"0"
                };
                UserSystemRepository.Add(us);
                UserSystemRepository.SaveChanges();
            }
            //var userSystem = user.AUTH_USER_SYSTEM.Single(us => us.AUTH_CITY.CITY_ID == city.CITY_ID
            //    && us.AUTH_SYSTEM.SYSTEM_ID == system.SYSTEM_ID);

            var userSystem = user.AUTH_USER_SYSTEM.Single(us => us.AUTH_CITY.CITY_ID == city.CITY_ID
                && us.AUTH_SYSTEM.SYSTEM_ID == system.SYSTEM_ID);
            InitUserModule(userSystem);
        }

        private void InitUserModule(AUTH_USER_SYSTEM userSystem)
        {
            foreach (var module in userSystem.AUTH_SYSTEM.AUTH_MODULE)
            {
                var userModules = userSystem.AUTH_USER_MODULE.Where(um => um.AUTH_MODULE.MODULE_ID == module.MODULE_ID
                    && um.AUTH_USER_SYSTEM.USER_SYSTEM_ID == userSystem.USER_SYSTEM_ID);
                if (userModules.Count() == 0)
                {
                    AUTH_USER_MODULE um = new AUTH_USER_MODULE()
                    {
                        //USER_MODULE_ID = Guid.NewGuid().ToString(),//之后再修改
                        USER_MODULE_ID = UserModuleRepository.GetNewID("AUTH_USER_MODULE", "USER_MODULE_ID"),
                        AUTH_USER_SYSTEM = userSystem,
                        AUTH_MODULE = module,
                        IS_ACTIVE = userSystem.AUTH_USER.USER_NAME == "Admin" ? "1" : "0"
                    };
                    userSystem.IS_ACTIVE = userSystem.AUTH_USER.USER_NAME == "Admin" ? "1" : "0";
                    SetParentUserModuleIsActiveFalse(um);
                    UserModuleRepository.Add(um);
                    UserModuleRepository.SaveChanges();
                }
                var userModule = userSystem.AUTH_USER_MODULE.Single(um => um.AUTH_MODULE.MODULE_ID == module.MODULE_ID
                    && um.AUTH_USER_SYSTEM.USER_SYSTEM_ID == userSystem.USER_SYSTEM_ID);
                InitUserFunctions(userModule);
            }
        }

        private void SetParentUserModuleIsActiveFalse(AUTH_USER_MODULE userModule)
        {
            var parentUserModule = userModule.AUTH_MODULE.PARENT_AUTH_MODULE.AUTH_USER_MODULE.FirstOrDefault(pum => pum.AUTH_USER_SYSTEM.AUTH_USER.USER_ID == userModule.AUTH_USER_SYSTEM.AUTH_USER.USER_ID);
            if (parentUserModule != null)
            {
                //parentUserModule.IS_ACTIVE ="0";
                parentUserModule.IS_ACTIVE ="0";
                if (parentUserModule.AUTH_MODULE.MODULE_ID != parentUserModule.AUTH_MODULE.PARENT_AUTH_MODULE.MODULE_ID)
                {
                    SetParentUserModuleIsActiveFalse(parentUserModule);
                }
            }
        }

        private void InitUserFunctions(AUTH_USER_MODULE userModule)
        {
            foreach (var function in userModule.AUTH_MODULE.AUTH_FUNCTION)
            {
                var userFunctions = userModule.AUTH_USER_FUNCTION.Where(uf => uf.AUTH_FUNCTION.FUNCTION_ID == function.FUNCTION_ID);
                if (userFunctions.Count() == 0)
                {
                    AUTH_USER_FUNCTION uf = new AUTH_USER_FUNCTION()
                    {
                       // USER_FUNCTION_ID = Guid.NewGuid().ToString(), GetNewID(),
                        USER_FUNCTION_ID =UserFunctionRepository.GetNewID("AUTH_USER_FUNCTION","USER_FUNCTION_ID"),
                        AUTH_USER_MODULE = userModule,
                        AUTH_FUNCTION = function,
                        IS_ACTIVE = userModule.AUTH_USER_SYSTEM.AUTH_USER.USER_NAME == "Admin" ? "1" : "0"
                    };
                    userModule.AUTH_USER_SYSTEM.IS_ACTIVE = userModule.AUTH_USER_SYSTEM.AUTH_USER.USER_NAME == "Admin" ? "1" : "0";
                    SetParentUserModuleIsActiveFalse(userModule);
                    userModule.IS_ACTIVE = userModule.AUTH_USER_SYSTEM.AUTH_USER.USER_NAME == "Admin"?"1":"0";
                    UserFunctionRepository.Add(uf);
                    UserFunctionRepository.SaveChanges();
                }
            }
        }

        #endregion

        #region

        private void SetMenu(Menu menu, AUTH_MODULE module)
        {
            IQueryable<AUTH_ROLE_MODULE> queryRoleModule = RoleModuleRepository.GetQueryable();
            HashSet<Menu> childMenuSet = new HashSet<Menu>();
            var modules = from m in module.AUTH_MODULES
                          orderby m.SHOW_ORDER
                          select m;
            foreach (var item in modules)
            {
                if (item != module)
                {
                    Menu childMenu = new Menu();
                    childMenu.ModuleID = item.MODULE_ID;
                    childMenu.ModuleName = item.MODULE_NAME;
                    childMenu.SystemID = item.AUTH_SYSTEM.SYSTEM_ID;
                    childMenu.SystemName = item.AUTH_SYSTEM.SYSTEM_NAME;
                    childMenu.ParentModuleID = item.PARENT_AUTH_MODULE.MODULE_ID;
                    childMenu.ParentModuleName = item.PARENT_AUTH_MODULE.MODULE_NAME;
                    childMenu.ModuleURL = item.MODULE_URL;
                    childMenu.iconCls = item.INDICATE_IMAGE;
                    childMenu.ShowOrder =Convert.ToInt32(item.SHOW_ORDER);
                    childMenuSet.Add(childMenu);
                    if (item.AUTH_MODULES.Count > 0)
                    {
                        SetMenu(childMenu, item);
                    }
                }
            }
            menu.children = childMenuSet.ToArray();
        }

        private void SetFunTree(Tree childTree, AUTH_MODULE item, AUTH_ROLE_MODULE roleModules)
        {
            var function = FunctionRepository.GetQueryable().Where(f => f.AUTH_MODULE.MODULE_ID == item.MODULE_ID);
            IQueryable<AUTH_ROLE_FUNCTION> queryRoleFunction = RoleFunctionRepository.GetQueryable();
            HashSet<Tree> functionTreeSet = new HashSet<Tree>();
            foreach (var func in function)
            {
                Tree funcTree = new Tree();
                var roleFunction = queryRoleFunction.FirstOrDefault(rf => rf.AUTH_FUNCTION.FUNCTION_ID == func.FUNCTION_ID && rf.AUTH_ROLE_MODULE.ROLE_MODULE_ID == roleModules.ROLE_MODULE_ID);
                if (roleFunction != null)
                {
                    funcTree.id = roleFunction.ROLE_FUNCTION_ID.ToString();
                    funcTree.text = "功能：" + func.FUNCTION_NAME;
                    int a = Convert.ToInt32(roleFunction.IS_ACTIVE);
                    funcTree.@checked = roleFunction == null ? false : Convert.ToBoolean(a);
                    funcTree.attributes = "function";
                    functionTreeSet.Add(funcTree);
                }
              
            }
            childTree.children = functionTreeSet.ToArray();
        }

        public void InitRoleSys(string roleID, string cityID, string systemID)
        {
            IQueryable<AUTH_ROLE> queryRole = RoleRepository.GetQueryable();
            IQueryable<AUTH_CITY> queryCity = CityRepository.GetQueryable();
            IQueryable<AUTH_SYSTEM> querySystem = SystemRepository.GetQueryable();
            var role = queryRole.Single(i => i.ROLE_ID == roleID);
            var city = queryCity.Single(i => i.CITY_ID == cityID);
            var system = querySystem.Single(i => i.SYSTEM_ID == systemID);
            InitRoleSystem(role, city, system);
        }

        public object GetRoleSystemDetails(string roleID, string cityID, string systemID)
        {
            IQueryable<AUTH_SYSTEM> querySystem = SystemRepository.GetQueryable();
            IQueryable<AUTH_MODULE> queryModule = ModuleRepository.GetQueryable();
            IQueryable<AUTH_ROLE_SYSTEM> queryRoleSystem = RoleSystemRepository.GetQueryable();
            IQueryable<AUTH_ROLE_MODULE> queryRoleModule = RoleModuleRepository.GetQueryable();
            var systems = querySystem.Single(i => i.SYSTEM_ID == systemID);
            var roleSystems = queryRoleSystem.FirstOrDefault(i => i.AUTH_SYSTEM.SYSTEM_ID == systemID && i.AUTH_ROLE.ROLE_ID == roleID && i.AUTH_CITY.CITY_ID == cityID);
                HashSet<Tree> RolesystemTreeSet = new HashSet<Tree>();
                Tree roleSystemTree = new Tree();
                roleSystemTree.id = roleSystems.ROLE_SYSTEM_ID.ToString();
                roleSystemTree.text = "系统：" + systems.SYSTEM_NAME;
                int a = Convert.ToInt32(roleSystems.IS_ACTIVE);
                roleSystemTree.@checked = Convert.ToBoolean(a);
                roleSystemTree.attributes = "system";

                var modules = queryModule.Where(m => m.AUTH_SYSTEM.SYSTEM_ID == systems.SYSTEM_ID && m.MODULE_ID == m.PARENT_AUTH_MODULE.MODULE_ID)
                                         .OrderBy(m => m.SHOW_ORDER)
                                         .Select(m => m);
                var modulelist = modules.ToArray();
                HashSet<Tree> moduleTreeSet = new HashSet<Tree>();
                foreach (var item in modules)
                {
                    Tree moduleTree = new Tree();
                    string moduleID = item.MODULE_ID.ToString();
                    //var roleModules = queryRoleModule.FirstOrDefault(i => i.Module.ModuleID == new Guid(moduleID) && i.RoleSystem.RoleSystemID == roleSystems.RoleSystemID);
                    var roleModules = queryRoleModule.FirstOrDefault(i => i.AUTH_MODULE.MODULE_ID == moduleID && i.AUTH_ROLE_SYSTEM.ROLE_SYSTEM_ID == roleSystems.ROLE_SYSTEM_ID);
                    moduleTree.id = roleModules.ROLE_MODULE_ID.ToString();
                    moduleTree.text = "模块：" + item.MODULE_NAME;
                    string b = roleModules.IS_ACTIVE == "1" ? "true" : "false";
                    moduleTree.@checked = bool.Parse(b);
                    moduleTree.attributes = "module";

                    moduleTreeSet.Add(moduleTree);
                    SetTree(moduleTree, item, roleSystems);
                    moduleTreeSet.Add(moduleTree);
                }
                roleSystemTree.children = moduleTreeSet.ToArray();
                RolesystemTreeSet.Add(roleSystemTree);
                return RolesystemTreeSet.ToArray();
        }
        private void SetTree(Tree tree, AUTH_MODULE module,AUTH_ROLE_SYSTEM roleSystems)
        {
            IQueryable<AUTH_ROLE_MODULE> queryRoleModule = RoleModuleRepository.GetQueryable();
            HashSet<Tree> childTreeSet = new HashSet<Tree>();
            var modules = from m in module.AUTH_MODULES
                          orderby m.SHOW_ORDER
                          select m;
            foreach (var item in modules)
            {
                if (item != module)
                {
                    Tree childTree = new Tree();
                    string moduleID = item.MODULE_ID.ToString();
                    //var roleModules = queryRoleModule.FirstOrDefault(i => i.Module.ModuleID == new Guid(moduleID) && i.RoleSystem.RoleSystemID == roleSystems.RoleSystemID);
                    var roleModules = queryRoleModule.FirstOrDefault(i => i.AUTH_MODULE.MODULE_ID == moduleID && i.AUTH_ROLE_SYSTEM.ROLE_SYSTEM_ID == roleSystems.ROLE_SYSTEM_ID);

                    childTree.id = roleModules.ROLE_MODULE_ID.ToString();
                    childTree.text = "模块：" + item.MODULE_NAME;
                    int a =Convert.ToInt32(roleModules.IS_ACTIVE);
                    childTree.@checked = roleModules == null ? false : Convert.ToBoolean(a);
                    childTree.attributes = "module";
                    childTreeSet.Add(childTree);
                    if (item.AUTH_MODULES.Count > 0)
                    {
                        SetTree(childTree, item, roleSystems);
                    }
                    if (item.AUTH_FUNCTION.Count > 0)
                    {
                        SetFunTree(childTree, item, roleModules);
                    }
                }
            }
            tree.children = childTreeSet.ToArray();
        }

        public bool ProcessRolePermissionStr(string rolePermissionStr)
        {
            string[] rolePermissionList = rolePermissionStr.Split(',');
            string type;
            string id;
            string isActive;
            bool result = false;
            for (int i = 0; i < rolePermissionList.Length - 1; i++)
            {
                string[] rolePermission = rolePermissionList[i].Split('^');
                type = rolePermission[0];
                id = rolePermission[1];
                isActive = rolePermission[2]=="true"?"1":"0";
                UpdateRolePermission(type, id, isActive);
                result = true;
            }
            return result;
        }

        public bool UpdateRolePermission(string type, string id, string isActive)
        {
            bool result = false;
            if (type == "system")
            {
                IQueryable<AUTH_ROLE_SYSTEM> queryRoleSystem = RoleSystemRepository.GetQueryable();
                //Guid sid = new Guid(id);
                //string sid = id;
                var system = queryRoleSystem.FirstOrDefault(i => i.ROLE_SYSTEM_ID == id);
                system.IS_ACTIVE = isActive.ToString();
                RoleSystemRepository.SaveChanges();
                result = true;
            }
            else if (type == "module")
            {
                IQueryable<AUTH_ROLE_MODULE> queryRoleModule = RoleModuleRepository.GetQueryable();
                //Guid mid = new Guid(id);
                var module = queryRoleModule.FirstOrDefault(i => i.ROLE_MODULE_ID == id);
                module.IS_ACTIVE = isActive.ToString();
                RoleModuleRepository.SaveChanges();
                result = true;
            }
            else if (type == "function")
            {
                IQueryable<AUTH_ROLE_FUNCTION> queryRoleFunction = RoleFunctionRepository.GetQueryable();
                //Guid fid = new Guid(id);
                var system = queryRoleFunction.FirstOrDefault(i => i.ROLE_FUNCTION_ID == id);
                system.IS_ACTIVE = isActive.ToString();
                RoleSystemRepository.SaveChanges();
                result = true;
            }
            return result;
        }

        public bool InitUserSystemInfo(string userID, string cityID, string systemID)
        {
            var user = UserRepository.GetQueryable().Single(u => u.USER_ID == userID);
            var city = CityRepository.GetQueryable().Single(c => c.CITY_ID == cityID);
            var system = SystemRepository.GetQueryable().Single(s => s.SYSTEM_ID == systemID);
            InitUserSystem(user, city, system);
            return true;
        }

        public object GetUserSystemDetails(string userID, string cityID, string systemID)
        {
            IQueryable<AUTH_SYSTEM> querySystem = SystemRepository.GetQueryable();
            IQueryable<AUTH_MODULE> queryModule = ModuleRepository.GetQueryable();
            IQueryable<AUTH_USER_SYSTEM> queryUserSystem = UserSystemRepository.GetQueryable();
            IQueryable<AUTH_USER_MODULE> queryUserModule = UserModuleRepository.GetQueryable();
            var systems = querySystem.Single(i => i.SYSTEM_ID == systemID);
            var userSystems = queryUserSystem.FirstOrDefault(i => i.AUTH_SYSTEM.SYSTEM_ID == systemID && i.AUTH_USER.USER_ID == userID && i.AUTH_CITY.CITY_ID == cityID);
            HashSet<Tree> userSystemTreeSet = new HashSet<Tree>();
            Tree userSystemTree = new Tree();
            userSystemTree.id = userSystems.USER_SYSTEM_ID.ToString();
            userSystemTree.text = "系统：" + systems.SYSTEM_NAME;
            userSystemTree.@checked = userSystems.IS_ACTIVE=="1"?true:false;
            userSystemTree.attributes = "system";

            var modules = queryModule.Where(m => m.AUTH_SYSTEM.SYSTEM_ID == systems.SYSTEM_ID && m.MODULE_ID == m.PARENT_AUTH_MODULE.MODULE_ID)
                                     .OrderBy(m => m.SHOW_ORDER)
                                     .Select(m => m);
            HashSet<Tree> moduleTreeSet = new HashSet<Tree>();
            foreach (var item in modules)
            {
                Tree moduleTree = new Tree();
                string moduleID = item.MODULE_ID.ToString();
                //var userModules = queryUserModule.FirstOrDefault(i => i.Module.ModuleID == new Guid(moduleID) && i.UserSystem.UserSystemID == userSystems.UserSystemID);
                var userModules = queryUserModule.FirstOrDefault(i => i.AUTH_MODULE.MODULE_ID == moduleID && i.AUTH_USER_SYSTEM.USER_SYSTEM_ID == userSystems.USER_SYSTEM_ID);

                moduleTree.id = userModules.USER_MODULE_ID.ToString();
                moduleTree.text = "模块：" + item.MODULE_NAME;
                int a = Convert.ToInt32(userModules.IS_ACTIVE);
                moduleTree.@checked =Convert.ToBoolean(a);
                moduleTree.attributes = "module";

                moduleTreeSet.Add(moduleTree);
                SetModuleTree(moduleTree, item, userSystems);
                moduleTreeSet.Add(moduleTree);
            }
            userSystemTree.children = moduleTreeSet.ToArray();
            userSystemTreeSet.Add(userSystemTree);
            return userSystemTreeSet.ToArray();
        }

        private void SetModuleTree(Tree tree,AUTH_MODULE module,AUTH_USER_SYSTEM userSystems)
        {
            HashSet<Tree> childTreeSet = new HashSet<Tree>();
            var modules = from m in module.AUTH_MODULES
                          orderby m.SHOW_ORDER
                          select m;
            foreach (var item in modules)
            {
                if (item != module)
                {
                    Tree childTree = new Tree();
                    string moduleID = item.MODULE_ID.ToString();
                    //var userModules = UserModuleRepository.GetQueryable().FirstOrDefault(i => i.Module.ModuleID == new Guid(moduleID) && i.UserSystem.UserSystemID == userSystems.UserSystemID);
                    var userModules = UserModuleRepository.GetQueryable().FirstOrDefault(i => i.AUTH_MODULE.MODULE_ID == moduleID && i.AUTH_USER_SYSTEM.USER_SYSTEM_ID == userSystems.USER_SYSTEM_ID);

                    childTree.id = userModules.USER_MODULE_ID.ToString();
                    childTree.text = "模块：" + item.MODULE_NAME;
                    int a = Convert.ToInt32(userModules.IS_ACTIVE);
                    childTree.@checked = userModules == null ? false : Convert.ToBoolean(a);
                    childTree.attributes = "module";
                    childTreeSet.Add(childTree);
                    if (item.AUTH_MODULES.Count > 0)
                    {
                        SetModuleTree(childTree, item, userSystems);
                    }
                    if (item.AUTH_FUNCTION.Count > 0)
                    {
                        SetModuleFunTree(childTree, item, userModules);
                    }
                }
            }
            tree.children = childTreeSet.ToArray();
        }

        private void SetModuleFunTree(Tree childTree, AUTH_MODULE item, AUTH_USER_MODULE userModules)
        {
            var function = FunctionRepository.GetQueryable().Where(f => f.AUTH_MODULE.MODULE_ID == item.MODULE_ID);
            HashSet<Tree> functionTreeSet = new HashSet<Tree>();
            foreach (var func in function)
            {
                Tree funcTree = new Tree();
                var userFunction = UserFunctionRepository.GetQueryable().FirstOrDefault(rf => rf.AUTH_FUNCTION.FUNCTION_ID == func.FUNCTION_ID && rf.AUTH_USER_MODULE.USER_MODULE_ID == userModules.USER_MODULE_ID);
                funcTree.id = userFunction.USER_FUNCTION_ID.ToString();
                funcTree.text = "功能：" + func.FUNCTION_NAME;
                int a = Convert.ToInt32(userFunction.IS_ACTIVE);
                funcTree.@checked = userFunction == null ? false : Convert.ToBoolean(a);
                funcTree.attributes = "function";
                functionTreeSet.Add(funcTree);
            }
            childTree.children = functionTreeSet.ToArray();
        }

        public bool ProcessUserPermissionStr(string userPermissionStr)
        {
            string[] rolePermissionList = userPermissionStr.Split(',');
            string type;
            string id;
            string isActive;
            bool result = false;
            for (int i = 0; i < rolePermissionList.Length - 1; i++)
            {
                string[] rolePermission = rolePermissionList[i].Split('^');
                type = rolePermission[0];
                id = rolePermission[1];
                isActive = rolePermission[2]=="true"?"1":"0";
                UpdateUserPermission(type, id, isActive);
                result = true;
            }
            return result;
        }

        private bool UpdateUserPermission(string type, string id, string isActive)
        {
            bool result = false;
            if (type == "system")
            {
                IQueryable<AUTH_USER_SYSTEM> queryUserSystem = UserSystemRepository.GetQueryable();
                //Guid sid = new Guid(id);
                var system = queryUserSystem.FirstOrDefault(i => i.USER_SYSTEM_ID == id);
                system.IS_ACTIVE = isActive;
                RoleSystemRepository.SaveChanges();
                result = true;
            }
            else if (type == "module")
            {
                IQueryable<AUTH_USER_MODULE> queryUserModule = UserModuleRepository.GetQueryable();
               // Guid mid = new Guid(id);
                var module = queryUserModule.FirstOrDefault(i => i.USER_MODULE_ID == id);
                module.IS_ACTIVE = isActive;
                RoleModuleRepository.SaveChanges();
                result = true;
            }
            else if (type == "function")
            {
                IQueryable<AUTH_USER_FUNCTION> queryUserFunction = UserFunctionRepository.GetQueryable();
                //Guid fid = new Guid(id);
                var system = queryUserFunction.FirstOrDefault(i => i.USER_FUNCTION_ID == id);
                system.IS_ACTIVE = isActive;
                RoleSystemRepository.SaveChanges();
                result = true;
            }
            return result;
        }

        #endregion


        #region IModuleService 成员


        public object GetDetails2(int page, int rows, string QueryString, string Value)
        {
            string SystemName = "";
            string ModuleName = "";
            if (QueryString == "SystemName")
            {
                SystemName = Value;
            }
            else
            {
                ModuleName = Value;
            }
            IQueryable<AUTH_MODULE> ModuleQuery = ModuleRepository.GetQueryable();
            var Module = ModuleQuery.Where(c => c.AUTH_SYSTEM.SYSTEM_NAME.Contains(SystemName) && c.MODULE_NAME.Contains(ModuleName))
                .Select(c => new
                {
                    c.MODULE_ID,
                    c.MODULE_NAME,
                    c.MODULE_URL,
                    ParentModule = c.PARENT_AUTH_MODULE.MODULE_NAME,
                    c.AUTH_SYSTEM.SYSTEM_NAME,
                    c.PARENT_MODULE_MODULE_ID ,
                    c.SHOW_ORDER 
                });
            Module = Module.OrderBy(c => c.PARENT_MODULE_MODULE_ID).ThenBy (c=>c.SHOW_ORDER );
            int total = Module.Count();
            Module = Module.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = Module.ToArray() };
        }

        #endregion
    }
}
