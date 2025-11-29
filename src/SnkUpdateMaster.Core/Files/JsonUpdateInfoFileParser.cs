using System.Text;
using System.Text.Json;

namespace SnkUpdateMaster.Core.Files
{
    public class JsonUpdateInfoFileParser : IUpdateInfoFileParser
    {
        public UpdateInfo Parse(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found at path {filePath}");
            }
            
            try
            {
                var fileBytes = File.ReadAllBytes(filePath);
                return Parse(fileBytes);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public UpdateInfo Parse(byte[] fileBytes)
        {
            var jsonString = Encoding.UTF8.GetString(fileBytes);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var dto = JsonSerializer.Deserialize<UpdateInfoDto>(jsonString, options) ??
                throw new Exception("Cant't parse file");
            return new UpdateInfo(
                dto.Id,
                dto.Version,
                dto.FileName,
                dto.Checksum,
                dto.ReleaseDate,
                dto.FileDir);
        }

        private sealed class UpdateInfoDto
        {
            public int Id { get; set; }

            public string Version { get; set; } = default!;

            public string FileName { get; set; } = default!;

            public string? FileDir { get; set; }

            public string Checksum { get; set; } = default!;

            public DateTime ReleaseDate { get; set; }
        }
    }
}
