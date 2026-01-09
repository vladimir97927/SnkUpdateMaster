namespace SnkUpdateMaster.FileSystem
{
    public static class FileStreamHelper
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

        public static async Task CopyToFileAsync(
            string sourcePath,
            string destinationPath,
            IProgress<double>? progress = null,
            CancellationToken cancellationToken = default)
        {
            var fileInfo = new FileInfo(sourcePath);
            var totalBytes = fileInfo.Length;
            await using var source = new FileStream(
                sourcePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                BufferSize,
                FileOptions.Asynchronous);
            await using var destination = new FileStream(
                destinationPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                BufferSize,
                FileOptions.Asynchronous);
            
            await CopyToAsync(source, destination, totalBytes, progress, cancellationToken);
        }

        public static async Task CopyToAsync(
            Stream source,
            Stream destination,
            CancellationToken cancellationToken = default)
        {
            await CopyToAsync(source, destination, -1, null, cancellationToken);
        }

        public static async Task CopyToAsync(
            Stream source,
            Stream destination,
            long totalBytes,
            IProgress<double>? progress,
            CancellationToken cancellationToken = default)
        {
            long totalBytesRead = 0;
            var buffer = new byte[BufferSize];
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, cancellationToken)) != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                totalBytesRead += bytesRead;
                if (progress != null && totalBytes > 0)
                {
                    double percentage = (double)totalBytesRead / totalBytes;
                    progress.Report(percentage);
                }
            }
        }
    }
}
