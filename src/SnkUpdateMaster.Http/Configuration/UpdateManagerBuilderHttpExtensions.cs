using Microsoft.Extensions.Logging;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Common;
using SnkUpdateMaster.Core.Downloader;
using SnkUpdateMaster.Core.Parser;
using SnkUpdateMaster.Core.UpdateSource;

namespace SnkUpdateMaster.Http.Configuration
{
    public static class UpdateManagerBuilderHttpExtensions
    {
        public static UpdateManagerBuilder WithHttpUpdateInfoProvider(
            this UpdateManagerBuilder builder,
            HttpClient httpClient,
            IUpdateInfoParser updateInfoParser,
            string updateInfoUrl)
        {
            IUpdateInfoProvider UpdateInfoProviderFactory(IDependencyResolver dr)
            {
                var loggerFactory = dr.Resolve<ILoggerFactory>();
                var logger = loggerFactory?.CreateLogger<HttpUpdateInfoProvider>();
                return new HttpUpdateInfoProvider(httpClient, updateInfoParser, updateInfoUrl, logger);
            }
            builder.RegisterFactory(UpdateInfoProviderFactory);
            return builder;
        }

        public static UpdateManagerBuilder WithHttpUpdateDownloader(
            this UpdateManagerBuilder builder,
            HttpClient httpClient,
            string downloadsDir)
        {
            IUpdateDownloader UpdateDownloaderFactory(IDependencyResolver dr)
            {
                var loggerFactory = dr.Resolve<ILoggerFactory>();
                var logger = loggerFactory?.CreateLogger<HttpUpdateDownloader>();
                return new HttpUpdateDownloader(httpClient, downloadsDir, logger);
            }
            builder.RegisterFactory(UpdateDownloaderFactory);
            return builder;
        }
    }
}
