namespace SnkUpdateMaster.FileSystem
{
    internal static class FileSystemStreamHelper
    {
        private const int BufferSize = 81920;

        public static async Task<byte[]> ReadAllBytesAsync(
            string filePath,
            CancellationToken cancellationToken = default)
        {
            await using var source = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                BufferSize,
                FileOptions.Asynchronous);

            await using var destination = new MemoryStream();
            await CopyToAsync(source, destination, cancellationToken);

            return destination.ToArray();
        }

        private static async Task CopyToAsync(
            Stream source,
            Stream destination,
            CancellationToken cancellationToken = default)
        {
            var buffer = new byte[BufferSize];
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, cancellationToken)) != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
            }
        }
    }
}
