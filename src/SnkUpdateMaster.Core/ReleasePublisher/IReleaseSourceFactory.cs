namespace SnkUpdateMaster.Core.ReleasePublisher
{
    /// <summary>
    /// Интерфейс фабрики для создания экземпляров <see cref="IReleaseSource"/>
    /// </summary>
    public interface IReleaseSourceFactory
    {
        /// <summary>
        /// Создает новый экземпляр источника данных релизов
        /// </summary>
        IReleaseSource Create();
    }
}
