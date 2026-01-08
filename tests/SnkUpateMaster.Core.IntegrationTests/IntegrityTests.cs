using SnkUpateMaster.Core.IntegrationTests.SeedWork;
using SnkUpdateMaster.Core.Integrity;

namespace SnkUpateMaster.Core.IntegrationTests
{
    [TestFixture]
    internal class IntegrityTests : TestBase
    {
        private string _zipUpdatePath = string.Empty;

        [SetUp]
        public new void BeforeEachTest()
        {
            _zipUpdatePath = ZipUpdateHelper.CreateMockZipUpdate();
        }

        [TearDown]
        public new void AfterEachTest()
        {
            if (File.Exists(_zipUpdatePath))
            {
                File.Delete(_zipUpdatePath);
            }
        }

        [Test]
        public void Sha256IntegrityTest()
        {
            var shaIntegrityProvider = new ShaIntegrityProvider();
            var shaIntegrityVerifier = new ShaIntegrityVerifier();

            var checkSum = shaIntegrityProvider.ComputeFileChecksum(_zipUpdatePath);

            var isVerified = shaIntegrityVerifier.VerifyFile(_zipUpdatePath, checkSum);

            Assert.That(isVerified, Is.True);
        }
    }
}
