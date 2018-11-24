using System;
using System.Collections.Generic;

namespace IoCLiteContainer.Interfaces
{
    public interface IInjection
    {
        T Resolve<T>() where T : class;
        IFromType UseSingleton();
        Dictionary<Type, Type> GetAllRegisteredTypes();
        bool IsRegistered<Type>();
    }
}