namespace SnkUpdateMaster.Core.ReleasePublisher
{
    /// <summary>
    /// Предоставляет краткую информаицю о данных релиза
    /// </summary>
    public class ReleaseInfo
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Версия релиза записанная строкой в формате major.minor.build
        /// </summary>
        public string Version { get; set; } = null!;

        /// <summary>
        /// Дата и время выпуска релиза
        /// </summary>
        public DateTime ReleaseDate {  get; set; }
    }
}
