using SnkUpdateMaster.Core.Common;
using SnkUpdateMaster.Core.Integrity;
using SnkUpdateMaster.Core.ReleasePublisher.Packager;

namespace SnkUpdateMaster.Core.ReleasePublisher
{
    /// <summary>
    /// Строитель для создания и настройки экземпляра <see cref="ReleaseManager"/>
    /// </summary>
    /// <remarks>
    /// Реализует паттерн "Строитель" для пошаговой настройки зависимостей <see cref="ReleaseManager"/>.
    /// Позволяет гибко конфигурировать компоненты системы публикации релизов через fluent-интерфейс.
    /// 
    /// <para><strong>Обязательные зависимости:</strong></para>
    /// <list type="bullet">
    /// <item><description><see cref="IReleaseSource"/></description></item>
    /// <item><description><see cref="IReleaseInfoSource"/></description></item>
    /// </list>
    /// 
    /// <para><strong>Зависимости, имеющие реализацию по умолчанию:</strong></para>
    /// <list type="bullet">
    /// <item><description><see cref="IReleasePackager"/></description></item>
    /// </list>
    /// </remarks>
    public class ReleaseManagerBuilder : DependencyBuilder<ReleaseManager>
    {
        /// <summary>
        /// Регистрирует упаковщик релиза в ZIP-архив
        /// </summary>
        /// <param name="integrityProviderType">Алгоритм вычисления контрольной суммы
        /// (по умолчанию SHA-256)</param>
        /// <returns>Текущий экземпляр строителя</returns>
        /// <remarks>
        /// Создает <see cref="ZipReleasePackager"/>
        /// </remarks>
        public ReleaseManagerBuilder WithZipPackager(IntegrityProviderType integrityProviderType = IntegrityProviderType.Sha256)
        {
            var integrityProvider = new ShaIntegrityProvider();
            switch (integrityProviderType)
            {
                case IntegrityProviderType.Sha256:
                    integrityProvider = new ShaIntegrityProvider();
                break;
            }
            RegisterInstance<IReleasePackager>(new ZipReleasePackager(integrityProvider));
            return this;
        }


        /// <summary>
        /// Создает экземпляр <see cref="ReleaseManager"/> с настроенными зависимостями
        /// </summary>
        /// <returns>Полностью сконфигурированный <see cref="ReleaseManager"/></returns>
        public override ReleaseManager Build()
        {
            var packager = ResolveRequired<IReleasePackager>();
            var releaseSource = ResolveRequired<IReleaseSource>();
            var releaseInfoSource = ResolveRequired<IReleaseInfoSource>();

            return new ReleaseManager(
                packager,
                releaseSource,
                releaseInfoSource);
        }
    }
}
