using Microsoft.Extensions.Logging;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Common;
using SnkUpdateMaster.Core.Downloader;
using SnkUpdateMaster.Core.Files;
using SnkUpdateMaster.Core.UpdateSource;

namespace SnkUpdateMaster.FileSystem.Configuration
{
    public static class UpdateManagerBuilderFileSystemExtensions
    {
        public static UpdateManagerBuilder WithFileSystemUpdateInfoProvider(
            this UpdateManagerBuilder builder,
            IUpdateInfoFileParser updateInfoFileParser,
            string updateFileInfoPath)
        {
            IUpdateInfoProvider UpdateInfoProviderFactory(IDependencyResolver dr)
            {
                var loggerFactory = dr.Resolve<ILoggerFactory>();
                var logger = loggerFactory?.CreateLogger<FileSystemUpdateInfoProvider>();
                return new FileSystemUpdateInfoProvider(updateInfoFileParser, updateFileInfoPath, logger);
            }
            builder.RegisterFactory(UpdateInfoProviderFactory);
            return builder;
        }

        public static UpdateManagerBuilder WithFileSystemUpdateDownloader(
            this UpdateManagerBuilder builder,
            string downloadsDir)
        {
            IUpdateDownloader UpdateDownloaderFactory(IDependencyResolver dr)
            {
                var loggerFactory = dr.Resolve<ILoggerFactory>();
                var logger = loggerFactory?.CreateLogger<FileSystemUpdateDownloader>();
                return new FileSystemUpdateDownloader(downloadsDir, logger);
            }
            builder.RegisterFactory(UpdateDownloaderFactory);
            return builder;
        }
    }
}
