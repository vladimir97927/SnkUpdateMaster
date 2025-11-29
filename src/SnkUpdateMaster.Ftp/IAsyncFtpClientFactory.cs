using FluentFTP;

namespace SnkUpdateMaster.Ftp
{
    /// <summary>
    /// Определяет фабрику для асинхронного создания и подключения экземпляров <see cref="IAsyncFtpClient"/>.
    /// </summary>
    /// <remarks>Этот интерфейс предоставляет метод для получения подключенного <see cref="IAsyncFtpClient"/>
    /// экземпляра, гарантируя готовность клиента к использованию. Реализации могут управлять установкой соединения,
    /// конфигурацией и управлением ресурсами.</remarks>
    public interface IAsyncFtpClientFactory
    {
        /// <summary>
        /// Асинхронно создает и подключает экземпляр FTP-клиента.
        /// </summary>
        /// <param name="cancellationToken">Токен для отслеживания запросов на отмену.</param>
        /// <returns>Задача, представляющая асинхронную операцию. Результат задачи содержит экземпляр <see
        /// cref="IAsyncFtpClient"/>, подключенный и готовый к использованию.</returns>
        Task<IAsyncFtpClient> GetConnectClientAsync(CancellationToken cancellationToken = default);
    }
}
