using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.Files;
using SnkUpdateMaster.Core.UpdateSource;

namespace SnkUpdateMaster.Ftp
{
    internal class FtpUpdateSource : IUpdateSource
    {
        private readonly IAsyncFtpClientFactory _ftpClientFactory;

        private readonly IUpdateInfoFileParser _updateInfoFileParser;

        private readonly string _updateInfoFilePath;

        public FtpUpdateSource(
            IAsyncFtpClientFactory ftpClientFactory,
            IUpdateInfoFileParser updateInfoFileParser,
            string updateInfoFilePath)
        {
            _ftpClientFactory = ftpClientFactory;
            _updateInfoFileParser = updateInfoFileParser;
            _updateInfoFilePath = updateInfoFilePath;
        }

        public async Task<UpdateInfo?> GetLastUpdatesAsync()
        {
            var client = await _ftpClientFactory.GetConnectClientAsync();
            if (!await client.FileExists(_updateInfoFilePath))
            {
                return null;
            }
            try
            {
                byte[] fileBytes;
                using var stream = new MemoryStream();
                bool isSuccess = await client.DownloadStream(stream, _updateInfoFilePath);
                if (!isSuccess)
                {
                    throw new FileNotFoundException($"Can't download update info file from {_updateInfoFilePath}");
                }
                fileBytes = stream.ToArray();
                var updateInfo = _updateInfoFileParser.Parse(fileBytes);
                return updateInfo;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
