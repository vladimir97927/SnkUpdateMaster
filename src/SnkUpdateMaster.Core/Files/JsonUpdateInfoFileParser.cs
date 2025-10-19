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
            var updateInfo = JsonSerializer.Deserialize<UpdateInfo>(jsonString);
            return updateInfo == null ? throw new Exception("Cant't parse file") : updateInfo;
        }
    }
}
