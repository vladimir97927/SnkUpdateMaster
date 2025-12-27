using System.IO.Compression;

namespace SnkUpateMaster.Core.IntegrationTests.SeedWork
{
    internal static class ZipUpdateHelper
    {
        public static string CreateMockZipUpdate()
        {
            var tempFilePath = Path.Combine(Path.GetTempPath(), "TestUpdate.zip");
            using var archive = ZipFile.Open(tempFilePath, ZipArchiveMode.Create);
            var testFileNames = new List<string>()
            {
                "testlib_1.dll",
                "testlib_2.dll",
                "testlib_3.dll",
                "testApp.exe",
                "testFolder/testFile.txt"
            };
            foreach (var fileName in testFileNames)
            {
                var entry = archive.CreateEntry(fileName);
                using var entryStream = entry.Open();
                var date = new byte[512];
                entryStream.Write(date, 0, date.Length);
            }

            return tempFilePath;
        }
    }
}
