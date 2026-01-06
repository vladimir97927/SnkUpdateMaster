
using System.Collections.Concurrent;

namespace SnkUpdateMaster.Core.Common
{
    internal class DependencyContainer : IDependencyContainer
    {
        private readonly ConcurrentDictionary<Type, DependencyDescriptor> _dependencies = [];

        public T? GetDependency<T>()
        {
            if (_dependencies.TryGetValue(typeof(T), out var dependency))
            {
                try
                {
                    return (T)ResolveDependency(dependency);
                }
                catch
                {
                    return default;
                }
            }

            return default;
        }

        public T GetRequiredDependency<T>()
        {
            if (_dependencies.TryGetValue(typeof(T), out var dependency))
            {
               return (T)ResolveDependency(dependency);        
            }

            throw new ArgumentNullException($"Please add {nameof(T)} dependency");
        }

        public void RegisterFactory<T>(Func<IDependencyContainer, T> factory)
        {
            _dependencies[typeof(T)] = new DependencyDescriptor
            {
                DependencyType = typeof(T),
                Factory = container => factory(container)
            };
        }

        public void RegisterInstance<T>(T instance)
        {
            _dependencies[typeof(T)] = new DependencyDescriptor
            {
                DependencyType = typeof(T),
                Instance = instance
            };
        }

        private object ResolveDependency(DependencyDescriptor descriptor)
        {
            if (descriptor.Instance != null)
            {
                return descriptor.Instance;
            }

            if (descriptor.Factory != null)
            {
                return descriptor.Factory(this);
            }

            throw new Exception($"Can not resolve dependency: {descriptor}");
        }
    }
}
