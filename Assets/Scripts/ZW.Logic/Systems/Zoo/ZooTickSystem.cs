using System;
using Core.JG.Logic.Core.Interfaces.Systems.Core;
using UniRx;
using ZW.Logic.Interfaces.Services.Environment;
using ZW.Logic.Interfaces.Streams.Zoo;
using ZW.Logic.Interfaces.Systems.Movements;
using ZW.Logic.Interfaces.Systems.Spawns;

namespace ZW.Logic.Systems.Zoo
{
    public class ZooTickSystem : IDisposable
    {
        private readonly IUpdateSystem _updateSystem;
        private readonly IEnvironmentService _environmentService;
        private readonly IAnimalSpawnSystem _spawnSystem;
        private readonly IAnimalMovementSystem _movementSystem;
        private readonly IDisposable _startedSubscription;

        public ZooTickSystem(
            IUpdateSystem updateSystem,
            IEnvironmentService environmentService,
            IAnimalSpawnSystem spawnSystem,
            IAnimalMovementSystem movementSystem,
            IZooStartedStream startedStream)
        {
            _updateSystem = updateSystem;
            _environmentService = environmentService;
            _spawnSystem = spawnSystem;
            _movementSystem = movementSystem;
            _startedSubscription = startedStream.Subscribe(_ => _updateSystem.Add(Tick));
        }

        public void Dispose()
        {
            _startedSubscription.Dispose();
            _updateSystem.Remove(Tick);
        }

        private void Tick()
        {
            var deltaTime = _environmentService.DeltaTime;
            _spawnSystem.Tick(deltaTime);
            _movementSystem.Tick(deltaTime);
        }
    }
}
