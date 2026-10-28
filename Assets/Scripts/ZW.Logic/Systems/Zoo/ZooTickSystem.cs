using System;
using Core.JG.Logic.Core.Interfaces.Systems.Core;
using ZW.Logic.Interfaces.Services.Environment;
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

        public ZooTickSystem(
            IUpdateSystem updateSystem,
            IEnvironmentService environmentService,
            IAnimalSpawnSystem spawnSystem,
            IAnimalMovementSystem movementSystem)
        {
            _updateSystem = updateSystem;
            _environmentService = environmentService;
            _spawnSystem = spawnSystem;
            _movementSystem = movementSystem;
            _updateSystem.Add(Tick);
        }

        public void Dispose()
        {
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
