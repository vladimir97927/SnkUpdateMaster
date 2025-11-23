using FluentFTP;

namespace SnkUpdateMaster.Ftp
{
    public class AsyncFtpClientFactory : IAsyncFtpClientFactory, IDisposable
    {
        private readonly string _host;

        private readonly string _username;

        private readonly string _password;

        private readonly int _port = 21;

        private AsyncFtpClient? _client;

        public AsyncFtpClientFactory(
            string host,
            string username,
            string password,
            int port = 21)
        {
            _host = host;
            _username = username;
            _password = password;
            _port = port;
        }

        public void Dispose()
        {
            if (_client != null && _client.IsConnected)
            {
                _client.Dispose();
            }
        }

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
