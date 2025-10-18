using SnkUpdateMaster.Core;
using SnkUpdateMaster.Core.UpdateSource;
using System.Net;

namespace SnkUpdateMaster.Ftp
{
    internal class FtpUpdateSource : IUpdateSource
    {
        private readonly string _ftpHost;

        private readonly string _username;

        private readonly string _password;

        private readonly string _updateInfoFilePath;

        private readonly IUpdateInfoFileParser _fileParser;

        public FtpUpdateSource(
            string ftpHost,
            string username,
            string password,
            string updateInfoFilePath,
            IUpdateInfoFileParser fileParser)
        {
            _ftpHost = ftpHost;
            _username = username;
            _password = password;
            _updateInfoFilePath = updateInfoFilePath;
            _fileParser = fileParser;
        }

        public Task<UpdateInfo?> GetLastUpdatesAsync()
        {

            var uri = new Uri($"ftp://{_ftpHost}{_updateInfoFilePath}");
           
        }
    }
}
