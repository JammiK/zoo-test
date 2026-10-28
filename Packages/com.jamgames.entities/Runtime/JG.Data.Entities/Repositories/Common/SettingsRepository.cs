using JG.Data.Entities.Interfaces.Entities.Common;
using JG.Data.Entities.Interfaces.Providers.GameData;
using JG.Data.Entities.Interfaces.Repositories.Common;

namespace JG.Data.Entities.Repositories.Common
{
    public class SettingsRepository : ISettingsRepository
    {
        readonly IGameDataProvider _gameDataProvider;

        public SettingsRepository(IGameDataProvider gameDataProvider)
        {
            _gameDataProvider = gameDataProvider;
        }

        public T Get<T>() where T : ISettingsEntity, IEntity
        {
            var settingsEntity = _gameDataProvider.GetSettings<T>();
            return settingsEntity;
        }
    }
}