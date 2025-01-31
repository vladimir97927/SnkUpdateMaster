using SnkUpdateMaster.Core;
using System.Text.Json;

namespace SnkUpateMaster.IntegrationTests
{
    [TestFixture]
    internal class Class1
    {
        [Test]
        public void Test()
        {
            var updatePackege = new UpdatePackage(
                "1.0.0", 
                1,
                new List<FileChange> 
                { 
                    new FileChange("App.exe", ChangeType.Add), 
                    new FileChange("test.dll", ChangeType.Modify) 
                });

            var json = JsonSerializer.Serialize(updatePackege);

            var test = JsonSerializer.Deserialize<UpdatePackage>(json);
        }
    }
}
