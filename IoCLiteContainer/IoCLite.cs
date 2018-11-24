using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IoCLiteContainer.Attributes;
using IoCLiteContainer.Exceptions;
using IoCLiteContainer.Interfaces;

namespace IoCLiteContainer
{
    // TODO: push dat terrible terror to NuGet.org (God bless those guys)
    public class IoCLite
    {
        private Dictionary<Type, Type> _registeredTypes;
        private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();
        private bool _isSingletonMode;

        /// <summary>
        /// Fabric method of IoCLite container
        /// </summary>
        /// <returns>New instance of container</returns>
        public static IoCLite CreateInstance() => new IoCLite();

        /// <summary>
        /// Declares instantiation method for all types in this container as Singleton.
        /// </summary>
        /// <returns><see cref="IFromType"/> builder for fluent syntax</returns>
        public void UseSingleton() => _isSingletonMode = true;

        /// <summary>
        /// Sets target type matching from <see cref="IoCLiteContainerConfiguration"/>
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public IoCLite SetConfiguration(IoCLiteContainerConfiguration config)
        {
            _registeredTypes = config.GetAllRegisteredTypes() ?? throw new ArgumentNullException("Invalid Config");
            return this;
        }

        /// <summary>
        /// Resolve registered dependencies with respect to generic parameter.
        /// </summary>
        /// <typeparam name="T">Registered root type</typeparam>
        /// <returns>Instance of resolved type</returns>
        public T Resolve<T>() where T : class
        {
            if (_registeredTypes == null)
            {
                throw new ArgumentNullException("No configuration passed");
            }

            if (!_registeredTypes.Any())
            {
                throw new NoItemRegisterdException("No entity has been registered yet.");
            }

            T resolveInstance = null;
            try
            {
                resolveInstance = (T)ResolveParameter(typeof(T));
            }
            catch (Exception ex)
            {
                throw new InvalidTypeRegistrationException(nameof(ex), ex);
            }

            if (_isSingletonMode && !_instances.ContainsKey(typeof(T)))
                _instances.Add(typeof(T), resolveInstance);

            return resolveInstance;
        }

        private object ResolveParameter(Type rootType)
        {
            Type pendingType = null;

            // Return instance from inner storage
            if (_isSingletonMode && _instances.ContainsKey(rootType))
                return _instances.First(f => f.Key == rootType).Value;  

            pendingType = _registeredTypes[rootType];

            var ctor = GetConstructor(pendingType);

            var ctorParams = GetConstructorParameters(ctor);

            //Instantiate as endpoint type
            if (!ctorParams.Any())
            {
                var simpleInstance = ctor.Invoke(null);
                ResolveProperties(pendingType, simpleInstance);

                return simpleInstance;
            }
            
            var paramLst = new List<object>(ctorParams.Count());

            //Collect all endpoint types in c-tor parameters
            for (int i = 0; i < ctorParams.Count(); i++)
            {
                var paramType = ctorParams.ElementAt(i).ParameterType;
                var resolvedParam = ResolveParameter(paramType);
                paramLst.Add(resolvedParam);
            }

            //Instantiate with dependency injections
            var instance = ctor.Invoke(paramLst.ToArray());
            ResolveProperties(rootType, instance);

            return instance;
        }

        private void ResolveProperties(Type rootType, object obj)
        {
            foreach (var property in rootType.GetProperties())
            {
                if (Attribute.GetCustomAttributes(property, typeof(InjectionRequiredAttribute), false).Any())
                {
                    if (property.CanWrite)
                    {
                        property.SetValue(obj, ResolveParameter(property.PropertyType));
                    }
                    else
                    {
                        throw new InvalidOperationException("One or more properties with private setter was found.");
                    }
                }
            }
        }

        private IEnumerable<ParameterInfo> GetConstructorParameters(ConstructorInfo constructor)
            => constructor.GetParameters()
                .Where(parameter => parameter.GetType().IsClass);

        private ConstructorInfo GetConstructor(Type pendingType)
        {
            return pendingType.GetConstructors()
                    .OrderByDescending(constructors => constructors.GetParameters().Length)
                    .FirstOrDefault()
                ?? throw new InvalidOperationException(nameof(pendingType));
        }

        //private Func<T> GetSimpleInstance()
        //{
        //    Type t = typeof(T);
        //    if (HasDefaultConstructor(t))
        //            return Expression.Lambda<Func<T>>(Expression.New(t)).Compile();


        //}

        //private bool HasDefaultConstructor(Type pendingType)
        //{
        //    return pendingType.IsValueType || pendingType.GetConstructor(Type.EmptyTypes) != null;
        //}
    }
}
