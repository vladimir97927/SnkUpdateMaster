using System.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Parser;
using SnkUpdateMaster.Core.UpdateSource;

namespace SnkUpdateMaster.FileSystem
{
    /// <summary>
    /// Provides update information by reading and parsing a file from the file system using a specified parser.
    /// </summary>
    /// <remarks>This provider reads update information from a file and parses it using the supplied parser.
    /// If the file does not exist, is empty, or cannot be accessed due to permissions or I/O errors, the provider
    /// returns null and logs the issue. Thread safety depends on the thread safety of the injected parser and
    /// logger.</remarks>
    /// <param name="updateInfoFileParser">The parser used to interpret the contents of the update info file.</param>
    /// <param name="updateInfoFilePath">The path to the update info file on the file system. Must not be null or empty.</param>
    /// <param name="logger">An optional logger used to record diagnostic messages and errors. If not specified, a no-op logger is used.</param>
    public sealed class FileSystemUpdateInfoProvider(
        IUpdateInfoParser updateInfoFileParser,
        string updateInfoFilePath,
        ILogger<FileSystemUpdateInfoProvider>? logger = null) : IUpdateInfoProvider
    {
        private readonly IUpdateInfoParser _updateInfoFileParser = updateInfoFileParser;

        private readonly string _updateInfoFilePath = updateInfoFilePath;

        private readonly ILogger<FileSystemUpdateInfoProvider> _logger = logger ?? NullLogger<FileSystemUpdateInfoProvider>.Instance;

        /// <summary>
        /// Asynchronously retrieves the most recent update information from the update info file, if available.
        /// </summary>
        /// <remarks>Returns <see langword="null"/> if the update info file does not exist, is empty,
        /// cannot be accessed, or cannot be parsed. If the operation is canceled, an <see
        /// cref="OperationCanceledException"/> is thrown.</remarks>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="UpdateInfo"/>
        /// object with the latest update information if the file exists and is valid; otherwise, <see
        /// langword="null"/>.</returns>
        public async Task<UpdateInfo?> GetLastUpdatesAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting to read update info from file: {UpdateInfoFilePath}", _updateInfoFilePath);

            try
            {
                if (!File.Exists(_updateInfoFilePath))
                {
                    _logger.LogWarning("Update info file does not exist: {UpdateInfoFilePath}", _updateInfoFilePath);
                    return null;
                }

                var fileBytes = await FileStreamHelper.ReadAllBytesAsync(
                    _updateInfoFilePath,
                    cancellationToken);

                if (fileBytes.Length == 0)
                {
                    _logger.LogWarning("Update info file is empty: {UpdateInfoFilePath}", _updateInfoFilePath);
                    return null;
                }

                var updateInfo = _updateInfoFileParser.Parse(fileBytes);
                _logger.LogInformation("Successfully read update info: {UpdateInfo}", updateInfo);

                return updateInfo;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Reading update info was canceled: {UpdateInfoFilePath}", _updateInfoFilePath);
                throw;
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Access denied when reading update info file: {UpdateInfoFilePath}", _updateInfoFilePath);
                return null;
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "I/O error while reading update info file: {UpdateInfoFilePath}", _updateInfoFilePath);
                return null;
            }
            catch (SecurityException ex)
            {
                _logger.LogError(ex, "Security error while reading update info file: {UpdateInfoFilePath}", _updateInfoFilePath);
                return null;
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Failed to parse update info file: {UpdateInfoFilePath}", _updateInfoFilePath);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting update info from file: {UpdateInfoFilePath}", _updateInfoFilePath);
                return null;
            }
        }
    }
}
