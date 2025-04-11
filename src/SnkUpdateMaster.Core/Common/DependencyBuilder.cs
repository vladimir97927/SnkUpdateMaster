namespace SnkUpdateMaster.Core.Common
{
    public abstract class DependencyBuilder<TResult>
    {
        private readonly Dictionary<Type, object> _dependencies = [];

        protected T GetDependency<T>()
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

        public DependencyBuilder<TResult> AddDependency<T>(T dependency)
        {
            if (dependency == null)
                throw new ArgumentNullException(nameof(dependency));
            _dependencies[typeof(T)] = dependency;
            return this;
        }

        public abstract TResult Build();
    }
}
