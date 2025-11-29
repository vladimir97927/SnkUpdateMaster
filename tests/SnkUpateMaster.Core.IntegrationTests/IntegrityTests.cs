using SnkUpateMaster.Core.IntegrationTests.SeedWork;
using SnkUpdateMaster.Core.Integrity;

namespace SnkUpateMaster.Core.IntegrationTests
{
    [TestFixture]
    internal class IntegrityTests : TestBase
    {
        [Test]
        public void Sha256IntegrityTest()
        {
            var shaIntegrityProvider = new ShaIntegrityProvider();
            var shaIntegrityVerifier = new ShaIntegrityVerifier();
            var filePath = @"SeedWork\TestApp.zip";

            var checkSum = shaIntegrityProvider.ComputeFileChecksum(filePath);

            var isVerified = shaIntegrityVerifier.VerifyFile(filePath, checkSum);

            Assert.That(isVerified, Is.True);
        }
    }
}
