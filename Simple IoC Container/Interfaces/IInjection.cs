using System;
using System.Collections.Generic;

namespace Simple_IoC_Container.Interfaces
{
    public interface IInjection
    {
        T Resolve<T>() where T : class;
        IFromType UseSingleton();
        Dictionary<Type, Type> GetAllRegisteredTypes();
        bool IsRegistered<Type>();
    }
}