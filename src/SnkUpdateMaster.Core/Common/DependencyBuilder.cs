namespace SnkUpdateMaster.Core.Common
{
    /// <summary>
    /// Базовый класс для построения объектов с внедрением зависимостей
    /// </summary>
    /// <typeparam name="TResult">Тип конечного объекта для построения</typeparam>
    /// <remarks>
    /// Реализует паттерн "Строитель" для пошаговой настройки зависимостей
    /// </remarks>
    public abstract class DependencyBuilder<TResult>
    {
        private readonly Dictionary<Type, object> _dependencies = [];

        /// <summary>
        /// Возвращает зарегистрированную зависимость по типу
        /// </summary>
        /// <typeparam name="T">Тип требуемой зависимости</typeparam>
        /// <returns>Экземпляр зависимости типа T</returns>
        /// <exception cref="InvalidCastException">
        /// Зависимость не может быть приведена к указанному типу
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Зависимость указанного типа не зарегистрирована
        /// </exception>
        public T GetDependency<T>()
        {
            if (_dependencies.TryGetValue(typeof(T), out var dependency))
            {
                try
                {
                    return (T)dependency;
                }
                catch
                {
                    throw new InvalidCastException($"Can not cast object to {nameof(T)} dependency");
                }
            }

            throw new ArgumentNullException($"Please add {nameof(T)} dependency");
        }

        /// <summary>
        /// Пытается получить зарегистрированную зависимость по типу
        /// </summary>
        /// <typeparam name="T">Тип требуемой зависимости</typeparam>
        /// <param name="dependency">Найденная зависимость, если она зарегистрирована</param>
        /// <returns>
        /// <c>true</c> если зависимость зарегистрирована<br/>
        /// <c>false</c> если зависимость отсутствует
        /// </returns>
        /// <exception cref="InvalidCastException">
        /// Зависимость не может быть приведена к указанному типу
        /// </exception>
        public bool TryGetDependency<T>(out T? dependency)
        {
            if (_dependencies.TryGetValue(typeof(T), out var dep))
            {
                try
                {
                    dependency = (T)dep;
                    return true;
                }
                catch
                {
                    dependency = default;
                    return false;
                }
            }
            dependency = default;
            return false;
        }

        /// <summary>
        /// Регистрирует зависимость в строителе
        /// </summary>
        /// <typeparam name="T">Тип зависимости (обычно интерфейс)</typeparam>
        /// <param name="dependency">Экземпляр зависимости</param>
        /// <returns>Текущий экземпляр строителя</returns>
        /// <exception cref="ArgumentNullException">
        /// Переданная зависимость равна null
        /// </exception>
        public DependencyBuilder<TResult> AddDependency<T>(T dependency)
        {
            if (dependency == null)
                throw new ArgumentNullException(nameof(dependency));
            _dependencies[typeof(T)] = dependency;
            return this;
        }

        /// <summary>
        /// Создает объект с настроенными зависимостями
        /// </summary>
        /// <returns>Экземпляр типа TResult</returns>
        public abstract TResult Build();
    }
}
