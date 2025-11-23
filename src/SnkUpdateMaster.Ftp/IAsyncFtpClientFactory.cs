using FluentFTP;

namespace SnkUpdateMaster.Ftp
{
    public interface IAsyncFtpClientFactory
    {
        Task<IAsyncFtpClient> GetConnectClientAsync(CancellationToken cancellationToken = default);
    }
}
