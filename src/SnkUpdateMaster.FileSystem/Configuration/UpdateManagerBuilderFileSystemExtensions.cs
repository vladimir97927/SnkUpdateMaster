using Microsoft.Extensions.Logging;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Common;
using SnkUpdateMaster.Core.Downloader;
using SnkUpdateMaster.Core.Parser;
using SnkUpdateMaster.Core.UpdateSource;

namespace SnkUpdateMaster.FileSystem.Configuration
{
    /// <summary>
    /// Provides extension methods for configuring file system-based update providers and downloaders on an <see
    /// cref="UpdateManagerBuilder"/> instance.
    /// </summary>
    /// <remarks>These extensions enable the use of file system sources for update information and update
    /// package downloads. They are intended to be used during the setup of an update manager to register file
    /// system-based implementations for update retrieval and downloading. Thread safety and configuration details
    /// depend on the underlying <see cref="UpdateManagerBuilder"/> and registered services.</remarks>
    public static class UpdateManagerBuilderFileSystemExtensions
    {
        /// <summary>
        /// Configures the update manager to use a file system-based update info provider with the specified file parser
        /// and update info file path.
        /// </summary>
        /// <remarks>This method registers a <see cref="FileSystemUpdateInfoProvider"/> with the update
        /// manager, enabling update information to be loaded from a file. The provider uses the specified parser and
        /// file path, and will log operations using the resolved logger if available.</remarks>
        /// <param name="builder">The update manager builder to configure.</param>
        /// <param name="updateInfoFileParser">The parser used to read and interpret the update info file.</param>
        /// <param name="updateFileInfoPath">The path to the update info file on the file system. Cannot be null or empty.</param>
        /// <returns>The same <see cref="UpdateManagerBuilder"/> instance, configured to use the file system update info
        /// provider.</returns>
        public static UpdateManagerBuilder WithFileSystemUpdateInfoProvider(
            this UpdateManagerBuilder builder,
            IUpdateInfoParser updateInfoFileParser,
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

        /// <summary>
        /// Configures the update manager to use a file system-based update downloader that stores downloaded update
        /// files in the specified directory.
        /// </summary>
        /// <remarks>Use this method to enable update downloads to be saved locally on disk. The specified
        /// directory should exist and be accessible by the application at runtime. If a logger is available in the
        /// dependency resolver, it will be used for logging download operations.</remarks>
        /// <param name="builder">The update manager builder to configure with the file system update downloader.</param>
        /// <param name="downloadsDir">The directory path where downloaded update files will be stored. Must be a valid, writable file system path.</param>
        /// <returns>The same instance of <see cref="UpdateManagerBuilder"/> with the file system update downloader registered.</returns>
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
