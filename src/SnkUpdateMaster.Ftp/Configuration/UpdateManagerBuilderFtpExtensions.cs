using SnkUpdateMaster.Core;

namespace SnkUpdateMaster.Ftp.Configuration
{
    public static class UpdateManagerBuilderFtpExtensions
    {
        public static UpdateManagerBuilder WithFtpUpdateProvider(
            this UpdateManagerBuilder builder,
            string host,
            string username,
            string password,
            string downloadsDir,
            int port = 21)
        {
            var ftpClientFactory = new AsyncFtpClientFactory(host, username, password, port);

            
        }
    }
}
