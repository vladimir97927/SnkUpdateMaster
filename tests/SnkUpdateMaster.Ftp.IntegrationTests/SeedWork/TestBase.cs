namespace SnkUpdateMaster.Ftp.IntegrationTests.SeedWork
{
    internal class TestBase
    {
        protected string FtpHost => IntegrationTestConfig.Ftp.host;

        protected string FtpUser => IntegrationTestConfig.Ftp.user;

        protected string FtpPassword => IntegrationTestConfig.Ftp.pass;

        protected int FtpPort => IntegrationTestConfig.Ftp.port;

        protected string UpdateInfoFilePath = "/manifest.json";

        protected string DownloadsDir = "downloads";

        protected string AppDir = "testApp";

        protected string VersionFileName = "version";

        protected Version CurrentVersion = new("1.0.0");

        [SetUp]
        public async Task BeforeEachTest()
        {
            Directory.CreateDirectory(DownloadsDir);
            CreateMockAppFiles();
            await CreateCurrentVersionFile();
        }

        [TearDown]
        public async Task AfterEachTest()
        {
            Directory.Delete(DownloadsDir, true);
            Directory.Delete(AppDir, true);
        }

        private void CreateMockAppFiles()
        {
            Directory.CreateDirectory(AppDir);
            var testFiles = new List<string>()
            {
                "testlib_1.dll",
                "testlib_2.dll",
                "testApp.exe",
                "testFolder\\testFile.txt"
            };
            foreach (var file in testFiles)
            {
                var path = Path.Combine(AppDir, file);
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.Create(path).Close();
            }
        }

        private async Task CreateCurrentVersionFile()
        {
            if (File.Exists(VersionFileName))
            {
                File.Delete(VersionFileName);
            }

            var stream = File.Create(VersionFileName);
            stream.Close();
            await File.WriteAllTextAsync(VersionFileName, CurrentVersion.ToString());
        }
    }
}
