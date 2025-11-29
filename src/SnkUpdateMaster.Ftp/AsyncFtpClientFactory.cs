using FluentFTP;

namespace SnkUpdateMaster.Ftp
{
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
