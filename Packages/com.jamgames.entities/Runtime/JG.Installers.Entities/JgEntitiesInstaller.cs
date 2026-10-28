using JG.Core.Installers.Containers;
using JG.Data.Entities.Providers.Contexts;
using JG.Data.Entities.Providers.GameData;
using JG.Data.Entities.Repositories.Common;
using JG.Logic.Entities.Repositories.Saves;
using JG.Logic.Entities.Services.Loaders;
using JG.Logic.Entities.Services.Saves;
using JG.Logic.Entities.Streams.Saves;
using JG.Logic.Entities.Systems.Saves;

namespace JG.Installers.Entities
{
    public class JgEntitiesInstaller
    {
        public static readonly DiContainerConfig Config = new DiContainerConfig();

        static JgEntitiesInstaller()
        {
            #region Streams

            Config.SelfBind<LoadContextsStream>();

            #endregion

            #region Systems

            Config.SelfBind<ContextLoadSystem>();
            Config.SelfBind<SaveSystem>();

            #endregion

            #region Providers

            Config.SelfBind<ContextProvider>();
            Config.SelfBind<GameDataProvider>();

            #endregion

            #region Services

            Config.SelfBind<GameDataLoadService>();
            Config.SelfBind<GameSaveJsonService>();
            Config.SelfBind<GameSavePlayerPrefsKeyService>();

            #endregion

            #region Repositories

            Config.SelfBind<GameSaveRepository>();
            Config.SelfBind<SettingsRepository>();

            #endregion
        }
    }
}