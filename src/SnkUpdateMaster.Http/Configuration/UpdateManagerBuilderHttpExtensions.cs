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
        /// <summary>
        /// Registers an HTTP-based update information provider with the specified update manager builder.
        /// </summary>
        /// <remarks>This method configures the update manager to retrieve update information from a
        /// remote HTTP endpoint using the provided <paramref name="httpClient"/> and <paramref
        /// name="updateInfoParser"/>. Ensure that the <paramref name="httpClient"/> is properly configured for your
        /// network and security requirements.</remarks>
        /// <param name="builder">The update manager builder to configure with the HTTP update information provider.</param>
        /// <param name="httpClient">The HTTP client used to send requests for retrieving update information. The caller is responsible for
        /// managing the lifetime of this instance.</param>
        /// <param name="updateInfoParser">The parser used to interpret the update information received from the HTTP endpoint. Cannot be null.</param>
        /// <param name="updateInfoUrl">The URL of the HTTP endpoint that provides update information. Cannot be null or empty.</param>
        /// <returns>The same <see cref="UpdateManagerBuilder"/> instance, enabling method chaining.</returns>
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

        /// <summary>
        /// Configures the update manager to use an HTTP-based update downloader with the specified HTTP client and
        /// downloads directory.
        /// </summary>
        /// <remarks>This method registers an HTTP-based update downloader with the update manager. The
        /// provided <paramref name="httpClient"/> is used for all HTTP requests made by the downloader. Ensure that the
        /// <paramref name="downloadsDir"/> exists and is accessible by the application.</remarks>
        /// <param name="builder">The update manager builder to configure.</param>
        /// <param name="httpClient">The HTTP client instance used to download update files. The caller is responsible for configuring the client
        /// as needed.</param>
        /// <param name="downloadsDir">The directory path where downloaded update files will be stored. Must be a valid, writable directory path.</param>
        /// <returns>The same <see cref="UpdateManagerBuilder"/> instance, configured to use the HTTP update downloader.</returns>
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
