using FluentFTP;

namespace SnkUpdateMaster.Ftp
{
    public interface IAsyncFtpClientFactory
    {
        AsyncFtpClient GetConnectClientAsync();
    }
}
