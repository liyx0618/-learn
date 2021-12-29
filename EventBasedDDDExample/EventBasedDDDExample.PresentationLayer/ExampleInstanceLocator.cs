using System;
using EventBasedDDD;
using Microsoft.Practices.Unity;

namespace EventBasedDDDExample.PresentationLayer
{
    public class ExampleInstanceLocator : IInstanceLocator
    {
        #region IInstanceLocator Members

        public T GetInstance<T>() where T : class
        {
            return UnityContainerHolder.UnityContainer.Resolve<T>();
        }

        public object GetInstance(Type instanceType)
        {
            return UnityContainerHolder.UnityContainer.Resolve(instanceType);
        }

        public bool IsTypeRegistered<T>()
        {
            return UnityContainerHolder.UnityContainer.IsRegistered<T>();
        }

        public bool IsTypeRegistered(Type type)
        {
            return UnityContainerHolder.UnityContainer.IsRegistered(type);
        }

        public void RegisterType<T>()
        {
            UnityContainerHolder.UnityContainer.RegisterType<T>();
        }

        public void RegisterType(Type type)
        {
            UnityContainerHolder.UnityContainer.RegisterType(type);
        }

        #endregion
    }
}
