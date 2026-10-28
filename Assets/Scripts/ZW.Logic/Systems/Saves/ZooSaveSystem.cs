using System;
using JG.Data.Entities.Interfaces.Repositories.Common;
using JG.Data.Entities.Interfaces.Systems.Saves;
using UniRx;
using ZW.Data.Interfaces.Entities.Settings;
using ZW.Logic.Interfaces.Handlers.Application;
using ZW.Logic.Interfaces.Streams.Animals;

namespace ZW.Logic.Systems.Saves
{
    public class ZooSaveSystem : IApplicationLifecycleHandler, IDisposable
    {
        private readonly ISaveSystem _saveSystem;
        private readonly IDisposable _changesSubscription;

        public ZooSaveSystem(
            ISaveSystem saveSystem,
            ISettingsRepository settingsRepository,
            IAnimalSpawnedStream spawnedStream,
            IAnimalDiedStream diedStream)
        {
            _saveSystem = saveSystem;

            var interval = TimeSpan.FromSeconds(settingsRepository.Get<IZooSettingsEntity>().AutosaveInterval);
            _changesSubscription = Observable
                .Merge(spawnedStream.AsUnitObservable(), diedStream.AsUnitObservable())
                .Sample(interval)
                .Subscribe(_ => _saveSystem.RequestSave());
        }

        public void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                _saveSystem.Save();
            }
        }

        public void OnApplicationQuit()
        {
            _saveSystem.Save();
        }

        public void Dispose()
        {
            _changesSubscription.Dispose();
        }
    }
}
