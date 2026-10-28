using JG.Data.Entities.Interfaces.Entities.Common;
using JG.Data.Entities.Interfaces.Repositories.Common;

namespace ZW.Logic.Tests.Fakes
{
    public class FakeSettingsRepository : ISettingsRepository
    {
        private readonly object _settings;

        public FakeSettingsRepository(object settings)
        {
            _settings = settings;
        }

        public T Get<T>() where T : ISettingsEntity, IEntity => (T)_settings;
    }
}
