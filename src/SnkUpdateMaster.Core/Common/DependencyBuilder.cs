using System.Collections.Concurrent;

namespace SnkUpdateMaster.Core.Common
{
    /// <summary>
    /// Базовый класс для построения объектов с внедрением зависимостей
    /// </summary>
    /// <typeparam name="TResult">Тип конечного объекта для построения</typeparam>
    /// <remarks>
    /// Реализует паттерн "Строитель" для пошаговой настройки зависимостей
    /// </remarks>
    public abstract class DependencyBuilder<TResult> : IDependencyRegistry, IDependencyResolver
    {
        private readonly ConcurrentDictionary<Type, Func<IDependencyResolver, object>> _factories = new();

        private readonly ConcurrentDictionary<Type, object> _instances = new();

        public T? Resolve<T>()
        {
            if (!TryResolve(typeof(T), out var value))
                return default;

            return value is T typed ? typed : default;
        }

        public T ResolveRequired<T>()
        {
            if (!TryResolve(typeof(T), out var value))
            {
                throw new InvalidOperationException($"Dependency '{typeof(T).FullName}' is not registered.");
            }

            if (value is T typed)
                return typed;

            throw new InvalidCastException($"Registered dependency for '{typeof(T).FullName}' has type '{value?.GetType().FullName}', which cannot be cast.");
        }

        public void RegisterFactory<T>(Func<IDependencyResolver, T> factory)
        {
            _factories[typeof(T)] = c => factory(c)!;
        }

        public void RegisterInstance<T>(T instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance), "Instance can not be null");
            }
            _instances[typeof(T)] = instance;
        }

        private bool TryResolve(Type type, out object? value)
        {
            if (_instances.TryGetValue(type, out value))
                return true;

            if (_factories.TryGetValue(type, out var factory))
            {
                value = factory(this);
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Создает объект с настроенными зависимостями
        /// </summary>
        /// <returns>Экземпляр типа TResult</returns>
        public abstract TResult Build();
    }
}
