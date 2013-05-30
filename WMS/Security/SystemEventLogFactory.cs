using System;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.Authority.Bll.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using THOK.Authority.Bll.Service;
using System.Web.Routing;

namespace Wms.Security
{
    public class SystemEventLogFactory : DefaultControllerFactory
    {
        private readonly IUnityContainer _container;
        public  ISystemEventLogService SystemEventLogService;
        public IExceptionalLogService ExceptionalLogService;
        public SystemEventLogFactory()
        {
            _container = new UnityContainer();          
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            section.Configure(_container, "defaultContainer");
            SystemEventLogService = _container.Resolve<ISystemEventLogService>() as SystemEventLogService;
            ExceptionalLogService = _container.Resolve<IExceptionalLogService>() as ExceptionalLogService;
            ServiceLocatorProvider sp = new ServiceLocatorProvider(GetServiceLocator);
            ServiceLocator.SetLocatorProvider(sp);
        }

        public SystemEventLogFactory(IUnityContainer container)
        {
            _container = container;
            ServiceLocatorProvider sp = new ServiceLocatorProvider(GetServiceLocator);
            ServiceLocator.SetLocatorProvider(sp);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType != null)
            {
                return _container.Resolve(controllerType) as IController;
            }
            return base.GetControllerInstance(requestContext, controllerType);
        }

        public IServiceLocator GetServiceLocator()
        {
            return new UnityServiceLocator(_container);
        }
    }
}