using Core.JG.Data.Interfaces.Services;
using ZW.Data.Interfaces.Managers.Animals;
using ZW.Data.Interfaces.Providers.Animals;
using ZW.Data.Interfaces.Types.Animals;
using ZW.Data.Models.Types;
using ZW.Logic.Interfaces.Services.Movements;
using ZW.Logic.Interfaces.Systems.Movements;

namespace ZW.Logic.Systems.Movements
{
    public class AnimalObstacleSystem : IAnimalObstacleSystem
    {
        private readonly IAnimalProvider _animalProvider;
        private readonly IAnimalManager _animalManager;
        private readonly IObstacleBounceService _bounceService;
        private readonly IRandomService _randomService;

        public AnimalObstacleSystem(
            IAnimalProvider animalProvider,
            IAnimalManager animalManager,
            IObstacleBounceService bounceService,
            IRandomService randomService)
        {
            _animalProvider = animalProvider;
            _animalManager = animalManager;
            _bounceService = bounceService;
            _randomService = randomService;
        }

        public void HitObstacle(int uid, PlanarVector obstacleNormal)
        {
            var animal = _animalProvider.Get(uid);
            if (animal == null || animal.State != AnimalState.Alive)
            {
                return;
            }

            var direction = _bounceService.GetDirectionAway(obstacleNormal, _randomService.NextFloat());
            if (direction.HasValue)
            {
                _animalManager.SetDirection(animal, direction.Value);
            }
        }
    }
}
