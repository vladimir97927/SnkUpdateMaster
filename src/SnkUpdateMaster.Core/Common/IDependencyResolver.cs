namespace SnkUpdateMaster.Core.Common
{
    public interface IDependencyResolver
    {
        T? Resolve<T>();

        T ResolveRequired<T>();
    }
}
