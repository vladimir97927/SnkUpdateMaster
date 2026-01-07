namespace SnkUpdateMaster.Core.Common
{
    public interface IDependencyRegistry
    {
        void RegisterFactory<T>(Func<IDependencyResolver, T> factory);

        void RegisterInstance<T>(T instance);
    }
}
