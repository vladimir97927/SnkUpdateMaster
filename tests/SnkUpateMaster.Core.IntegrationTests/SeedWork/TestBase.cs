namespace SnkUpateMaster.Core.IntegrationTests.SeedWork
{
    internal class TestBase
    {
        protected string AppDir = "testApp";

        protected string VersionFileName = "version";

        protected Version CurrentVersion = new("1.0.0");

        protected string BackupDir => Path.Combine(
            Path.GetDirectoryName(Path.GetFullPath(AppDir)) ?? Path.GetTempPath(),
            $"{Path.GetFileName(Path.TrimEndingDirectorySeparator(Path.GetFullPath(AppDir)))}_backup");

        [SetUp]
        public async Task BeforeEachTest()
        {
            CreateMockAppFiles();
            await CreateCurrentVersionFile();
        }

        [TearDown]
        public void AfterEachTest()
        {
            if (Directory.Exists(AppDir))
            {
                Directory.Delete(AppDir, true);
            }

            if (Directory.Exists(BackupDir))
            {
                Directory.Delete(BackupDir, true);
            }

            if (File.Exists(VersionFileName))
            {
                File.Delete(VersionFileName);
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
    }
}
