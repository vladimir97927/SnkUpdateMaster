using System.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Files;
using SnkUpdateMaster.Core.UpdateSource;

namespace SnkUpdateMaster.FileSystem
{
    public sealed class FileSystemUpdateInfoProvider(
        IUpdateInfoFileParser updateInfoFileParser,
        string updateInfoFilePath,
        ILogger<FileSystemUpdateInfoProvider>? logger = null) : IUpdateInfoProvider
    {
        private readonly IUpdateInfoFileParser _updateInfoFileParser = updateInfoFileParser;

        private readonly string _updateInfoFilePath = updateInfoFilePath;

        private readonly ILogger<FileSystemUpdateInfoProvider> _logger = logger ?? NullLogger<FileSystemUpdateInfoProvider>.Instance;

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

                var fileBytes = await FileSystemStreamHelper.ReadAllBytesAsync(
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
