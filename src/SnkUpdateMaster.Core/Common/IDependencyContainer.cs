namespace SnkUpdateMaster.Core.Common
{
    internal interface IDependencyContainer
    {
        void RegisterFactory<T>(Func<IDependencyContainer, T> factory);

        void RegisterInstance<T>(T instance);

        T? GetDependency<T>();

        T GetRequiredDependency<T>();
    }
}
