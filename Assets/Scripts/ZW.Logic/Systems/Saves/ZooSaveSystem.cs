using System;
using JG.Data.Entities.Interfaces.Systems.Saves;
using UniRx;
using ZW.Logic.Interfaces.Handlers.Application;
using ZW.Logic.Interfaces.Streams.Animals;

namespace ZW.Logic.Systems.Saves
{
    public class ZooSaveSystem : IApplicationLifecycleHandler, IDisposable
    {
        private readonly ISaveSystem _saveSystem;
        private readonly IDisposable _spawnedSubscription;
        private readonly IDisposable _diedSubscription;

        public ZooSaveSystem(ISaveSystem saveSystem, IAnimalSpawnedStream spawnedStream, IAnimalDiedStream diedStream)
        {
            _saveSystem = saveSystem;
            _spawnedSubscription = spawnedStream.Subscribe(_ => _saveSystem.RequestSave());
            _diedSubscription = diedStream.Subscribe(_ => _saveSystem.RequestSave());
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
            _spawnedSubscription.Dispose();
            _diedSubscription.Dispose();
        }
    }
}
