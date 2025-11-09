using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Files;

namespace SnkUpdateMaster.Ftp.Configuration
{
    public static class UpdateManagerBuilderFtpExtensions
    {
        public static UpdateManagerBuilder WithFtpUpdateSource(
            this UpdateManagerBuilder builder,
            )
        {

        }

        public static UpdateManagerBuilder WithFtpUpdateProvider(
            this UpdateManagerBuilder builder,
            IUpdateInfoFileParser updateInfoFileParser,
            string host,
            string username,
            string password,
            string updateFileInfoPath,
            string downloadsDir,
            int port = 21)
        {
            var ftpClientFactory = new AsyncFtpClientFactory(host, username, password, port);
            var updateSource = new FtpUpdateSource(ftpClientFactory, updateInfoFileParser, updateFileInfoPath);
        }
    }
}
