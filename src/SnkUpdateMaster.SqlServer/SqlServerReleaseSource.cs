using SnkUpdateMaster.Core.ReleasePublisher;
using SnkUpdateMaster.SqlServer.Database;

namespace SnkUpdateMaster.SqlServer
{
    internal class SqlServerReleaseSource(SnkUpdateMasterContext context) : IReleaseSource
    {
        private readonly SnkUpdateMasterContext _context = context;

        public async Task<Release?> GetReleaseAsync(int id)
        {
            return await _context.Releases.FindAsync(id);
        }

        public async Task RemoveReleaseAsync(Release release)
        {
            _context.Remove(release);
            await _context.SaveChangesAsync();
        }

        public async Task UploadReleaseAsync(Release release)
        {
            await _context.AddAsync(release);
            await _context.SaveChangesAsync();
        }
    }
}
