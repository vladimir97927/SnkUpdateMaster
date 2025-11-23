using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Downloader;
using SnkUpdateMaster.Core.Files;
using SnkUpdateMaster.Core.UpdateSource;

namespace SnkUpdateMaster.Ftp.Configuration
{
    public static class UpdateManagerBuilderFtpExtensions
    {
        public static UpdateManagerBuilder WithFtpUpdateSource(
            this UpdateManagerBuilder builder,
            IUpdateInfoFileParser updateInfoFileParser,
            IAsyncFtpClientFactory asyncFtpClientFactory,
            string updateFileInfoPath)
        {
            var updateSource = new FtpUpdateSource(asyncFtpClientFactory, updateInfoFileParser, updateFileInfoPath);

            builder.AddDependency<IUpdateInfoProvider>(updateSource);

            return builder;
        }

        public static UpdateManagerBuilder WithFtpUpdateDownloader(
            this UpdateManagerBuilder builder,
            IAsyncFtpClientFactory asyncFtpClientFactory,
            string updateFileDir,
            string downloadsDir)
        {
            var updateDownloader = new FtpUpdateDownloader(asyncFtpClientFactory, downloadsDir, updateFileDir);

            builder.AddDependency<IUpdateDownloader>(updateDownloader);

            return builder;
        }
    }
}
