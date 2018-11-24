using System;
using System.Collections.Generic;

namespace IoCLite.Container.Interfaces
{
    public interface IInjection
    {
        T Resolve<T>() where T : class;
        IFromType UseSingleton();
        Dictionary<Type, Type> GetAllRegisteredTypes();
        bool IsRegistered<Type>();
    }
}