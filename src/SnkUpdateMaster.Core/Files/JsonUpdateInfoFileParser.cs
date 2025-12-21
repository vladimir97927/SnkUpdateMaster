using System.Text;
using System.Text.Json;

namespace SnkUpdateMaster.Core.Files
{
    /// <summary>
    /// Предоставляет функциональность для парсинга информации об обновлениях из файлов JSON..
    /// </summary>
    /// <remarks>Этот класс реализует интерфейс <see cref="IUpdateInfoFileParser"/>.
    /// Он получает информацию из JSON-файлов, предоставленных в виде путей к файлам или байтовых массивов. Ожидается, что JSON-данные
    /// будут соответствовать определенной структуре, которая включает такие поля, Id, Version, FileName, Checksum, ReleaseDate,
    /// и необязательный параметр FileDir.</remarks>
    public class JsonUpdateInfoFileParser : IUpdateInfoFileParser
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Парсит указанный файл и извлекает информацию об обновлениях.
        /// </summary>
        /// <param name="filePath">Полный путь к файлу для анализа.</param>
        /// <returns>Объект <see cref="UpdateInfo"/>, содержащий информацию об обновлении.</returns>
        /// <exception cref="FileNotFoundException">Вызывается, если файл не существует по указанному пути <paramref name="filePath"/>.</exception>
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

        /// <summary>
        /// Парсит предоставленный массив байтов в объект <see cref="UpdateInfo"/>.
        /// </summary>
        /// <param name="fileBytes">Массив байтов, представляющий файл для парсинга. Должен содержать корректную строку JSON в кодировке UTF-8.</param>
        /// <returns>An <see cref="UpdateInfo"/>Объект, содержащий данные об обновлении.</returns>
        /// <exception cref="Exception">Вызывается, если десериализация приводит к null объекту.</exception>
        public UpdateInfo Parse(byte[] fileBytes)
        {
            var jsonString = Encoding.UTF8.GetString(fileBytes);
            var dto = JsonSerializer.Deserialize<UpdateInfoDto>(jsonString, _jsonOptions) ??
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
