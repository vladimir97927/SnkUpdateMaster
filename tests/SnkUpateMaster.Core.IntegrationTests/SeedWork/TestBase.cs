namespace SnkUpateMaster.Core.IntegrationTests.SeedWork
{
    internal class TestBase
    {
        protected string AppDir = "testApp";

        protected string VersionFileName = "version";

        protected Version CurrentVersion = new("1.0.0");

        [SetUp]
        public async Task BeforeEachTest()
        {
            CreateMockAppFiles();
            await CreateCurrentVersionFile();
        }

        [TearDown]
        public void AfterEachTest()
        {
            Directory.Delete(AppDir, true);
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
