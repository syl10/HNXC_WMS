using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.Authority.Bll.Interfaces;
using Microsoft.Practices.Unity;
using System.Configuration;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.ServiceLocation;
using System.Web.Routing;
using THOK.Authority.Bll.Service;

namespace THOK.Security
{
    public class ServiceFactory
    {
        private readonly IUnityContainer _container;
        public ServiceFactory()
        {
            _container = new UnityContainer();          
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            section.Configure(_container, "defaultContainer");
            ServiceLocatorProvider sp = new ServiceLocatorProvider(GetServiceLocator);
            ServiceLocator.SetLocatorProvider(sp);
        }

        public T GetService<T>()
        {
            return _container.Resolve<T>();
        }

        public IServiceLocator GetServiceLocator()
        {
            return new UnityServiceLocator(_container);
        }
    }
}