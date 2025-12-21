using FluentFTP;

namespace SnkUpdateMaster.Ftp
{
    /// <summary>
    /// Фабрика для создания и управления асинхронным FTP-подключением.
    /// </summary>
    /// <remarks>Эта фабрика гарантирует повторное использование одного экземпляра FTP-клиента, пока он
    /// остаётся подключённым. Если клиент отключён, новое соединение будет установлено при вызове <see
    /// cref="GetConnectClientAsync"/>. Фабрика предназначена для управления жизненным циклом FTP-клиента.</remarks>
    /// <param name="host">Имя хоста</param>
    /// <param name="username">Имя пользователя</param>
    /// <param name="password">Пароль</param>
    /// <param name="port">Порт (по умолчанию 21)</param>
    public class AsyncFtpClientFactory(
        string host,
        string username,
        string password,
        int port = 21) : IAsyncFtpClientFactory, IDisposable
    {
        private readonly string _host = host;

        private readonly string _username = username;

        private readonly string _password = password;

        private readonly int _port = port;

        private AsyncFtpClient? _client;

        public void Dispose()
        {
            if (_client != null && _client.IsConnected)
            {
                _client.Dispose();
            }
        }

        /// <summary>
        /// Возвращает экземпляр FTP-клиента, подключаясь к серверу при необходимости.
        /// </summary>
        /// <remarks>Если клиент ещё не подключен, создаётся новый экземпляр <see
        /// cref="AsyncFtpClient"/>, который подключается к серверу, используя предоставленные учётные данные и информацию о хосте.</remarks>
        /// <param name="cancellationToken">Токен для отслеживания запросов на отмену.</param>
        /// <returns>Экземпляр <see cref="IAsyncFtpClient"/>, подключенный к FTP-серверу.</returns>
        public async Task<IAsyncFtpClient> GetConnectClientAsync(CancellationToken cancellationToken = default)
        {
            if (_client == null || !_client.IsConnected)
            {
                _client = new AsyncFtpClient(_host, _username, _password, _port);
                await _client.AutoConnect(cancellationToken);
            }

            return _client;
        }
    }
}
