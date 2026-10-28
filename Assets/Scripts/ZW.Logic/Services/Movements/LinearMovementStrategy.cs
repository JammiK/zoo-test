using System.Linq;
using JG.Data.Entities.Interfaces.Repositories.Common;
using ZW.Data.Interfaces.Entities.Animals;
using ZW.Data.Interfaces.Entities.Movements;
using ZW.Data.Interfaces.Models.Animals;
using ZW.Logic.Extensions;
using ZW.Logic.Interfaces.Services.Movements;
using ZW.Logic.Models.Movements;

namespace ZW.Logic.Services.Movements
{
    public class LinearMovementStrategy : IAnimalMovementStrategy
    {
        private readonly IEntityRepository<ILinearMovementEntity> _configRepository;
        private readonly IRandomDirectionService _randomDirectionService;

        public LinearMovementStrategy(
            IEntityRepository<ILinearMovementEntity> configRepository,
            IRandomDirectionService randomDirectionService)
        {
            _configRepository = configRepository;
            _randomDirectionService = randomDirectionService;
        }

        public bool CanMove(IAnimalEntity entity)
        {
            return GetConfig(entity) != null;
        }

        public MovementDecision Decide(IAnimalModel animal, MovementContext context)
        {
            var config = GetConfig(animal.Entity);
            var direction = animal.Direction;
            var cooldown = animal.MoveCooldown - context.DeltaTime;

            if (!context.IsInsideArea)
            {
                direction = context.ReturnDirection;
            }
            else if (cooldown <= 0f)
            {
                direction = _randomDirectionService.Next();
                cooldown = config.TurnInterval;
            }

            return new MovementDecision(direction, cooldown, new AnimalMotion(direction.Scale(config.Speed), 0f));
        }

        private ILinearMovementEntity GetConfig(IAnimalEntity entity)
        {
            return _configRepository.GetFirst(config => config.Animals.Any(animal => animal.Id == entity.Id));
        }
    }
}
