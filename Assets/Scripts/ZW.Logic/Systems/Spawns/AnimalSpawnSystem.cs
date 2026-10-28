using Core.JG.Data.Interfaces.Services;
using JG.Data.Entities.Interfaces.Providers.Contexts;
using JG.Data.Entities.Interfaces.Repositories.Common;
using ZW.Data.Interfaces.Contexts;
using ZW.Data.Interfaces.Entities.Animals;
using ZW.Data.Interfaces.Entities.Settings;
using ZW.Data.Interfaces.Factories.Animals;
using ZW.Data.Interfaces.Managers.Zoo;
using ZW.Logic.Interfaces.Services.Movements;
using ZW.Logic.Interfaces.Services.Spawns;
using ZW.Logic.Interfaces.Streams.Animals;
using ZW.Logic.Interfaces.Systems.Spawns;

namespace ZW.Logic.Systems.Spawns
{
    public class AnimalSpawnSystem : IAnimalSpawnSystem
    {
        private readonly IContextProvider _contextProvider;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IEntityRepository<IAnimalEntity> _animalRepository;
        private readonly IRandomService _randomService;
        private readonly IAnimalSpawnPickService _spawnPickService;
        private readonly ISpawnPointService _spawnPointService;
        private readonly IRandomDirectionService _randomDirectionService;
        private readonly IAnimalFactory _animalFactory;
        private readonly IZooManager _zooManager;
        private readonly IAnimalSpawnedStream _spawnedStream;

        public AnimalSpawnSystem(
            IContextProvider contextProvider,
            ISettingsRepository settingsRepository,
            IEntityRepository<IAnimalEntity> animalRepository,
            IRandomService randomService,
            IAnimalSpawnPickService spawnPickService,
            ISpawnPointService spawnPointService,
            IRandomDirectionService randomDirectionService,
            IAnimalFactory animalFactory,
            IZooManager zooManager,
            IAnimalSpawnedStream spawnedStream)
        {
            _contextProvider = contextProvider;
            _settingsRepository = settingsRepository;
            _animalRepository = animalRepository;
            _randomService = randomService;
            _spawnPickService = spawnPickService;
            _spawnPointService = spawnPointService;
            _randomDirectionService = randomDirectionService;
            _animalFactory = animalFactory;
            _zooManager = zooManager;
            _spawnedStream = spawnedStream;
        }

        public void Tick(float deltaTime)
        {
            var zoo = _contextProvider.Get<IZooContext>();
            var cooldown = zoo.SpawnCooldown - deltaTime;
            if (cooldown > 0f)
            {
                _zooManager.SetSpawnCooldown(cooldown);
                return;
            }

            var settings = _settingsRepository.Get<IZooSettingsEntity>();
            _zooManager.SetSpawnCooldown(_randomService.Range(settings.SpawnIntervalMin, settings.SpawnIntervalMax));
            if (zoo.Animals.Count < settings.MaxAnimals)
            {
                Spawn();
            }
        }

        private void Spawn()
        {
            var entity = _spawnPickService.Pick(_animalRepository.GetAll(), _randomService.NextFloat());
            if (entity == null)
            {
                return;
            }

            var point = _spawnPointService.FindFreePoint(entity.SpawnRadius);
            if (!point.HasValue)
            {
                return;
            }

            var animal = _animalFactory.Create(
                _zooManager.TakeNextUid(),
                entity,
                point.Value,
                _randomDirectionService.Next());

            _zooManager.AddAnimal(animal);
            _spawnedStream.OnNext(animal);
        }
    }
}
