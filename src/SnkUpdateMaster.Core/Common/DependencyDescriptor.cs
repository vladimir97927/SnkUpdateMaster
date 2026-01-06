namespace SnkUpdateMaster.Core.Common
{
    internal class DependencyDescriptor
    {
        public Type? DependencyType { get; set; }

        public Type? ImplementationType { get; set; }

        public object? Instance { get; set; }

        public Func<IDependencyContainer, object>? Factory { get; set; }
    }
}
