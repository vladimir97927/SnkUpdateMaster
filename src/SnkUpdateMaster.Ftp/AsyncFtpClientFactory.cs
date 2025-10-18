using FluentFTP;

namespace SnkUpdateMaster.Ftp
{
    public class AsyncFtpClientFactory : IAsyncFtpClientFactory
    {
        private readonly AsyncFtpClient _client;

        public AsyncFtpClientFactory(string host, string username, string password, int port = 21)
        {
            
        }

        public FtpClient GetConnectClient()
        {
            throw new NotImplementedException();
        }

        public AsyncFtpClient GetConnectClientAsync()
        {
            throw new NotImplementedException();
        }
    }
}
