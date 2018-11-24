using System;
using System.Collections.Generic;
using System.Linq;
using IoCLiteContainer.Interfaces;

namespace IoCLiteContainer
{
    public class IoCLiteContainerConfiguration : IFromType, IToType
    {
        private readonly Dictionary<Type, Type> _registeredTypes = new Dictionary<Type, Type>();
        private Type _tmpTypeFrom;

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
        /// Declares type matching for container to resolve.
        /// </summary>
        /// <typeparam name="TFrom">Type to be resolver</typeparam>
        /// <typeparam name="TTo">Actual type of previous parameter</typeparam>
        /// <returns><see cref="IFromType"/> builder for fluent syntax</returns>
        public IFromType Register<TFrom, TTo>() where TTo : TFrom
            => Bind<TFrom>().To<TTo>();

        /// <summary>
        /// Get all registered types.
        /// </summary>
        /// <returns><see cref="Dictionary{Type,Type}"/> of registered types</returns>
        public Dictionary<Type, Type> GetAllRegisteredTypes()
            => _registeredTypes;

        /// <summary>
        /// Check if passed type is registered.
        /// </summary>
        /// <typeparam name="TType">Type to be checked</typeparam>
        /// <returns><see cref="true"/> if type is registered</returns>
        public bool IsRegistered<TType>()
            => _registeredTypes.Any(a => a.Key == typeof(TType));

    }
}
