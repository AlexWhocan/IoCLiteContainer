﻿using System;
using System.Collections.Generic;
using System.Linq;
using IoCLiteContainer.Attributes;
using IoCLiteContainer.Exceptions;
using IoCLiteContainer.Interfaces;

namespace IoCLiteContainer
{
    // TODO: Create builder for type matching as ConfigurationCollection(Or something like that)
    // TODO: Implement functionality to pass user's fabric predicates or ready objects
    // TODO: push dat terrible terror of NuGet.org (God bless those guys)
    public class IoCLite : IInjection, IFromType, IToType
    {
        private readonly Dictionary<Type, Type> _registeredTypes = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();
        private Type _tmpTypeFrom;
        private bool _isSingletonMode;

        public static IoCLite CreateInstance() => new IoCLite();

        /// <summary>
        /// Resolve registered dependencies with respect to generic parameter.
        /// </summary>
        /// <typeparam name="T">Registered root type</typeparam>
        /// <returns>Instance of resolved type</returns>
        public T Resolve<T>() where T : class
        {
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

        /// <summary>
        /// Registers generic passed type as pending to be registered.
        /// </summary>
        /// <typeparam name="TFrom">Type to be resolved</typeparam>
        /// <returns>Builder for To() method</returns>
        public IToType Bind<TFrom>()
        {
            _tmpTypeFrom = typeof(TFrom);
            return this;
        }

        /// <summary>
        /// Registers generic passed type as resolving for previously registered type.
        /// </summary>
        /// <typeparam name="TTo">Resolving type</typeparam>
        /// <returns><see cref="IFromType"/> builder for fluent syntax</returns>
        public IFromType To<TTo>()
        {
            if (_registeredTypes.ContainsKey(_tmpTypeFrom))
                _registeredTypes[_tmpTypeFrom] = typeof(TTo);
            else
            {
                _registeredTypes.Add(_tmpTypeFrom, typeof(TTo));
                _tmpTypeFrom = null;
            }

            return this;
        }

        /// <summary>
        /// Declares instantiation method for all types in this container as Singleton.
        /// </summary>
        /// <returns><see cref="IFromType"/> builder for fluent syntax</returns>
        public IFromType UseSingleton()
        {
            _isSingletonMode = true;
            return this;
        }

        /// <summary>
        /// Declares type matching for container to resolve.
        /// </summary>
        /// <typeparam name="TFrom">Type to be resolver</typeparam>
        /// <typeparam name="TTo">Actual type of previous parameter</typeparam>
        /// <returns><see cref="IFromType"/> builder for fluent syntax</returns>
        public IFromType Register<TFrom, TTo>() where TTo : TFrom
            => Bind<TFrom>().To<TTo>();
        
        public Dictionary<Type, Type> GetAllRegisteredTypes()
            => _registeredTypes;
        
        public bool IsRegistered<TType>()
            => _registeredTypes.Any(a => a.Key == typeof(TType));

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

        private object ResolveParameter(Type rootType)
        {
            Type pendingType = null;

            if (_isSingletonMode && _instances.ContainsKey(rootType))
            {
                return _instances.First(f => f.Key == rootType).Value;
            }
            else
                pendingType = _registeredTypes[rootType];

            var ctor = pendingType.GetConstructors()
                            .OrderByDescending(constructors => constructors.GetParameters().Length)
                            .FirstOrDefault()
                        ?? throw new InvalidOperationException(nameof(pendingType));
            var ctorParams = ctor.GetParameters()
                .Where(parameter => parameter.GetType().IsClass);

            if (!ctorParams.Any())
            {
                var simpleInstance = Activator.CreateInstance(pendingType);
                ResolveProperties(pendingType, simpleInstance);

                return simpleInstance;
            }
            
            var paramLst = new List<object>(ctorParams.Count());

            for (int i = 0; i < ctorParams.Count(); i++)
            {
                var paramType = ctorParams.ElementAt(i).ParameterType;
                var resolvedParam = ResolveParameter(paramType);
                paramLst.Add(resolvedParam);
            }

            var instance = ctor.Invoke(paramLst.ToArray());
            ResolveProperties(rootType, instance);

            return instance;
        }
    }
}
